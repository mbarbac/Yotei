using static Yotei.Tools.Diagnostics.ConsoleEx;
using static System.ConsoleColor;

namespace Runner;

// ========================================================
public class EntryClearDiskArtifacts : MenuEntry
{
    /// <summary>
    /// The root directory from which clear disk artifacts.
    /// </summary>
    public static string? Root { get; set; }

    /// <inheritdoc/>
    public override string Header() => "Clean Disk Artifacts";

    /// <inheritdoc/>
    public override void Execute()
    {
        WriteLine(true, Green, Program.FatSeparator);
        WriteLine(true, Green, Header());
        WriteLine(true);

        Root ??= Program.GetSolutionDirectory();
        Root = Program.EditDirectory(Root, "Root Directory", okEmpty: true);
        
        if (Root != null && Root.Length > 0) Execute(Root, Root, false);
    }

    // ----------------------------------------------------

    /// <summary>
    /// Executes on the given path.
    /// </summary>
    /// <param name="root"></param>
    /// <param name="path"></param>
    /// <param name="delete"></param>
    static void Execute(string root, string path, bool delete)
    {
        var directory = new DirectoryInfo(path);
        if (!directory.Exists) return;

        var reduced = path.Remove(root);
        if (reduced.Length > 0 && reduced[0] == '\\') reduced = reduced[1..];

        var comparison = StringComparison.OrdinalIgnoreCase;
        if (path.EndsWith(".git", comparison)) return;
        if (path.EndsWith(".vs", comparison)) return;

        // Delete files...
        if (delete)
        {
            var files = directory.GetFiles();
            foreach (var file in files)
            {
                Write(true, Blue, "- File: ");
                Write(true, $"...\\{reduced}\\{file.Name}");
                try
                {
                    file.Delete();
                    WriteLine(true, Green, " - deleted");
                }
                catch (Exception e)
                {
                    WriteLine(true, Red, $" - {e.Message}");
                }
            }
        }

        // Directories...
        var dirs = directory.GetDirectories();
        foreach (var dir in dirs)
        {
            var temp = dir.FullName.ToUpper();
            var remove =
                delete ||
                temp.EndsWith("\\BIN", comparison) ||
                temp.EndsWith("\\OBJ", comparison) ||
                temp.EndsWith("\\GENERATED", comparison);

            Execute(root, dir.FullName, remove);
        }

        // Delete this directory...
        if (delete)
        {
            Write(true, Cyan, "- Path: ");
            Write(true, $"...\\{reduced}");
            try
            {
                directory.Delete();
                WriteLine(true, Green, " - deleted");
            }
            catch (Exception e)
            {
                WriteLine(true, Red, $" -- {e.Message}");
            }
        }
    }
}