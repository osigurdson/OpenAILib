// Copyright (c) 2023 Owen Sigurdson
// MIT License

using OpenAILib.HttpHandling;

namespace OpenAILib.Embeddings
{
    internal class EmbeddingsClient
    {
        private const string EmbeddingsEndpointName = "embeddings";
        private readonly OpenAIHttpClient _httpClient;

        public EmbeddingsClient(OpenAIHttpClient httpClient)
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
        public async Task<double[]> GetEmbeddingAsync(string text, CancellationToken cancellationToken = default)
        {
            const string originalModel = "text-embedding-ada-002";
            var request = new EmbeddingRequest
            {
                Model = originalModel,
                Input = text
            };

            var (embeddingResponse, responseText) = await _httpClient.PostAsync<EmbeddingRequest, EmbeddingResponse>(
                originalRequestUri: EmbeddingsEndpointName, 
                request: request, 
                cacheResponses: true, 
                cancellationToken: cancellationToken);
            
            if (embeddingResponse.Data == null ||  embeddingResponse.Data.Count != 1)
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
