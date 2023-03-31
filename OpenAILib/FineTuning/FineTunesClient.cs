// Copyright (c) 2023 Owen Sigurdson
// MIT License

using OpenAILib.Files;
using OpenAILib.HttpHandling;
using OpenAILib.Models;
using OpenAILib.Serialization;
using System.Collections.Concurrent;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;

namespace OpenAILib.FineTuning
{
    internal class FineTunesClient : IFineTunesClient , IFineTuneModelNameProvider
    {
        private const string OpenAILibFineTuneFileMarker = "openailib-ft";
        private readonly FilesClient _filesClient;
        private readonly FineTunesLowLevelClient _fineTunesClient;
        private readonly ModelsClient _modelsClient;
        private static readonly ConcurrentDictionary<string, string> s_fineTuneIdToModelNameMap = new ConcurrentDictionary<string, string>();

        public FineTunesClient(OpenAIHttpClient httpClient)
        {
            _fineTunesClient = new FineTunesLowLevelClient(httpClient);
            _filesClient = new FilesClient(httpClient);
            _modelsClient = new ModelsClient(httpClient);
        }

        public async Task<FineTuneInfo> CreateFineTuneAsync(List<FineTunePair> trainingData, CancellationToken cancellationToken = default)
        {
            return await CreateFineTuneAsync(trainingData, new FineTuneSpec01(), cancellationToken);
        }

        public async Task<FineTuneInfo> CreateFineTuneAsync(List<FineTunePair> trainingData, Action<IFineTuneSpec01> spec, CancellationToken cancellationToken = default)
        {
            var settings = new FineTuneSpec01();
            spec(settings);
            return await CreateFineTuneAsync(trainingData, settings, cancellationToken);
        }

        public async Task<List<FineTuneEvent>> GetEventsAsync(FineTuneInfo fineTune, CancellationToken cancellationToken = default)
        {
            var fineTuneResponse = await _fineTunesClient.GetFineTuneAsync(fineTune.FineTuneId, cancellationToken);
            var result = fineTuneResponse.Events
                .Select(evt => FineTuneEvent.FromFineTuneResponse(evt))
                .ToList();

            return result;
        }

        public async IAsyncEnumerable<FineTuneEvent> GetEventStreamAsync(FineTuneInfo fineTune, [EnumeratorCancellation] CancellationToken cancellationToken = default)
        {
            await foreach (var fineTuneEventResponse in _fineTunesClient.GetEventStreamAsync(fineTune.FineTuneId, cancellationToken))
            {
                if (string.IsNullOrEmpty(fineTuneEventResponse.Message))
                {
                    continue;
                }
                yield return FineTuneEvent.FromFineTuneResponse(fineTuneEventResponse);
            }
        }

        public async Task<FineTuneStatus> GetStatusAsync(FineTuneInfo fineTune, CancellationToken cancellationToken = default)
        {
            var fineTuneResponse = await _fineTunesClient.GetFineTuneAsync(fineTune.FineTuneId, cancellationToken);

            // TODO: verify that these strings are what is actually returned
            switch (fineTuneResponse.Status)
            {
                case "succeeded":
                    return FineTuneStatus.Succeeded;
                case "failed":
                    return FineTuneStatus.Failed;
                default:
                    break;
            }
            return FineTuneStatus.NotReady;
        }

        public async Task<FineTuneInfo> CreateFineTuneVariantAsync(FineTuneInfo fineTune, Action<IFineTuneSpec01> spec, CancellationToken cancellationToken = default)
        {
            var settings = new FineTuneSpec01();
            spec(settings);

            var promptSuffix = settings.GetPromptSuffix();
            var completionSuffix = settings.GetCompletionSuffix();
            if (promptSuffix != fineTune.PromptSuffix || completionSuffix != fineTune.CompletionSuffix)
            {
                throw new ArgumentException($"Fine tune variant must use the same prompt / completion suffix as the original model.", nameof(fineTune));
            }

            var fineTuneResponse = await _fineTunesClient.GetFineTuneAsync(fineTune.FineTuneId, cancellationToken);
            if (fineTuneResponse.TrainingFiles.Count == 0)
            {
                throw new ArgumentException($"Finetune '{fineTuneResponse.Id}' does not have any associated training files. Cannot create variant.", nameof(fineTune));
            }
            var trainingFileId = fineTuneResponse.TrainingFiles[0].Id;


            var validationData = settings.GetValidationData();
            string? validationFileId = null;
            if (validationData.Count > 0)
            {
                var lookup = await GetOpenAILibManagedFineTuneFiles(cancellationToken);
                var processedValidationData = FineTuneTrainingDataProcessor.ProcessFineTuneData(validationData, fineTune.PromptSuffix, fineTune.CompletionSuffix);
                validationFileId = await EnsureTrainingDataUploadedAsync(processedValidationData, lookup, cancellationToken);
            }

            var request = settings.ToRequest(trainingFileId, validationFileId);
            var fineTuneVariantId = await _fineTunesClient.CreateFineTuneAsync(request, cancellationToken);

            return new FineTuneInfo(fineTuneVariantId, fineTune.PromptSuffix, fineTune.CompletionSuffix);
        }

