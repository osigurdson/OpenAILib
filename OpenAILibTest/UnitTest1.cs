using OpenAILib;

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
            var client = new OpenAIClient(OrganizationId, ApiKey);
            var vector = await client.GetEmbeddingAsync("my text to embed");
            Assert.AreEqual(1536, vector.Length);
        }

        [TestMethod]
        public async Task TestGetCompletionAsync()
        {
            var client = new OpenAIClient(OrganizationId, ApiKey);
            var completion = await client.GetCompletionAsync("1+1=");
            Assert.IsTrue(completion.Contains("2"));
        }
    }
}