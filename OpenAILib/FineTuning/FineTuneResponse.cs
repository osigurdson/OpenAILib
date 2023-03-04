// Copyright (c) 2023 Owen Sigurdson
// MIT License

using OpenAILib.Files;
using System.Text.Json.Serialization;

namespace OpenAILib.FineTuning
{
    internal class FineTuneResponse
    {
        [JsonPropertyName("id")]
        public string Id { get; init; }

        [JsonPropertyName("object")]
        public string Object { get; init; }

        [JsonPropertyName("model")]
        public string Model { get; init; }

        [JsonPropertyName("created_at")]
        public long CreatedAt { get; init; }

        [JsonPropertyName("events")]
        public List<FineTuneEventResponse> Events { get; init; }

        [JsonPropertyName("fine_tuned_model")]
        public string FineTunedModel { get; init; }

        [JsonPropertyName("hyperparams")]
        public HyperParams Hyperparams { get; init; }

        [JsonPropertyName("organization_id")]
        public string OrganizationId { get; init; }

        [JsonPropertyName("result_files")]
        public List<FileResponse> ResultFiles { get; init; }

        [JsonPropertyName("status")]
        public string Status { get; init; }

        [JsonPropertyName("training_files")]
        public List<FileResponse> TrainingFiles { get; init; }

        [JsonPropertyName("validation_files")]
        public List<FileResponse> ValidationFiles { get; init; }

        [JsonPropertyName("updated_at")]
        public long UpdatedAt { get; init; }

        [JsonConstructor]
        public FineTuneResponse(string id, string @object, string model, long createdAt,
            List<FineTuneEventResponse> events, string fineTunedModel, HyperParams hyperparams,
            string organizationId, List<FileResponse> resultFiles, string status, List<FileResponse> trainingFiles,
            List<FileResponse> validationFiles, long updatedAt)
        {
            Id = id;
            Object = @object;
            Model = model;
            CreatedAt = createdAt;
            Events = events;
            FineTunedModel = fineTunedModel;
            Hyperparams = hyperparams;
            OrganizationId = organizationId;
            ResultFiles = resultFiles;
            Status = status;
            TrainingFiles = trainingFiles;
            ValidationFiles = validationFiles;
            UpdatedAt = updatedAt;
        }

        internal class HyperParams
        {
            [JsonPropertyName("batch_size")]
            public int? BatchSize { get; init; }

            [JsonPropertyName("learning_rate_multiplier")]
            public double? LearningRateMultiplier { get; init; }

            [JsonPropertyName("n_epochs")]
            public int NEpochs { get; init; }

            [JsonPropertyName("prompt_loss_weight")]
            public double PromptLossWeight { get; init; }

            [JsonConstructor]
            public HyperParams(int? batchSize, double? learningRateMultiplier, int nEpochs, double promptLossWeight)
            {
                BatchSize = batchSize;
                LearningRateMultiplier = learningRateMultiplier;
                NEpochs = nEpochs;
                PromptLossWeight = promptLossWeight;
            }
        }
    }
}
