using static Yotei.Tools.Diagnostics.ConsoleEx;
using static System.ConsoleColor;

namespace Runner;

// ========================================================
public class EntryPackage : MenuEntry
{
    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    /// <param name="project"></param>
    public EntryPackage(Project project) => Project = project.ThrowWhenNull();

    /// <summary>
    /// The project this instance refers to.
    /// </summary>
    public Project Project { get; }

    /// <inheritdoc/>
    public override string Header() => Project.NameExtensionVersion;

    /// <inheritdoc/>
    public override void Execute()
    {
        BuildMode mode = default;
        SemanticVersion newversion = default!;

        if (!Project.IsPackable(out var oldversion))
        {
            WriteLine(true);
            WriteLine(true, Red, "Project is not a packable one.");
            return;
        }
        var debugversion = oldversion!.IncreasePreRelease("v001");
        var localversion = oldversion!.PreRelease.IsEmpty ? oldversion with { PreRelease = "v001" } : oldversion;
        var releaseversion = oldversion!.IncreasePatch();

        var menu = new MenuConsole() {
            new MenuEntry("Previous"),
            new MenuEntry($"Debug: {debugversion}"),
            new MenuEntry($"Local: {localversion}"),
            new MenuEntry($"Release: {releaseversion}"),
            new MenuEntry("Explicit selection"),
        };

        WriteLine(true);
        WriteLine(Green, Program.SlimSeparator);
        Write(true, Green, "Building package: "); WriteLine(Header());
        WriteLine(true);
        WriteLine(true, Green, "Please select the desired mode:");
        WriteLine(true);

        var result = menu.Run(Green, Program.Timeout);
        switch (result)
        {
            case 1: mode = BuildMode.Debug; newversion = debugversion; break;
            case 2: mode = BuildMode.Local; newversion = localversion; break;
            case 3: mode = BuildMode.Release; newversion = releaseversion; break;
            case 4:
                if (!MenuBuilder.CaptureMode(out mode)) return;
                if (!MenuBuilder.CaptureVersion(mode, oldversion, out newversion)) return;
                break;

            default: return;
        }

        var backups = new BuildBackups();
        var done = Execute(backups, Project, mode, newversion);
        if (!done)
        {
            WriteLine(true);
            WriteLine(true, Red, Program.FatSeparator);
            WriteLine(true, Red, "Errors have been detected...");

            backups.Restore();
        }
    }

    // ----------------------------------------------------

    /// <summary>
    /// Builds the given project using the given build mode, provided it is a packable one.
    /// </summary>
    /// <param name="backups"></param>
    /// <param name="project"></param>
    /// <param name="mode"></param>
    /// <param name="newversion"></param>
    /// <returns></returns>
    public static bool Execute(
        BuildBackups backups,
        Project project, BuildMode mode, SemanticVersion newversion)
    {
        backups.ThrowWhenNull();
        project.ThrowWhenNull();
        newversion.ThrowWhenNull();

        backups.Add(project);
        return ExecuteInternal(backups, project, mode, newversion);
    }

    /// <summary>
    /// Invoked to build the given project.
    /// </summary>
    static bool ExecuteInternal(
        BuildBackups backups,
        Project project, BuildMode mode, SemanticVersion newversion)
    {
        project.ThrowWhenNull();
        newversion.ThrowWhenNull();

        // Initializing...
        WriteLine(true);
        WriteLine(true, Green, Program.FatSeparator);
        Write(true, Green, "Compiling project: "); Write(project.NameVersion);
        Write(true, Green, " for mode: "); WriteLine(mode.ToString());
        Write(true, Green, "New version: "); WriteLine(newversion);

        if (!project.IsPackable(out var oldversion))
        {
            WriteLine(true);
            WriteLine(true, Red, "Project is not a packable one.");
            return false;
        }

        if (newversion.CompareTo(oldversion) < 0 ||
            (newversion.CompareTo(oldversion) == 0 && mode != BuildMode.Local))
        {
            WriteLine(true);
            Write(true, Red, "New version value must be greater than the old one.");
            return false;
        }

        // Processing...
        WriteLine(true);
        WriteLine(true, Green, "Deleting old files...");
        if (!DeleteFiles(project, mode)) return false;

        WriteLine(true);
        WriteLine(true, Green, "Compiling project...");
        if (!CompileProject(project, mode)) return false;

        WriteLine(true);
        WriteLine(true, Green, "Pushing package files...");
        if (!PushPackage(project, mode)) return false;

        WriteLine(true);
        WriteLine(true, Green, "Updating references...");
        if (!UpdateReferences(project, mode, backups, oldversion!, newversion)) return false;

        if (mode == BuildMode.Release)
        {
            newversion = newversion.IncreasePatch() with { PreRelease = "v001" };
            var done = ExecuteInternal(backups, project, BuildMode.Debug, newversion);
            if (!done) return false;
        }

        // Finishing...
        return false;
    }

