// Copyright (c) 2023 Owen Sigurdson
// MIT License

namespace OpenAILib
{
    /// <summary>
    /// Exception thrown when unexpected response is returned from the OpenAI service
    /// </summary>
    public class OpenAIException : Exception
    {
        public OpenAIException(string message) : base(message)
        {
        }
    }
}
