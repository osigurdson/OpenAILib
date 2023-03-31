
using OpenAILib.HttpHandling;

namespace OpenAILib.ChatCompletions
{
    internal class ChatCompletionsClient
    {
        private const string ChatCompletionsEndPointName = "chat/completions";
        private readonly OpenAIHttpClient _httpClient;

        public ChatCompletionsClient(OpenAIHttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<ChatCompletionResponse> CreateChatCompletionAsync(ChatCompletionRequest request, CancellationToken cancellationToken = default)
        {
            var (response, _) =  await _httpClient.PostAsync<ChatCompletionRequest, ChatCompletionResponse>(
                originalRequestUri:  ChatCompletionsEndPointName, 
                request: request, 
                cacheResponses: true,
                cancellationToken: cancellationToken);

            return response;
        }
    }
}
