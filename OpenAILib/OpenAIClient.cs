// MIT License

using OpenAILib.Completions;
using OpenAILib.Embeddings;
using System.Net.Http.Headers;

namespace OpenAILib
{
    public class OpenAIClient
    {
        private readonly HttpClient _httpClient;
        private readonly EmbeddingsClient _embeddingsClient;
        private readonly CompletionsClient _completionsClient;

        public OpenAIClient(string organizationId, string apiKey, string url = "https://api.openai.com/v1/")
        {
            var uri = new Uri(url);
            _httpClient = new HttpClient
            {
                BaseAddress = uri
            };

            _httpClient
                .DefaultRequestHeaders
                .Authorization = new AuthenticationHeaderValue("Bearer", apiKey);

            _httpClient
                .DefaultRequestHeaders
                .Add("OpenAI-Organization", organizationId);

            _embeddingsClient = new EmbeddingsClient(_httpClient);
            _completionsClient = new CompletionsClient(_httpClient);
        }

        public async Task<string> GetCompletionAsync(string prompt)
        {
            return await _completionsClient.GetCompletionAsync(prompt);
        }

        public async Task<double[]> GetEmbeddingAsync(string text)
        {
            return await _embeddingsClient.GetEmbeddingAsync(text);
        }
    }
}