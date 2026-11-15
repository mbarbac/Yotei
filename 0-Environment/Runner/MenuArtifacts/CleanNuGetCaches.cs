using static System.ConsoleColor;
namespace Runner;

// ========================================================
public class CleanNuGetCaches : ConsoleMenu.Entry
{
    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public override string Headline() => "Clean NuGet caches";

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public override void Execute()
    {
        Console.WriteLineEx(true);
        Console.WriteLineEx(true, Green, Program.FatSeparator);
        Console.WriteLineEx(true, Green, Headline());
        Console.WriteLineEx(true);

        var path = AppContext.BaseDirectory;
        SysCommand.Execute(
            "dotnet",
            "nuget locals all --clear",
            path);
    }
}