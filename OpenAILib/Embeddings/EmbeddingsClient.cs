// Copyright (c) 2023 Owen Sigurdson
// MIT License

using OpenAILib.HttpHandling;
using OpenAILib.ResponseCaching;
using System.Net.Http.Json;
using System.Text.Json;

namespace OpenAILib.Embeddings
{
    internal class EmbeddingsClient
    {
        private const string EndPointName = "embeddings";
        private readonly HttpClient _httpClient;
        private readonly IResponseCache _responseCache;

        public EmbeddingsClient(HttpClient httpClient, IResponseCache responseCache)
        {
            _httpClient = httpClient;
            _responseCache = responseCache;
        }

        /// <summary>
        /// Retrieves an embedding vector for the specified text using the recommended default model ('text-embedding-ada-002').
        /// </summary>
        /// <param name="text">The input text for which to obtain an embedding vector.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the asynchronous operation. The task result contains the associated embedding vector as a double array.</returns>
        /// <remarks>
        /// The default model used by this method may change in future versions of the library. 
        /// As a result, the embedding vectors returned by this method may also change. 
        /// If you require consistent embeddings across different versions of the library, you should specify a model explicitly.
        /// </remarks>
        public async Task<double[]> GetEmbeddingAsync(string text)
        {
            const string originalModel = "text-embedding-ada-002";
            var request = new EmbeddingRequest
            {
                Model = originalModel,
                Input = text
            };

            var content = JsonContent.Create(request);

            var requestHash = RequestHashCalculator.CalculateHash(EndPointName, await content.ReadAsStringAsync());
            if (!_responseCache.TryGetResponseAsync(requestHash, out var responseText))
            {
                var response = await _httpClient.PostAsync(_httpClient.LazyUri(EndPointName), content);
                response.EnsureSuccessStatusCode();
                responseText = await response.Content.ReadAsStringAsync();
                _responseCache.PutResponse(requestHash, responseText);
            }

            if (string.IsNullOrEmpty(responseText))
            {
                throw new ArgumentException($"No result returned for embedding text: '{text}'");
            }

            var embeddingResponse = JsonSerializer.Deserialize<EmbeddingResponse>(responseText);
            
            if (embeddingResponse == null ||
                embeddingResponse.Data == null || 
                embeddingResponse.Data.Count != 1)
            {
                throw new OpenAIException($"Failed to deserialize embedding response '{responseText}'.");
            }

            var embeddingText = embeddingResponse.Data[0].Embedding;
            if (embeddingText == null)
            {
                throw new OpenAIException($"Failed to deserialize embedding response '{responseText}'.");
            }
            return embeddingText;
        }
    }
}
