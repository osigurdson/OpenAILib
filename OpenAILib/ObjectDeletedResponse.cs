// Copyright (c) 2023 Owen Sigurdson
// MIT License

using System.Text.Json.Serialization;


namespace OpenAILib
{
    internal class ObjectDeletedResponse
    {
        [JsonPropertyName("id")]
        public string Id { get; init; }

        [JsonPropertyName("object")]
        public string Object { get; init; }

        [JsonPropertyName("deleted")]
        public bool Deleted { get; init; }

        public ObjectDeletedResponse(string id, string @object, bool deleted)
        {
            Id = id;
            Object = @object;
            Deleted = deleted;
        }
    }
}
