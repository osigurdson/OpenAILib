// Copyright (c) 2023 Owen Sigurdson
// MIT License

namespace OpenAILib.FineTuning
{
    /// <summary>
    /// Represents an event that occurred during fine-tuning of a machine learning model.
    /// </summary>
    public class FineTuneEvent
    {
        /// <summary>
        /// Gets the message associated with the event.
        /// </summary>
        public string Message { get; }

        /// <summary>
        /// Gets the timestamp of the event in UTC time
        /// </summary>
        public DateTime TimeStampUtc { get; }

        /// <summary>
        /// Gets the level of the event
        /// </summary>
        public string Level { get; }

        /// <summary>
        /// Represents an event that occurred during fine-tuning of a model
        /// </summary>
        public FineTuneEvent(DateTime timeStamp, string message, string level)
        {
            TimeStampUtc = timeStamp;
            Message = message;
            Level = level;
        }

        internal static FineTuneEvent FromFineTuneResponse(FineTuneEventResponse response)
        {
            var timeStampUtc = DateTimeOffset.FromUnixTimeSeconds(response.CreatedAt).UtcDateTime;
            return new FineTuneEvent(timeStampUtc, response.Message, response.Level);
        }
    }
}
