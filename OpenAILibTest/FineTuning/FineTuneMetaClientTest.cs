
// Copyright (c) 2023 Owen Sigurdson
// MIT License

using OpenAILib.Files;
using OpenAILib.FineTuning;
using OpenAILib.Models;
using OpenAILib.Tests.TestCorpora;
using System.Diagnostics;

namespace OpenAILib.Tests.FineTuning
{
    [TestClass]
    public class FineTuneMetaClientTest
    {
        [TestMethod]
        public async Task IntegrationTestBasicWalkThrough()
        {
            var httpClient = TestHttpClient.CreateHttpClient();
            var ftc = new FineTuneMetaClient(httpClient);

            // Step 1 - create the training data
            var trainingData = SquadOxygen.Create();

            // Step 2 - create the fine tune (fineTune 'a')
            var fineTuneA = await ftc.CreateFineTuneAsync(trainingData);

            // Step 3 - listen to all events / wait for completion
            var streamingEvents = new List<string>();
            await foreach (var evt in ftc.GetEventStreamAsync(fineTuneA))
            {
                Debug.WriteLine(evt.Message);
                streamingEvents.Add(evt.Message);
            }

            // Step 4a - verify status is successful
            var status = await ftc.GetStatusAsync(fineTuneA);
            Assert.AreEqual(FineTuneStatus.Succeeded, status);

            // Step 4b - check direct event access is equivelent to streamed events
            var events = await ftc.GetEventsAsync(fineTuneA);
            CollectionAssert.AreEqual(streamingEvents, events.Select(evt => evt.Message).ToList());

            // Step 5 - use new model
            // arrange
            var args = new OpenAIClientArgs(
                organizationId: TestCredentials.OrganizationId,
                apiKey: TestCredentials.ApiKey);

            var client = new OpenAIClient(args);
            var (success, modelA) = await ftc.TryGetFineTuneModelNameAsync(fineTuneA, CancellationToken.None);
            Assert.IsTrue(success);
            Assert.IsNotNull(modelA);

            // TODO a) automatically embed prompt suffix and stop when using fine time 
            //      b) allow completion to be created directly with fine tune in spec
            //      Test code below is temporarily filling these gaps
            var prompt = $"What is the electron configuration of oxygen?{fineTuneA.PromptSuffix}";
            var completion = await client.GetCompletionAsync(prompt, spec => spec.Model(modelA).Stop(fineTuneA.CompletionSuffix));
            Debug.WriteLine(completion);
            Assert.IsNotNull(completion);

            // Step 6 - create a variant with a new model (model 'b')

            // Step 7 - use variant model

            // Step 8 - refine model with new training data (model 'c')

            // Step 9 - use refined model

            // Step 10 - delete models a,b and c

            // TODO: a) add delete directly to fine tune 'metaclient'
            //       b) automatically perform garbage collection for files that are no longer needed
            var modelClient = new ModelsClient(httpClient);
            var filesClient = new FilesClient(httpClient);
            var basicFineTunesClient = new FineTunesClient(httpClient);

            var fineTuneAFileId = (await basicFineTunesClient.GetFineTuneAsync(fineTuneA.FineTuneId)).TrainingFiles[0].Id;
            var fineTuneADeleted = await modelClient.DeleteAsync(modelA);
            Assert.IsTrue(fineTuneADeleted);

            var squadTrainingDataDeleted = await filesClient.DeleteAsync(fineTuneAFileId);
            Assert.IsTrue(squadTrainingDataDeleted);

            // Step 11 - verify files are cleaned up
        }
    }
}
