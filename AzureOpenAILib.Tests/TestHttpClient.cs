// Copyright (c) 2023 Owen Sigurdson
// MIT License

using OpenAILib.HttpHandling;

namespace OpenAILib.Tests
{
    public static class TestHttpClient
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

        public static LazyHttpClient CreateAzureHttpClient()
        {
            var httpClient = AzureOpenAIHttpClient
                .CreateHttpClient(
                    new AzureOpenAIClientArgs(
                            apiKey: TestCredentials.AzureApiKey,
                            resourceName: TestCredentials.AzureResourceName,
                            deploymentId: TestCredentials.AzureDeploymentId,
                            apiVersion: TestCredentials.AzureApiVersion
                            ));

            return httpClient;
        }
    }
}
