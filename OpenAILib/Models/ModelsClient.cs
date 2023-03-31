// Copyright (c) 2023 Owen Sigurdson
// MIT License

using OpenAILib.HttpHandling;

namespace OpenAILib.Models
{
    internal class ModelsClient
    {
        private const string ModelsEndPointName = "models";
        private readonly OpenAIHttpClient _httpClient;

        public ModelsClient(OpenAIHttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<List<ModelResponse>> GetModelsAsync(CancellationToken cancellationToken = default)
        {
            var response = await _httpClient.GetAsync<ModelListResponse>(ModelsEndPointName, cancellationToken);
            return response.Data;
        }

        public async Task<bool> DeleteAsync(string model, CancellationToken cancellationToken = default)
        {
            return await _httpClient.DeleteAsync($"{ModelsEndPointName}/{model}", cancellationToken);
        }
    }
}
