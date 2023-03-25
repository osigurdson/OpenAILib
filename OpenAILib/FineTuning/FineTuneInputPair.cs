// Copyright (c) 2023 Owen Sigurdson
// MIT License

using System.Text.Json.Serialization;

namespace OpenAILib.FineTuning
{
    /// <summary>
    /// Represents a pair of a prompt and a completion used for fine-tuning
    /// </summary>
    public class FineTunePair
    {
        /// <summary>
        /// Gets the prompt used for fine-tuning.
        /// </summary>
        [JsonPropertyName("prompt")]
        public string Prompt { get; init; }

        /// <summary>
        /// Gets the completion generated during fine-tuning.
        /// </summary>
        [JsonPropertyName("completion")]
        public string Completion { get; init; }

        /// <summary>
        /// Initializes a new instance of the <see cref="FineTunePair"/> class with the specified prompt and completion.
        /// </summary>
        /// <param name="prompt">The prompt used for fine-tuning.</param>
        /// <param name="completion">The completion / ideal response used for fine-tuning.</param>
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
