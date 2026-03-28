using static Yotei.Tools.ConsoleExtensions;
using static System.ConsoleColor;

namespace Runner;

// ========================================================
public class CompilePackage(Project project, BuildMode mode) : ConsoleMenuEntry
{
    readonly Project Project = project.ThrowWhenNull();
    readonly BuildMode Mode = mode;

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns><inheritdoc/></returns>
    public override string Header() => $"Compile {Mode}";

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public override void Execute()
    {
        var backups = new BackupMaster();
        var saved = backups.Add(Project);

        try
        {
            if (!OnExecute())
            {
                WriteLineEx(true);
                WriteLineEx(true, Green, Program.SlimSeparator);
                WriteLineEx(true, Green, "Reverting to previous state...");

                backups.Restore(display: true);

                WriteLineEx(true);
                WriteEx(true, Green, "Press [Enter] to continue...");
                Console.ReadLine();
            }
        }
        catch (Exception ex)
        {
            WriteLineEx(true);
            WriteLineEx(true, Red, "Exception intercepted:");
            WriteLineEx(true, ex.ToDisplayString());

            WriteLineEx(true);
            WriteLineEx(true, Green, Program.SlimSeparator);
            WriteLineEx(true, Green, "Reverting to previous state...");

            backups.Restore(display: true);
            WriteEx(true, Green, "Press [Enter] to continue...");
            Console.ReadLine();
        }
    }

    // ----------------------------------------------------

    /// <summary>
    /// Compiles this package under the protection of a backups umbrella.
    /// </summary>
    bool OnExecute()
    {
        WriteLineEx(true);
        WriteLineEx(true, Green, Program.SlimSeparator);
        WriteEx(true, Green, "Project: "); WriteLineEx(true, project.Name);
        WriteEx(true, Green, "   Mode: "); WriteLineEx(true, Mode.ToString());

        // Cleaning old packages from directory...
        WriteLineEx(true);
        WriteLineEx(true, Green, "Cleaning old package files...");

        GetNuFiles(project, out var regulars, out var symbols);
        DeleteFiles(regulars);
        DeleteFiles(symbols);

        // Dispatching...
        switch (Mode)
        {
            case BuildMode.Debug: return OnExecuteDebug();
            case BuildMode.Local: return OnExecuteLocal();
            case BuildMode.Release: return OnExecuteRelease();
        }

        return false;
    }

    /// <summary>
    /// Manages build mode.
    /// </summary>
    bool OnExecuteDebug() => CompileProject(Project, Mode);

    /// <summary>
    /// Manages local mode.
    /// </summary>
    bool OnExecuteLocal()
    {
        SemanticVersion? version = null;
        try
        {
            if (Project.GetVersion(out version))
            {
                if (version.PreRelease.IsEmpty)
                {
                    var temp = version with { PreRelease = "v0001" };
                    Project.UpdateVersion(temp);
                    Project.SaveContents();

                    WriteLineEx(true);
                    WriteEx(true, Green, "Using Version: "); WriteLineEx(true, temp);
                }
            }

            if (!CompileProject(Project, Mode)) return false;
            if (!PushPackage(Project, Mode)) return false;
            return true;
        }
        finally
        {
            if (version != null)
            {
                Project.UpdateVersion(version);
                Project.SaveContents();
            }
        }
    }

    /// <summary>
    /// Manages release mode.
    /// </summary>
    bool OnExecuteRelease()
    {
        SemanticVersion? version = null;
        try
        {
            if (Project.GetVersion(out version))
            {
                if (!version.PreRelease.IsEmpty)
                {
                    var temp = version with { PreRelease = "" };
                    Project.UpdateVersion(temp);
                    Project.SaveContents();

                    WriteLineEx(true);
                    WriteEx(true, Green, "Using Version: "); WriteLineEx(true, temp);
                }
            }

            if (!CompileProject(Project, Mode)) return false;

            WriteLineEx(true);
            WriteLineEx(true, Green, Program.SlimSeparator);
            WriteEx(true, Green, "Do you want to push the package?: ");
            
            var str = ReadLineEx(true)?.ToUpper();
            switch (str)
            {
                case "Y":
                case "YES":
                    if (!PushPackage(Project, Mode)) return false;
                    break;
            }
            return true;
        }
        finally
        {
            if (version != null)
            {
                Project.UpdateVersion(version);
                Project.SaveContents();
            }
        }
    }

    // ----------------------------------------------------

    /// <summary>
    /// Gets the NuGet existing package files of the given project, if any.
    /// </summary>
    /// <param name="project"></param>
    /// <param name="regulars"></param>
    /// <param name="symbols"></param>
    static void GetNuFiles(
        Project project,
        out List<FileInfo> regulars, out List<FileInfo> symbols)
    {
        var dir = new DirectoryInfo(project.Directory);
        if (dir.Exists)
        {
            regulars = [.. dir.GetFiles("*.nupkg", SearchOption.AllDirectories)];
            symbols = [.. dir.GetFiles("*.snupkg", SearchOption.AllDirectories)];
        }
        else
        {
            regulars = [];
            symbols = [];
        }
    }

    /// <summary>
    /// Invoked to delete the given collection of files.
    /// </summary>
    /// <param name="items"></param>
    static void DeleteFiles(IEnumerable<FileInfo> items)
    {
        foreach (var item in items)
        {
            try { item.Delete(); }
            catch (Exception e)
            {
                WriteEx(true, Red, "Cannot delete file: ");
                WriteLineEx(true, item.FullName);
                WriteLineEx(true, Red, $"- {e.Message}");
                throw;
            }
        }
    }

    // ----------------------------------------------------

    /// <summary>
    /// Invoked to compile the project.
    /// </summary>
    static bool CompileProject(Project project, BuildMode mode)
    {
        WriteLineEx(true);

        var name = Path.GetFileName(project.FullName);
        var dir = Path.GetDirectoryName(project.FullName);
        var done = SysCommand.Execute("dotnet", $"build -c {mode} {name}", dir);

        return done == 0;
    }

    // ----------------------------------------------------

    /// <summary>
    /// Invoked to push the project files.
    /// </summary>
    static bool PushPackage(Project project, BuildMode mode)
    {
        // If project is not a packable one, we return 'true' by convention...
        if (!project.IsPackable()) return true;

        // Informational header...
        WriteLineEx(true);
        WriteLineEx(true, Green, "Pushing package...");
        WriteLineEx(true);

        // Intercepting anomalies...
        GetNuFiles(project, out var files, out _);

        if (files.Count == 0)
        {
            WriteLineEx(true, Red, "No package files found!");
            return false;
        }
        if (files.Count > 1)
        {
            WriteLineEx(true, Red, "Too many package files found!");
            return false;
        }

        // Pushing files...
        var file = files[0];
        var cmd = mode switch
        {
            BuildMode.Local => $"nuget push {file} -s {Program.LocalRepoSource}",
            BuildMode.Release => $"nuget push {file} -s {Program.NuGetRepoSource}",
            _ => throw new ArgumentException($"Invalid push mode: {mode}")
        };

        var done = SysCommand.Execute("dotnet", cmd, file.DirectoryName);
        if (done != 0)
        {
            WriteEx(true, Red, "Cannot push package files for: ");
            WriteLineEx(true, project.Name);
        }
        return done == 0;
    }
}