// Copyright (c) 2023 Owen Sigurdson
// MIT License

using OpenAILib.HttpHandling;

namespace OpenAILib.Completions
{
    internal class CompletionsClient
    {
        private const string CompletionsEndPointName = "completions";
        private readonly OpenAIHttpClient _httpClient;

        public CompletionsClient(OpenAIHttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<CompletionResponse> GetCompletionAsync(CompletionRequest request, CancellationToken cancellationToken = default)
        {
            var (completionResponse, responseText) = await _httpClient.PostAsync<CompletionRequest, CompletionResponse>(
                originalRequestUri: CompletionsEndPointName,
                request: request,
                cacheResponses: true,
                cancellationToken: cancellationToken
               );

            if (completionResponse.Choices.Count < 1)
            {
                throw new OpenAIException($"Failed to deserialize completion response '{responseText}'");
            }

            return completionResponse;
        }
    }
}
