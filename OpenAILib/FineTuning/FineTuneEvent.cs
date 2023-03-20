// Copyright (c) 2023 Owen Sigurdson
// MIT License

namespace OpenAILib.FineTuning
{
    internal class FineTuneEvent
    {
        public FineTuneEvent(DateTime timeStamp, string message, string level)
        {
            TimeStamp = timeStamp;
            Message = message;
            Level = level;
        }

        internal static FineTuneEvent FromFineTuneResponse(FineTuneEventResponse response)
        {
            var timeStamp = DateTimeOffset.FromUnixTimeSeconds(response.CreatedAt).UtcDateTime;
            return new FineTuneEvent(timeStamp, response.Message, response.Level);
        }

        public DateTime TimeStamp { get; }
        public string Message { get; }
        public string Level { get; }
    }
}
