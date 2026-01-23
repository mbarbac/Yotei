using static Yotei.Tools.ConsoleExtensions;
using static System.Console;
using static System.ConsoleColor;

namespace Runner;

// ========================================================
public class ClearLocalPackages : ConsoleMenuEntry
{
    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns><inheritdoc/></returns>
    public override string Header() => "Clean Local Packages";

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public override void Execute()
    {
        Clear();
        WriteLineEx(true);
        WriteLineEx(true, Green, Program.FatSeparator);
        WriteLineEx(true, Green, Header());

        CleanNuGet();
        CleanLocalRepo();
    }

    // ----------------------------------------------------

    /// <summary>
    /// Invoked to clean NuGet caches.
    /// </summary>
    static void CleanNuGet()
    {
        WriteLineEx(true);
        WriteLineEx(true, Green, Program.SlimSeparator);
        WriteLineEx(true, Green, "Cleaning NuGet Caches...");
        WriteLineEx(true);

        var path = AppContext.BaseDirectory;

        SysCommand.Execute(
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
        WriteLineEx(true);
        WriteLineEx(true, Green, Program.SlimSeparator);
        WriteLineEx(true, Green, "Cleaning Local Repo...");
        WriteLineEx(true);

        var dir = new DirectoryInfo(Program.LocalRepoPath);
        try
        {
            var files = dir.GetFiles();
            foreach (var file in files)
            {
                try { file.Delete(); }
                catch { }
            }
        }
        catch (DirectoryNotFoundException)
        {
            WriteLineEx(true);
            WriteEx(true, Red, "Cannot find packages' folder: ");
            WriteLineEx(true, Program.LocalRepoPath);
        }
    }
}