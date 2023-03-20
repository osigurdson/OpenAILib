// Copyright (c) 2023 Owen Sigurdson
// MIT License

using OpenAILib.HttpHandling;

namespace OpenAILib.Tests
{
    internal static class TestHttpClient
    {
        public static HttpClient CreateHttpClient()
        {
            var httpClient = OpenAIHttpClient
                .CreateHttpClient(
                    new OpenAIClientArgs(
                            organizationId: TestCredentials.OrganizationId,
                            apiKey: TestCredentials.ApiKey));

            return httpClient;
        }
    }
}
