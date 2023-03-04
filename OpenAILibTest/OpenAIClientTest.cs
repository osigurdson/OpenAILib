// Copyright (c) 2023 Owen Sigurdson
// MIT License

namespace OpenAILib.Tests
{
    [TestClass]
    public class OpenAIClientTest
    {
        [TestMethod]
        public async Task TestGetCompletionWithoutCaching()
        {
            // arrange
            var args = new OpenAIClientArgs(
                organizationId: TestCredentials.OrganizationId,
                apiKey: TestCredentials.ApiKey);

            var client = new OpenAIClient(args);

            // act
            var completion = await client.GetCompletionAsync("1 + 1 = ");

            // assert
            StringAssert.Contains(completion, "2");
        }

        [TestMethod]
        public async Task TestGetEmbeddingWithoutCaching()
        {
            // arrange
            var args = new OpenAIClientArgs(
                organizationId: TestCredentials.OrganizationId,
                apiKey: TestCredentials.ApiKey);

            var client = new OpenAIClient(args);

            // act
            var vector = await client.GetEmbeddingAsync("dog");

            // assert
            Assert.AreEqual(1536, vector.Length);
        }
    }
}
