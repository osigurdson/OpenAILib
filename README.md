# OpenAILib

Simple OpenAI completions and embeddings library with no dependencies.

## Quick Example

```csharp
using OpenAILib;

var client = new OpenAIClient(organizationId: "organizationId", apiKey: "apiKey");

// likely returns "2"
var result = await client.GetCompletionAsync("1 + 1 = ");

// returns a high dimensional vector representing the specified text
var vector = await client.GetEmbeddingAsync("dog");
```

## ChatGPT Sequences
Create your own command line chat bot using the ChatGPT model (using the default 'gpt-3.5-turbo' model). As described in the OpenAI API documentation, typically a conversation is formatted with a system message first, followed by alternating user and assistant messages. The initial 'System' message helps set behavior of the system. This is discussed in more detail at https://platform.openai.com/docs/guides/chat/introduction

```csharp
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
```

## Customized Completions
Use 'spec' to include any customizations to the request (such as MaxTokens, Temperature, etc.). Any unspecified values utilize the current OpenAI defaults. Note that (where possible) the default values are not hard code into the library so you will always get the correct up-to-date defaults.

```csharp
// Customized ChatGPT - using 'spec' to increase temperature and reduce the
// number of tokens returned
var sequence02 = new List<ChatMessage>
{
    new ChatMessage(ChatRole.System, "You are a helpful assistant"),
    new ChatMessage(ChatRole.Assistant, "What is a good name for a cat?"),
};

var response02 = await client.GetChatCompletionAsync(sequence02, spec => spec.Temperature(2.0).MaxTokens(10));
Console.WriteLine(response02);
```

This can also be used to specify alternate stock OpenAI completion model as well as any custom fine tuned model. Example from the OpenAI Playground utilizing the code-davinci-002 model shown below.

```csharp
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
```

## Embeddings
Embedding vectors are useful because they represent words or concepts as high-dimensional numerical vectors, which can be used to analyze relationships and similarities between different words or concepts in natural language processing tasks. Typically this will be used in conjunction with other AI and machine learning techniques such as PCA, TSNE, etc.

```csharp
// returns a high dimensional normalized vector representing the specified text
var vector = await client.GetEmbeddingAsync("dog");

// Should be fairly close to 1.0 as the vector is normalized
Console.WriteLine($"length: {vector.Length}, sum: {vector.Sum()}");
```

## Optional response caching
When conducting tests and experiments, it can be advantageous to cache responses. This is especially true when you need to repeatedly make the same requests. To enable response caching, you can create an OpenAIClient using OpenAIClientArgs, as demonstrated in the code below:

```csharp
var client = new OpenAIClient(
    new OpenAIClientArgs(organizationId: OrganizationId, apiKey: ApiKey)
    {
        ResponseCache = new TempFileResponseCache()
    });
```

The responses are saved in either $TMPDIR/OpenAILibCache for Linux or C:\Users[username]\AppData\Local\Temp\OpenAILibCache for Windows. However, if you need a more advanced caching approach, you can create a custom caching strategy by implementing the IResponseCache interface, which is illustrated in the code snippet below.

```csharp
public interface IResponseCache
{
    bool TryGetResponse(Guid key, out string? cachedResponse);
    void PutResponse(Guid key, string response);
}
```
You can use a variety of options to implement a key-value store, including Redis, Cassandra, DynamoDB, or even a basic in-memory structure. 
