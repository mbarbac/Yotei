namespace Dev.Janitor;

// ========================================================
public class ClearArtifacts : MenuEntry
{
    public string Header => "Clear Development Artifacts.";

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public override void OnPrint() => WriteLine(Header);

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public override void OnExecute()
    {
        WriteLine();
        WriteLine(Program.Color, Program.FatSeparator);
        WriteLine(Program.Color, Header);

        Write(Color.Green, "From: ");
        var root = EditLine(Program.SolutionRoot());

        WriteLine();
        if (root.Length > 0) Executor(root, root, false, 0);
    }

    /// <summary>
    /// Executing on the given directory.
    /// </summary>
    void Executor(Directory root, Directory directory, bool delete, int indent)
    {
        var files = directory.GetFiles();
        var dirs = directory.GetDirectories();

        // Folders...
        foreach (var dir in dirs)
        {
            var temp = dir.Path.ToUpper();
            var remove =
                delete ||
                temp.EndsWith("\\BIN") || temp.EndsWith("\\OBJ") ||
                temp.EndsWith("\\GENERATED");

            Executor(root, dir, remove, indent + 1);
            if (delete) Delete(root, dir);
        }

        // Files...
        if (delete) foreach (var file in files) Delete(root, file);
    }

    /// <summary>
    /// Deleting the given directory.
    /// </summary>
    void Delete(Directory root, Directory directory)
    {
        var reduced = directory.Path[root.Path.Length..];
        Write($"...\\{reduced} ... ");

        try
        {
            _Directory.Delete(directory.Path);
            WriteLine(Program.Color, "Ok");
        }
        catch (Exception ex) { WriteLine(Color.Red, ex.Message); }
    }

    /// <summary>
    /// Deleting the given file.
    /// </summary>
    void Delete(Directory root, File file)
    {
        var reduced = file.Path[root.Path.Length..];
        Write($"...\\{reduced} ... ");

        try
        {
            _File.Delete(file.Path);
            WriteLine(Program.Color, "Ok");
        }
        catch (Exception ex) { WriteLine(Color.Red, ex.Message); }
    }
}