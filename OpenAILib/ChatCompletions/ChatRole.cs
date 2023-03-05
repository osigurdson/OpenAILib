// Copyright (c) 2023 Owen Sigurdson
// MIT License

using OpenAILib.Serialization;
using System.Text.Json.Serialization;

namespace OpenAILib.ChatCompletions
{
    [JsonConverter(typeof(JsonEnumMemberConverter<ChatRole>))]
    internal enum ChatRole
    {
        System,
        User,
        Assistant
    }
}
