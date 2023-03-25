// Copyright (c) 2023 Owen Sigurdson
// MIT License

// Well known models deliberately placed in the OpenAILib namespace so that
// consumers only need a single using. I likely would not use this approach on internal projects
// but when there are potentially 1000s of users of the library, decisions should be in favor
// of consumers
namespace OpenAILib
{
    /// <summary>
    /// Represents well known chat completion models - these cannot be used with standard completions
    /// </summary>
    public static class ChatCompletionModels
    {
        public const string Gpt35Turbo = "gpt-3.5-turbo";
    }
}
