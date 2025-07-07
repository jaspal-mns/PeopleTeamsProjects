using Azure.AI.OpenAI;
using Azure.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.AI;
using ModelContextProtocol.Client;

var config = new ConfigurationBuilder()
    .AddUserSecrets<Program>()
    .AddJsonFile("appsettings.json", optional: true)
    .AddJsonFile("appsettings.Development.json", optional: true)
    .AddEnvironmentVariables()
    .Build();

string endpoint = config["AZURE_OPENAI_ENDPOINT"] ?? throw new InvalidOperationException("AZURE_OPENAI_ENDPOINT not found");
//Print endpoint
Console.WriteLine($"Using Azure OpenAI endpoint: {endpoint}");

string apiKey = config["AZURE_OPENAI_API_KEY"] ?? throw new InvalidOperationException("AZURE_OPENAI_API_KEY not found");


// Create an IChatClient using Azure OpenAI.
IChatClient client =
    new ChatClientBuilder(
        new AzureOpenAIClient(new Uri(endpoint),
        new Azure.AzureKeyCredential(apiKey))
        .GetChatClient("gpt-4").AsIChatClient())
    .UseFunctionInvocation()
    .Build();

// Create the MCP client
// Configure it to start and connect to your MCP server.
IMcpClient mcpClient = await McpClientFactory.CreateAsync(
    new StdioClientTransport(new()
    {
        Command = "dotnet",
        Arguments = ["run"],
        WorkingDirectory = "../PeopleTeamsMcpServer", // Use relative path instead of absolute path
        Name = "People Team MCP Server",
    }));

// List all available tools from the MCP server.
Console.WriteLine("Available tools:");
IList<McpClientTool> tools = await mcpClient.ListToolsAsync();
foreach (McpClientTool tool in tools)
{
    Console.WriteLine($"{tool}");
}
Console.WriteLine();

// Conversational loop that can utilize the tools via prompts.
List<ChatMessage> messages = [];
while (true)
{
    Console.Write("Prompt: ");
    messages.Add(new(ChatRole.User, Console.ReadLine()));

    List<ChatResponseUpdate> updates = [];
    await foreach (ChatResponseUpdate update in client
        .GetStreamingResponseAsync(messages, new() { Tools = [.. tools] }))
    {
        Console.Write(update);
        updates.Add(update);
    }
    Console.WriteLine();

    messages.AddMessages(updates);
}
