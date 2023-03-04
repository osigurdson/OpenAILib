// Copyright (c) 2023 Owen Sigurdson
// MIT License

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
