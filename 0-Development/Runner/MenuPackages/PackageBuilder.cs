using static Yotei.Tools.ConsoleEx;
using static System.ConsoleColor;

namespace Runner;

// =============================================================
public class PackageBuilder : ConsoleMenuEntry
{
    BuildMode Mode = BuildMode.Debug;

    /// <inheritdoc/>
    public override string Header() => "Build NuGet Package";

    /// <inheritdoc/>
    public override void Execute()
    {
        Console.Clear();
        var option = 0; do
        {
            var root = Program.GetSolutionDirectory();
            var projects = Program.FindProjects(root);
            projects = [.. projects.Where(x => x.IsPackable())];
            projects.Sort((x, y) => x.Name.CompareTo(y.Name));

            if (projects.Count == 0)
            {
                WriteLine(true);
                Write(true, Red, "No package projects found from: "); WriteLine(true, root);
                return;
            }

            WriteLine(true);
            WriteLine(true, Green, Program.FatSeparator);
            WriteLine(true, Green, Header());

            var menu = new ConsoleMenu { new("Exit") };
            var items = projects.Select(x => new ConsoleMenuEntry(x.NameVersion)).ToArray();
            menu.AddRange(items);
            option = menu.Run(Program.MenuOptions, option);

            if (option > 0)
            {
                var project = projects[option - 1];
                Execute(project, ref Mode);
            }
        }
        while (option > 0);
    }

    // ----------------------------------------------------

    /// <summary>
    /// Invoked to build the package of the given project.
    /// </summary>
    /// <param name="project"></param>
    static void Execute(Project project, ref BuildMode mode)
    {
        WriteLine(true);
        WriteLine(true, Green, Program.SlimSeparator);
        Write(true, Green, "Project: "); WriteLine(true, project.NameVersion);

        WriteLine(true);
        if (!Program.CaptureBuildMode(ref mode)) return;

        WriteLine(true);
        Write(true, Green, "Enter compile version: ");
        if (!project.GetVersion(out var version)) return;

        var captured = EditLine(true, Program.Timeout, version);
        if (captured is null || captured.Length == 0) return;

        // Executing under a backups' umbrella...
        var backups = new BuildBackups();
        var saved = backups.Add(project);
        saved.AddRange(project);

        try
        {
            // Saving compilation version...
            var updated = new SemanticVersion(captured);
            if (!project.SetVersion(updated)) return;
            project.SaveContents();

            // Cleaning directory...
            WriteLine(true);
            WriteLine(true, Green, "Cleaning old package files...");
            GetNuFiles(project, out var regulars, out var symbols);
            DeleteFiles(regulars);
            DeleteFiles(symbols);

            // Compiling project...
            var name = Path.GetFileName(project.FullName);
            var dir = Path.GetDirectoryName(project.FullName);
            var code = SysCommand.Execute("dotnet", $"build -c {mode} {name}", dir);
            if (code != 0) return;

            // No push for debug mode...
            if (mode == BuildMode.Debug) return;

            // Pushing package for local and release modes...
            PushPackage(project, mode);
        }
        catch (Exception ex) // Recovering from exceptions...
        {
            WriteLine(true);
            WriteLine(true, Red, "Exception intercepted:");
            WriteLine(true, ex.ToDisplayString());

            WriteLine(true);
            WriteLine(true, Green, Program.SlimSeparator);
            WriteLine(true, Green, "Reverting to previous state...");
        }
        finally
        {
            // We put back the original version...
            backups.Restore();
        }
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
    /// Pushes the package of the given project using the given build mode.
    /// </summary>
    static bool PushPackage(Project project, BuildMode mode)
    {
        GetNuFiles(project, out var files, out _);
        if (files.Count == 0)
        {
            WriteLine(true); WriteLine(true, Red, "No package files found!");
            return false;
        }
        if (files.Count > 1)
        {
            WriteLine(true); WriteLine(true, Red, "Too many files found!");
            return false;
        }

        var file = files[0];
        var cmd = mode switch
        {
            BuildMode.Local => $"nuget push {file} -s {Program.LocalRepoSource}",
            BuildMode.Release => $"nuget push {file} -s {Program.NuGetRepoSource}",
            _ => throw new ArgumentException("Invalid build mode.").WithData(mode)
        };

        WriteLine(true);
        Write(true, Green, "Pushing with: "); WriteLine(true, cmd);
        var done = SysCommand.Execute("dotnet", cmd, file.DirectoryName);
        if (done != 0)
        {
            Write(true, Red, "Cannot push package files for: ");
            WriteLine(true, project.Name);
        }
        return done == 0;
    }
}