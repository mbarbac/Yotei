using static Yotei.Tools.ConsoleEx;
using static System.ConsoleColor;

namespace Runner;

// ========================================================
public class ChangeVersion(Project project) : ConsoleMenuEntry
{
    readonly Project Project = project.ThrowWhenNull();

    /// <inheritdoc/>
    public override string Header() => "Change Version";

    /// <inheritdoc/>
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
            WriteLine(true);
            WriteLine(true, Red, "Exception intercepted:");
            WriteLine(true, e.ToDisplayString());

            WriteLine(true);
            WriteLine(true, Green, Program.SlimSeparator);
            WriteLine(true, Green, "Reverting to previous state...");
            backups.Restore(display: true);

            WriteLine(true);
            Write(true, Green, "Press [Enter] to continue...");
            
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
        WriteLine(true);
        WriteLine(true, Green, Program.SlimSeparator);
        Write(true, Green, "Project: "); WriteLine(true, project.Name);

        if (!project.GetVersion(out var original))
        {
            WriteLine(true);
            WriteLine(true, Red, "Cannot obtain the original version of this project.");
            return false;
        }

        WriteLine(true);
        Write(true, Green, "Current Version: "); WriteLine(true, original);
        Write(true, Green, "    New Version: ");
        var str = EditLine(true, Program.Timeout, original);
        if (str is null || str.Length == 0) return false;
        var updated = new SemanticVersion(str);

        var reduced = updated with { PreRelease = "" };
        WriteLine(true);
        Write(true, Green, "Release Version: ");
        str = EditLine(true, Program.Timeout, reduced);
        reduced = new SemanticVersion(str ?? "");

        var enlarged = updated.PreRelease.IsEmpty ? updated with { PreRelease = "v0001" } : updated;
        Write(true, Green, "  Local Version: ");
        str = EditLine(true, Program.Timeout, enlarged);
        enlarged = new SemanticVersion(str ?? "");

        // Setting project's version...
        if (!project.SetVersion(updated)) throw new Exception("Cannot set project version.");
        project.SaveContents();

        // Updating references if needed...
        if (reduced.IsEmpty && enlarged.IsEmpty) return true; 

        var root = Program.GetSolutionDirectory();
        var items = Program.FindProjects(root);
        var first = true;

        WriteLine(true);
        WriteLine(true, Green, "Updating references...");
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
                    if (first) WriteLine(true); first = false;
                    Write(true, Green, "Updating: "); WriteLine(true, item.NameExtension);

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