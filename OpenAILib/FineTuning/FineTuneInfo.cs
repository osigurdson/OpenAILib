// Copyright (c) 2023 Owen Sigurdson
// MIT License

using System.Text.Json.Serialization;

namespace OpenAILib.FineTuning
{
    internal class FineTuneInfo
    {
        [JsonPropertyName("fineTuneId")]
        public string FineTuneId { get; }

        [JsonPropertyName("promptSuffix")]
        public string PromptSuffix { get; }

        [JsonPropertyName("completionSuffix")]
        public string CompletionSuffix { get; }

        [JsonConstructor]
        public FineTuneInfo(string fineTuneId, string promptSuffix, string completionSuffix)
        {
            FineTuneId = fineTuneId;
            PromptSuffix = promptSuffix;
            CompletionSuffix = completionSuffix;
        }
    }
}
