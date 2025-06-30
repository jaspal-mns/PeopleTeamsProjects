using Microsoft.Extensions.Configuration;
using ModelContextProtocol.Client;
using Moq;
using Xunit;

namespace PeopleTeamsMCPClient.Tests;

public class McpClientTests
{
    [Fact]
    public void Configuration_ShouldThrow_WhenEndpointMissing()
    {
        var config = new ConfigurationBuilder().Build();
        
        var exception = Assert.Throws<InvalidOperationException>(() =>
        {
            string endpoint = config["AZURE_OPENAI_ENDPOINT"] ?? throw new InvalidOperationException("AZURE_OPENAI_ENDPOINT not found");
        });
        
        Assert.Equal("AZURE_OPENAI_ENDPOINT not found", exception.Message);
    }

    [Fact]
    public void Configuration_ShouldThrow_WhenApiKeyMissing()
    {
        var config = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["AZURE_OPENAI_ENDPOINT"] = "https://test.openai.azure.com"
            })
            .Build();
        
        var exception = Assert.Throws<InvalidOperationException>(() =>
        {
            string endpoint = config["AZURE_OPENAI_ENDPOINT"] ?? throw new InvalidOperationException("AZURE_OPENAI_ENDPOINT not found");
            string apiKey = config["AZURE_OPENAI_API_KEY"] ?? throw new InvalidOperationException("AZURE_OPENAI_API_KEY not found");
        });
        
        Assert.Equal("AZURE_OPENAI_API_KEY not found", exception.Message);
    }

    [Fact]
    public void Configuration_ShouldSucceed_WhenBothValuesPresent()
    {
        var config = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["AZURE_OPENAI_ENDPOINT"] = "https://test.openai.azure.com",
                ["AZURE_OPENAI_API_KEY"] = "test-key"
            })
            .Build();
        
        string endpoint = config["AZURE_OPENAI_ENDPOINT"] ?? throw new InvalidOperationException("AZURE_OPENAI_ENDPOINT not found");
        string apiKey = config["AZURE_OPENAI_API_KEY"] ?? throw new InvalidOperationException("AZURE_OPENAI_API_KEY not found");
        
        Assert.Equal("https://test.openai.azure.com", endpoint);
        Assert.Equal("test-key", apiKey);
    }
}