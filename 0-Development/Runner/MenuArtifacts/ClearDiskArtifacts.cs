#pragma warning disable IDE0057

using static Yotei.Tools.ConsoleEx;
using static System.ConsoleColor;

namespace Runner;

// ========================================================
public class ClearDiskArtifacts : ConsoleMenuEntry
{
    static StringComparison Comparison = StringComparison.OrdinalIgnoreCase;
    static string? Root;

    /// <inheritdoc/>
    public override string Header() => "Clear Disk Artifacts";

    /// <inheritdoc/>
    public override void Execute()
    {
        Console.Clear();
        WriteLine(true);
        WriteLine(true, Green, Program.SlimSeparator);
        WriteLine(true, Green, Header());

        WriteLine(true);
        Root ??= Program.GetSolutionDirectory();
        Root = Program.EditDirectory(Root, "Root directory: ");
        if (Root is null || Root.Length == 0) return;

        WriteLine(true);
        WriteLine(true, Green, Program.SlimSeparator);
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
        Write(true, Green, $"...{reduced}");
        if (delete) Write(true, DarkYellow, " -- DELETE");
        WriteLine(true);

        // Child files...
        if (delete)
        {
            var row = Console.CursorTop;
            int len = 0;
            var files = directory.GetFiles();

            foreach (var file in files)
            {
                Console.CursorTop = row;
                Console.CursorLeft = 0; Write(DebugEx.Header(len + 1));
                Console.CursorLeft = 0; Write(true, file.Name);
                len = file.Name.Length;

                try { file.Delete(); }
                catch (Exception e)
                {
                    WriteLine(true, Red, $" Delete: {e.Message}");
                    row = Console.CursorTop;
                    len = 0;
                }
            }

            Console.CursorTop = row;
            Console.CursorLeft = 0; Write(DebugEx.Header(len + 1));
            Console.CursorLeft = 0;
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
                Write(true, Red, $" Delete: ");
                Write(true, $" {directory.FullName} ");
                WriteLine(true, Red, $"Error: {e.Message}");
            }
        }
    }
}