        public async Task<string?> GetFineTuneModelNameAsync(FineTuneInfo fineTune, CancellationToken cancellationToken = default)
        {
            // Standard GetOrAdd method cannot be used with async responses. Code is a little clunkier
            // but the behavior is the same.
            if (s_fineTuneIdToModelNameMap.ContainsKey(fineTune.FineTuneId))
            {
                return s_fineTuneIdToModelNameMap[fineTune.FineTuneId];
            }

            var fineTuneResponse = await _fineTunesClient.GetFineTuneAsync(fineTune.FineTuneId, cancellationToken);
            var fineTunedModelName = fineTuneResponse.FineTunedModel;
            if (string.IsNullOrEmpty(fineTunedModelName))
            {
                return null;
            }
            s_fineTuneIdToModelNameMap.TryAdd(fineTune.FineTuneId, fineTunedModelName);
            return fineTunedModelName;
        }

        public async Task DeleteFineTuneAsync(FineTuneInfo fineTune, bool deleteTrainingFiles, CancellationToken cancellationToken = default)
        {
            var fineTuneResponse = await _fineTunesClient.GetFineTuneAsync(fineTune.FineTuneId, cancellationToken);

            if (deleteTrainingFiles)
            {
                var trainingFiles = fineTuneResponse.TrainingFiles.Select(file => file.Id);
                var validationFiles = fineTuneResponse.ValidationFiles.Select(file => file.Id);
                var allFileIds = trainingFiles.Concat(validationFiles).ToHashSet();
                foreach (var fileId in allFileIds)
                {
                    await _filesClient.DeleteAsync(fileId, cancellationToken);
                }
            }

            if (string.IsNullOrWhiteSpace(fineTuneResponse.FineTunedModel))
            {
                // No fine tuned model, nothing to delete
                return;
            }
            else
            {
                await _modelsClient.DeleteAsync(fineTuneResponse.FineTunedModel);
            }
        }

        /// <summary>
        /// Gets all training and validation files created by the OpenAILib
        /// </summary>
        public async Task<HashSet<string>> GetFineTuneTrainingFilesAsync(CancellationToken cancellationToken = default)
        {
            var files = await GetOpenAILibManagedFineTuneFiles(cancellationToken);
            return files.Select(file => file.Value.Id).ToHashSet();

        }

        private async Task<FineTuneInfo> CreateFineTuneAsync(List<FineTunePair> trainingData, FineTuneSpec01 settings, CancellationToken cancellationToken = default)
        {
            var promptSuffix = settings.GetPromptSuffix();
            var completionSuffix = settings.GetCompletionSuffix();

            var lookup = await GetOpenAILibManagedFineTuneFiles(cancellationToken);
            var processedTrainingData = FineTuneTrainingDataProcessor.ProcessFineTuneData(trainingData, promptSuffix, completionSuffix);
            var trainingFileId = await EnsureTrainingDataUploadedAsync(processedTrainingData, lookup, cancellationToken);

            string? validationFileId = null;
            var validationData = settings.GetValidationData();
            if (validationData.Count > 0)
            {
                var processedValidationData = FineTuneTrainingDataProcessor.ProcessFineTuneData(validationData, promptSuffix, completionSuffix);
                validationFileId = await EnsureTrainingDataUploadedAsync(processedValidationData, lookup, cancellationToken);
            }

            var fineTuneRequest = settings.ToRequest(trainingFileId, validationFileId);
            var fineTuneId = await _fineTunesClient.CreateFineTuneAsync(fineTuneRequest, cancellationToken);
            return new FineTuneInfo(fineTuneId, promptSuffix, completionSuffix);
        }

        private async Task<string> EnsureTrainingDataUploadedAsync(List<FineTunePair> trainingData, Dictionary<string, FileResponse> lookup, CancellationToken cancellationToken)
        {
            // provide allocation hint - assume at least 10 bytes per pair
            var memoryStream = new MemoryStream(trainingData.Count * 10);
            JsonLinesSerializer.Serialize(memoryStream, trainingData);
            memoryStream.Position = 0;
            var fileName = GetFileName(memoryStream.GetBuffer());
            string fileId;
            if (lookup.TryGetValue(fileName, out var fileInfo))
            {
                fileId = fileInfo.Id;
            }
            else
            {
                fileId = await _filesClient.UploadStreamAsync(memoryStream, FilePurpose.FineTune, fileName, cancellationToken);
            }
            return fileId;
        }

        private async Task<Dictionary<string, FileResponse>> GetOpenAILibManagedFineTuneFiles(CancellationToken cancellationToken)
        {
            // creates a lookup for all OpenAILib (i.e. this library) managed files
            var allFiles = await _filesClient.GetFilesAsync(cancellationToken);
            var lookup = allFiles.Where(file => file.Filename.StartsWith(OpenAILibFineTuneFileMarker))
                .ToDictionary(file => file.Filename, file => file);
            return lookup;
        }

        private static string GetFileName(ReadOnlySpan<byte> bytes)
        {
            var hashBytes = SHA1.HashData(bytes);
            return $"{OpenAILibFineTuneFileMarker}.{Convert.ToHexString(hashBytes).ToLower()}.jsonl";
        }
    }

    internal interface IFineTuneModelNameProvider
    {
        Task<string?> GetFineTuneModelNameAsync(FineTuneInfo fineTune, CancellationToken cancellationToken = default);
    }
}
