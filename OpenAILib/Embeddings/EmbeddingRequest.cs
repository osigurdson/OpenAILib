// MIT License

using System.Text.Json.Serialization;

namespace OpenAILib.Embeddings
{
    internal class EmbeddingRequest
    {
        [JsonPropertyName("model")]
        public string? Model { get; set; }

        [JsonPropertyName("input")]
        public string? Input { get; set; }
    }
}
