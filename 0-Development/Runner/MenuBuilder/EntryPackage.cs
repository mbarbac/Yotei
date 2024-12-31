using static Yotei.Tools.Diagnostics.ConsoleEx;
using static System.ConsoleColor;
using System.Runtime.InteropServices.Marshalling;

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

    // ----------------------------------------------------

    /// <inheritdoc/>
    public override void Execute()
    {
        WriteLine(true);
        WriteLine(Green, Program.SlimSeparator);
        Write(true, Green, "Building package: "); WriteLine(Header());

        if (!Project.IsPackable(out var oldversion))
        {
            WriteLine(true);
            WriteLine(true, Red, "Project is not a packable one.");
            return;
        }

        var debugversion = MenuBuilder.IncreaseVersion(oldversion, BuildMode.Debug);
        var localversion = MenuBuilder.IncreaseVersion(oldversion, BuildMode.Local);
        var releaseversion = MenuBuilder.IncreaseVersion(oldversion, BuildMode.Release);

        var menu = new MenuConsole() {
            new MenuEntry("Previous"),
            new MenuEntry($"Debug: {debugversion}"),
            new MenuEntry($"Local: {localversion}"),
            new MenuEntry($"Release: {releaseversion}"),
            new MenuEntry("Explicit selection"),
        };

        WriteLine(true);
        WriteLine(true, Green, "Please select the desired mode:");
        WriteLine(true);
        var result = menu.Run(Green, Program.Timeout);

        BuildMode mode = default;
        SemanticVersion newversion = default!;
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

        var backups = new ProjectBackups();
        var done = false;
        try { done = Execute(backups, true, mode, newversion); }
        catch (Exception e)
        {
            WriteLine(true);
            WriteLine(true, Red, "Exception intercepted: ");
            WriteLine(true, e.ToDisplayString());
        }
        finally
        {
            if (!done)
            {
                WriteLine(true);
                WriteLine(true, Red, Program.FatSeparator);
                WriteLine(true, Red, "Errors have been detected...");

                backups.Restore();
            }
        }
    }

    // ----------------------------------------------------

    /// <summary>
    /// Invoked to build this project for the given build mode and new semantic version.
    /// </summary>
    /// <param name="backups"></param>
    /// <param name="saveBackup"></param>
    /// <param name="mode"></param>
    /// <param name="newversion"></param>
    public bool Execute(
        ProjectBackups backups,
        bool saveBackup,
        BuildMode mode, SemanticVersion newversion)
    {
        backups.ThrowWhenNull();
        newversion.ThrowWhenNull();

        // Initializing...
        WriteLine(true);
        WriteLine(true, Green, Program.FatSeparator);
        Write(true, Green, "Compiling project: "); Write(true, Project.NameVersion);
        Write(true, Green, " for mode: "); WriteLine(true, mode.ToString());
        Write(true, Green, "New version: "); WriteLine(newversion);

        if (!Project.IsPackable(out var oldversion))
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
        if (saveBackup)
        {
            WriteLine(true);
            WriteLine(true, Green, "Saving project backup...");
            backups.Update(Project);
        }

        WriteLine(true);
        WriteLine(true, Green, "Deleting old files...");
        if (!DeleteFiles(mode)) return false;

        WriteLine(true);
        WriteLine(true, Green, "Compiling project...");
        if (!CompileProject(mode)) return false;

        WriteLine(true);
        WriteLine(true, Green, "Pushing package files...");
        if (!PushPackage(mode)) return false;

        WriteLine(true);
        WriteLine(true, Green, "Updating references...");
        if (!UpdateReferences(mode, backups, oldversion!, newversion)) return false;

        // Special case when building for release mode...
        if (mode == BuildMode.Release)
        {
            newversion = newversion.IncreasePatch() with { PreRelease = "v001" };
            mode = BuildMode.Debug;

            // We must not save again the backup...
            var done = Execute(backups, false, mode, newversion);
            if (!done) return false;
        }

        // Finishing...
        return false;
    }

    // ----------------------------------------------------

    /// <summary>
    /// Invoked to delete previous files.
    /// </summary>
    bool DeleteFiles(BuildMode mode)
    {
        // Delete build files...
        GetNuPackageFiles(out var regulars, out var symbols);
        if (!DeleteList(regulars)) return false;
        if (!DeleteList(symbols)) return false;

        // Invoked to delete the given list of files...
        static bool DeleteList(List<FileInfo> files)
        {
            foreach (var file in files)
            {
                try { file.Delete(); }
                catch (Exception e)
                {
                    Write(true, Red, "Cannot delete file: "); WriteLine(file.FullName);
                    WriteLine(Red, $"- {e.Message}");
                    return false;
                }
            }
            return true;
        }

        // Deletes the local repository...
        if (mode == BuildMode.Local)
        {
            var dir = new DirectoryInfo(Program.LocalRepoPath);
            var files = dir.GetFiles($"{Project.Name}*.*");
            foreach (var file in files)
            {
                try { file.Delete(); }
                catch (Exception e)
                {
                    Write(true, Red, "Cannot delete file: "); WriteLine(file.FullName);
                    WriteLine(Red, $"- {e.Message}");
                    return false;
                }
            }
        }

        // Finishing...
        return true;
    }

    /// <summary>
    /// Invoked to obtain the build NuGet package files in the project directory branch.
    /// </summary>
    void GetNuPackageFiles(out List<FileInfo> regulars, out List<FileInfo> symbols)
    {
        var dir = new DirectoryInfo(Project.Directory);
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
    bool CompileProject(BuildMode mode)
    {
        var code = Command.Execute(
            "dotnet",
            $"build -c {mode} {Path.GetFileName(Project.FullName)}",
            Path.GetDirectoryName(Project.FullName));

        return code == 0;
    }

    // ----------------------------------------------------

    /// <summary>
    /// Invoked to push the package files.
    /// </summary>
    bool PushPackage(BuildMode mode)
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
    /// Invoked to update the references to this project in all projects in the solution.
    /// </summary>
    bool UpdateReferences(
        BuildMode mode,
        ProjectBackups backups,
        SemanticVersion oldversion, SemanticVersion newversion)
    {
        var pname = Project.Name;

        var root = Program.GetSolutionDirectory();
        var items = MenuBuilder.FindProjects(root);

        foreach (var item in items)
        {
            var captured = false;

            var nrefs = item.GetNuReferences();
            foreach (var nref in nrefs)
            {
                var nname = nref.Name;
                if (string.Compare(nname, pname, ignoreCase: true) != 0) continue;

                var nversion = nref.Version;

                Black-Black-BLA....

                if (nversion.CompareTo(oldversion) != 0) continue;

                Write(true, Green, "Updating package on project: ");
                WriteLine(true, item.Name);

                if (!captured)
                {
                    backups.Update(item);
                    captured = true;
                }
                nref.Version = newversion;
            }

            if (captured) item.SaveContents();
        }

        return true;
    }
}