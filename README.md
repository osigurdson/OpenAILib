# OpenAILib

Simple OpenAI completions and embeddings library with no dependencies.

## Quick Example

```csharp
var client = new OpenAIClient(organizationId: "organizationId", apiKey: "apiKey");

// likely returns "2"
var result = await client.GetCompletionAsync("1 + 1 = ");

// returns a high dimensional vector representing the specified text
var vector = await client.GetEmbeddingAsync("dog");
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
