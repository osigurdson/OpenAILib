// Copyright (c) 2023 Owen Sigurdson
// MIT License

using OpenAILib.HttpHandling;

namespace OpenAILib.Tests
{
    internal static class TestHttpClient
    {
        public static OpenAIHttpClient CreateHttpClient()
        {
            var args = new OpenAIClientArgs(
                            organizationId: TestCredentials.OrganizationId,
                            apiKey: TestCredentials.ApiKey);

            var httpClient = new OpenAIHttpClient(args);
            return httpClient;
        }
    }
}
