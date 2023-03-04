// Copyright (c) 2023 Owen Sigurdson
// MIT License

using System.Text.Json;

namespace OpenAILib
{
    /// <summary>
    /// Utility methods for common HttpClient operations
    /// </summary>
    internal static class OpenAIHttpClientExtensions
    {
        /// <summary>
        /// <see cref="HttpClient"/> extension which handles object deletion from various endpoints - creates endpoint specific exception if required
        /// </summary>
        public static async Task<bool> OpenAIDeleteAsync(this HttpClient httpClient, string endpointName, string resource)
        {
            var url = $"{endpointName}/{resource}";
            using var httpResponse = await httpClient.DeleteAsync(url);
            if (httpResponse.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return false;
            }
            httpResponse.EnsureSuccessStatusCode();
            using var responseStream = await httpResponse.Content.ReadAsStreamAsync();
            var response = JsonSerializer.Deserialize<ObjectDeletedResponse>(responseStream);

            if (response == null)
            {
                throw new OpenAIException($"Failed to deserialize deletion response for endpoint '{endpointName}'.");
            }

            return response.Deleted;
        }
    }
}