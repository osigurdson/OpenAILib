// MIT License

using System.Text.Json.Serialization;

namespace OpenAILib.Completions
{
    internal class CompletionResponse
    {
        [JsonPropertyName("id")]
        public string? Id { get; set; }

        [JsonPropertyName("object")]
        public string? Object { get; set; }

        [JsonPropertyName("created")]
        public long Created { get; set; }

        [JsonPropertyName("model")]
        public string? Model { get; set; }

        [JsonPropertyName("choices")]
        public List<CompletionChoiceResponse>? Choices { get; set; }

        [JsonPropertyName("usage")]
        public UsageResponse? Usage { get; set; }
    }
}
