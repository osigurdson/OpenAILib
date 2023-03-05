// Copyright (c) 2023 Owen Sigurdson
// MIT License

namespace OpenAILib
{
    /// <summary>
    /// Represents an Open AI chat message completion with specified <paramref name="Role"/> and <paramref name="Message"/>
    /// </summary>
    /// <param name="Role">Role associated with the message</param>
    /// <param name="Message">Message text</param>
    public record ChatMessage(ChatRole Role, string Message);
}
