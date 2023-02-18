// MIT License

using System.Text.Json.Serialization;

namespace OpenAILib.Completions
{
    internal class Logprobs
    {
        [JsonPropertyName("tokens")]
        public List<string>? Tokens { get; set; }

        [JsonPropertyName("token_logprobs")]
        public List<double>? TokenLogprobs { get; set; }

        [JsonPropertyName("top_logprobs")]
        public IList<IDictionary<string, double>>? TopLogprobs { get; set; }

        [JsonPropertyName("text_offset")]
        public List<int>? TextOffsets { get; set; }
    }
}
