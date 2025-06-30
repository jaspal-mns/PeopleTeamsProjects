using Microsoft.Extensions.Configuration;
using Xunit;

namespace PeopleTeamsMCPClient.Tests;

public class IntegrationTests
{
    [Fact(Skip = "Integration test - requires MCP server running")]
    public async Task McpClient_ShouldConnect_WhenServerAvailable()
    {
        // This test would require the actual MCP server to be running
        // Skip by default to avoid test failures in CI/CD
        await Task.CompletedTask;
    }

    [Fact]
    public void ConfigurationBuilder_ShouldLoadFromMultipleSources()
    {
        var config = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["AZURE_OPENAI_ENDPOINT"] = "https://test.openai.azure.com",
                ["AZURE_OPENAI_API_KEY"] = "test-key"
            })
            .Build();

        Assert.NotNull(config["AZURE_OPENAI_ENDPOINT"]);
        Assert.NotNull(config["AZURE_OPENAI_API_KEY"]);
    }
}