// Copyright (c) 2023 Owen Sigurdson
// MIT License

using OpenAILib.ChatCompletions;
using System.Text.Json;

namespace OpenAILib.Tests.ChatCompletions
{
    [TestClass]
    public class ChatCompletionResponseTest
    {
        [TestMethod]
        public void TestSerialization()
        {
            // arrange
            const string SampleResponse = @"
            {
                ""id"": ""chatcmpl-IDXYZ"",
                ""object"": ""chat.completion"",
                ""created"": 100,
                ""model"": ""gpt-3.5-turbo-0301"",
                ""usage"": {
                    ""prompt_tokens"": 10,
                    ""completion_tokens"": 20,
                    ""total_tokens"": 30
                },      
                ""choices"": 
                [
                    {
                        ""message"": {
                        ""role"": ""assistant"",
                        ""content"": ""The 2020 World Series was played at Globe Life Field in Arlington, Texas.""
                    },
                    ""finish_reason"": ""stop"",
                    ""index"": 0
                    }
                ]
            }";

            // act
            var initialResponse = JsonSerializer.Deserialize<ChatCompletionResponse>(SampleResponse);
            var serialized = JsonSerializer.Serialize(initialResponse);
            var response = JsonSerializer.Deserialize<ChatCompletionResponse>(serialized);

            // assert

            // smoke test a few items - declaritively defined
            Assert.IsNotNull(response);
            Assert.AreEqual("chatcmpl-IDXYZ", response.Id);
            Assert.AreEqual(100, response.Created);

            Assert.AreEqual(1, response.Choices.Count);
            Assert.AreEqual("The 2020 World Series was played at Globe Life Field in Arlington, Texas.", response.Choices[0].Message.Content);
            Assert.AreEqual(0, response.Choices[0].Index);

            Assert.AreEqual(30, response.Usage.TotalTokens);

            Assert.AreEqual(ChatRole.Assistant, response.Choices[0].Message.Role);
        }
    }
}
