using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.AI;
using Azure.AI.OpenAI;
using Azure.Identity;

var config = new ConfigurationBuilder()
    .AddUserSecrets<Program>()
    .Build();
string endpoint = config["AZURE_OPENAI_ENDPOINT"] ?? throw new InvalidOperationException("AZURE_OPENAI_ENDPOINT not found");
string deployment = config["AZURE_OPENAI_GPT_NAME"] ?? throw new InvalidOperationException("AZURE_OPENAI_GPT_NAME not found");
string apiKey = config["AZURE_OPENAI_API_KEY"] ?? throw new InvalidOperationException("AZURE_OPENAI_API_KEY not found");

IChatClient client =
    new AzureOpenAIClient(new Uri(endpoint), new Azure.AzureKeyCredential(apiKey))
        .GetChatClient(deployment)
        .AsIChatClient();

string text = File.ReadAllText("benefits.md");
string prompt = $"""
    Summarize the the following text in 20 words or less:
    {text}
    """;

// Submit the prompt and print out the response.
ChatResponse response = await client.GetResponseAsync(
    prompt,
    new ChatOptions { MaxOutputTokens = 400 });
Console.WriteLine(response);