using static System.ConsoleColor;
namespace Runner;

// ========================================================
public class MenuPackage : ConsoleMenu.Entry
{
    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public override string Headline() => "Manage Packages";

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public override void Execute()
    {
        var root = Program.GetSolutionDirectory();

        int position = 0; do
        {
            var projects = Program.FindProjects(root);
            projects = [.. projects.Where(x => x.GetVersion(out _))];
            if (projects.Count == 0)
            {
                Console.WriteLineEx(true, Red, "Cannot find any project.");
                return;
            }

            var items = projects.Select(x => new ConsoleMenu.Entry(x.ToString()));
            var menu = new ConsoleMenu { ToDebug = Program.ToDebug, Timeout = Program.Timeout }
            .Add(new("Exit"))
            .AddRange(items);

            Console.PreventBufferOverflow(menu.Count + 5);
            Console.WriteLineEx(true);
            Console.WriteLineEx(true, Green, Program.FatSeparator);
            Console.WriteLineEx(true, Green, Headline());
            Console.WriteLineEx(true);

            if (position >= menu.Count) position = 0;
            position = menu.Run(position);

            if (position > 0)
            {
                var project = projects[position - 1];
                Execute(project);
            }
        }
        while (position > 0);
    }

    // ----------------------------------------------------

    /// <summary>
    /// Invoked to select the action to execute with the given project.
    /// </summary>
    static void Execute(ProjectFile project)
    {
        var menu = new ConsoleMenu { ToDebug = Program.ToDebug, Timeout = Program.Timeout }
        .Add(new("Exit"))
        .Add(new("Change version"))
        .Add(new("Compile Debug"))
        .Add(new("Compile Local"))
        .Add(new("Compile Release"));

        int position = 0; do
        {
            Console.PreventBufferOverflow(menu.Count + 5);
            Console.WriteLineEx(true);
            Console.WriteLineEx(true, Green, Program.FatSeparator);
            Console.WriteLineEx(true, Green, project.ToString());
            Console.WriteLineEx(true);
            position = menu.Run(position);
        }
        while (position > 0);
    }
}