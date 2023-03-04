// Copyright (c) 2023 Owen Sigurdson
// MIT License

using System.Net.Http.Json;
using System.Text.Json;

namespace OpenAILib.FineTuning
{
    internal class FineTunesClient
    {
        private const string FineTunesEndpointName = "fine-tunes";

        private readonly HttpClient _httpClient;

        public FineTunesClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<string> CreateFineTuneAsync(FineTuneRequest request)
        {
            var content = JsonContent.Create(request);
            var httpResponse = await _httpClient.PostAsync(FineTunesEndpointName, content);
            httpResponse.EnsureSuccessStatusCode();
            var text = await httpResponse.Content.ReadAsStringAsync();
            var responseStream = await httpResponse.Content.ReadAsStreamAsync();
            var response = JsonSerializer.Deserialize<FineTuneResponse>(responseStream);

            if (response == null || response.Id == null)
            {
                throw new OpenAIException("Failed to deserialize fine tune response");
            }

            return response.Id;
        }

        public async Task<IReadOnlyList<FineTuneResponse>> GetFineTunesAsync()
        {
            var httpResponse = await _httpClient.GetAsync(FineTunesEndpointName);
            httpResponse.EnsureSuccessStatusCode();
            var responseStream = await httpResponse.Content.ReadAsStreamAsync();
            var responseText = await httpResponse.Content.ReadAsStringAsync();
            var response = JsonSerializer.Deserialize<FineTuneListResponse>(responseStream);
            if (response == null || response.Data == null)
            {
         
                throw new OpenAIException("Failed to deserialize fine tune responses ");
            }
            return response.Data;
        }

        public async Task<FineTuneResponse> GetFineTuneAsync(string fineTuneId)
        {
            var httpResponse = await _httpClient.GetAsync($"{FineTunesEndpointName}/{fineTuneId}");
            httpResponse.EnsureSuccessStatusCode();
            var response = await httpResponse.Content.ReadFromJsonAsync<FineTuneResponse>();
            if (response == null)
            {
                throw new OpenAIException($"Failed to deserialize fine tune '{fineTuneId}'.");
            }
            return response;
        }
    }
}
