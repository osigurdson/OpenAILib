# OpenAILib

Simple OpenAI completions and embeddings library with no dependencies. Includes ChatGPT completion sequences and simplified fine-tuning workflows.

## Simple Example

```csharp
using OpenAILib;

var client = new OpenAIClient(organizationId: "organizationId", apiKey: "apiKey");
var result = await client.GetCompletionAsync("1 + 1 = "); // likely returns "2"
```

## ChatGPT Sequences
Create your own command line chatbot using the ChatGPT model (using the default 'gpt-3.5-turbo' model). As described in the OpenAI API documentation, typically a conversation is formatted with a system message first, followed by alternating user and assistant messages. The initial 'System' message helps set behavior of the system. This is discussed in more detail at https://platform.openai.com/docs/guides/chat/introduction

```csharp
var sequence01 = new List<ChatMessage>
{
    new ChatMessage(ChatRole.System, "You are a helpful assistant"),
};

Console.WriteLine("Ask any question. Type 'done' to exit.");
Console.WriteLine();
Console.Write("Me: ");
string question;
while ((question = Console.ReadLine()) != "done")
{
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
    Console.WriteLine();
    Console.Write("Me: ");
}
```

## Customized Chat Completions
Use 'spec' to include any customizations to the request (such as MaxTokens, Temperature, etc.). Any unspecified values utilize the current OpenAI defaults. Note that (where possible) the default values are not hard-coded into the library so you will always get the correct up-to-date defaults from OpenAI.

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

### ChatGPT 4.0 support

Simply use ChatCompletionModels.Gpt40 via 'spec' as shown below (must be enabled in your OpenAI account).

```csharp
client.GetChatCompletionAsync(..., spec => spec.Model(ChatCompletionModels.Gpt40))

```

# Customized Completions
'spec' can also be used to specify alternate OpenAI completion model as well as any custom fine tuned model (discussed below).  The following example uses
the davinci model as shown in playground https://platform.openai.com/examples/default-mood-color.

```csharp

const string prompt = @"
                The CSS code for a color like a blue sky at dusk:

                background-color: #";

var responseText = await client.GetCompletionAsync(prompt, spec => spec
                                                            .Model(CompletionModels.TextDavinci0003)
                                                            .Temperature(0)
                                                            .MaxTokens(64)
                                                            .TopProbability(1)
                                                            .FrequencyPenalty(0)
                                                            .PresencePenalty(0)
                                                            .Stop(";"));
```

## Simplified fine-tuning
To fine-tune using the OpenAI APIs directly, you need to use the 'files', 'fine-tunes', 'models', and 'completions' endpoints. Additionally, you must format the training data with suitable suffix information, include white spaces in the correct location, and format the file in the expected jsonl (json lines format). When making the completion request, it is crucial to use the same suffixes, stop information, and whitespace characters correctly.

However, with OpenAILib (this library), all of these details are handled for you. This makes it much simpler to get started with fine-tuning. Example below:

```csharp
// Create some training data for classification
// This is far too little training data to create a robust model but will train quickly (< 5 minutes) / cheaply (<$0.01)
// and provide a walkthrough of how fine tuning works - see https://platform.openai.com/docs/guides/fine-tuning for more details.
// Samples below are from fetch20_newgroups (as discussed on the openai link above)

// The nice thing about this approach is you can source your data from anywhere (Excel file, csv, database, service, etc.). It is not necessary
// to deal with jsonl formatting, prompt suffixes, completion suffixes and stop information. A content-addressing strategy is utilized meaning that
// the same training data will not be uploaded more than once
var trainingData = new List<FineTunePair>
{
    // hockey examples
    new (prompt: "NHL: Oilers trade Anderson to Leafs", completion: "hockey"),
    new (prompt: "Hockey Equipment for Sale", completion: "hockey"),
    new (prompt: "NHL: Senators fire head coach", completion: "hockey"),
    new (prompt: "Re: NHL All-Rookie Team", completion: "hockey"),
    new (prompt: "Re: Rookie Hockey player wins tournament", completion: "hockey"),
    new (prompt: "Re: New Hockey Trivia Question", completion: "hockey"),
    new (prompt: "Re: NHL Playoff Predictions", completion: "hockey"),
    new (prompt: "Re: College Hockey: NCAA Tourney", completion: "hockey"),
    new (prompt: "Re: NHL Team to Oklahoma City?", completion: "hockey"),
    new (prompt: "Re: Hockey Pool Update", completion: "hockey"),

    // baseball examples
    new (prompt: "MLB: Angels trade for Justin Upton", completion: "baseball"),
    new (prompt: "Baseball Card Values", completion: "baseball"),
    new (prompt: "MLB: Yankees sign Gerrit Cole to record deal", completion: "baseball"),
    new (prompt: "Re: 1996 Yankees vs 2004 Red Sox", completion: "baseball"),
    new (prompt: "Re: NL East: Mets vs. Braves", completion: "baseball"),
    new (prompt: "Re: Red Sox Dynasty?", completion: "baseball"),
    new (prompt: "Re: Yankees/Diamondbacks Game 7", completion: "baseball"),
    new (prompt: "Re: Baseball Trivia Question", completion: "baseball"),
    new (prompt: "Re: MLB Playoff Predictions", completion: "baseball"),
    new (prompt: "Re: College Baseball: NCAA Tourney", completion: "baseball")
};

// Create the fine tune using OpenAI default model ('curie') and parameters
var fineTunedModel = await client.FineTuning.CreateFineTuneAsync(trainingData);

// Wait for it to complete - this can take several minutes
// Typically an initial message stating that the fine tune job has been created is shown immediately, then
// a few minutes later the job will get queued and finally messages regarding the training process are provided.
// Once the job starts it usually only takes 30s or so to train - please be patient waiting for the job
// to start however.
await foreach (var evt in client.FineTuning.GetEventStreamAsync(fineTunedModel))
{
    Console.WriteLine(evt.Message);
}

Console.WriteLine("Write a blog post title about hockey or baseball, we will try to classify it - type 'done' to quit");
string question;
while ((question = Console.ReadLine()) != "done")
{
    var fineTunedCompletion = await client.GetCompletionAsync(question, spec => spec.Model(fineTunedModel));
    Console.WriteLine(fineTunedCompletion);
}

// Delete the model and associated training data when done
await client.FineTuning.DeleteFineTuneAsync(fineTunedModel, deleteTrainingFiles: true);
```

### Saving fine-tune information for later use

The resulting object 'fineTunedModel' (FineTuneInfo) can be serialized to json and saved to a file/db/service and used at a later time to create completions, or check the status of the fine tune. Use System.Text.Json.JsonSerializer for this purpose.

### Fine-tune variants
It is possible to create a variant of the original fine tune using 'CreateFineTuneVariantAsync' using a different model or parameters. Training data does not need to be uploaded again and multiple variants can be created simultaneously

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

# Release Notes
2.0.0 - Bumped version to 2.0.0 as the code-davinci models have been deprecated by OpenAI (March 23, 2023). Associated 'CompletionModels' constant has been removed representing a (minor) breaking change. This version includes simplified fine-tuning capability and support for ChatGPT4.0 (ChatCompletionModels.Gpt40).
