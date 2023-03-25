// Copyright (c) 2023 Owen Sigurdson
// MIT License

using System.Text.Json.Serialization;

namespace OpenAILib.FineTuning
{
    /// <summary>
    /// Represents a fine tuned model
    /// </summary>
    public class FineTuneInfo
    {
        /// <summary>
        /// Gets the OpenAI ID of the fine-tuning operation.
        /// </summary>
        [JsonPropertyName("fineTuneId")]
        public string FineTuneId { get; }

        /// <summary>
        /// Gets the suffix of the prompt used for fine-tuning.
        /// </summary>
        [JsonPropertyName("promptSuffix")]
        public string PromptSuffix { get; }

        /// <summary>
        /// Gets the suffix of the completion generated during fine-tuning or to be used for generating completions.
        /// </summary>
        [JsonPropertyName("completionSuffix")]
        public string CompletionSuffix { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="FineTuneInfo"/> class with the specified fine-tune ID, prompt suffix, and completion suffix.
        /// </summary>
        /// <param name="fineTuneId">The ID of the fine tune</param>
        /// <param name="promptSuffix">The suffix of the prompt used for fine-tuning - automatically applied when creating completions using the model</param>
        /// <param name="completionSuffix">The suffix of the completion used for fine-tuning - automatically applied when creating completions using the model</param>
        [JsonConstructor]
        public FineTuneInfo(string fineTuneId, string promptSuffix, string completionSuffix)
        {
            FineTuneId = fineTuneId;
            PromptSuffix = promptSuffix;
            CompletionSuffix = completionSuffix;
        }
    }
}
