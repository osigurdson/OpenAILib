// Copyright (c) 2023 Owen Sigurdson
// MIT License

using OpenAILib;

var organizationId = Environment.GetEnvironmentVariable("OpenAI_OrganizationId");
var apiKey = Environment.GetEnvironmentVariable("OpenAI_ApiKey");
var client = new OpenAIClient(organizationId: organizationId, apiKey: apiKey);

// Chat sequence
var sequence01 = new List<ChatMessage>
{
    new ChatMessage(ChatRole.System, "You are a helpful assistant"),
};

Console.WriteLine("Ask any question. Type 'done' to exit.");
while (true)
{
    Console.WriteLine();
    Console.Write("Me: ");
    var question = Console.ReadLine();
    if (string.IsNullOrEmpty(question))
    {
        continue;
    }
    if (question == "done")
    {
        break;
    }
    sequence01.Add(new ChatMessage(ChatRole.User, question));
    var response01 = await client.GetChatCompletionAsync(sequence01);
    Console.Write("ChatGPT: ");
    Console.WriteLine(response01);
    sequence01.Add(new ChatMessage(ChatRole.Assistant, response01));
}

// Customized ChatGPT - using 'spec' to increase temperature and reduce the
// number of tokens returned
var sequence02 = new List<ChatMessage>
{
    new ChatMessage(ChatRole.System, "You are a helpful assistant"),
    new ChatMessage(ChatRole.Assistant, "What is a good name for a cat?"),
};

var response02 = await client.GetChatCompletionAsync(sequence02, spec => spec.Temperature(2.0).MaxTokens(10));
Console.WriteLine(response02);


// Code completion - using an example from OpenAI playground using the 
// 'code-davinci-002' model using parameters used in the playground sample
const string CodeSample01 = @"
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

var response03 = await client.GetCompletionAsync(CodeSample01, spec => spec
                                                            .Model(CompletionModels.CodeDavinci0002)
                                                            .Temperature(0)
                                                            .MaxTokens(64)
                                                            .TopProbability(1)
                                                            .FrequencyPenalty(0)
                                                            .PresencePenalty(0)
                                                            .Stop("#####"));

Console.WriteLine(response03);

// returns a high dimensional normalized vector representing the specified text
var vector = await client.GetEmbeddingAsync("dog");

// Should be fairly close to 1.0 as the vector is normalized
Console.WriteLine($"length: {vector.Length}, sum: {vector.Sum()}");






