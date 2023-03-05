// Copyright (c) 2023 Owen Sigurdson
// MIT License

using OpenAILib.Serialization;
using System.Text.Json.Serialization;

// Note that this is deliberately placed in the OpenAILib base namespace so that
// consumers of the API only require a single using statement
namespace OpenAILib
{
    [JsonConverter(typeof(JsonEnumMemberConverter<ChatRole>))]
    public enum ChatRole
    {
        System,
        User,
        Assistant
    }
}
