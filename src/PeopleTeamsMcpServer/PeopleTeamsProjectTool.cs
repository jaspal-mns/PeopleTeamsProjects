using System.ComponentModel;
using ModelContextProtocol.Server;
using PeopleTeamsProjectsRepository.Repository;

namespace PeopleTeamsMcpServer;

[McpServerToolType]
public static class PeopleTeamsProjectTool
{
    [McpServerTool, Description("Lists all projects in the People and Teams context.")]
    public static string List(Repositories repositories) => $"Hello from C#: {repositories.ToFullList()}";

}