using System.ComponentModel;
using ModelContextProtocol.Server;
using PeopleTeamsProjectsRepository.Repository;

namespace PeopleTeamsMcpServer;

[McpServerToolType]
public static class PeopleTeamsProjectTool
{
    [McpServerTool, Description("Lists all projects in the People and Teams context.")]
    public static string List(Repositories repositories) => $"Hello from C#: {repositories.ToFullList()}";

    [McpServerTool, Description("Ability to add person to a team.")]
     public static string AddPersonToTeam(Repositories repositories, string personName, string teamName) => $"Hello from C#: {repositories.AddPersonToTeam(personName, teamName)}";

}