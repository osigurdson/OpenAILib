// Copyright (c) 2023 Owen Sigurdson
// MIT License

using OpenAILib.ResponseCaching;
using System.Diagnostics;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;

namespace OpenAILib.HttpHandling
{
    internal class OpenAIHttpClient
    {
        private readonly HttpClient _httpClient;
        private readonly IResponseCache _responseCache;
        private readonly Func<string, Uri> _uriTransformer;

        public OpenAIHttpClient(OpenAIClientArgs args)
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

            // The utility of this will be more clear in an upcoming commit which adds support for
            // the Azure OpenAI service
            var uriTransformer = (string uri) => new Uri(uri, UriKind.RelativeOrAbsolute);

            _httpClient = httpClient;
            _uriTransformer = uriTransformer;
            _responseCache = args.ResponseCache;
        }

        public async Task<TResponse> GetAsync<TResponse>(string originalRequestUri, CancellationToken cancellationToken = default)
        {
            var requestUri = _uriTransformer(originalRequestUri);
            using var httpResponse = await _httpClient.GetAsync(requestUri, cancellationToken);
            var responseStream = await httpResponse.Content.ReadAsStreamAsync(cancellationToken);
            var response = Deserialize<TResponse>(responseStream, requestUri);
            return response;
        }

        public async Task<HttpResponseMessage> GetHttpResponseAsync(string originalRequestUri, CancellationToken cancellationToken = default)
        {
            var requestUri = _uriTransformer(originalRequestUri);
            var httpResponse = await _httpClient.GetAsync(requestUri, cancellationToken);
            httpResponse.EnsureSuccessStatusCode();
            return httpResponse;
        }

        /// <summary>
        /// <see cref="HttpClient"/> extension which handles object deletion from various endpoints - creates endpoint specific exception if required
        /// </summary>
        public async Task<bool> DeleteAsync(string originalRequestUri, CancellationToken cancellationToken = default)
        {
            var requestUri = _uriTransformer(originalRequestUri);
            using var httpResponse = await RetryDeleteOnConflictAsync(_httpClient, requestUri, cancellationToken);
            if (httpResponse.StatusCode == HttpStatusCode.NotFound)
            {
                return false;
            }
            httpResponse.EnsureSuccessStatusCode();
            using var responseStream = await httpResponse.Content.ReadAsStreamAsync(cancellationToken);
            var response = Deserialize<ObjectDeletedResponse>(responseStream, requestUri);
            return response.Deleted;
        }

        private static async Task<HttpResponseMessage> RetryDeleteOnConflictAsync(HttpClient httpClient, Uri requestUri, CancellationToken cancellationToken)
        {
            // Unfortunately, the 'files' endpoint isn't robust against fast
            // create / delete operations like this and '429 - Conflict' can
            // be returned from the service. For this reason, we retry every 5 seconds
            // for up to two minutes
            var stopwatch = Stopwatch.StartNew();
            while (stopwatch.Elapsed < TimeSpan.FromMinutes(2))
            {
                var httpResponse = await httpClient.DeleteAsync(requestUri, cancellationToken);
                if (httpResponse.StatusCode != HttpStatusCode.Conflict)
                {
                    return httpResponse;
                }
                // Since we are not returning the response, it is explicitly disposed here
                httpResponse.Dispose();
                await Task.Delay(5000, cancellationToken);
            }

            // After we timeout, we make a final call to delete and
            // pass the result back to the caller
            return await httpClient.DeleteAsync(requestUri);
        }

        public async Task<(TResponse response, string responseText)> PostAsync<TRequest, TResponse>(
            string originalRequestUri,
            TRequest request,
            bool cacheResponses = true,
            CancellationToken cancellationToken = default)
        {
            var requestUri = _uriTransformer(originalRequestUri);
            var content = JsonContent.Create(request);
            var requestHash = RequestHashCalculator.CalculateHash(requestUri.ToString(), await content.ReadAsStringAsync());
            if (!cacheResponses || !_responseCache.TryGetResponseAsync(requestHash, out var responseText))
            {
                var httpResponse = await _httpClient.PostAsync(requestUri, content, cancellationToken);
                httpResponse.EnsureSuccessStatusCode();
                responseText = await httpResponse.Content.ReadAsStringAsync(cancellationToken);
                _responseCache.PutResponse(requestHash, responseText);
            }

            if (responseText == null)
            {
                throw new OpenAIException($"Failed to deserialize response from endpoint '{requestUri}'.");
            }

            _responseCache.PutResponse(requestHash, responseText);
            var response = Deserialize<TResponse>(responseText, requestUri);
            return (response, responseText);
        }

        public async Task<TResponse> PostAsync<TResponse>(string originalRequestUri, HttpContent content, CancellationToken cancellationToken)
        {
            var requestUri = _uriTransformer(originalRequestUri);
            var httpResponse = await _httpClient.PostAsync(requestUri, content, cancellationToken);
            httpResponse.EnsureSuccessStatusCode();

            var responseStream = await httpResponse.Content.ReadAsStreamAsync();
            var response = Deserialize<TResponse>(responseStream, requestUri);

            return response;
        }

        public async Task<(TResponse response, string responseText)> PostAsync<TRequest, TResponse>(
            string originalRequestUri,
            TRequest request,
            IResponseCache responseCache,
            CancellationToken cancellationToken = default)
        {
            var requestUri = _uriTransformer(originalRequestUri);
            var content = JsonContent.Create(request);
            var requestHash = RequestHashCalculator.CalculateHash(requestUri.ToString(), await content.ReadAsStringAsync());
            if (!responseCache.TryGetResponseAsync(requestHash, out var responseText))
            {
                var httpResponse = await _httpClient.PostAsync(requestUri, content, cancellationToken);
                httpResponse.EnsureSuccessStatusCode();
                responseText = await httpResponse.Content.ReadAsStringAsync(cancellationToken);
                responseCache.PutResponse(requestHash, responseText);
            }

            if (responseText == null)
            {
                throw new OpenAIException($"Failed to deserialize response from endpoint '{requestUri}'.");
            }

            var response =  Deserialize<TResponse>(responseText, requestUri);
            return (response, responseText);
        }

        public HttpClient GetHttpClient()
        {
            return _httpClient;
        }

        public Uri GetTransformedUri(string originalRequestUri)
        {
            return _uriTransformer(originalRequestUri);
        }

        private static T Deserialize<T>(Stream stream, Uri requestUri)
        {
            var response = JsonSerializer.Deserialize<T>(stream);
            if (response == null)
            {
                throw new ArgumentException($"Failed to deserialize response from endpoint '{requestUri}'.", nameof(stream));
            }
            return response;
        }

        private static T Deserialize<T>(string text, Uri requestUri)
        {
            var response = JsonSerializer.Deserialize<T>(text);
            if (response == null)
            {
                // Uri.ToString() returns the fully qualified path - not type name
                throw new ArgumentException($"Failed to deserialize response from endpoint '{requestUri}'.", nameof(text));
            }
            return response;
        }
    }
}
