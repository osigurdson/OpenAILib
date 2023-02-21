// Copyright (c) 2023 Owen Sigurdson
// MIT License

using OpenAILib.ResponseCaching;

namespace OpenAILib
{
    /// <summary>
    /// Represents arguments associated with initializing <see cref="OpenAIClient"/>
    /// </summary>
    public class OpenAIClientArgs
    {
        internal const string OpenAIUrl = "https://api.openai.com/v1/";

        /// <summary>
        /// OpenAI organization id
        /// </summary>
        public string OrganizationId { get; init; }

        /// <summary>
        /// OpenAI api key
        /// </summary>
        public string ApiKey { get; init; }

        /// <summary>
        /// Base url of the OpenAI API service (https://api.openai.com/v1/)
        /// </summary>
        public string Url { get; init; } = OpenAIUrl;

        /// <summary>
        /// Optional <see cref="IResponseCache"/> implementation. Use the built-in <see cref="TempFileResponseCache"/> to cache
        /// responses in the temp directory.
        /// </summary>
        public IResponseCache ResponseCache { get; init; } = new NullResponseCache();

        public OpenAIClientArgs(string organizationId, string apiKey)
        {
            OrganizationId = organizationId;
            ApiKey = apiKey;
        }
    }
}