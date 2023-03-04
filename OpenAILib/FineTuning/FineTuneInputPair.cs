// Copyright (c) 2023 Owen Sigurdson
// MIT License

using System.Text.Json.Serialization;

namespace OpenAILib.FineTuning
{
    internal class FineTunePair
    {
        [JsonPropertyName("prompt")]
        public string Prompt { get; init; }

        [JsonPropertyName("completion")]
        public string Completion { get; init; }

        [JsonConstructor]
        public FineTunePair(string prompt, string completion)
        {
            Prompt = prompt;
            Completion = completion;
        }

        public override bool Equals(object? obj)
        {
            if (obj == null || obj is not FineTunePair)
            {
                return false;
            }

            var other = (FineTunePair)obj;

            return string.Equals(Prompt, other.Prompt) && string.Equals(Completion, other.Completion);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hash = 17;
                hash = hash * 23 + (Prompt?.GetHashCode() ?? 0);
                hash = hash * 23 + (Completion?.GetHashCode() ?? 0);
                return hash;
            }
        }
    }
}
