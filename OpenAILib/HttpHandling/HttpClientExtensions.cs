// Copyright (c) 2023 Owen Sigurdson
// MIT License

namespace OpenAILib.HttpHandling
{
    // <summary>
    // The HttpClientExtensions class implements extension methods for HttpClient.
    // </summary>
    public static class HttpClientExtensions
    {
        // <summary>
        // The LazyUri method creates a new Uri instance by adding query parameters from the LazyHttpClient instance.
        // </summary>
        public static Uri LazyUri(this HttpClient httpClient, string requestUri)
        {
            var lazyHttpClient = httpClient as LazyHttpClient;
            if (lazyHttpClient != null)
            {
                return (httpClient as LazyHttpClient).ReformUri(new Uri(requestUri, UriKind.RelativeOrAbsolute));
            }

            var uri = httpClient.BaseAddress == null ? new Uri(requestUri, UriKind.RelativeOrAbsolute) : new Uri(httpClient.BaseAddress, new Uri(requestUri, UriKind.RelativeOrAbsolute));

            return uri;
        }
    }
}