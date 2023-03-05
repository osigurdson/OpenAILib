// Copyright (c) 2023 Owen Sigurdson
// MIT License

using System.Text.Json.Serialization;

namespace OpenAILib.Completions
{
    internal class CompletionResponse
    {
        [JsonPropertyName("id")]
        public string Id { get; }

        [JsonPropertyName("object")]
        public string Object { get; }

        [JsonPropertyName("created")]
        public long Created { get;}

        [JsonPropertyName("model")]
        public string Model { get; }

        [JsonPropertyName("choices")]
        public List<CompletionChoiceResponse> Choices { get; }

        [JsonPropertyName("usage")]
        public UsageResponse Usage { get; }

        public CompletionResponse(string id, string @object, long created, string model, List<CompletionChoiceResponse> choices, UsageResponse usage)
        {
            Id = id;
            Object = @object;
            Created = created;
            Model = model;
            Choices = choices;
            Usage = usage;
        }
    }
}
