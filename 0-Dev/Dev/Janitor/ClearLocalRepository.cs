namespace Dev.Janitor;

// ========================================================
public class ClearLocalRepository : MenuEntry
{
    public string Header => "Clear Local Repository.";

    //// <summary>
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
        WriteLine();

        var dir = new Directory(Program.LocalRepoPath);
        var files = dir.GetFiles();
        foreach (var file in files)
        {
            Write(Color.Green, "Deleting file: "); WriteLine(file.NameAndExtension);
            var done = file.Delete();
            if (!done) WriteLine(Color.Red, "Cannot delete file...");
        }
    }
}