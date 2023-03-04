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

        /// <summary>
        /// Returns a text completion for the given prompt using OpenAI's default model and settings.
        /// </summary>
        /// <param name="prompt">The text prompt to generate a completion for.</param>
        /// <returns>The generated completion text as a string.</returns>
        /// <exception cref="OpenAIException">Thrown if the completion request fails or the response cannot be deserialized.</exception>
        public async Task<string> GetCompletionAsync(string prompt)
        {
            var request = new CompletionRequest
            {
                Model = "text-davinci-003",
                Prompt = prompt,
                Temperature = 0.7,
                MaxTokens = 2048,
            };

            var response = await GetCompletionAsync(request);
            // TODO - refactor CompletionResponse to be non nullable
            var responseText = response?.Choices?[0]?.Text;
            if (responseText == null)
            {
                throw new OpenAIException("Failed to deserialize completion response");
            }
            return responseText;

        }
    }
}
