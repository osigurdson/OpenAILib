
using OpenAILib.ResponseCaching;

namespace OpenAILib.ChatCompletions
{
    internal class ChatCompletionsClient
    {
        private const string ChatCompletionsEndpointName = "chat/completions";
        private readonly HttpClient _httpClient;
        private readonly IResponseCache _responseCache;

        public ChatCompletionsClient(HttpClient httpClient, IResponseCache responseCache)
        {
            _httpClient = httpClient;
            this._responseCache = responseCache;
        }

        public async Task<ChatCompletionResponse> CreateChatCompletionAsync(ChatCompletionRequest request)
        {
            return await _httpClient.OpenAIPostAsync<ChatCompletionRequest, ChatCompletionResponse>(ChatCompletionsEndpointName, request, _responseCache);
        }
    }
}
