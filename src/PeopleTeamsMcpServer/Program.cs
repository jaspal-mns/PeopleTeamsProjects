using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using PeopleTeamsProjectsRepository.Repository;

var builder = Host.CreateApplicationBuilder(args);
builder.Logging.AddConsole(consoleLogOptions =>
{
    // Configure all logs to go to stderr
    consoleLogOptions.LogToStandardErrorThreshold = LogLevel.Trace;
});

builder.Services
    .AddMcpServer()
    .WithStdioServerTransport()
    .WithToolsFromAssembly();

builder.Services.AddSingleton<Repositories>();

var logger = LoggerFactory.Create(builder => builder.AddConsole()).CreateLogger("PeopleTeamsMcpServer");
logger.LogInformation("Starting PeopleTeams MCP Server at {Timestamp}", DateTime.UtcNow);

await builder.Build().RunAsync();