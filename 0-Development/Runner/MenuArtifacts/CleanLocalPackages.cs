using static System.ConsoleColor;
using static Yotei.Tools.Diagnostics.ConsoleWrapper;

namespace Runner.Artifacts;

// ========================================================
public class CleanLocalPackages : MenuEntry
{
    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public override string Header() => "Clean Local Packages";

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public override void Execute()
    {
        WriteLine(true);
        WriteLine(true, Green, Program.FatSeparator);
        WriteLine(true, Green, Header());
        CleanNuGet();
        CleanLocalRepo();
    }

    // ----------------------------------------------------

    /// <summary>
    /// Invoked to clean NuGet caches...
    /// </summary>
    static void CleanNuGet()
    {
        WriteLine(true);
        WriteLine(true, Green, Program.SlimSeparator);
        WriteLine(true, Green, "Cleaning NuGet Caches...");
        WriteLine(true);

        var path = AppContext.BaseDirectory;

        Command.Execute(
            "dotnet",
            "nuget locals all --clear",
            path);
    }

    /// <summary>
    /// Invoked to clean NuGet caches...
    /// </summary>
    static void CleanLocalRepo()
    {
        WriteLine(true);
        WriteLine(true, Green, Program.SlimSeparator);
        WriteLine(true, Green, "Cleaning Local Repo...");
        WriteLine(true);

        var dir = new DirectoryInfo(Program.LocalRepoPath);
        var files = dir.GetFiles();
        foreach (var file in files)
        {
            try { file.Delete(); }
            catch { }
        }
    }
}