// Copyright (c) 2023 Owen Sigurdson
// MIT License

using System.Text.Json.Serialization;

namespace OpenAILib.Models
{
    internal class ModelListResponse
    {
        [JsonPropertyName("object")]
        public string Object { get; }

        [JsonPropertyName("data")]
        public List<ModelResponse> Data { get; }

        [JsonConstructor]
        public ModelListResponse(string @object, List<ModelResponse> data)
        {
            Data = data;
            Object = @object;
        }
    }
}
