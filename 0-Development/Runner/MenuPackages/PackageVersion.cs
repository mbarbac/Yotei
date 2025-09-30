using static Yotei.Tools.ConsoleEx;
using static System.ConsoleColor;

namespace Runner;

// =============================================================
public class PackageVersion : ConsoleMenuEntry
{
    /// <inheritdoc/>
    public override string Header() => "Change Package Version";

    /// <inheritdoc/>
    public override void Execute()
    {
        Console.Clear();
        var option = 0; do
        {
            var root = Program.GetSolutionDirectory();
            var projects = Program.FindProjects(root);
            projects = [.. projects.Where(x => x.GetVersion(out _))];
            projects.Sort((x, y) => x.Name.CompareTo(y.Name));

            if (projects.Count == 0)
            {
                WriteLine(true);
                Write(true, Red, "No versioned projects found from: "); WriteLine(true, root);
                return;
            }

            WriteLine(true);
            WriteLine(true, Green, Program.FatSeparator);
            WriteLine(true, Green, Header());

            var menu = new ConsoleMenu { new("Exit") };
            var items = projects.Select(x => new ConsoleMenuEntry(x.NameVersion)).ToArray();
            menu.AddRange(items);
            option = menu.Run(Program.MenuOptions, option);

            if (option > 0)
            {
                var project = projects[option - 1];
                var backups = new BuildBackups();
                var saved = backups.Add(project);
                saved.AddRange(project);

                try { Execute(project, backups); }
                catch (Exception ex)
                {
                    WriteLine(true);
                    WriteLine(true, Red, "Exception intercepted:");
                    WriteLine(true, ex.ToDisplayString());

                    WriteLine(true);
                    WriteLine(true, Green, Program.SlimSeparator);
                    WriteLine(true, Green, "Reverting to previous state...");
                    backups.Restore();

                    WriteLine(true);
                    Write(true, Green, "Press [Enter] to continue...");
                    Console.ReadLine();
                }
            }
        }
        while (option > 0);
    }

    // ----------------------------------------------------

    /// <summary>
    /// Invoked to manage the version of the given project.
    /// </summary>
    static void Execute(Project project, BuildBackups backups)
    {
        bool done;
        string? str;

        WriteLine(true);
        WriteLine(true, Green, Program.SlimSeparator);
        Write(true, Green, "Project: "); WriteLine(true, project.NameVersion);

        if (!project.GetVersion(out var original))
        {
            WriteLine(true);
            WriteLine(true, Red, "Cannot obtain the version of this project.");
            return;
        }

        WriteLine(true);
        Write(true, Green, "New version: ");
        //done = EditLine(true, Program.Timeout, original, out str);


        str = EditLine(true, Program.Timeout, original);
        if (str is null || str.Length == 0) return;
        var updated = new SemanticVersion(str);

        WriteLine(true);
        Write(true, Green, "'Release' references ([Blank|Escape] to ignore): ");
        str = EditLine(true, Program.Timeout, updated with { PreRelease = "" });
        var reduced = new SemanticVersion(str ?? "");

        Write(true, Green, "'Local' references ([Blank|Escape] to ignore):   ");
        str = EditLine(true, Program.Timeout, updated.PreRelease.IsEmpty ? updated with { PreRelease = "v0001" } : updated);
        var enlarged = new SemanticVersion(str ?? "");

        // Setting the project's version...
        if (!project.SetVersion(updated)) throw new Exception("Cannot set project version.");
        project.SaveContents();

        // Updating references...
        if (reduced.IsEmpty && enlarged.IsEmpty) return;

        var root = Program.GetSolutionDirectory();
        var items = Program.FindProjects(root);
        var first = true;

        foreach (var item in items)
        {
            // Pre-saving state...
            var temps = item.Select(x => new ProjectLine(x)).ToList();
            var modified = false;

            // Iterating item's lines...
            var ilines = item.Where(x => x.IsNuReference()).ToList();
            foreach (var iline in ilines)
            {
                if (!iline.GetNuName(out var iname)) continue;
                if (!iline.GetNuVersion(out var iversion)) continue;
                if (string.Compare(project.Name, iname, ignoreCase: true) != 0) continue;

                if (!modified) // First-time modification...
                {
                    if (first) WriteLine(true);
                    Write(true, Green, "Updating: "); WriteLine(true, item.NameExtension);
                    first = false;

                    var saved = backups.Add(item);
                    saved.Clear();
                    saved.AddRange(temps);
                    modified = true;
                }

                // We'll ignore empty values, but return 'true' to keep it going...
                done = iversion.PreRelease.IsEmpty switch
                {
                    true => reduced.IsEmpty || iline.SetNuVersion(reduced),
                    false => enlarged.IsEmpty || iline.SetNuVersion(enlarged),
                };
                if (!done) throw new Exception($"Cannot modify line: {iline}");
            }

            // If modified save contents...
            if (modified) item.SaveContents();
        }
    }
}