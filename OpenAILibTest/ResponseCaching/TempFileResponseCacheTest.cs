// Copyright (c) 2023 Owen Sigurdson
// MIT License

using OpenAILib.ResponseCaching;

namespace OpenAILib.Tests.ResponseCaching
{
    [TestClass]
    public class TempFileResponseCacheTest
    {
        [TestMethod]
        public void TestPutGetResponse()
        {
            // arrange
            const string expectedContent = "content";
            var key = Guid.NewGuid();
            var expectedPath = Path.Combine(TempFileResponseCache.CacheDirectory, $"{key}.json");
            if (File.Exists(expectedPath))
            {
                File.Delete(expectedPath);
            }

            // act
            var tempFileCache = new TempFileResponseCache();
            tempFileCache.PutResponse(key, expectedContent);
            var itemExists = tempFileCache.TryGetResponseAsync(key, out var cachedResponse);

            // assert
            Assert.IsTrue(itemExists);
            Assert.AreEqual(expectedContent, cachedResponse);

            File.Delete(expectedPath);
        }

        [TestMethod]
        public void TestGetResponseWhenKeyDoesNotExist()
        {
            // arrange
            var tempFileCache = new TempFileResponseCache();

            // act
            var itemExists = tempFileCache.TryGetResponseAsync(Guid.NewGuid(), out var content);

            // assert
            Assert.IsFalse(itemExists);
            Assert.AreEqual(default, content);
        }
    }
}
