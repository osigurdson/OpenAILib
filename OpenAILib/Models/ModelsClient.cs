// Copyright (c) 2023 Owen Sigurdson
// MIT License

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

        public async Task<bool> DeleteAsync(string model)
        {
            return await _httpClient.OpenAIDeleteAsync($"{ModelsEndpointName}/{model}");
        }


    }
}
