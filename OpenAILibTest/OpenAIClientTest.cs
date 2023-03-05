// Copyright (c) 2023 Owen Sigurdson
// MIT License

using OpenAILib.ResponseCaching;

namespace OpenAILib.Tests
{
    [TestClass]
    public class OpenAIClientTest
    {
        [TestMethod]
        public async Task TestGetCompletionWithoutCaching()
        {
            // arrange
            var args = new OpenAIClientArgs(
                organizationId: TestCredentials.OrganizationId,
                apiKey: TestCredentials.ApiKey);

            var client = new OpenAIClient(args);

            // act
            var completion = await client.GetCompletionAsync("1 + 1 = ");

            // assert
            StringAssert.Contains(completion, "2");
        }

        [TestMethod]
        public async Task TestGetEmbeddingWithoutCaching()
        {
            // arrange
            var args = new OpenAIClientArgs(
                organizationId: TestCredentials.OrganizationId,
                apiKey: TestCredentials.ApiKey);

            var client = new OpenAIClient(args);

            // act
            var vector = await client.GetEmbeddingAsync("dog");

            // assert
            Assert.AreEqual(1536, vector.Length);
        }

        [TestMethod]
        public async Task TestGetCompletionAsyncWithCaching()
        {
            // arrange
            var dictionary = new Dictionary<Guid, string>();
            var cache = new DictionaryResponseCache(dictionary);

            var client = new OpenAIClient(
                new OpenAIClientArgs(organizationId: TestCredentials.OrganizationId, apiKey: TestCredentials.ApiKey)
                {
                    ResponseCache = cache
                });

            // act
            var completion = await client.GetCompletionAsync("1+1=");

            // assert
            Assert.IsTrue(completion.Contains("2"));
            Assert.AreEqual(1, dictionary.Count);
        }

        [TestMethod]
        public async Task TestGetCompletionsExplainCodeSample()
        {
            var client = new OpenAIClient(new OpenAIClientArgs(organizationId: TestCredentials.OrganizationId, apiKey: TestCredentials.ApiKey));
            const string prompt = @"
            class Log:
                def __init__(self, path):
                    dirname = os.path.dirname(path)
                    os.makedirs(dirname, exist_ok=True)
                    f = open(path, ""a+"")

                    # Check that the file is newline-terminated
                    size = os.path.getsize(path)
                    if size > 0:
                        f.seek(size - 1)
                        end = f.read(1)
                        if end != ""\n"":
                            f.write(""\n"")
                    self.f = f
                    self.path = path

                def log(self, event):
                    event[""_event_id""] = str(uuid.uuid4())
                    json.dump(event, self.f)
                    self.f.write(""\n"")

                def state(self):
                    state = {""complete"": set(), ""last"": None}
                    for line in open(self.path):
                        event = json.loads(line)
                        if event[""type""] == ""submit"" and event[""success""]:
                            state[""complete""].add(event[""id""])
                            state[""last""] = event
                    return state

            #####
            Here's what the above class is doing:
            1.";
            var responseText = await client.GetCompletionAsync(prompt, spec => spec
                                                                        .Model(CompletionModels.CodeDavinci0002)
                                                                        .Temperature(0)
                                                                        .MaxTokens(64)
                                                                        .TopProbability(1)
                                                                        .FrequencyPenalty(0)
                                                                        .PresencePenalty(0)
                                                                        .Stop("#####"));

            // Response I got...

            //  It creates a directory for the log file if it doesn't exist
            // 2.It opens the log file in append mode
            // 3.It checks that the log file is newline - terminated
            // 4.It writes a newline - terminated JSON object to the log file

            StringAssert.Contains(responseText, "2. ");
        }

        [TestMethod]
        public async Task TestGetCompletionAsyncEnglishToOtherLanguagesExample()
        {
            var client = new OpenAIClient(new OpenAIClientArgs(organizationId: TestCredentials.OrganizationId, apiKey: TestCredentials.ApiKey));
            const string prompt = @"
                         Translate this into 1. French, 2. Spanish and 3. Japanese:
                         What rooms do you have available?

                         1.";

            var responseText = await client.GetCompletionAsync(prompt, spec => spec
                                                                                .Model(CompletionModels.TextDavinci0003)
                                                                                .Temperature(0.3)
                                                                                .MaxTokens(100)
                                                                                .TopProbability(1)
                                                                                .FrequencyPenalty(0)
                                                                                .PresencePenalty(0));

            // Response I got...

            //  Quels sont les chambres que vous avez disponibles ?
            // 2. ¿Qué habitaciones tienes disponibles?
            // 3.どの部屋が利用可能ですか？
            StringAssert.Contains(responseText, "2. ");
        }

        [TestMethod]
        public async Task TestGetChatCompletionAsync()
        {
            var client = new OpenAIClient(new OpenAIClientArgs(organizationId: TestCredentials.OrganizationId, apiKey: TestCredentials.ApiKey));
            var sequence = new List<ChatMessage>
                {
                    new ChatMessage(ChatRole.System, "You are a helpful assistant"),
                    new ChatMessage(ChatRole.User, "What is a good name for a cat?")
                };

            var response = await client.GetChatCompletionAsync(sequence);

            // Response I got...
            // There are many great names for cats
            Assert.IsTrue(response.Length > 5);

        }

        [TestMethod]
        public async Task TestGetChatCompletionAsyncWithSpec()
        {
            var client = new OpenAIClient(new OpenAIClientArgs(organizationId: TestCredentials.OrganizationId, apiKey: TestCredentials.ApiKey));
            var sequence = new List<ChatMessage>
                {
                    new ChatMessage(ChatRole.System, "You are a helpful assistant"),
                    new ChatMessage(ChatRole.User, "What is a good name for a cat?")
                };

            var response = await client.GetChatCompletionAsync(sequence, spec => spec.MaxTokens(5).Temperature(0).User("fred"));

            // Response I got... (cut off due to only 5 tokens Max
            // There are many great

            StringAssert.Contains(response, "There");
        }

        private class DictionaryResponseCache : IResponseCache
        {
            private readonly Dictionary<Guid, string> _cache = new();

            public DictionaryResponseCache(Dictionary<Guid, string> cache)
            {
                _cache = cache;
            }

            public void PutResponse(Guid key, string response)
            {
                _cache[key] = response;
            }

            public bool TryGetResponseAsync(Guid key, out string? cachedResponse)
            {
                return _cache.TryGetValue(key, out cachedResponse);
            }

            public Dictionary<Guid, string> GetUnderlyingDictionary()
            {
                return _cache;
            }
        }
    }
}
