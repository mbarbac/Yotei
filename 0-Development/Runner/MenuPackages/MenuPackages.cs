using static System.ConsoleColor;

namespace Runner;

// ========================================================
public class MenuPackages : ConsoleMenuEntry
{
    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns><inheritdoc/></returns>
    public override string Header() => "Manage Projects";

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public override void Execute()
    {
        var position = 0; do
        {
            Console.Clear();
            Console.WriteLine(true, "");
            Console.WriteLine(true, Green, Program.FatSeparator);
            Console.WriteLine(true, Green, Header());
            Console.WriteLine(true, "");

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

    // ----------------------------------------------------

    /// <summary>
    /// Executes on the given project.
    /// </summary>
    /// <param name="project"></param>
    static void Execute(Project project)
    {
        var items = new List<ConsoleMenuEntry> { new("Exit") };
        if (project.GetVersion(out _)) items.Add(new ChangeVersion(project));
        items.Add(new CompilePackage(project, BuildMode.Debug));
        items.Add(new CompilePackage(project, BuildMode.Local));
        items.Add(new CompilePackage(project, BuildMode.Release));

        var menu = new ConsoleMenu().AddRange([.. items]);

        Console.Clear();
        int position; do
        {
            Console.WriteLine(true, "");
            Console.WriteLine(true, Green, Program.FatSeparator);
            Console.WriteLine(true, Green, project.NameVersion);
            Console.WriteLine(true, "");

            position = menu.Run(Program.MenuOptions);
        }
        while (position > 0);
    }
}