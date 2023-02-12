namespace Dev.Janitor;

// ========================================================
/// <summary>
/// Clean development artifacts.
/// </summary>
public class MenuJanitor : MenuItem
{
    /// <summary>
    /// Print the head title of this menu item, which must end with a new line.
    /// </summary>
    public override void PrintHead() => WriteLine("Clean development artifacts.");

    /// <summary>
    /// Executes the actions in this menu item.
    /// </summary>
    public override void Execute()
    {
        WriteLine();
        WriteLine(Color.Green, Menu.SeparatorLine);
        Write(Color.Green, "Cleaning development artifacts from: ");

        var root = EditLine(Program.SolutionRoot());
        if (root.Length > 0) Execute(root, root, false, 0);
    }

    /// <summary>
    /// Cleaning artifacts for the given directory.
    /// </summary>
    void Execute(Directory root, Directory directory, bool delete, int indent)
    {
        var header = new string(' ', indent * 2);
        var reduced = directory.Path[root.Path.Length..];

        // Files...
        if (delete)
        {
            var other = new string(' ', 3 + (indent * 2));
            var files = directory.GetFiles();

            foreach (var file in files)
            {
                Write(Color.Green, $"{other}- File: "); Write($"...\\{reduced}\\{file}");

                try { file.Delete(); WriteLine(Color.Green, "... Ok"); }
                catch (Exception e) { WriteLine(Color.Red, $" ... {e.Message}"); }
            }
        }

        // Folders...
        var dirs = directory.GetDirectories();
        foreach (var dir in dirs)
        {
            var temp = dir.Path.ToUpper();
            var remove = delete || temp.EndsWith("\\BIN") || temp.EndsWith("\\OBJ");

            Execute(root, dir, remove, indent + 1);
        }

        // This directory...
        if (delete)
        {
            Write(Color.Green, $"{header}- Folder: "); Write($"...\\{reduced}");

            try { directory.Delete(); WriteLine(Color.Green, "... Ok"); }
            catch (Exception e) { WriteLine(Color.Red, $" ... {e.Message}"); }
        }
    }
}