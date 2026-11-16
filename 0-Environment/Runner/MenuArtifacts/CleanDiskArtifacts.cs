using static System.ConsoleColor;
namespace Runner;

// ========================================================
public class CleanDiskArtifacts : ConsoleMenu.Entry
{
    static string? Root;
    static readonly StringComparison Comparison = StringComparison.OrdinalIgnoreCase;

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public override string Headline() => "Clean Disk Artifacts";

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public override void Execute()
    {
        Console.WriteLineEx(true);
        Console.WriteLineEx(true, Green, Program.FatSeparator);
        Console.WriteLineEx(true, Green, Headline());
        Console.WriteLineEx(true);

        Root ??= Program.GetSolutionDirectory();

        Console.WriteLine(Green, "(Leave blank to abort)");
        Root = Program.EditDirectory(Root, "Root: ");
        if (Root is null) return;

        Console.WriteLineEx(true);
        var (num, err) = Execute(Root, Root, false);
        Console.WriteLineEx(true);
        Console.WriteEx(true, Green, "Deleted: "); Console.WriteLineEx(true, num.ToString());
        Console.WriteEx(true, Green, "Errors:  "); Console.WriteLineEx(true, Red, err.ToString());

        if (num != 0)
        {
            Console.WriteLineEx(true);
            Console.WriteEx(true, Red, "You may want to clean artifacts again!");
        }
    }

    /// <summary>
    /// Executes on the given path.
    /// </summary>
    [SuppressMessage("", "IDE0057")]
    static (int num, int err) Execute(string root, string path, bool delete)
    {
        var dir = new DirectoryInfo(path);
        if (!dir.Exists) return (0, 0);
        if (path.EndsWith(".git", Comparison)) return (0, 0);
        if (path.EndsWith(".vs", Comparison)) return (0, 0);

        var num = 0;
        var err = 0;
        var reduced = path.Remove(0, root.Length);
        if (reduced.Length > 0 && reduced[0] == '\\') reduced = reduced[1..];

        // Header...
        Console.WriteEx(true, Cyan, $"...\\{reduced}");
        if (delete) Console.WriteEx(true, DarkYellow, " -- DELETE");
        Console.WriteLineEx(true);

        // Child files...
        if (delete)
        {
            var files = dir.GetFiles(); Console.PreventBufferOverflow(files.Length + 5);
            var row = Console.CursorTop;
            var len = 0;

            static void ConsolePosition(int top, int left)
            {
                Console.CursorTop = top;
                Console.CursorLeft = left;
            }

            foreach (var file in files)
            {
                ConsolePosition(row, 0); Console.Write(new string(' ', len + 1));
                ConsolePosition(row, 0); Console.Write(file.Name);

                try { file.Delete(); len = file.Name.Length; num++; }
                catch (Exception e)
                {
                    Console.CursorTop = row;
                    Console.CursorLeft = 0; Console.Write(new string(' ', len + 1));
                    Console.CursorLeft = 0;
                    Console.WriteEx(true, Magenta, "File Error: ");
                    Console.WriteLineEx(true, Red, e.Message);

                    err++;
                    row = Console.CursorTop;
                    len = 0;
                }
            }
            Console.CursorTop = row;
            Console.CursorLeft = 0; Console.Write(new string(' ', len + 1));
            Console.CursorLeft = 0;
        }

        // Child directories...
        var temps = dir.GetDirectories();
        foreach (var temp in temps)
        {
            var name = temp.FullName;
            var remove =
                delete ||
                name.EndsWith("\\bin", Comparison) ||
                name.EndsWith("\\obj", Comparison) ||
                name.EndsWith("\\generated", Comparison) ||
                name.EndsWith("\\testresults", Comparison);

            var (xnum, xerr) = Execute(root, name, remove);
            num += xnum;
            err += xerr;
        }

        // This directory...
        if (delete)
        {
            try { dir.Delete(); num++; }
            catch (Exception e)
            {
                Console.WriteEx(true, Magenta, "Directory Error: ");
                Console.WriteLineEx(true, Red, e.Message);
                err++;
            }
        }

        // Finishing...
        return (num, err);
    }
}