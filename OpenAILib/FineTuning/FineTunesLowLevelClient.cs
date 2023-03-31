// Copyright (c) 2023 Owen Sigurdson
// MIT License

using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Threading.Channels;
using OpenAILib.HttpHandling;

namespace OpenAILib.FineTuning
{
    internal class FineTunesLowLevelClient
    {
        private const string FineTunesEndpointName = "fine-tunes";

        private readonly OpenAIHttpClient _httpClient;

        public FineTunesLowLevelClient(OpenAIHttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<string> CreateFineTuneAsync(FineTuneRequest request, CancellationToken cancellationToken = default)
        {
            var (response, _) = await _httpClient.PostAsync<FineTuneRequest, FineTuneResponse>(
                originalRequestUri: FineTunesEndpointName,
                request: request,
                cacheResponses: false,
                cancellationToken: cancellationToken);

            return response.Id;
        }

        public async Task<IReadOnlyList<FineTuneResponse>> GetFineTunesAsync(CancellationToken cancellationToken = default)
        {
            var response = await _httpClient.GetAsync<FineTuneListResponse>(
                originalRequestUri: FineTunesEndpointName,
                cancellationToken: cancellationToken);

            return response.Data;
        }

        public async Task<FineTuneResponse> GetFineTuneAsync(string fineTuneId, CancellationToken cancellationToken = default)
        {
            var response = await _httpClient.GetAsync<FineTuneResponse>(
                originalRequestUri: $"{FineTunesEndpointName}/{fineTuneId}",
                cancellationToken: cancellationToken);

            return response;
        }

        public async IAsyncEnumerable<FineTuneEventResponse> GetEventStreamAsync(string fineTuneId, [EnumeratorCancellation] CancellationToken cancellationToken = default)
        {
            // If the ServerSentEventSubscriber has to re-connect (extremely likely), all events will be returned again. For
            // this reason, a hashset of all previously observed events is maintained so consumers do not have to deal
            // with filtering events manually
            var existingMessages = new HashSet<string>();

            // The server sent events subscriber requires a string based url and access to the underlying HttpClient
            // The uri is manually tranformed for this reason
            var originalRequestUri = $"{FineTunesEndpointName}/{fineTuneId}/events?stream=true";
            var transformedUri = _httpClient.GetTransformedUri(originalRequestUri).ToString();
            var rawHttpClient = _httpClient.GetHttpClient();

            await using var sseProcessor = new ServerSentEventsSubscriber(rawHttpClient, transformedUri, cancellationToken);
            var channel = Channel.CreateUnbounded<FineTuneEventResponse>();
            sseProcessor.NewPayloadMessage += (_, ssePayloadText) =>
            {
                if (!existingMessages.Add(ssePayloadText))
                {
                    return;
                }

                if (ssePayloadText == ServerSentEventsSubscriber.OpenAISseTerminationMessage)
                {
                    channel.Writer.Complete();
                    return;
                }

                var eventResponse = JsonSerializer.Deserialize<FineTuneEventResponse>(ssePayloadText);
                if (eventResponse == null)
                {
                    return;
                }
                
                channel.Writer.WriteAsync(eventResponse, cancellationToken);
            };

            await foreach (var fineTuneResponse in channel.Reader.ReadAllAsync())
            {
                yield return fineTuneResponse;
            }
        }
    }
}
