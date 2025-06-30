using PeopleTeamsProjectsRepository.Repository;

// ========== Program / CLI ==========
namespace PeopleTeamsProjectsConsole;

/*─────────────────────────────────────────────────────────────*
 |                      Program / Tool                         |
 *────────────────────────────────────────────────────────────*/
static class Program
{
    static int Main()

    
    {


        var repos = new Repositories();

        Console.WriteLine("People-Teams-Projects tool (type 'help' for commands, 'exit' to quit)");

        while (Console.ReadLine() is { } line)
        {
            var cmd = line.Trim().ToLowerInvariant();
            switch (cmd)
            {
                case "list":
                    Console.Write( repos.ToFullList());
                    break;
                case "help":
                    Console.WriteLine("Commands:\n  list  – list entities\n  exit  – quit");
                    break;
                case "exit":
                case "quit":
                    return 0;
                case "":
                    // Ignore empty line
                    break;
                default:
                    Console.WriteLine($"Unknown command '{cmd}'. Type 'help' for options.");
                    break;
            }
        }

        return 0;
    }
}