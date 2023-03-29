// Copyright (c) 2023 Owen Sigurdson
// MIT License

using OpenAILib.ResponseCaching;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace OpenAILib.Tests
{
    [TestClass]
    public class EmbeddingTest
    {
        [TestMethod]
        public async Task TestGetEmbeddingAsync()
        {
            var client = new AzureOpenAIClient(
                new AzureOpenAIClientArgs(apiKey: TestCredentials.AzureApiKey,
                resourceName: TestCredentials.AzureResourceName,
                deploymentId: EmbeddingsModels.TextEmbeddingAda002
            )
                {
                    ResponseCache = new TempFileResponseCache()
                });

            var vector = await client.GetEmbeddingAsync("my text to embed");
            Assert.AreEqual(1536, vector.Length);
        }
    }
}
