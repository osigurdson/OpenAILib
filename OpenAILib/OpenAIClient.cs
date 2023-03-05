// Copyright (c) 2023 Owen Sigurdson
// MIT License

using OpenAILib.ChatCompletions;
using OpenAILib.Completions;
using OpenAILib.Embeddings;
using OpenAILib.Models;

namespace OpenAILib
{

    public class OpenAIClient
    {
        private readonly HttpClient _httpClient;
        private readonly EmbeddingsClient _embeddingsClient;
        private readonly ChatCompletionsClient _chatCompletionsClient;

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
            _httpClient = OpenAIHttpClient.CreateHttpClient(args);
            _embeddingsClient = new EmbeddingsClient(_httpClient, args.ResponseCache);
            _chatCompletionsClient = new ChatCompletionsClient(_httpClient, args.ResponseCache);
        }

        public async Task<string> GetCompletionAsync(string prompt)
        {
            var chatCompletionRequest = new ChatCompletionRequest(StockModels.Gpt35Turbo, new List<ChatMessageRequest>
            {
                new ChatMessageRequest(ChatRole.System, "You are a helpful assistant"),
                new ChatMessageRequest(ChatRole.User, prompt)
            });

            var response = await _chatCompletionsClient.CreateChatCompletionAsync(chatCompletionRequest);
            if (response.Choices.Count == 0)
            {
                return "";
            }
            return response.Choices[0].Message.Content;
        }

        public async Task<double[]> GetEmbeddingAsync(string text)
        {
            return await _embeddingsClient.GetEmbeddingAsync(text);
        }
    }
}