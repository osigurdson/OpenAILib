// Copyright (c) 2023 Owen Sigurdson
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

        /// <summary>
        /// Initializes a new instance of <see cref="OpenAIClient"/> with specified organization ID and api key credentials
        /// </summary>
        /// <param name="organizationId">OpenAI organization id</param>
        /// <param name="apiKey">OpenAI api key</param>
        /// <param name="url">Base url of the OpenAI API service</param>
        public OpenAIClient(string organizationId, string apiKey, string url = OpenAIClientArgs.OpenAIUrl) 
            : this(new OpenAIClientArgs(organizationId, apiKey)
            {
                Url = url
            })
        {
        }

        public OpenAIClient(OpenAIClientArgs args)
        {
            var uri = new Uri(args.Url);
            _httpClient = new HttpClient
            {
                BaseAddress = uri
            };

            _httpClient
                .DefaultRequestHeaders
                .Authorization = new AuthenticationHeaderValue("Bearer", args.ApiKey);

            _httpClient
                .DefaultRequestHeaders
                .Add("OpenAI-Organization", args.OrganizationId);

            _completionsClient = new CompletionsClient(_httpClient, args.ResponseCache);
            _embeddingsClient = new EmbeddingsClient(_httpClient, args.ResponseCache);
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