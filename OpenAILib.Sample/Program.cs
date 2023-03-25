// Copyright (c) 2023 Owen Sigurdson
// MIT License

using OpenAILib;
using OpenAILib.Sample;

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

// returns a high dimensional normalized vector representing the specified text
var vector = await client.GetEmbeddingAsync("dog");

// Should be fairly close to 1.0 as the vector is normalized
Console.WriteLine($"length: {vector.Length}, sum: {vector.Sum()}");


// Try fine tuning
await FineTuningSample.Sample01Async();






