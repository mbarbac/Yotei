using static Yotei.Tools.ConsoleEx;
using static System.ConsoleColor;

namespace Runner;

// ========================================================
public class MenuVersions : ConsoleMenuEntry
{
    /// <inheritdoc/>
    public override string Header() => "Manage Package Versions";

    /// <inheritdoc/>
    public override void Execute()
    {
        var option = 0; do
        {
            var root = Program.GetSolutionDirectory();
            var projects = Program.FindProjects(root);
            projects = projects.Where(x => x.GetVersion(out _)).ToList();
            projects.Sort((x, y) => x.Name.CompareTo(y.Name));

            if (projects.Count == 0)
            {
                WriteLine(true);
                Write(true, Red, "No projects found from: "); WriteLine(true, root);
                return;
            }

            WriteLine(true);
            WriteLine(true, Green, Program.FatSeparator);
            WriteLine(true, Green, Header());

            var menu = new ConsoleMenu { new("Exit") };
            var items = projects.Select(x => new ConsoleMenuEntry(x.NameVersion)).ToList();
            menu.AddRange(items);
            option = menu.Run(true, Green, Program.Timeout, option);

            if (option > 0)
            {
                var project = projects[option - 1];
                Execute(project);
            }
        }
        while (option > 0);
    }

    // ----------------------------------------------------

    /// <summary>
    /// Invoked to manage the version of the given project.
    /// </summary>
    /// <param name="project"></param>
    static void Execute(Project project)
    {
        WriteLine(true);
        WriteLine(true, Green, Program.SlimSeparator);
        Write(true, Green, "Project: "); WriteLine(true, project.NameVersion);

        if (!project.GetVersion(out var original))
        {
            WriteLine(true);
            WriteLine(true, Red, "Cannot obtain the version of this project.");
            return;
        }
        string str = original;

        WriteLine(true);
        Write(true, Green, "New version: ");
        var done = EditLine(true, Program.Timeout, str, out str!);
        if (!done || str is null || str.Length == 0) return;

        // Executing under a backup umbrella...
        var backups = new BuildBackups();
        var saved = backups.Add(project);
        saved.AddRange(project);

        try
        {
            // Setting and saving the project's version...
            var updated = new SemanticVersion(str);
            var reduced = updated.PreRelease.IsEmpty ? updated : updated with { PreRelease = "" };
            var enlarged = updated.PreRelease.IsEmpty ? updated with { PreRelease = "v0001" } : updated;

            if (!project.SetVersion(updated))
            {
                WriteLine(true);
                WriteLine(true, Red, "Cannot set the new version of this project.");
                return;
            }
            project.SaveContents();

            // Updating references in all solution's projects...
            WriteLine(true);
            WriteLine(true, Green, "Updating references... ");

            var root = Program.GetSolutionDirectory();
            var items = Program.FindProjects(root);
            var first = true;

            foreach (var item in items)
            {
                // Pre-saving state...
                var temps = item.Select(x => new ProjectLine(x)).ToList();
                var modified = false;

                // Iterating through item's lines...
                var ulines = item.Where(x => x.IsNuReference()).ToList();
                foreach (var uline in ulines)
                {
                    if (!uline.GetNuName(out var uname)) continue;
                    if (!uline.GetNuVersion(out var uversion)) continue;
                    if (string.Compare(project.Name, uname, ignoreCase: true) != 0) continue;

                    if (!modified) // First-time modification...
                    {
                        if (first) WriteLine(true);
                        Write(true, Green, "Modifying: "); WriteLine(true, item.NameExtension);
                        first = false;

                        saved = backups.Add(item);
                        saved.Clear();
                        saved.AddRange(temps);
                        modified = true;
                    }

                    done = uversion.PreRelease.IsEmpty
                        ? uline.SetNuVersion(reduced)
                        : uline.SetNuVersion(enlarged);

                    if (!done) throw new Exception($"Cannot modify line: {uline}");
                }

                // If modified, saved the new contents...
                if (modified) item.SaveContents();
            }

        }
        catch (Exception ex) // Intercepting failures and reverting to original state...
        {
            WriteLine(true);
            WriteLine(true, Red, "Exception intercepted:");
            WriteLine(true, ex.ToDisplayString());

            WriteLine(true);
            WriteLine(true, Red, "Reverting to previous state...");
            backups.Restore();

            WriteLine(true);
            Write(true, Green, "Press [Enter] to continue...");
            Console.ReadLine();
        }
    }
}