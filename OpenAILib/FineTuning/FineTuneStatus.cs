// Copyright (c) 2023 Owen Sigurdson
// MIT License

namespace OpenAILib.FineTuning
{
    /// <summary>
    /// Represents the status of a fine-tuning operation for a machine learning model.
    /// </summary>
    public enum FineTuneStatus
    {
        /// <summary>
        /// The fine-tune is not yet ready.
        /// </summary>
        NotReady = 0,

        /// <summary>
        /// The fine-tuning operation succeeded.
        /// </summary>
        Succeeded = 100,

        /// <summary>
        /// The fine-tuning operation failed.
        /// </summary>
        Failed = 101
    }
}
