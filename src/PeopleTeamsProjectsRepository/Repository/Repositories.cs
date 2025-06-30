using System.Text;
using PeopleTeamsProjectsRepository.Data;
using PeopleTeamsProjectsSample.Repository;

namespace PeopleTeamsProjectsRepository.Repository;

public class Repositories
{
    // Instantiate in-memory repositories with seeded data
    public IRepository<Person> People { get; } = new InMemoryRepository<Person>(SeedData.People);
    public IRepository<Team> Teams { get; } = new InMemoryRepository<Team>(SeedData.Teams);
    public IRepository<Project> Projects { get; } = new InMemoryRepository<Project>(SeedData.Projects);
    // ---- Local Functions --------------------------------------
    public string ToFullList()
    {
        var builder = new StringBuilder();

        // People ------------------------------------------------
        builder.AppendLine("People:");
        foreach (var p in People.GetAll())
            builder.AppendLine($"  • {p.Name}");

        // Teams -------------------------------------------------
        builder.AppendLine();
        builder.AppendLine("Teams:");
        foreach (var t in Teams.GetAll())
        {
            var memberNames = t.MemberIds.Select(id => People.Get(id)?.Name ?? "?");
            builder.AppendLine($"  • {t.Name}  [Members: {string.Join(", ", memberNames)}]");
        }

        // Projects with teams & members -------------------------
        builder.AppendLine();
        builder.AppendLine("Projects:");
        foreach (var pr in Projects.GetAll())
        {
            builder.AppendLine($"  • {pr.Name}");

            if (pr.TeamIds.Count == 0)
            {
                builder.AppendLine("      – <no teams>");
                continue;
            }

            foreach (var teamId in pr.TeamIds)
            {
                var team = Teams.Get(teamId);
                if (team is null) continue;

                var memberNames = team.MemberIds.Select(id => People.Get(id)?.Name ?? "?");
                builder.AppendLine($"      – {team.Name}: {string.Join(", ", memberNames)}");
            }
        }

        return builder.ToString();
    }

    //AddPersonToTeam
    public string AddPersonToTeam(string personName, string teamName)
    {
        var person = People.GetAll().FirstOrDefault(p => p.Name.Equals(personName, StringComparison.OrdinalIgnoreCase));
        if (person is null)
            return $"Person '{personName}' not found.";

        var team = Teams.GetAll().FirstOrDefault(t => t.Name.Equals(teamName, StringComparison.OrdinalIgnoreCase));
        if (team is null)
            return $"Team '{teamName}' not found.";

        // Update person with new team
        person = person with { TeamId = team.Id };
        People.Update(person);

        // Add person to team's member list
        team = team with { MemberIds = team.MemberIds.Append(person.Id).ToList() };
        Teams.Update(team);

        return $"Added {person.Name} to {team.Name}.";
    }
}