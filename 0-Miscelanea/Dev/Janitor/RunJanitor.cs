namespace Dev.Janitor;

// ========================================================
/// <summary>
/// Clears development artifacts.
/// </summary>
public class RunJanitor : Runner
{
    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    /// <param name="args"></param>
    public RunJanitor(string[] args) => Arguments = args.ThrowIfNull();
    string[] Arguments;

    /// <summary>
    /// Prints the header title of this action, ending in a new line.
    /// </summary>
    public override void PrintHead() => WriteLine("Clean artifacts.");

    /// <summary>
    /// Executes this action.
    /// </summary>
    public override void Execute()
    {
        WriteLine();
        WriteLine("Cleaning development artifacts...");

        var temp = Arguments.Length > 0 ? Arguments[0] : Program.DefaultRoot().Path;
        var edit = Arguments.Length <= 0;
        var empty = false;
        var root = Program.CaptureDirectory("Base directory", temp, edit, empty);

        Execute(root, root, false, 0);
    }

    void Execute(Directory root, Directory path, bool delete, int indent)
    {
        var header = new string(' ', indent * 2);
        var reduced = path.Path[root.Path.Length..];

        // Files...
        if (delete)
        {           
            var other = header + "   ";
            var files = path.GetFiles();

            foreach (var file in files)
            {
                Write(Color.Green, $"{other}- File: "); Write($"...\\{reduced}\\{file}");

                try { file.Delete(); WriteLine(Color.Green, "... Ok");  }
                catch (Exception e) { WriteLine(Color.Red, $" ... {e.Message}"); }
            }
        }

        // Folders...
        var dirs = path.GetDirectories();
        foreach (var dir in dirs)
        {
            var temp = dir.Path.ToUpper();
            var remove = delete || temp.EndsWith("\\BIN") || temp.EndsWith("\\OBJ");

            Execute(root, dir, remove, indent + 1);
        }

        // Me...
        if (delete)
        {
            Write(Color.Green, $"{header}- Folder: "); Write($"...\\{reduced}");

            try { path.Delete(); WriteLine(Color.Green, "... Ok"); }
            catch (Exception e) { WriteLine(Color.Red, $" ... {e.Message}"); }
        }
    }
}