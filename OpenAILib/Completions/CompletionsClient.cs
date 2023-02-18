// MIT License

using System.Net.Http.Json;
using System.Text.Json;

namespace OpenAILib.Completions
{
    internal class CompletionsClient
    {
        private readonly HttpClient _httpClient;

        public CompletionsClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
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
                MaxTokens = 2048
            };

            var content = JsonContent.Create(request);
            var response = await _httpClient.PostAsync("completions", content);
            response.EnsureSuccessStatusCode();
            var responseText = await response.Content.ReadAsStringAsync();
            var completionResponse = JsonSerializer.Deserialize<CompletionResponse>(responseText);

            if (completionResponse == null || 
                completionResponse.Choices == null || 
                completionResponse.Choices.Count != 1)
            {
                throw new OpenAIException($"Failed to deserialize completion response '{responseText}'.");
            }

            var resultText = completionResponse.Choices[0].Text ?? string.Empty;
            return resultText;
        }
    }
}
