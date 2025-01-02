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
    public override string Header() => Project.NameVersion;

    // ----------------------------------------------------

    /// <inheritdoc/>
    public override void Execute()
    {
        WriteLine(true);
        WriteLine(Green, Program.SlimSeparator);
        Write(true, Green, "Package: "); WriteLine(Header());

        // Capturing build mode...
        WriteLine(true);
        WriteLine(true, Green, "Please select the desired build mode:");
        WriteLine(true);
        if (!MenuBuilder.CaptureMode(out var mode)) return;

        // Executing under backup protection...
        var backups = new BuildBackups();
        var done = false;
        try
        {
            done = Execute(backups, mode);
        }
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
    /// Invoked to build and push the project this instance refers to.
    /// <br/> Once built and pushed, its version is updated, and the references to it in all
    /// other projects of the solution updated.
    /// </summary>
    /// <param name="backups"></param>
    /// <param name="mode"></param>
    /// <returns></returns>
    public bool Execute(BuildBackups backups, BuildMode mode)
    {
        backups.ThrowWhenNull();
        backups.AddOrIgnore(Project);

        WriteLine(true);
        WriteLine(true, Green, Program.FatSeparator);
        Write(true, Green, "Compiling project: "); Write(true, Project.Name);
        Write(true, Green, " for mode: "); WriteLine(true, mode.ToString());

        // Intercepting not-packable projects...
        if (!Project.IsPackable(out var version))
        {
            WriteLine(true);
            WriteLine(true, Red, "Project is not a packable one.");
            return false;
        }

        // Adjusting current version...
        if ((mode == BuildMode.Debug || mode == BuildMode.Local) &&
            version.PreRelease.IsEmpty)
        {
            version = version with { PreRelease = "v001" };
            Project.SetVersion(version, out _);
            Project.SaveContents();
        }

        if (mode == BuildMode.Release && !version.PreRelease.IsEmpty)
        {
            version = version with { PreRelease = "" };
            Project.SetVersion(version, out _);
            Project.SaveContents();
        }

        Write(true, Green, "Building version: "); WriteLine(version);

        // Processing...
        WriteLine(true);
        WriteLine(true, Green, "Deleting old files...");
        if (!DeleteFiles()) return false;

        WriteLine(true);
        WriteLine(true, Green, "Compiling project...");
        if (!CompileProject(mode)) return false;

        WriteLine(true);
        WriteLine(true, Green, "Pushing package files...");
        if (!PushPackage(mode))
        {
            WriteLine(true);
            WriteLine(true, Red, "Error while pushing package...");
            WriteLine(true, Green, "Do you want to continue [Y/N]? ");
            if (!MenuBuilder.CaptureYesNo()) return false;
        }

        // Updates references to the **pushed** package, used for Local/Release only...
        WriteLine(true);
        WriteLine(true, Green, "Updating references...");
        if (!UpdateReferences(backups, mode, version)) return false;

        // Updating version...
        switch (mode)
        {
            case BuildMode.Debug:
            case BuildMode.Local:
                version = version.IncreasePreRelease("v001");
                break;

            case BuildMode.Release:
                version = version.IncreasePatch() with { PreRelease = "v001" };
                break;
        }
        WriteLine(true);
        Write(true, Green, "Updating working version to: ");
        WriteLine(true, version);

        Project.SetVersion(version, out _);
        Project.SaveContents();

        // Finishing...
        return true;
    }

    // ----------------------------------------------------

    /// <summary>
    /// Invoked to delete previous files.
    /// </summary>
    bool DeleteFiles()
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
        GetNuPackageFiles(out var files, out _);
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
    }

    // ----------------------------------------------------

    /// <summary>
    /// Invoked to update the references to this project in all projects in the solution.
    /// </summary>
    bool UpdateReferences(
        BuildBackups backups,
        BuildMode mode,
        SemanticVersion version)
    {
        var root = Program.GetSolutionDirectory();
        var items = MenuBuilder.FindProjects(root);
        var pname = Project.Name;

        foreach (var item in items)
        {
            var nrefs = item.GetNuReferences();
            foreach (var nref in nrefs)
            {
                var nname = nref.Name;
                if (string.Compare(nname, pname, ignoreCase: true) != 0) continue;

                var nversion = nref.Version;
                if (mode == BuildMode.Debug && nversion.PreRelease.IsEmpty) continue;
                if (mode == BuildMode.Local && nversion.PreRelease.IsEmpty) continue;
                if (mode == BuildMode.Release && !nversion.PreRelease.IsEmpty) continue;

                Write(true, Green, "On project: "); Write(true, item.Name);
                Write(true, Green, " From: "); Write(true, nversion);
                Write(true, Green, " To: "); Write(true, version);
                WriteLine(true);

                backups.AddOrIgnore(item);
                nref.Version = version;
                item.SaveContents();
            }
        }

        return true;
    }
}