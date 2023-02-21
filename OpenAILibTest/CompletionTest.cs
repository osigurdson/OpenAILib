using OpenAILib;
using OpenAILib.ResponseCaching;

namespace OpenAILibTest
{
    [TestClass]
    public class CompletionTest
    {
        private static readonly string? ApiKey = Environment.GetEnvironmentVariable("OpenAI-Apikey");
        private static readonly string? OrganizationId =  Environment.GetEnvironmentVariable("OpenAI-OrganizationId");

        [TestMethod]
        public async Task TestGetCompletionAsync()
        {
            // arrange
            var dictionary = new Dictionary<Guid, string>();
            var cache = new DictionaryResponseCache(dictionary);

            var client = new OpenAIClient(
                new OpenAIClientArgs(organizationId: OrganizationId, apiKey: ApiKey)
                {
                    ResponseCache = cache
                });

            // act
            var completion = await client.GetCompletionAsync("1+1=");

            // assert
            Assert.IsTrue(completion.Contains("2"));
            Assert.AreEqual(1, dictionary.Count);


        }

        private class DictionaryResponseCache : IResponseCache
        {
            private readonly Dictionary<Guid, string> _cache = new();

            public DictionaryResponseCache(Dictionary<Guid, string> cache)
            {
                _cache = cache;
            }

            public void PutResponse(Guid key, string response)
            {
                _cache[key] = response;
            }

            public bool TryGetResponseAsync(Guid key, out string? cachedResponse)
            {
                return _cache.TryGetValue(key, out cachedResponse);
            }

            public Dictionary<Guid, string> GetUnderlyingDictionary()
            {
                return _cache;
            }
        }
    }
}