using static Yotei.Tools.ConsoleEx;
using static System.ConsoleColor;

namespace Runner;

// =============================================================
public class MenuPackages : ConsoleMenuEntry
{
    /// <inheritdoc/>
    public override string Header() => "Manage Projects";

    /// <inheritdoc/>
    public override void Execute()
    {
        var position = 0; do
        {
            Console.Clear();
            WriteLine(true);
            WriteLine(true, Green, Program.FatSeparator);
            WriteLine(true, Green, Header());
            WriteLine(true);

            var root = Program.GetSolutionDirectory();
            var projects = Program.FindProjects(root);
            var items = projects.Select(x => new ConsoleMenuEntry(x.NameVersion)).ToArray();

            var menu = new ConsoleMenu { new("Exit") };
            menu.AddRange(items);

            if (position > items.Length) position = 0;
            position = menu.Run(Program.MenuOptions);
            
            if (position > 0)
            {
                var project = projects[position - 1];
                Execute(project);
            }
        }
        while (position > 0);
    }

    // ---------------------------------------------------------

    /// <summary>
    /// Executes on the given project.
    /// </summary>
    /// <param name="project"></param>
    static void Execute(Project project)
    {
        var items = new List<ConsoleMenuEntry> { new("Exit") };
        if (project.GetVersion(out _)) items.Add(new ChangeVersion(project));
        items.Add(new("Compile Debug"));
        items.Add(new("Compile Local"));
        items.Add(new("Compile Release"));

        var menu = new ConsoleMenu().AddRange([.. items]);

        Console.Clear();
        int position; do
        {
            WriteLine(true);
            WriteLine(true, Green, Program.FatSeparator);
            WriteLine(true, Green, project.NameVersion);
            WriteLine(true);

            position = menu.Run(Program.MenuOptions);
        }
        while (position > 0);
    }
}