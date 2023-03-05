// Copyright (c) 2023 Owen Sigurdson
// MIT License

using System.Text.Json.Serialization;

namespace OpenAILib.ChatCompletions
{
    internal class ChatMessageRequest
    {
        [JsonPropertyName("role")]
        public ChatRole Role { get; }

        [JsonPropertyName("content")]
        public string Content { get; }

        [JsonConstructor]
        public ChatMessageRequest(ChatRole role, string content)
        {
            Role = role;
            Content = content;
        }
    }
}
