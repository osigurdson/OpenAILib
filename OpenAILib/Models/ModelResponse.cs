// Copyright (c) 2023 Owen Sigurdson
// MIT License

using System.Text.Json.Serialization;

namespace OpenAILib.Models
{
    internal class ModelResponse
    {
        [JsonPropertyName("id")]
        public string Id { get; }

        [JsonPropertyName("object")]
        public string Object { get; }

        [JsonPropertyName("owned_by")]
        public string OwnedBy { get; }

        [JsonConstructor]
        public ModelResponse(string id, string @object, string ownedBy)
        {
            Id = id;
            Object = @object;
            OwnedBy = ownedBy;
        }
    }
}
