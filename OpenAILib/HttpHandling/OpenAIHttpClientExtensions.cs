// Copyright (c) 2023 Owen Sigurdson
// MIT License

using OpenAILib.ResponseCaching;
using System.Diagnostics;
using System.Net;
using System.Net.Http.Json;
using System.Text.Json;

namespace OpenAILib.HttpHandling
{
    /// <summary>
    /// Utility methods for common HttpClient operations
    /// </summary>
    internal static class OpenAIHttpClientExtensions
    {
        /// <summary>
        /// <see cref="HttpClient"/> extension which handles object deletion from various endpoints - creates endpoint specific exception if required
        /// </summary>
        public static async Task<bool> OpenAIDeleteAsync(this HttpClient httpClient, string requestUri)
        {
            using var httpResponse = await RetryDeleteOnConflictAsync(httpClient, requestUri);
            if (httpResponse.StatusCode == HttpStatusCode.NotFound)
            {
                return false;
            }
            httpResponse.EnsureSuccessStatusCode();
            using var responseStream = await httpResponse.Content.ReadAsStreamAsync();
            var response = Deserialize<ObjectDeletedResponse>(responseStream, requestUri);
            return response.Deleted;
        }

        private static async Task<HttpResponseMessage> RetryDeleteOnConflictAsync(HttpClient httpClient, string requestUri)
        {
            // Unfortunately, the 'files' endpoint isn't robust against fast
            // create / delete operations like this and '429 - Conflict' can
            // be returned from the service. For this reason, we retry every 5 seconds
            // for up to two minutes
            var stopwatch = Stopwatch.StartNew();
            while (stopwatch.Elapsed < TimeSpan.FromMinutes(2))
            {
                var httpResponse = await httpClient.DeleteAsync(requestUri);
                if (httpResponse.StatusCode != HttpStatusCode.Conflict)
                {
                    return httpResponse;
                }
                // Since we are not returning the response, it is explicitly disposed here
                httpResponse.Dispose();
                await Task.Delay(5000);
            }

            // After we timeout, we make a final call to delete and
            // pass the result back to the caller
            return await httpClient.DeleteAsync(requestUri);
        }


        public static async Task<TResponse> OpenAIPostAsync<TRequest, TResponse>(this HttpClient httpClient, string requestUri, TRequest request)
        {
            var content = JsonContent.Create(request);
            using var httpResponse = await httpClient.PostAsync(requestUri, content);
            httpResponse.EnsureSuccessStatusCode();
            var responseStream = await httpResponse.Content.ReadAsStreamAsync();
            var response = Deserialize<TResponse>(responseStream, requestUri);
            return response;
        }

        public static async Task<TResponse> OpenAIPostAsync<TRequest, TResponse>(
            this HttpClient httpClient,
            string requestUri,
            TRequest request,
            IResponseCache responseCache)
        {
            var content = JsonContent.Create(request);
            var requestHash = RequestHashCalculator.CalculateHash(requestUri, await content.ReadAsStringAsync());
            if (!responseCache.TryGetResponseAsync(requestHash, out var responseText))
            {
                var response = await httpClient.PostAsync(requestUri, content);
                response.EnsureSuccessStatusCode();
                responseText = await response.Content.ReadAsStringAsync();
                responseCache.PutResponse(requestHash, responseText);
            }

            if (responseText == null)
            {
                throw new OpenAIException($"Failed to deserialize response from endpoint '{requestUri}'.");
            }
            return Deserialize<TResponse>(responseText, requestUri);
        }

        private static T Deserialize<T>(Stream stream, string requestUri)
        {
            var response = JsonSerializer.Deserialize<T>(stream);
            if (response == null)
            {
                throw new ArgumentException($"Failed to deserialize response from endpoint '{requestUri}'.", nameof(stream));
            }
            return response;
        }

        private static T Deserialize<T>(string text, string requestUri)
        {
            var response = JsonSerializer.Deserialize<T>(text);
            if (response == null)
            {
                throw new ArgumentException($"Failed to deserialize response from endpoint '{requestUri}'.", nameof(text));
            }
            return response;
        }
    }
}