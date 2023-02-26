// Copyright (c) 2023 Owen Sigurdson
// MIT License

using System.Net.Http.Headers;

namespace OpenAILib
{
    internal static class OpenAIHttpClient
    {
        public static HttpClient CreateHttpClient(OpenAIClientArgs args)
        {
            var uri = new Uri(args.Url);
            var httpClient = new HttpClient
            {
                BaseAddress = uri
            };

            httpClient
                .DefaultRequestHeaders
                .Authorization = new AuthenticationHeaderValue("Bearer", args.ApiKey);

            httpClient
                .DefaultRequestHeaders
                .Add("OpenAI-Organization", args.OrganizationId);

            return httpClient;

        }
    }
}