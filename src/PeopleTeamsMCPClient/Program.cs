using Azure.AI.OpenAI;
using Microsoft.Extensions.AI;
using ModelContextProtocol.Client;
using Microsoft.AspNetCore.SignalR;

var builder = WebApplication.CreateBuilder(args);
builder.Configuration.AddUserSecrets<Program>();
builder.WebHost.UseUrls("http://localhost:5001");

// Add services
builder.Services.AddSignalR();

// Configure Azure OpenAI
string endpoint = builder.Configuration["AZURE_OPENAI_ENDPOINT"] ?? throw new InvalidOperationException("AZURE_OPENAI_ENDPOINT not found");
string apiKey = builder.Configuration["AZURE_OPENAI_API_KEY"] ?? throw new InvalidOperationException("AZURE_OPENAI_API_KEY not found");

IChatClient chatClient = new ChatClientBuilder(
    new AzureOpenAIClient(new Uri(endpoint), new Azure.AzureKeyCredential(apiKey))
    .GetChatClient("gpt-4o-mini").AsIChatClient())
    .UseFunctionInvocation()
    .Build();

builder.Services.AddSingleton(chatClient);

var app = builder.Build();

app.UseDefaultFiles();
app.UseStaticFiles();
app.MapHub<ChatHub>("/chathub");
app.MapFallbackToFile("index.html");

app.Run();

public class ChatHub : Microsoft.AspNetCore.SignalR.Hub
{
    private readonly IChatClient _chatClient;
    private static IMcpClient? _mcpClient;
    private readonly Dictionary<string, List<ChatMessage>> _conversations = new();

    public ChatHub(IChatClient chatClient)
    {
        _chatClient = chatClient;
    }

    private async Task<IMcpClient> GetMcpClientAsync()
    {
        if (_mcpClient == null)
        {
            _mcpClient = await McpClientFactory.CreateAsync(
                new StdioClientTransport(new()
                {
                    Command = "dotnet",
                    Arguments = ["run", "--project", "../PeopleTeamsMcpServer/PeopleTeamsMcpServer.csproj"],
                    WorkingDirectory = Directory.GetCurrentDirectory(),
                    Name = "People Team MCP Server",
                }));
        }
        return _mcpClient;
    }

    public async Task SendMessage(string message)
    {
        var connectionId = Context.ConnectionId;
        
        if (!_conversations.ContainsKey(connectionId))
            _conversations[connectionId] = new List<ChatMessage>();

        _conversations[connectionId].Add(new ChatMessage(ChatRole.User, message));

        var mcpClient = await GetMcpClientAsync();
        var tools = await mcpClient.ListToolsAsync();
        
        var updates = new List<ChatResponseUpdate>();
        var responseText = new System.Text.StringBuilder();
        
        await foreach (var update in _chatClient.GetStreamingResponseAsync(
            _conversations[connectionId], new() { Tools = [.. tools] }))
        {
            if (!string.IsNullOrEmpty(update.Text))
                responseText.Append(update.Text);
            updates.Add(update);
        }
        
        if (responseText.Length > 0)
            await Clients.Caller.SendAsync("ReceiveMessage", responseText.ToString(), CancellationToken.None);
        
        _conversations[connectionId].AddMessages(updates);
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        _conversations.Remove(Context.ConnectionId);
        await base.OnDisconnectedAsync(exception);
    }
}