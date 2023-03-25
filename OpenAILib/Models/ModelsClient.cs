// Copyright (c) 2023 Owen Sigurdson
// MIT License

using OpenAILib.HttpHandling;
using System.Text.Json;

namespace OpenAILib.Models
{

    internal class ModelsClient
    {
        private const string ModelsEndpointName = "models";
        private readonly HttpClient _httpClient;

        public ModelsClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<List<ModelResponse>> GetModelsAsync(CancellationToken cancellationToken = default)
        {
            var httpResponse = await _httpClient.GetAsync(ModelsEndpointName, cancellationToken);
            httpResponse.EnsureSuccessStatusCode();
            var responseStream = await httpResponse.Content.ReadAsStreamAsync();
            var response = JsonSerializer.Deserialize<ModelListResponse>(responseStream);
            if (response == null)
            {
                throw new OpenAIException("Failed to deserialize model responses");
            }
            return response.Data;
        }

        public async Task<bool> DeleteAsync(string model)
        {
            return await _httpClient.OpenAIDeleteAsync($"{ModelsEndpointName}/{model}");
        }
    }
}
