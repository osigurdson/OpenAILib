// Copyright (c) 2023 Owen Sigurdson
// MIT License

using OpenAILib.ChatCompletions;

namespace OpenAILib.Tests.ChatCompletions
{
    [TestClass]
    public class ChatCompletionsClientTest
    {
        [TestMethod]
        public async Task TestCreateChatCompletionAsync()
        {
            var httpClient = TestHttpClient.CreateHttpClient();
            var chatClient = new ChatCompletionsClient(httpClient);

            // example as discussed in the open ai API documentation
            var chatRequest = new ChatCompletionRequest("gpt-3.5-turbo", new List<ChatMessageRequest>
            {
                new ChatMessageRequest(ChatRole.System, "You are a helpful assistant"),
                new ChatMessageRequest(ChatRole.User, "Who won the world series in 2020?"),
                new ChatMessageRequest(ChatRole.Assistant, "The Los Angeles Dodgers won the World Series in 2020."),
                new ChatMessageRequest(ChatRole.User, "Where was it played?")
            });

            var response = await chatClient.CreateChatCompletionAsync(chatRequest);

            Assert.AreEqual(1, response.Choices.Count);
            StringAssert.Contains(response.Choices[0].Message.Content, "Arlington");
            Assert.AreEqual(response.Choices[0].Message.Role, ChatRole.Assistant);
        }
    }
}
