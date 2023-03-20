// Copyright (c) 2023 Owen Sigurdson
// MIT License

using System.Text.Json.Serialization;

namespace OpenAILib.Completions
{
    internal class Logprobs
    {
        [JsonPropertyName("tokens")]
        public List<string> Tokens { get; }

        [JsonPropertyName("token_logprobs")]
        public List<double> TokenLogProbs { get; }

        [JsonPropertyName("top_logprobs")]
        public IList<IDictionary<string, double>> TopLogProbs { get; }

        [JsonPropertyName("text_offset")]
        public List<int> TextOffsets { get; }

        [JsonConstructor]
        public Logprobs(List<string> tokens, List<double> tokenLogProbs, List<IDictionary<string, double>> topLogProbs, List<int> textOffsets)
        {
            Tokens = tokens;
            TokenLogProbs = tokenLogProbs;
            TopLogProbs = topLogProbs;
            TextOffsets = textOffsets;
        }
    }
}
