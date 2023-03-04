// Copyright (c) 2023 Owen Sigurdson
// MIT License

using OpenAILib.FineTuning;
using System.Text.Json;

namespace OpenAILib.Tests.FineTuning
{
    [TestClass]
    public class FineTuneResponseTest
    {
        [TestMethod]
        public void TestDeserialize()
        {
            // arrange
            var jsonText = @"  {
                  ""object"": ""fine-tune"",
                  ""id"": ""ft-exampleId"",
                  ""hyperparams"": {
                    ""n_epochs"": 4,
                    ""batch_size"": 1,
                    ""prompt_loss_weight"": 0.01,
                    ""learning_rate_multiplier"": 0.1
                  },
                  ""organization_id"": ""org-example"",
                  ""model"": ""curie"",
                  ""training_files"": [
                    {
                      ""object"": ""file"",
                      ""id"": ""file-example"",
                      ""purpose"": ""fine-tune"",
                      ""filename"": ""example.jsonl"",
                      ""bytes"": 807,
                      ""created_at"": 1677437336,
                      ""status"": ""processed"",
                      ""status_details"": null
                    }
                  ],
                  ""validation_files"": [],
                  ""result_files"": [
                    {
                      ""object"": ""file"",
                      ""id"": ""file-result-example"",
                      ""purpose"": ""fine-tune-results"",
                      ""filename"": ""compiled_results.csv"",
                      ""bytes"": 1756,
                      ""created_at"": 1677438628,
                      ""status"": ""processed"",
                      ""status_details"": null
                    }
                  ],
                  ""created_at"": 1677438115,
                  ""updated_at"": 1677438628,
                  ""status"": ""succeeded"",
                  ""fine_tuned_model"": ""curie:ft-personal-example""
                }";

            // act
            var deserialized = JsonSerializer.Deserialize<FineTuneResponse>(jsonText);

            // assert - since serialization is defined declartively, only a basic smoke test is performed
            Assert.IsNotNull(deserialized);
            Assert.AreEqual(1, deserialized.ResultFiles.Count);
            Assert.AreEqual(1756, deserialized.ResultFiles[0].Bytes);
        }
    }
}