    // ----------------------------------------------------

    /// <summary>
    /// Invoked to clear previous build files and repository.
    /// </summary>
    static bool DeleteFiles(Project project, BuildMode mode)
    {
        GetNuGetPackageFiles(project, out var regulars, out var symbols);
        if (!DeleteList(regulars)) return false;
        if (!DeleteList(symbols)) return false;

        bool DeleteList(List<FileInfo> files)
        {
            foreach (var file in files)
            {
                try { file.Delete(); }
                catch (Exception e)
                {
                    Write(Red, "Cannot delete file: "); WriteLine(true, file.FullName);
                    WriteLine(Red, $"- {e.Message}");
                    return false;
                }
            }
            return true;
        }

        if (mode == BuildMode.Local)
        {
            var dir = new DirectoryInfo(Program.LocalRepoPath);
            var files = dir.GetFiles($"{project.Name}*.*");
            foreach (var file in files)
            {
                try { file.Delete(); }
                catch (Exception e)
                {
                    Write(Red, "Cannot delete file: "); WriteLine(true, file.FullName);
                    WriteLine(Red, $"- {e.Message}");
                    return false;
                }
            }
        }

        return true;
    }

    /// <summary>
    /// Gets the NuGet package files starting from the project directory.
    /// </summary>
    static void GetNuGetPackageFiles(
        Project project,
        out List<FileInfo> regulars,
        out List<FileInfo> symbols)
    {
        var dir = new DirectoryInfo(project.Directory);
        if (dir.Exists)
        {
            regulars = dir.GetFiles("*.nupkg", SearchOption.AllDirectories).ToList();
            symbols = dir.GetFiles("*.snupkg", SearchOption.AllDirectories).ToList();
        }
        else
        {
            regulars = [];
            symbols = [];
        }
    }

    // ----------------------------------------------------

    /// <summary>
    /// Invoked to compile the given project.
    /// </summary>
    static bool CompileProject(Project project, BuildMode mode)
    {
        var code = Command.Execute(
            "dotnet",
            $"build -c {mode} {Path.GetFileName(project.FullName)}",
            Path.GetDirectoryName(project.FullName));

        return code == 0;
    }

    // ----------------------------------------------------

    /// <summary>
    /// Invoked to push the package files.
    /// </summary>
    static bool PushPackage(Project project, BuildMode mode)
    {
        /*
        GetNuGetPackageFiles(project, out var files, out _);
        if (files.Count == 0)
        {
            WriteLine(true, Red, "No package files found!");
            return false;
        }
        if (files.Count > 1)
        {
            WriteLine(true, Red, "Too many package files found!");
            return false;
        }

        var file = files[0];
        var cmd = mode switch
        {
            BuildMode.Debug => $"nuget push {file} -s {Program.NuGetRepoSource}",
            BuildMode.Local => $"nuget push {file} -s {Program.LocalRepoSource}",
            BuildMode.Release => $"nuget push {file} -s {Program.NuGetRepoSource}",
            _ => throw new ArgumentException("Unknown build mode.").WithData(mode)
        };

        var done = Command.Execute(
            "dotnet",
            cmd,
            file.DirectoryName);

        if (done != 0) WriteLine(true, Red, "Cannot push package files!");
        return done == 0;
        */
        return true;
    }

    // ----------------------------------------------------

    /// <summary>
    /// Invoked to update the references to the given project in all projects in the current
    /// solution.
    /// </summary>
    /// <returns></returns>
    static bool UpdateReferences(
        Project project,
        BuildMode mode,
        BuildBackups backups,
        SemanticVersion oldversion, SemanticVersion newversion)
    {
        var root = Program.GetSolutionDirectory();
        var projects = MenuBuilder.FindProjects(root);






        return false;
    }
}