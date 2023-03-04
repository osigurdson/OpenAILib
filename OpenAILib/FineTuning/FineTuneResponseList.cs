// Copyright (c) 2023 Owen Sigurdson
// MIT License

using System.Text.Json.Serialization;

namespace OpenAILib.FineTuning
{
    internal class FineTuneListResponse
    {
        [JsonPropertyName("object")]
        public string Object { get; init; }

        [JsonPropertyName("data")]
        public IReadOnlyList<FineTuneResponse> Data { get; init; }

        [JsonConstructor]
        public FineTuneListResponse(string @object, IReadOnlyList<FineTuneResponse> data)
        {
            Object = @object;
            Data = data;
        }
    }
}
