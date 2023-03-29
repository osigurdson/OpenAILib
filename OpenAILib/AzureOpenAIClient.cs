// Copyright (c) 2023 Owen Sigurdson
// MIT License

using OpenAILib.ChatCompletions;
using OpenAILib.Completions;
using OpenAILib.Embeddings;
using OpenAILib.FineTuning;
using OpenAILib.HttpHandling;

namespace OpenAILib
{
    public class AzureOpenAIClient
    {
        private readonly HttpClient _httpClient;
        private readonly EmbeddingsClient _embeddingsClient;
        private readonly ChatCompletionsClient _chatCompletionsClient;
        private readonly CompletionsClient _completionsClient;
        private readonly FineTunesClient _fineTuningClient;

        /// <summary>
        /// Initializes a new instance of <see cref="AzureOpenAIClient"/> with specified
        /// api key credentials
        /// </summary>
        /// <param name="apiKey">Azure OpenAI api key</param>
        /// <param name="url">Base url of the Azure OpenAI API service</param>
        public AzureOpenAIClient(string apiKey, string resourceName, string deploymentId, string apiVersion = "2022-12-01", string url = AzureOpenAIClientArgs.AzureOpenAIUrl)
            : this(
            new AzureOpenAIClientArgs(apiKey, resourceName, deploymentId, apiVersion)
            {
                Url = url
            })
        {
        }

        public AzureOpenAIClient(AzureOpenAIClientArgs args)
        {
            _httpClient = AzureOpenAIHttpClient.CreateHttpClient(args);
            _embeddingsClient = new EmbeddingsClient(_httpClient, args.ResponseCache);
            _completionsClient = new CompletionsClient(_httpClient, args.ResponseCache);
            _chatCompletionsClient = new ChatCompletionsClient(
                _httpClient,
                args.ResponseCache);
            _fineTuningClient = new FineTunesClient(_httpClient);
        }

        /// <summary>
        /// Gets a completion for the specified <paramref name="prompt"/> using the best available
        /// model and defaults
        /// </summary>
        /// <param name="prompt">The text displayed as a prompt to suggest the completion</param>
        /// <returns>A <see cref="string"/> representing the completion</returns>
        /// <remarks>
        /// Currently using the 'gpt-35-turbo' (ChatGPT) model prefixed with a recommended System
        /// role message
        /// </remarks>
        public async Task<string> GetCompletionAsync(string prompt)
        {
            var sequence = new List<ChatMessage>
            {
                new ChatMessage(ChatRole.System, "You are a helpful assistant"),
                new ChatMessage(ChatRole.User, prompt)
            };
            return await GetChatCompletionAsync(sequence);
        }

        /// <summary>
        /// Gets a completion for the specified <paramref name="prompt"/> using the parameters
        /// specified in <paramref name="spec"/>
        /// </summary>
        /// <param name="prompt">The prompt to generate completions for</param>
        /// <param name="spec">
        /// Used to configure the completion request (set the model, max tokens, etc). OpenAI
        /// current  defaults used for any unspecified parameter. See remarks for discussion of
        /// model used
        /// </param>
        /// <returns>A <see cref="string"/> representing the completion</returns>
        /// <remarks>
        /// Defaults to model recommended by Azure OpenAI for most used cases if not specified (currently
        /// 'text-davinci-003')
        /// </remarks>
        public async Task<string> GetCompletionAsync(string prompt, Action<ICompletionSpecV01> spec)
        {
            var completionSpecOptions = new CompletionSpecV01();

            // apply any specifications
            spec(completionSpecOptions);

            var completionRequest = await completionSpecOptions.ToCompletionRequestAsync(
                prompt,
                _fineTuningClient);
            var response = await _completionsClient.GetCompletionAsync(completionRequest);
            return response.Choices[0].Text.Trim();
        }

        public async Task<string> GetChatCompletionAsync(IEnumerable<ChatMessage> sequence)
        {
            return await GetChatCompletionAsync(sequence, new ChatCompletionSpecV01());
        }

        public async Task<string> GetChatCompletionAsync(
            IEnumerable<ChatMessage> sequence,
            Action<IChatCompletionSpecV01> spec)
        {
            var chatCompletionSpec = new ChatCompletionSpecV01();
            spec(chatCompletionSpec);
            return await GetChatCompletionAsync(sequence, chatCompletionSpec);
        }

        private async Task<string> GetChatCompletionAsync(
            IEnumerable<ChatMessage> sequence,
            ChatCompletionSpecV01 chatCompletionSpec)
        {
            var chatCompletionRequest = chatCompletionSpec.ToRequest(sequence);
            var response = await _chatCompletionsClient
                .CreateChatCompletionAsync(chatCompletionRequest);
            if (response.Choices.Count == 0)
            {
                return string.Empty;
            }
            return response.Choices[0].Message.Content;
        }

        /// <summary>
        /// Gets an embedding vector for the specified <paramref name="text"/>
        /// </summary>
        /// <param name="text">Text to obtain the embedding vector for</param>
        /// <returns>A 1536 dimensional vector representing the embedding</returns>
        public async Task<double[]> GetEmbeddingAsync(string text)
        {
            return await _embeddingsClient.GetEmbeddingAsync(text);
        }

        public IFineTunesClient FineTuning => _fineTuningClient;
    }
}