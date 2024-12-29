using static Yotei.Tools.Diagnostics.ConsoleEx;
using static System.ConsoleColor;

namespace Runner.Artifacts;

// ========================================================
public class CleanLocalPackagesEntry : MenuEntry
{
    /// <inheritdoc/>
    public override string Header() => "Clean Local Packages";

    /// <inheritdoc/>
    public override void Execute()
    {
        WriteLine(true);
        WriteLine(true, Green, Program.FatSeparator);
        WriteLine(true, Green, Header());
        WriteLine(true);

        CleanNuGet();
        CleanLocalRepo();
    }

    // ----------------------------------------------------

    /// <summary>
    /// Invoked to clean NuGet caches.
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

    // ----------------------------------------------------

    /// <summary>
    /// Invoked to clean local repo.
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