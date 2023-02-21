using OpenAILib;
using OpenAILib.ResponseCaching;

namespace OpenAILibTest
{
    [TestClass]
    public class EmbeddingTest
    {
        private static readonly string? ApiKey = Environment.GetEnvironmentVariable("OpenAI-Apikey");
        private static readonly string? OrganizationId = Environment.GetEnvironmentVariable("OpenAI-OrganizationId");

        [TestMethod]
        public async Task TestGetEmbeddingAsync()
        {
            var client = new OpenAIClient(
                new OpenAIClientArgs(organizationId: OrganizationId, apiKey: ApiKey)
                {
                    ResponseCache = new TempFileResponseCache()
                });

            var vector = await client.GetEmbeddingAsync("my text to embed");
            Assert.AreEqual(1536, vector.Length);
        }
    }
}
