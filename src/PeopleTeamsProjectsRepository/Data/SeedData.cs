using PeopleTeamsProjectsRepository.Repository;

namespace PeopleTeamsProjectsRepository.Data;

/*─────────────────────────────────────────────────────────────*
 |                         Seed Data                           |
 *────────────────────────────────────────────────────────────*/
static class SeedData
{
    public static readonly Person[]  People;
    public static readonly Team[]    Teams;
    public static readonly Project[] Projects;

    static SeedData()
    {
        // ---------- People ----------
        var alice   = new Person("Alice");
        var bob     = new Person("Bob");
        var charlie = new Person("Charlie");
        var dana    = new Person("Dana");
        var evan    = new Person("Evan");
        var fay     = new Person("Fay");

        People = [ alice, bob, charlie, dana, evan, fay ];

        // ---------- Teams ----------
        var blue   = new Team("Blue",   [alice.Id, bob.Id]);
        var red    = new Team("Red",    [charlie.Id, dana.Id]);
        var green  = new Team("Green",  [evan.Id]);
        var yellow = new Team("Yellow", [fay.Id]);

        Teams = [ blue, red, green, yellow ];

        // Set TeamId on people (records are immutable -> recreate)
        alice   = alice   with { TeamId = blue.Id };
        bob     = bob     with { TeamId = blue.Id };
        charlie = charlie with { TeamId = red.Id };
        dana    = dana    with { TeamId = red.Id };
        evan    = evan    with { TeamId = green.Id };
        fay     = fay     with { TeamId = yellow.Id };
        People = [ alice, bob, charlie, dana, evan, fay ];

        // ---------- Projects & Assignments ----------
        Projects = [
            new Project("Oak Tree",   [blue.Id, red.Id]),
            new Project("Pine Cone",  [green.Id]),
            new Project("Maple Leaf", [yellow.Id]),
            new Project("Cedar Branch", [])
        ];
    }
}