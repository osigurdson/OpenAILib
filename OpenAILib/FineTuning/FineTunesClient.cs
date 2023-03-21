// Copyright (c) 2023 Owen Sigurdson
// MIT License

using System.Net.Http.Json;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Threading.Channels;
using OpenAILib.HttpHandling;

namespace OpenAILib.FineTuning
{
    internal class FineTunesClient
    {
        private const string FineTunesEndpointName = "fine-tunes";

        private readonly HttpClient _httpClient;

        public FineTunesClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<string> CreateFineTuneAsync(FineTuneRequest request, CancellationToken cancellationToken = default)
        {
            var content = JsonContent.Create(request);
            var httpResponse = await _httpClient.PostAsync(FineTunesEndpointName, content, cancellationToken);
            httpResponse.EnsureSuccessStatusCode();
            var responseStream = await httpResponse.Content.ReadAsStreamAsync(cancellationToken);
            var response = JsonSerializer.Deserialize<FineTuneResponse>(responseStream);

            if (response == null || response.Id == null)
            {
                throw new OpenAIException("Failed to deserialize fine tune response");
            }

            return response.Id;
        }

        public async Task<IReadOnlyList<FineTuneResponse>> GetFineTunesAsync(CancellationToken cancellationToken = default)
        {
            var httpResponse = await _httpClient.GetAsync(FineTunesEndpointName, cancellationToken);
            httpResponse.EnsureSuccessStatusCode();
            var responseStream = await httpResponse.Content.ReadAsStreamAsync(cancellationToken);
            var response = JsonSerializer.Deserialize<FineTuneListResponse>(responseStream);
            if (response == null || response.Data == null)
            {

                throw new OpenAIException("Failed to deserialize fine tune responses ");
            }
            return response.Data;
        }

        public async Task<FineTuneResponse> GetFineTuneAsync(string fineTuneId, CancellationToken cancellationToken = default)
        {
            var httpResponse = await _httpClient.GetAsync($"{FineTunesEndpointName}/{fineTuneId}", cancellationToken);
            httpResponse.EnsureSuccessStatusCode();
            var response = await httpResponse.Content.ReadFromJsonAsync<FineTuneResponse>();
            if (response == null)
            {
                throw new OpenAIException($"Failed to deserialize fine tune '{fineTuneId}'.");
            }
            return response;
        }

        public async IAsyncEnumerable<FineTuneEventResponse> GetEventStreamAsync(string fineTuneId, [EnumeratorCancellation] CancellationToken cancellationToken = default)
        {
            // If the ServerSentEventSubscriber has to re-connect (extremely likely), all events will be returned again. For
            // this reason, a hashset of all previously observed events is maintained so consumers do not have to deal
            // with filtering events manually
            var existingMessages = new HashSet<string>();
            var url = $"{FineTunesEndpointName}/{fineTuneId}/events?stream=true";
            await using var sseProcessor = new ServerSentEventsSubscriber(_httpClient, url, cancellationToken);
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
