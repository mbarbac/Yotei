using static System.ConsoleColor;

namespace Runner;

// ========================================================
public class ChangeVersion(Project project) : ConsoleMenuEntry
{
    readonly Project Project = project.ThrowWhenNull();

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns><inheritdoc/></returns>
    public override string Header() => "Change Version";

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public override void Execute() => OnExecute(interactive: true);

    /// <summary>
    /// Executes on this instance's project and returns the result of that execution.
    /// </summary>
    public bool OnExecute(bool interactive)
    {
        var backups = new BuildBackups();
        var saved = backups.Add(Project);
        saved.AddRange(Project);

        try { return Consumate(backups); }
        catch (Exception e)
        {
            Console.WriteLine(true, "");
            Console.WriteLine(true, Red, "Exception intercepted:");
            Console.WriteLine(true, e.ToDisplayString());

            Console.WriteLine(true, "");
            Console.WriteLine(true, Green, Program.SlimSeparator);
            Console.WriteLine(true, Green, "Reverting to previous state...");
            backups.Restore(display: true);

            Console.WriteLine(true, "");
            Console.Write(true, Green, "Press [Enter] to continue...");
            
            if (interactive) Console.ReadLine();
            return false;
        }
    }

    // ----------------------------------------------------

    /// <summary>
    /// Executes under a backups-protected umbrella.
    /// </summary>
    bool Consumate(BuildBackups backups)
    {
        Console.WriteLine(true, "");
        Console.WriteLine(true, Green, Program.SlimSeparator);
        Console.Write(true, Green, "Project: "); Console.WriteLine(true, project.Name);

        if (!project.GetVersion(out var original))
        {
            Console.WriteLine(true, "");
            Console.WriteLine(true, Red, "Cannot obtain the original version of this project.");
            return false;
        }

        Console.WriteLine(true, "");
        Console.Write(true, Green, "Current Version: "); Console.WriteLine(true, original);
        Console.Write(true, Green, "    New Version: ");
        var str = Console.EditLine(true, Program.Timeout, original);
        if (str is null || str.Length == 0) return false;
        var updated = new SemanticVersion(str);

        var reduced = updated with { PreRelease = "" };
        Console.WriteLine(true, "");
        Console.Write(true, Green, "Release Version: ");
        str = Console.EditLine(true, Program.Timeout, reduced);
        reduced = new SemanticVersion(str ?? "");

        var enlarged = updated.PreRelease.IsEmpty ? updated with { PreRelease = "v0001" } : updated;
        Console.Write(true, Green, "  Local Version: ");
        str = Console.EditLine(true, Program.Timeout, enlarged);
        enlarged = new SemanticVersion(str ?? "");

        // Setting project's version...
        if (!project.SetVersion(updated)) throw new Exception("Cannot set project version.");
        project.SaveContents();

        // Updating references if needed...
        if (reduced.IsEmpty && enlarged.IsEmpty) return true; 

        var root = Program.GetSolutionDirectory();
        var items = Program.FindProjects(root);
        var first = true;

        Console.WriteLine(true, "");
        Console.WriteLine(true, Green, "Updating references...");
        foreach (var item in items)
        {
            // Pre-saving state---
            var temps = item.Select(x => new ProjectLine(x)).ToList();
            var modified = false;

            // Iterating item's lines...
            var ilines = item.Where(x => x.IsNuReference()).ToList();
            foreach (var iline in ilines)
            {
                if (!iline.GetNuName(out var iname)) continue;
                if (!iline.GetNuVersion(out var iversion)) continue;
                if (string.Compare(project.Name, iname, ignoreCase: true) != 0) continue;

                // Saving the item's original state...
                if (!modified)
                {
                    modified = true;
                    if (first) Console.WriteLine(true, ""); first = false;
                    Console.Write(true, Green, "Updating: ");
                    Console.WriteLine(true, item.NameExtension);

                    var saved = backups.Add(item);
                    saved.Clear();
                    saved.AddRange(temps);
                }

                // Modifying the reference...
                var done = iversion.PreRelease.IsEmpty switch
                {
                    true => reduced.IsEmpty || iline.SetNuVersion(reduced),
                    false => enlarged.IsEmpty || iline.SetNuVersion(enlarged),
                };
                if (!done) throw new Exception($"Cannot modify line: {iline}");
            }

            // Saving the modified item...
            if (modified) item.SaveContents();
        }

        // Finishing...
        return true;
    }
}