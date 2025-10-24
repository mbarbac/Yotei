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
        Console.Clear();
        Console.WriteLine(true, "");
        Console.WriteLine(true, Green, Program.FatSeparator);
        Console.WriteLine(true, Green, Header());

        CleanNuGet();
        CleanLocalRepo();
    }

    // ----------------------------------------------------

    /// <summary>
    /// Invoked to clean NuGet caches.
    /// </summary>
    static void CleanNuGet()
    {
        Console.WriteLine(true, "");
        Console.WriteLine(true, Green, Program.SlimSeparator);
        Console.WriteLine(true, Green, "Cleaning NuGet Caches...");
        Console.WriteLine(true, "");

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
        Console.WriteLine(true, "");
        Console.WriteLine(true, Green, Program.SlimSeparator);
        Console.WriteLine(true, Green, "Cleaning Local Repo...");
        Console.WriteLine(true, "");

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
            Console.WriteLine(true, "");
            Console.Write(true, Red, "Cannot find packages' folder: ");
            Console.WriteLine(true, Program.LocalRepoPath);
        }
    }
}