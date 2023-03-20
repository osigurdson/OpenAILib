// Copyright (c) 2023 Owen Sigurdson
// MIT License

using OpenAILib.FineTuning;

namespace OpenAILib.Tests.FineTuning
{
    [TestClass]
    public class FineTuneSpec01Test
    {
        [TestMethod]
        public void TestToRequestWithDefaults()
        {
            const string trainingFileId = "trainingFile";
            const string? validationFileId = null;
            // arrange
            var settings = new FineTuneSpec01();

            // act
            var request = settings.ToRequest(trainingFileId, validationFileId);

            // assert
            // fine tune request items
            Assert.AreEqual(trainingFileId, request.TrainingFile);  // Training fileId is required
            Assert.IsNull(request.ValidationFile);                  // Validation fileId is optional
            Assert.IsNull(request.Model);                           // Model is optional - defaults to 'curie'
            Assert.IsNull(request.NEpochs);                          // All other items are options
            Assert.IsNull(request.BatchSize);
            Assert.IsNull(request.LearningRateMultiplier);
            Assert.IsNull(request.PromptLossWeight);
            Assert.IsNull(request.ComputeClassificationMetrics);
            Assert.IsNull(request.ClassificationBetas);
            Assert.IsNull(request.Suffix);
        }

        [TestMethod]
        public void TestDefaultPromptCompletionSuffix()
        {
            // arrange
            var settings = new FineTuneSpec01();

            // act
            Assert.AreEqual(FineTuneSpec01.DefaultPromptSuffix, settings.GetPromptSuffix());
            Assert.AreEqual(FineTuneSpec01.DefaultCompletionSuffix, settings.GetCompletionSuffix());
        }

        [TestMethod]
        public void TestPromptCompletionSuffix()
        {
            const string expectedPromptSuffix = "promptSuffix";
            const string expectedCompletionSuffix = "completionSuffix";

            // arrange
            var settings = new FineTuneSpec01();

            // act
            settings.PromptSuffix(expectedPromptSuffix);
            settings.CompletionSuffix(expectedCompletionSuffix);

            // assert
            Assert.AreEqual(expectedPromptSuffix, settings.GetPromptSuffix());
            Assert.AreEqual(expectedCompletionSuffix, settings.GetCompletionSuffix());
        }

        [TestMethod]
        public void TestModel()
        {
            const string expectedModel = "model";
            // arrange
            var settings = new FineTuneSpec01();

            // act
            settings.Model(expectedModel);
            var request = settings.ToRequest("id", null);

            // assert
            Assert.AreEqual(expectedModel, request.Model);
        }

        [TestMethod]
        public void TestEpochs()
        {
            const int expectedEpochs = 10;
            // arrange
            var settings = new FineTuneSpec01();

            // act
            settings.Epochs(expectedEpochs);
            var request = settings.ToRequest("id", null);

            // assert
            Assert.AreEqual(expectedEpochs, request.NEpochs);
        }

        [TestMethod]
        public void TestBatchSize()
        {
            const int expectedBatchSize = 32;
            // arrange
            var settings = new FineTuneSpec01();

            // act
            settings.BatchSize(expectedBatchSize);
            var request = settings.ToRequest("id", null);

            // assert
            Assert.AreEqual(expectedBatchSize, request.BatchSize);
        }

        [TestMethod]
        public void TestLearningRateMultiplier()
        {
            const double expectedLearningRateMultiplier = 0.5;
            // arrange
            var settings = new FineTuneSpec01();

            // act
            settings.LearningRateMultiplier(expectedLearningRateMultiplier);
            var request = settings.ToRequest("id", null);

            // assert
            Assert.AreEqual(expectedLearningRateMultiplier, request.LearningRateMultiplier);
        }

        [TestMethod]
        public void TestPromptLossWeight()
        {
            const double expectedPromptLossWeight = 0.1;
            // arrange
            var settings = new FineTuneSpec01();

            // act
            settings.PromptLossWeight(expectedPromptLossWeight);
            var request = settings.ToRequest("id", null);

            // assert
            Assert.AreEqual(expectedPromptLossWeight, request.PromptLossWeight);
        }

        [TestMethod]
        public void TestComputeClassificationMetrics()
        {
            const bool expectedComputeClassificationMetrics = true;
            // arrange
            var settings = new FineTuneSpec01();

            // act
            settings.ComputeClassificationMetrics(expectedComputeClassificationMetrics);
            var request = settings.ToRequest("id", null);

            // assert
            Assert.AreEqual(expectedComputeClassificationMetrics, request.ComputeClassificationMetrics);
        }

        [TestMethod]
        public void TestClassificationBetas()
        {
            const double expectedClassificationBeta1 = 0.5;
            const double expectedClassificationBeta2 = 0.9;
            // arrange
            var settings = new FineTuneSpec01();

            // act
            settings.ClassificationBetas(expectedClassificationBeta1, expectedClassificationBeta2);
            var request = settings.ToRequest("id", null);

            // assert
            Assert.IsNotNull(request.ClassificationBetas);
            Assert.AreEqual(expectedClassificationBeta1, request.ClassificationBetas[0]);
            Assert.AreEqual(expectedClassificationBeta2, request.ClassificationBetas[1]);
        }

        [TestMethod]
        public void TestSuffix()
        {
            const string expectedSuffix = "-v2";
            // arrange
            var settings = new FineTuneSpec01();

            // act
            settings.Suffix(expectedSuffix);
            var request = settings.ToRequest("id", null);

            // assert
            Assert.AreEqual(expectedSuffix, request.Suffix);
        }

        [TestMethod]
        public void TestPromptSuffix()
        {
            const string expectedPromptSuffix = "expectedPromptSuffix";

            // arrange
            var settings = new FineTuneSpec01();

            // act
            settings.PromptSuffix(expectedPromptSuffix);

            // assert
            Assert.AreEqual(expectedPromptSuffix, settings.GetPromptSuffix());
        }

        [TestMethod]
        public void TestCompletionSuffix()
        {
            const string expectedCompletionSuffix = "expectedCompletionSuffix";

            // arrange
            var settings = new FineTuneSpec01();

            // act
            settings.CompletionSuffix(expectedCompletionSuffix);

            // assert
            Assert.AreEqual(expectedCompletionSuffix, settings.GetCompletionSuffix());
        }

        [TestMethod]
        public void TestValidationData()
        {
            // arrange
            var expectedValidationData = new List<FineTunePair>
            {
                new FineTunePair("prompt1", "completion1"),
                new FineTunePair("prompt2", "completion2"),
                new FineTunePair("prompt3", "completion3"),
                new FineTunePair("prompt4", "completion4"),
                new FineTunePair("prompt5", "completion5")
            };

            var settings = new FineTuneSpec01();

            // act
            settings.ValidationData(expectedValidationData);

            // assert
            Assert.AreEqual(expectedValidationData, settings.GetValidationData());
        }
    }
}
