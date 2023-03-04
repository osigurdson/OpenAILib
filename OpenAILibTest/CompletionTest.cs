// Copyright (c) 2023 Owen Sigurdson
// MIT License

using OpenAILib.Completions;
using OpenAILib.ResponseCaching;

namespace OpenAILib.Tests
{
    [TestClass]
    public class CompletionTest
    {
        [TestMethod]
        public async Task TestGetCompletionAsync()
        {
            // arrange
            var dictionary = new Dictionary<Guid, string>();
            var cache = new DictionaryResponseCache(dictionary);

            var client = new OpenAIClient(
                new OpenAIClientArgs(organizationId: TestCredentials.OrganizationId, apiKey: TestCredentials.ApiKey)
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