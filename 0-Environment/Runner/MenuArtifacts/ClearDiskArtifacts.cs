#pragma warning disable IDE0057

using static Yotei.Tools.ConsoleExtensions;
using static System.Console;
using static System.ConsoleColor;

namespace Runner;

// ========================================================
public class ClearDiskArtifacts : ConsoleMenuEntry
{
    static readonly StringComparison Comparison = StringComparison.OrdinalIgnoreCase;
    static string? Root;

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns><inheritdoc/></returns>
    public override string Header() => "Clear Disk Artifacts";

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public override void Execute()
    {
        Clear();
        WriteLineEx(true);
        WriteLineEx(true, Green, Program.SlimSeparator);
        WriteLineEx(true, Green, Header());

        WriteLineEx(true);
        Root ??= Program.GetSolutionDirectory();
        Root = Program.EditDirectory(Root, "Root directory: ");
        if (Root is null || Root.Length == 0) return;

        WriteLineEx(true);
        WriteLineEx(true, Green, Program.SlimSeparator);
        Execute(Root, Root, false);
    }

    // ----------------------------------------------------

    /// <summary>
    /// Executes on the given path.
    /// </summary>
    static void Execute(string root, string path, bool delete)
    {
        var directory = new DirectoryInfo(path);
        if (!directory.Exists) return;
        if (path.EndsWith(".git", Comparison)) return;
        if (path.EndsWith(".vs", Comparison)) return;

        var reduced = path.Remove(0, root.Length);
        if (reduced.Length > 0 && reduced[0] == '\\') reduced = reduced[1..];
        WriteEx(true, Green, $"...{reduced}");
        
        if (delete) WriteEx(true, DarkYellow, " -- DELETE");
        WriteLineEx(true);

        // Child files...
        if (delete)
        {
            var row = CursorTop;
            int len = 0;
            var files = directory.GetFiles();

            foreach (var file in files)
            {
                CursorTop = row;
                CursorLeft = 0; WriteEx(true, new string(' ', len + 1));
                CursorLeft = 0; WriteEx(true, file.Name);
                len = file.Name.Length;

                try { file.Delete(); }
                catch (Exception e)
                {
                    WriteLineEx(true, Red, $" Delete: {e.Message}");
                    row = CursorTop;
                    len = 0;
                }
            }

            CursorTop = row;
            CursorLeft = 0; WriteEx(true, new string(' ', len + 1));
            CursorLeft = 0;
        }

        // Child directories...
        var dirs = directory.GetDirectories();
        foreach (var dir in dirs)
        {
            var temp = dir.FullName;
            var remove =
                delete ||
                temp.EndsWith("\\bin", Comparison) ||
                temp.EndsWith("\\obj", Comparison) ||
                temp.EndsWith("\\generated", Comparison) ||
                temp.EndsWith("\\testresults", Comparison);

            Execute(root, temp, remove);
        }

        // Delete this directory...
        if (delete)
        {
            try { directory.Delete(); }
            catch (Exception e)
            {
                WriteEx(true, Red, $" Delete: ");
                WriteEx(true, $" {directory.FullName} ");
                WriteLineEx(true, Red, $"Error: {e.Message}");
            }
        }
    }
}