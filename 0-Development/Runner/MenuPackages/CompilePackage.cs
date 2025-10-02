using static Yotei.Tools.ConsoleEx;
using static System.ConsoleColor;

namespace Runner;

// ========================================================
public class CompilePackage(Project project, BuildMode mode) : ConsoleMenuEntry
{
    readonly Project Project = project.ThrowWhenNull();
    readonly BuildMode Mode = mode;

    /// <inheritdoc/>
    public override string Header() => $"Compile {Mode}";

    /// <inheritdoc/>
    public override void Execute() => OnExecute(interactive: true);

    /// <summary>
    /// Executes on this instance's project and returns the result of that execution.
    /// </summary>
    public bool OnExecute(bool interactive)
    {
        var backups = new BuildBackups();
        var saved = backups.Add(Project);
        saved.AddRange(Project);

        try
        {
            var done = Consumate();
            backups.Restore(display: false);
            return done;
        }
        catch (Exception e)
        {
            WriteLine(true);
            WriteLine(true, Red, "Exception intercepted:");
            WriteLine(true, e.ToDisplayString());

            WriteLine(true);
            WriteLine(true, Green, Program.SlimSeparator);
            WriteLine(true, Green, "Reverting to previous state...");
            backups.Restore(display: true);

            WriteLine(true);
            Write(true, Green, "Press [Enter] to continue...");

            if (interactive) Console.ReadLine();
            return false;
        }
    }

    // ----------------------------------------------------

    /// <summary>
    /// Executes under a backups-protected umbrella.
    /// </summary>
    bool Consumate()
    {
        WriteLine(true);
        WriteLine(true, Green, Program.SlimSeparator);
        Write(true, Green, "Project: "); WriteLine(true, project.Name);
        Write(true, Green, "   Mode: "); WriteLine(true, Mode.ToString());

        // Cleaning old packages from directory...
        WriteLine(true);
        WriteLine(true, Green, "Cleaning old package files...");
        GetNuFiles(project, out var regulars, out var symbols);
        DeleteFiles(regulars);
        DeleteFiles(symbols);

        // Debug mode...
        if (Mode == BuildMode.Debug)
        {
            CompileProject(Project, Mode);
            return true;
        }

        // Local mode...
        if (Mode == BuildMode.Local)
        {
            if (project.GetVersion(out var version))
            {
                if (version.PreRelease.IsEmpty)
                {
                    version = version with { PreRelease = "v0001" };
                    project.SetVersion(version);
                    project.SaveContents();
                }
                WriteLine(true);
                Write(true, Green, "Version: "); WriteLine(true, version);
            }
            if (!CompileProject(Project, Mode)) return false;
            if (!PushPackage(Project, Mode)) return false;
            return true;
        }

        // Release mode...
        if (Mode == BuildMode.Release)
        {
            if (project.GetVersion(out var version))
            {
                if (!version.PreRelease.IsEmpty)
                {
                    version = version with { PreRelease = "" };
                    project.SetVersion(version);
                    project.SaveContents();
                }
                WriteLine(true);
                Write(true, Green, "Version: "); WriteLine(true, version);
            }
            if (!CompileProject(Project, Mode)) return false;
            if (!PushPackage(Project, Mode)) return false;
            return true;
        }

        // Unknown mode...
        return false;
    }

    // ----------------------------------------------------

    /// <summary>
    /// Gets the NuGet package files in the project directory and branch.
    /// </summary>
    static void GetNuFiles(
        Project project, out List<FileInfo> regulars, out List<FileInfo> symbols)
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
                Write(true, Red, "Cannot delete file: "); WriteLine(item.FullName);
                WriteLine(Red, $"- {e.Message}");
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
        WriteLine(true);

        var name = Path.GetFileName(project.FullName);
        var dir = Path.GetDirectoryName(project.FullName);
        var done = SysCommand.Execute("dotnet", $"build -c {mode} {name}", dir);

        return done == 0;
    }

    // ----------------------------------------------------

    /// <summary>
    /// Invoked to compile the project.
    /// </summary>
    static bool PushPackage(Project project, BuildMode mode)
    {
        if (!project.IsPackable()) return true; // We don't push, but it's ok...

        WriteLine(true);
        WriteLine(true, Green, "Pushing package...");
        WriteLine(true);

        GetNuFiles(project, out var files, out _);
        if (files.Count == 0)
        {
            WriteLine(true, Red, "No package files found!");
            return false;
        }
        if (files.Count > 1)
        {
            WriteLine(true, Red, "Too many files found!");
            return false;
        }

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
            Write(true, Red, "Cannot push package files for: ");
            WriteLine(true, project.Name);
        }
        return done == 0;
    }
}