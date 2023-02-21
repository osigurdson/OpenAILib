// Copyright (c) 2023 Owen Sigurdson
// MIT License

using System.Text.Json.Serialization;

namespace OpenAILib.Embeddings
{
    internal class EmbeddingResponse
    {
        [JsonPropertyName("object")]
        public string? Object { get; set; }

        [JsonPropertyName("data")]
        public List<EmbeddingDetail>? Data { get; set; }

        [JsonPropertyName("model")]
        public string? Model { get; set; }

        [JsonPropertyName("usage")]
        public UsageResponse? Usage { get; set; }


        public class EmbeddingDetail
        {
            [JsonPropertyName("object")]
            public string? Object { get; set; }

            [JsonPropertyName("embedding")]
            public double[]? Embedding { get; set; }

            [JsonPropertyName("index")]
            public int Index { get; set; }
        }
    }
}
