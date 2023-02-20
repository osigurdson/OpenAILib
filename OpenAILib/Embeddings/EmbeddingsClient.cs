// MIT License

using System.Net.Http.Json;
using System.Text.Json;

namespace OpenAILib.Embeddings
{
    internal class EmbeddingsClient
    {
        private readonly HttpClient _httpClient;

        public EmbeddingsClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
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
            var response = await _httpClient.PostAsync("embeddings", content);
            response.EnsureSuccessStatusCode();

            var responseText = await response.Content.ReadAsStringAsync();
            var embeddingResponse = JsonSerializer.Deserialize<EmbeddingResponse>(responseText);
            
            if (embeddingResponse == null || embeddingResponse.Data == null || 
                embeddingResponse.Data.Count != 1 || embeddingResponse.Data[0].Embedding == null)
            {
                throw new OpenAIException($"Failed to deserialize embedding response '{responseText}'.");
            }
            return embeddingResponse.Data[0].Embedding;
        }
    }
}
