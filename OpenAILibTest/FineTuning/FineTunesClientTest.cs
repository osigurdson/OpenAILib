// Copyright (c) 2023 Owen Sigurdson
// MIT License

using OpenAILib.Completions;
using OpenAILib.Files;
using OpenAILib.FineTuning;
using OpenAILib.Models;
using OpenAILib.ResponseCaching;
using OpenAILib.Serialization;

namespace OpenAILib.Tests.FineTuning
{
    [TestClass]
    public class FineTunesClientTest
    {
        [TestMethod]
        [TestCategory("Integration")]
        public async Task TestFineTuneBasicInteraction()
        {
            var httpClient = TestHttpClient.CreateHttpClient();
            var filesClient = new FilesClient(httpClient);
            var fineTuneClient = new FineTunesClient(httpClient);
            var modelsClient = new ModelsClient(httpClient);
            var completionsClient = new CompletionsClient(httpClient, new NullResponseCache());

            // Create training fine tuning prompt / completion pairs
            var trainingData = TestCorpora.SquadOxygen.Create();
            using var ms = new MemoryStream();
            JsonLinesSerializer.Serialize(ms, trainingData);
            ms.Position = 0;

            // Upload to files endpoint
            var userFileName = $"f{Guid.NewGuid()}";
            var openaiFileId = await filesClient.UploadStreamAsync(ms, FilePurpose.FineTune, userFileName);

            // create fine tune request
            var fineTuneRequest = new FineTuneRequest(openaiFileId);
            var fineTuneId = await fineTuneClient.CreateFineTuneAsync(fineTuneRequest);


            // wait for completion - this can take a very long time
            // not super appropriate for a unit test but good to smoke check the entire process occasionally as 
            // this is how people will end up using this in practice
            string fineTunedModel;
            while (true)
            {
                Thread.Sleep(5000);
                var fineTune = await fineTuneClient.GetFineTuneAsync(fineTuneId);
                if (!string.IsNullOrEmpty(fineTune.FineTunedModel))
                {
                    fineTunedModel = fineTune.FineTunedModel;
                    break;
                }
                var lastEvent = fineTune.Events.LastOrDefault();
                if (lastEvent != null)
                {
                    Console.WriteLine(lastEvent.Message);
                }
            }

            // our new model is ready
            Assert.IsNotNull(fineTunedModel);
            Console.WriteLine($"Our new model: '{fineTunedModel}'");

            // try to use it
            var completionRequest = new CompletionRequest(model: fineTunedModel, prompt: "What is the boiling point of oxygen?")
            {
                MaxTokens = 100
            };

            var customResponse = await completionsClient.GetCompletionAsync(completionRequest);
            Assert.AreEqual(1, customResponse?.Choices?.Count);
            var customResponseText = customResponse?.Choices?[0]?.Text;

            Console.WriteLine(customResponseText);
            Assert.IsNotNull(customResponseText);

            // Now clean it up
            Thread.Sleep(5000);

            var modelIsDeleted = await modelsClient.DeleteAsync(fineTunedModel);
            Assert.IsTrue(modelIsDeleted);

            var fileIsDeleted = await filesClient.DeleteAsync(openaiFileId);
            Assert.IsTrue(modelIsDeleted);
        }
    }
}
