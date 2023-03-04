// Copyright (c) 2023 Owen Sigurdson
// MIT License

using System.Text.Json.Serialization;

namespace OpenAILib.Files
{
    internal record FileResponse
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("object")]
        public string Object { get; set; }

        [JsonPropertyName("bytes")]
        public int Bytes { get; set; }

        [JsonPropertyName("created_at")]
        public long CreatedAt { get; set; }

        [JsonPropertyName("filename")]
        public string Filename { get; set; }

        [JsonPropertyName("purpose")]
        public string Purpose { get; set; }

        [JsonConstructor]
        public FileResponse(
            string id,
            string @object,
            int bytes,
            long createdAt,
            string fileName,
            string purpose)
        {
            Id = id;
            Object = @object;
            Bytes = bytes;
            CreatedAt = createdAt;
            Filename = fileName;
            Purpose = purpose;
        }
    }

}
