// Copyright (c) 2023 Owen Sigurdson
// MIT License

using System.Text.Json.Serialization;

namespace OpenAILib.ChatCompletions
{
    internal class ChatCompletionChoice
    {
        [JsonPropertyName("message")]
        public ChatCompletionMessageResponse Message { get; }

        [JsonPropertyName("finish_reason")]
        public string FinishReason { get; }

        [JsonPropertyName("index")]
        public int Index { get;  }

        [JsonConstructor]
        public ChatCompletionChoice(ChatCompletionMessageResponse message, string finishReason, int index)
        {
            Message = message;
            FinishReason = finishReason;
            Index = index;
        }
    }
}
