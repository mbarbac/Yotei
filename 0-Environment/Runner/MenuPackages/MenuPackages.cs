using static Yotei.Tools.ConsoleExtensions;
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
        var root = Program.GetSolutionDirectory();

        var position = 0; do
        {
            var projects = Program.FindProjects(root);
            projects = [.. projects.Where(x => x.GetVersion(out _))];

            var items = projects.Select(x => new ConsoleMenuEntry(x.ToString())).ToArray();
            var menu = new ConsoleMenu { ToDebug = Program.ToDebug, Timeout = Program.Timeout }
            .Add(new("Exit"))
            .AddRange(items);

            WriteLineEx(true);
            WriteLineEx(true, Green, Program.FatSeparator);
            WriteLineEx(true, Green, Header());
            WriteLineEx(true);

            if (position > items.Length) position = 0;
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
    /// Invoked to select the action to execute on the given project, if any.
    /// </summary>
    /// <param name="project"></param>
    static void Execute(Project project)
    {
        var menu = new ConsoleMenu { ToDebug = Program.ToDebug, Timeout = Program.Timeout }
        .Add(new("Exit"));

        if (project.GetVersion(out _)) menu.Add(new ChangeVersion(project));
        menu.Add(new CompilePackage(project, BuildMode.Debug));
        menu.Add(new CompilePackage(project, BuildMode.Local));
        menu.Add(new CompilePackage(project, BuildMode.Release));

        Console.Clear();
        int position = 0; do
        {
            WriteLineEx(true);
            WriteLineEx(true, Green, Program.FatSeparator);
            WriteLineEx(true, Green, project.ToString());
            WriteLineEx(true);

            position = menu.Run(position);
        }
        while (position > 0);
    }
}