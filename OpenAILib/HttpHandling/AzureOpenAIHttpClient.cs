// Copyright (c) 2023 Owen Sigurdson
// MIT License

namespace OpenAILib.HttpHandling
{
    // <summary>
    // The AzureOpenAIHttpClient class provides a static method to create HttpClient instances for Azure OpenAI API calls.
    // </summary>
    public static class AzureOpenAIHttpClient
    {
        // <summary>
        // The CreateHttpClient method creates an instance of LazyHttpClient configured with the specified AzureOpenAIClientArgs.
        // </summary>
        public static LazyHttpClient CreateHttpClient(AzureOpenAIClientArgs args)
        {
            var url = args.Url.Replace("{resourceName}", args.ResourceName)
                .Replace("{deploymentId}", args.DeploymentId);

            var uri = new Uri(url);

            var httpClient = new LazyHttpClient
            {
                BaseAddress = uri
            };

            httpClient.AddParam("api-version", args.ApiVersion);

            httpClient
                .DefaultRequestHeaders
                .Add("api-key", args.ApiKey);

            return httpClient;
        }
    }
}