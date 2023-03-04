// Copyright (c) 2023 Owen Sigurdson
// MIT License

using System.Text.Json.Serialization;

namespace OpenAILib.FineTuning
{
    internal class FineTuneEventResponse
    {
        [JsonPropertyName("object")]
        public string? Object { get; set; }

        [JsonPropertyName("created_at")]
        public long CreatedAt { get; set; }

        [JsonPropertyName("level")]
        public string? Level { get; set; }

        [JsonPropertyName("message")]
        public string? Message { get; set; }
    }
}
