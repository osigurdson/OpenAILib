// Copyright (c) 2023 Owen Sigurdson
// MIT License

using OpenAILib.FineTuning;
using OpenAILib.Tests.TestCorpora;
using System.Diagnostics;

namespace OpenAILib.Tests.FineTuning
{
    [TestClass]
    public class FineTunesClientTest
    {
        [TestMethod]
        public async Task IntegrationTestBasicWalkThrough()
        {
            var httpClient = TestHttpClient.CreateHttpClient();

            // test api as exposed on OpenAIClient
            IFineTunesClient ftc = new FineTunesClient(httpClient);

            // Step 1 - create the training data
            var trainingData = Fetch20.CreateHockeyBaseballBlogTitles();

            // Step 2 - create the fine tune (fineTune 'a')
            var fineTuneA = await ftc.CreateFineTuneAsync(trainingData);

            // Step 3 - listen to all events / wait for completion
            var streamingEvents = new List<string>();
            await foreach (var evt in ftc.GetEventStreamAsync(fineTuneA))
            {
                streamingEvents.Add(evt.Message);
            }

            // Step 4a - verify status is successful
            var fineTuneAStatus = await ftc.GetStatusAsync(fineTuneA);
            Assert.AreEqual(FineTuneStatus.Succeeded, fineTuneAStatus);

            // Step 4b - check direct event access is equivelent to streamed events
            var events = await ftc.GetEventsAsync(fineTuneA);
            CollectionAssert.AreEqual(streamingEvents, events.Select(evt => evt.Message).ToList());

            // Step 5 - use new model
            // arrange
            var args = new OpenAIClientArgs(
                organizationId: TestCredentials.OrganizationId,
                apiKey: TestCredentials.ApiKey);

            var client = new OpenAIClient(args);
           
            var modelAPrompt = $"nhl";
            var modelACompletion = await client.GetCompletionAsync(modelAPrompt, spec => spec.Model(fineTuneA));
            Debug.WriteLine(modelACompletion);
            Assert.IsNotNull(modelACompletion);

            StringAssert.Contains(modelACompletion, "hockey");

            // Step 6a - create a variant using 'ada' instead (fineTune 'b')
            const string expectedModelBSuffix = "model-b-must-be-lower-case";
            var fineTuneB = await ftc.CreateFineTuneVariantAsync(fineTuneA, spec => spec
                                                                            .Model(FineTuneBaseModels.Ada)
                                                                            .ModelSuffix(expectedModelBSuffix));
            await foreach (var evt in ftc.GetEventStreamAsync(fineTuneB))
            {
                Debug.WriteLine(evt.Message);
            }
            var fineTuneBStatus = await ftc.GetStatusAsync(fineTuneB);
            Assert.AreEqual(FineTuneStatus.Succeeded, fineTuneBStatus);

            // Step 7 - use variant model
            var modelBPrompt = $"mlb";
            var modelBCompletion = await client.GetCompletionAsync(modelBPrompt, spec => spec.Model(fineTuneB));
            Debug.WriteLine(modelBCompletion);
            Assert.IsNotNull(modelBCompletion);
            StringAssert.Contains(modelBCompletion, "baseball");

            // Step 8 - delete the models
            await ftc.DeleteFineTuneAsync(fineTuneA, false);
            await ftc.DeleteFineTuneAsync(fineTuneB, true);
        }
    }
}
