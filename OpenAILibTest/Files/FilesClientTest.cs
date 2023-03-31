﻿// Copyright (c) 2023 Owen Sigurdson
// MIT License

using OpenAILib.Files;
using OpenAILib.FineTuning;
using OpenAILib.Serialization;

namespace OpenAILib.Tests.Files
{
    [TestClass]
    public class FilesClientTest 
    {
        [TestMethod]
        public async Task BasicUsageTest()
        {
            var httpClient = TestHttpClient.CreateHttpClient();
            var filesClient = new FilesClient(httpClient);

            // Get initial set of files
            var initialFiles = (await filesClient.GetFilesAsync())
                .Select(file => file.Id)
                .ToHashSet();

            // Create fine tune set
            var fineTuneSet = new List<FineTunePair>
            {
                new (prompt: "Zero", completion: "0.0"),
                new (prompt: "One", completion: "1.0")
            };

            // Upload file
            var userFileName = $"f-{Guid.NewGuid()}.jsonl";
            var fileId = await filesClient.UploadStreamAsync(SerializeFineTuneSet(fineTuneSet), FilePurpose.FineTune, userFileName);

            // Verify fileId did not previously exist
            Assert.IsFalse(initialFiles.Contains(fileId));

            // Verify user file name is correctly assigned
            var fileInfo = await filesClient.GetFileInfoAsync(fileId);
            Assert.AreEqual(userFileName, fileInfo.Filename);

            // Read file contents from service
            var bytes = await filesClient.GetFileContentAsync(fileId);
            var returnedFineTuneSet = JsonLinesSerializer
                .Deserialize<FineTunePair>(bytes)
                .ToList();

            // Compare contents
            Assert.AreEqual(fineTuneSet.Count, returnedFineTuneSet.Count);
            for (int i = 0; i < fineTuneSet.Count; i++)
            {
                Assert.AreEqual(fineTuneSet[i].Prompt, returnedFineTuneSet[i].Prompt);
                Assert.AreEqual(fineTuneSet[i].Completion, returnedFineTuneSet[i].Completion);
            }

            // Check the set of files includes the new file
            var newFileSet = (await filesClient.GetFilesAsync())
                  .Select(file => file.Id)
                  .ToHashSet();

            Assert.IsTrue(newFileSet.Contains(fileId));

            bool fileIsDeleted = await filesClient.DeleteAsync(fileId);
            Assert.IsTrue(fileIsDeleted);

            // Deleting non-existent file does not throw an exception
            bool nonExistentFileNotDeleted = await filesClient.DeleteAsync(fileId);
            Assert.IsFalse(nonExistentFileNotDeleted);
        }

        private static Stream SerializeFineTuneSet(List<FineTunePair> fineTuneSet)
        {
            var memoryStream = new MemoryStream();
            JsonLinesSerializer.Serialize(memoryStream, fineTuneSet);
            memoryStream.Position = 0;
            return memoryStream;
        }
    }
}
