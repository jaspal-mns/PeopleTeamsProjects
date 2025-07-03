using System;
using System.IO;
using System.Threading.Tasks;
using ModelContextProtocol.Client;
using Xunit;
using Xunit.Abstractions;

namespace PeopleTeamsProjectsTests;

public class McpTests(ITestOutputHelper outputHelper)
{
    [Fact]
    public async Task Connect()
    {
        var filePath = GetFilePath("src/PeopleTeamsMcpServer/PeopleTeamsMcpServer.csproj");
        outputHelper.WriteLine($"MCP project {filePath}");
        // new SseClientTransport()
        await using var mcpClient = await McpClientFactory.CreateAsync(new StdioClientTransport(new()
        {
            Name = "People Teams Server",
            Command = "dotnet",
            Arguments = ["run","--project", filePath],
        }));
        var tools = await mcpClient.ListToolsAsync();
        foreach (var tool in tools)
        {
            outputHelper.WriteLine($"{tool.Name}: {tool.Description} {tool.JsonSchema}");
        }
        
    }

    private string GetFilePath( string fileName)
    {
        string currentDirectory = AppDomain.CurrentDomain.BaseDirectory;
        while (Directory.GetParent(currentDirectory) != null)
        {
            currentDirectory = Directory.GetParent(currentDirectory)!.FullName;
            string filePath = Path.Combine(currentDirectory, fileName);
            if (File.Exists(filePath))
            {
                return filePath;
            }
        }
        throw new FileNotFoundException($"File {fileName} not found in resources or bin folder");
    }

}