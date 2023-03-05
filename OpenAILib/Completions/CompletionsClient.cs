// Copyright (c) 2023 Owen Sigurdson
// MIT License

using OpenAILib.ResponseCaching;
using System.Net.Http.Json;
using System.Text.Json;

namespace OpenAILib.Completions
{
    internal class CompletionsClient
    {
        private const string CompletionsEndpointName = "completions";
        private readonly HttpClient _httpClient;
        private readonly IResponseCache _responseCache;

        public CompletionsClient(HttpClient httpClient, IResponseCache responseCache)
        {
            _httpClient = httpClient;
            _responseCache = responseCache;
        }

        public async Task<CompletionResponse> GetCompletionAsync(CompletionRequest request)
        {
            var content = JsonContent.Create(request);

            var requestHash = RequestHashCalculator.CalculateHash(CompletionsEndpointName, await content.ReadAsStringAsync());
            if (!_responseCache.TryGetResponseAsync(requestHash, out var responseText))
            {
                var response = await _httpClient.PostAsync(CompletionsEndpointName, content);
                response.EnsureSuccessStatusCode();
                responseText = await response.Content.ReadAsStringAsync();
                _responseCache.PutResponse(requestHash, responseText);
            }

            if (string.IsNullOrEmpty(responseText))
            {
                throw new ArgumentException($"No result returned for completion request.");
            }

            var completionResponse = JsonSerializer.Deserialize<CompletionResponse>(responseText);
            if (completionResponse == null || completionResponse.Choices == null || completionResponse.Choices.Count < 1)
            {
                throw new OpenAIException("Failed to deserialize completion response");
            }

            return completionResponse;
        }
    }
}
