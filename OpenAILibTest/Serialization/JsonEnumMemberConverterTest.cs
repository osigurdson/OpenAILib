// Copyright (c) 2023 Owen Sigurdson
// MIT License

using OpenAILib.Serialization;
using System.Runtime.Serialization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace OpenAILib.Tests.Serialization
{
    [TestClass]
    public class JsonEnumMemberConverterTest
    {
        [TestMethod]
        public void TestRoundTrip()
        {
            // check string format
            var oneText = JsonSerializer.Serialize(TestEnum.One);
            var twoText = JsonSerializer.Serialize(TestEnum.Two);
            var thirtyThreeText = JsonSerializer.Serialize(TestEnum.ThirtyThree);
            Assert.AreEqual("\"one\"", oneText);
            Assert.AreEqual("\"two\"", twoText);
            Assert.AreEqual("\"thirty-three\"", thirtyThreeText);

            // check round-tripped values
            var oneValue = JsonSerializer.Deserialize<TestEnum>(oneText);
            var twoValue = JsonSerializer.Deserialize<TestEnum>(twoText);
            var thirtyThreeValue = JsonSerializer.Deserialize<TestEnum>(thirtyThreeText);
            Assert.AreEqual(TestEnum.One, oneValue);
            Assert.AreEqual(TestEnum.Two, twoValue);
            Assert.AreEqual(TestEnum.ThirtyThree, thirtyThreeValue);
        }

        [JsonConverter(typeof(JsonEnumMemberConverter<TestEnum>))]
        private enum TestEnum
        {
            One,
            Two,
            [EnumMember(Value = "thirty-three")]
            ThirtyThree = 33
        }
    }
}
