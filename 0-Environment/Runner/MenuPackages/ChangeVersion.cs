using static Yotei.Tools.ConsoleExtensions;
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
    public override string Header() => $"Change Version";

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public override void Execute()
    {
        var backups = new BackupMaster();
        var saved = backups.Add(Project);

        try
        {
            if (!OnExecute(backups))
            {
                WriteLineEx(true);
                WriteLineEx(true, Green, Program.SlimSeparator);
                WriteLineEx(true, Green, "Reverting to previous state...");

                backups.Restore(display: true);

                WriteLineEx(true);
                WriteEx(true, Green, "Press [Enter] to continue...");
                Console.ReadLine();
            }
        }
        catch (Exception ex)
        {
            WriteLineEx(true);
            WriteLineEx(true, Red, "Exception intercepted:");
            WriteLineEx(true, ex.ToDisplayString());

            WriteLineEx(true);
            WriteLineEx(true, Green, Program.SlimSeparator);
            WriteLineEx(true, Green, "Reverting to previous state...");

            backups.Restore(display: true);
            WriteEx(true, Green, "Press [Enter] to continue...");
            Console.ReadLine();
        }
    }

    // ----------------------------------------------------

    /// <summary>
    /// Processes this package under the protection of a backups umbrella.
    /// </summary>
    bool OnExecute(BackupMaster backups)
    {
        WriteLineEx(true);
        WriteLineEx(true, Green, Program.SlimSeparator);
        WriteEx(true, Green, "Project: "); WriteLineEx(true, project.Name);

        if (!project.GetVersion(out var original))
        {
            WriteLineEx(true);
            WriteLineEx(true, Red, "Cannot obtain the original version of this project.");
            return false;
        }

        WriteLineEx(true);
        WriteEx(true, Green, "Current Version: "); WriteLineEx(true, original);
        WriteEx(true, Green, "    New Version: ");

        var str = EditLineEx(true, Program.Timeout, original);
        if (str is null || str.Length == 0) return false;
        var updated = new SemanticVersion(str);

        var reduced = updated with { PreRelease = "" };
        WriteLineEx(true);
        WriteEx(true, Green, "Release Version: ");
        str = EditLineEx(true, Program.Timeout, reduced);
        reduced = new SemanticVersion(str ?? "");

        var enlarged = updated.PreRelease.IsEmpty ? updated with { PreRelease = "v0001" } : updated;
        WriteEx(true, Green, "  Local Version: ");
        str = EditLineEx(true, Program.Timeout, enlarged);
        enlarged = new SemanticVersion(str ?? "");

        // Setting project's version...
        if (!project.UpdateVersion(updated)) throw new Exception("Cannot set project version.");
        project.SaveContents();

        // Updating references if needed...
        if (reduced.IsEmpty && enlarged.IsEmpty) return true;

        var root = Program.GetSolutionDirectory();
        var items = Program.FindProjects(root);
        var first = true;

        WriteLineEx(true);
        WriteLineEx(true, Green, "Updating references...");
        foreach (var item in items)
        {
            // Pre-saving state---
            var temps = item.Select(x => new ProjectLine(x)).ToList();
            var modified = false;

            // Iterating item's lines...
            var ilines = item.Where(x => x.IsNuReference()).ToList();
            foreach (var iline in ilines)
            {
                if (!iline.GetNuPackageName(out var iname)) continue;
                if (!iline.GetNuPackageVersion(out var iversion)) continue;
                if (string.Compare(project.Name, iname, ignoreCase: true) != 0) continue;

                // Saving the item's original state...
                if (!modified)
                {
                    modified = true;
                    if (first) WriteLineEx(true); first = false;
                    WriteEx(true, Green, "Updating: ");
                    WriteLineEx(true, item.ToString());

                    var saved = backups.Add(item);
                    saved.Clear();
                    saved.AddRange(temps);
                }

                // Modifying the reference...
                var done = iversion.PreRelease.IsEmpty switch
                {
                    true => reduced.IsEmpty || iline.UpdateNuPackageVersion(reduced),
                    false => enlarged.IsEmpty || iline.UpdateNuPackageVersion(enlarged),
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