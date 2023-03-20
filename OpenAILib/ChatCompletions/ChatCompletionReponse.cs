// Copyright (c) 2023 Owen Sigurdson
// MIT License

using System.Text.Json.Serialization;

namespace OpenAILib.ChatCompletions
{
    internal class ChatCompletionResponse
    {
        [JsonPropertyName("id")]
        public string Id { get; }

        [JsonPropertyName("object")]
        public string ObjectType { get; set; }

        [JsonPropertyName("created")]
        public int Created { get; set; }

        [JsonPropertyName("model")]
        public string Model { get; }

        [JsonPropertyName("usage")]
        public UsageResponse Usage { get; }

        [JsonPropertyName("choices")]
        public List<ChatCompletionChoice> Choices { get; }

        [JsonConstructor]
        public ChatCompletionResponse(string id, string objectType, int created, string model, UsageResponse usage, List<ChatCompletionChoice> choices)
        {
            Id = id;
            ObjectType = objectType;
            Created = created;
            Model = model;
            Usage = usage;
            Choices = choices;
        }
    }
}
