// Copyright (c) 2023 Owen Sigurdson
// MIT License

using OpenAILib.Serialization;

namespace OpenAILib.Tests.Serialization
{
    [TestClass]
    public class Base64UrlConverterTest
    {
        [TestMethod]
        public void TestRoundTrip()
        {
            var rand = new Random();
            for (int i = 0; i < 1000; i++)
            {
                int length = rand.Next(0, 100 + rand.Next(50));
                var bytes = new byte[length];
                rand.NextBytes(bytes);
                var bytesAsBase64String = Base64UrlConverter.ConvertToBase64Url(bytes);
                var bytesRoundTrip = Base64UrlConverter.ConvertFromBase64Url(bytesAsBase64String);

                Assert.IsFalse(bytesAsBase64String.Contains('+'));
                Assert.IsFalse(bytesAsBase64String.Contains('/'));
                Assert.IsFalse(bytesAsBase64String.Contains('='));
                CollectionAssert.AreEqual(bytes, bytesRoundTrip);
            }
        }
    }
}
