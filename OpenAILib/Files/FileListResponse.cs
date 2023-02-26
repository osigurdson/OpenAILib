// Copyright (c) 2023 Owen Sigurdson
// MIT License

using System.Text.Json.Serialization;

namespace OpenAILib.Files
{
    internal class FileListResponse
    {
        [JsonPropertyName("data")]
        public List<FileResponse>? Data { get; set; }

        [JsonPropertyName("object")]
        public string? Object { get; set; }
    }
}
