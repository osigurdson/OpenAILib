// Copyright (c) 2023 Owen Sigurdson
// MIT License

using OpenAILib.FineTuning;
using OpenAILib.Serialization;

namespace OpenAILib.Tests.Serialization
{
    [TestClass]
    public class JsonLinesSerializerTest
    {
        [TestMethod]
        public void TestSerializeRoundTrip()
        {
            // arrange
            var fineTuneSet = new List<FineTunePair>
            {
                new FineTunePair { Prompt = "Zero", Completion = "0.0"},
                new FineTunePair { Prompt = "One", Completion = "1.0"}
            };

            // act
            var memoryStream = new MemoryStream();
            JsonLinesSerializer.Serialize(memoryStream, fineTuneSet);
            memoryStream.Position = 0;

            var result = JsonLinesSerializer
                .Deserialize<FineTunePair>(memoryStream)
                .ToList();

            // assert
            Assert.AreEqual(fineTuneSet.Count, result.Count);
            for (int i = 0; i < fineTuneSet.Count; i++)
            {
                Assert.AreEqual(fineTuneSet[i].Prompt, result[i].Prompt);
                Assert.AreEqual(fineTuneSet[i].Completion, result[i].Completion);
            }
        }
    }
}
