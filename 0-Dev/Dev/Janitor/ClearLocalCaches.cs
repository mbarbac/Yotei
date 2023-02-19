namespace Dev.Janitor;

// ========================================================
public class ClearLocalCaches : MenuEntry
{
    public string Header => "Clear Local Caches.";

    /// <inheritdoc>
    /// </inheritdoc>
    public override void OnPrint() => WriteLine(Header);

    /// <inheritdoc>
    /// </inheritdoc>
    public override void OnExecute()
    {
        WriteLine();
        WriteLine(Program.Color, Program.FatSeparator);
        WriteLine(Program.Color, Header);
        WriteLine();

        Command.Execute(
            Program.DotNetExe,
            "nuget locals all --clear",
            Program.ProjectRoot());
    }
}