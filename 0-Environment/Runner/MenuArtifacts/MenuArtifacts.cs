using static System.ConsoleColor;
namespace Runner;

// ========================================================
public class MenuArtifacts : ConsoleMenu.Entry
{
    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public override string Headline() => "Manage Artifacts";

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public override void Execute()
    {
        var menu = new ConsoleMenu { ToDebug = Program.ToDebug, Timeout = Program.Timeout }
        .Add(new("Exit"))
        .Add(new CleanNuGetCaches())
        .Add(new CleanLocalRepo())
        .Add(new CleanDiskArtifacts());

        int position = 0; do
        {
            Console.PreventBufferOverflow(menu.Count + 5);
            Console.WriteLineEx(true);
            Console.WriteLineEx(true, Green, Program.FatSeparator);
            Console.WriteLineEx(true, Green, Headline());
            Console.WriteLineEx(true);
            if ((position = menu.RunVertical(position)) > 0) menu[position].Execute();
        }
        while (position > 0);
    }
}