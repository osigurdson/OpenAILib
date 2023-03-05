// Copyright (c) 2023 Owen Sigurdson
// MIT License

using System.Text.Json.Serialization;

namespace OpenAILib.Completions
{
    internal class CompletionChoiceResponse
    {
        [JsonPropertyName("text")]
        public string Text { get;}

        [JsonPropertyName("index")]
        public int Index { get;}

        [JsonPropertyName("logprobs")]
        public Logprobs? Logprobs { get;}

        [JsonPropertyName("finish_reason")]
        public string FinishReason { get;}

        [JsonConstructor()]
        public CompletionChoiceResponse(string text, int index, Logprobs logprobs, string finishReason)
        {
            Text = text;
            Index = index;
            Logprobs = logprobs;
            FinishReason = finishReason;
        }
    }
}
