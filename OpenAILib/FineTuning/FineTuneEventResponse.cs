// Copyright (c) 2023 Owen Sigurdson
// MIT License

using System.Text.Json.Serialization;

namespace OpenAILib.FineTuning
{
    internal class FineTuneEventResponse
    {
        [JsonPropertyName("object")]
        public string Object { get; }

        [JsonPropertyName("created_at")]
        public long CreatedAt { get; }

        [JsonPropertyName("level")]
        public string Level { get; }

        [JsonPropertyName("message")]
        public string Message { get; }

        [JsonConstructor]
        public FineTuneEventResponse(string @object, long createdAt, string level, string message)
        {
            Object = @object;
            CreatedAt = createdAt;
            Level = level;
            Message = message;
        }
    }
}
