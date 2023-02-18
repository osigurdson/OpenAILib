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