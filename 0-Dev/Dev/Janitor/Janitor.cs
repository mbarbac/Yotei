namespace Dev.Janitor;

// ========================================================
public class Janitor : MenuEntry
{
    public string Header => "Manage Local Artifacts.";

    /// <inheritdoc>
    /// </inheritdoc>
    public override void OnPrint() => WriteLine(Header);

    /// <inheritdoc>
    /// </inheritdoc>
    public override void OnExecute()
    {
        var done = -1; do
        {
            WriteLine();
            WriteLine(Program.Color, Program.FatSeparator);
            WriteLine(Program.Color, Header);
            WriteLine();

            done = Menu.Run(
                Program.Color,
                new MenuEntry(() => WriteLine("Exit")),
                new ClearArtifacts(),
                new ClearLocalCaches(),
                new ClearLocalRepository());
        }
        while (done > 0);
    }
}