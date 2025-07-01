using System.ComponentModel;
using ModelContextProtocol.Server;
using PeopleTeamsProjectsRepository.Repository;
using System.Diagnostics;
using Microsoft.Extensions.Logging;

namespace PeopleTeamsMcpServer;

[McpServerToolType]
public static class PeopleTeamsProjectTool
{
    private static readonly ActivitySource ActivitySource = new("PeopleTeamsMcpServer");
    private static readonly ILogger Logger = LoggerFactory.Create(builder => builder.AddConsole()).CreateLogger("PeopleTeamsProjectTool");

    [McpServerTool, Description("Lists all projects in the People and Teams context.")]
    public static string List(Repositories repositories)
    {
        using var activity = ActivitySource.StartActivity("List");
        Logger.LogInformation("Executing List tool at {Timestamp}", DateTime.UtcNow);
        
        var result = repositories.ToFullList();
        Logger.LogInformation("List tool completed, result length: {Length}", result.Length);
        
        return $"Hello from C#: {result}";
    }

    [McpServerTool, Description("Ability to add person to a team.")]
    public static string AddPersonToTeam(Repositories repositories, string personName, string teamName)
    {
        using var activity = ActivitySource.StartActivity("AddPersonToTeam");
        activity?.SetTag("person.name", personName);
        activity?.SetTag("team.name", teamName);
        
        Logger.LogInformation("Executing AddPersonToTeam: {PersonName} to {TeamName} at {Timestamp}", personName, teamName, DateTime.UtcNow);
        
        var result = repositories.AddPersonToTeam(personName, teamName);
        Logger.LogInformation("AddPersonToTeam completed for {PersonName}", personName);
        
        return $"Hello from C#: {result}";
    }
}