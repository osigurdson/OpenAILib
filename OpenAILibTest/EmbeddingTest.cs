// Copyright (c) 2023 Owen Sigurdson
// MIT License

using OpenAILib.ResponseCaching;

namespace OpenAILib.Tests
{
    [TestClass]
    public class EmbeddingTest
    {
        [TestMethod]
        public async Task TestGetEmbeddingAsync()
        {
            var client = new OpenAIClient(
                new OpenAIClientArgs(organizationId: TestCredentials.OrganizationId, apiKey: TestCredentials.ApiKey)
                {
                    ResponseCache = new TempFileResponseCache()
                });

            var vector = await client.GetEmbeddingAsync("my text to embed");
            Assert.AreEqual(1536, vector.Length);
        }
    }
}
