using Azure.AI.OpenAI;
using Microsoft.Extensions.AI;
using ModelContextProtocol.Client;
using Microsoft.AspNetCore.SignalR;
using System.Diagnostics;

var builder = WebApplication.CreateBuilder(args);
builder.Configuration.AddUserSecrets<Program>();
builder.WebHost.UseUrls("http://localhost:5001");

// Add services
builder.Services.AddSignalR();


builder.Services.AddLogging(logging => logging.AddConsole());

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
    private readonly ILogger<ChatHub> _logger;
    private static IMcpClient? _mcpClient;
    private readonly Dictionary<string, List<ChatMessage>> _conversations = new();
    private static readonly ActivitySource ActivitySource = new("PeopleTeamsChat");

    public ChatHub(IChatClient chatClient, ILogger<ChatHub> logger)
    {
        _chatClient = chatClient;
        _logger = logger;
    }

    private async Task<IMcpClient> GetMcpClientAsync()
    {
        using var activity = ActivitySource.StartActivity("GetMcpClient");
        
        if (_mcpClient == null)
        {
            using var initActivity = ActivitySource.StartActivity("InitializeMcpClient");
            _logger.LogInformation("Initializing MCP client connection");
            
            _mcpClient = await McpClientFactory.CreateAsync(
                new StdioClientTransport(new()
                {
                    Command = "dotnet",
                    Arguments = ["run", "--project", "../PeopleTeamsMcpServer/PeopleTeamsMcpServer.csproj"],
                    WorkingDirectory = Directory.GetCurrentDirectory(),
                    Name = "People Team MCP Server",
                }));
            
            _logger.LogInformation("MCP client initialized successfully");
        }
        return _mcpClient;
    }

    public async Task SendMessage(string message)
    {
        using var activity = ActivitySource.StartActivity("SendMessage");
        var connectionId = Context.ConnectionId;
        
        _logger.LogInformation("Received message from {ConnectionId}: {Message}", connectionId, message);
        activity?.SetTag("connection.id", connectionId);
        activity?.SetTag("message.length", message.Length);
        
        if (!_conversations.ContainsKey(connectionId))
            _conversations[connectionId] = new List<ChatMessage>();

        _conversations[connectionId].Add(new ChatMessage(ChatRole.User, message));

        using var mcpActivity = ActivitySource.StartActivity("McpOperations");
        var mcpClient = await GetMcpClientAsync();
        
        using var toolsActivity = ActivitySource.StartActivity("ListTools");
        var tools = await mcpClient.ListToolsAsync();
        _logger.LogInformation("Retrieved {ToolCount} tools from MCP server", tools.Count);
        toolsActivity?.SetTag("tools.count", tools.Count);
        toolsActivity?.Stop();
        
        mcpActivity?.Stop();
        
        using var aiActivity = ActivitySource.StartActivity("GetAIResponse");
        var updates = new List<ChatResponseUpdate>();
        var responseText = new System.Text.StringBuilder();
        
        await foreach (var update in _chatClient.GetStreamingResponseAsync(
            _conversations[connectionId], new() { Tools = [.. tools] }))
        {
            if (!string.IsNullOrEmpty(update.Text))
                responseText.Append(update.Text);
            updates.Add(update);
        }
        aiActivity?.Stop();
        
        if (responseText.Length > 0)
        {
            _logger.LogInformation("Sending response to {ConnectionId}, length: {Length}", connectionId, responseText.Length);
            await Clients.Caller.SendAsync("ReceiveMessage", responseText.ToString(), CancellationToken.None);
        }
        
        _conversations[connectionId].AddMessages(updates);
        _logger.LogInformation("Completed message processing for {ConnectionId}", connectionId);
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        _conversations.Remove(Context.ConnectionId);
        await base.OnDisconnectedAsync(exception);
    }
}