namespace Dev.Janitor;

// ========================================================
/// <summary>
/// Represents this program.
/// </summary>
internal class Program
{
    const string Separator = "******************************";
    const int IndentSize = 2;

    /// <summary>
    /// The program entry point.
    /// </summary>
    /// <param name="args"></param>
    static void Main(string[] args)
    {
        GetArguments(args, out var root);

        WriteLine(Color.Green, Separator);
        Write(Color.Green, "Cleans development artifacts from: "); WriteLine(root);
        WriteLine(Color.Green, Separator);

        Execute(root, root, false, 0);

        WriteLine();
        WriteLine(Color.Green, Separator);
        Write("Press [Enter] to finalize...");
        ReadLine();
    }

    /// <summary>
    /// Executes this program from the given path.
    /// </summary>
    static void Execute(string root, string path, bool delete, int indent)
    {
        var reduced = path[root.Length..];
        var Dheader = new string(' ', indent * IndentSize);
        var color = delete ? Color.Magenta : Console.ForegroundColor;

        Write(Color.Cyan, $"{Dheader}- Folder: "); WriteLine(color, $"<root>\\{reduced}");

        // Files...
        if (delete)
        {
            var Fheader = new string(' ', (indent + 1) * IndentSize);

            var files = Directory.GetFiles(path);
            foreach (var file in files)
            {
                var temp = Path.GetFileName(file);
                Write(Color.Cyan, $"{Fheader}- File: "); WriteLine(color, $"<root>\\{temp}");

                try { File.Delete(file); }
                catch { WriteLine(Color.Red, $"{Fheader}- Cannot delete file: {temp}"); }
            }
        }

        // Subfolders...
        var dirs = Directory.GetDirectories(path);
        foreach (var dir in dirs)
        {
            var temp = dir.ToUpper();
            var remove = temp.EndsWith("\\BIN") || temp.EndsWith("\\OBJ");

            Execute(root, dir, remove || delete, indent + 1);

            if (delete)
            {
                try { Directory.Delete(dir); }
                catch { WriteLine(Color.Red, $"{Dheader}- Cannot delete folder: {reduced}"); }
            }
        }
    }

    /// <summary>
    /// Validates and captures program arguments.
    /// </summary>
    static void GetArguments(string[] args, out string root)
    {
        if (args.Length < 1) Environment.FailFast(
            "Program needs at least one argument: root directory.");

        root = args[0].Trim();
        if (root.Length == 0) Environment.FailFast(
            "Root directory is empty.");

        if (!Directory.Exists(root)) Environment.FailFast(
            $"Root directory does not exist.");
    }
}
