using static System.ConsoleColor;
namespace Runner;

// ========================================================
public class MenuPackage : ConsoleMenu.Entry
{
    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public override string Headline() => "Manage Packages";

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public override void Execute()
    {
        var root = Program.GetSolutionDirectory();

        int position = 0; do
        {
            var projects = Program.FindProjects(root);
            projects = [.. projects.Where(x => x.GetVersion(out _))];
            if (projects.Count == 0)
            { Console.WriteLineEx(true, Red, "Cannot find any project."); return; }

            var items = projects.Select(x => new ConsoleMenu.Entry(x.ToString()));
            var menu = new ConsoleMenu { ToDebug = Program.ToDebug, Timeout = Program.Timeout }
            .Add(new("Exit"))
            .AddRange(items);

            Console.PreventBufferOverflow(menu.Count + 5);
            Console.WriteLineEx(true);
            Console.WriteLineEx(true, Green, Program.FatSeparator);
            Console.WriteLineEx(true, Green, Headline());
            Console.WriteLineEx(true);

            if (position >= menu.Count) position = 0;
            position = menu.RunVertical(position);
            if (position > 0) { var project = projects[position - 1]; Execute(project); }
        }
        while (position > 0);
    }

    // ----------------------------------------------------

    /// <summary>
    /// Invoked to select the action to execute with the given project.
    /// </summary>
    static void Execute(ProjectFile project)
    {
        Console.WriteLineEx(true);
        Console.WriteLineEx(true, Green, Program.FatSeparator);
        Console.WriteEx(true, Cyan, "Package: ");
        Console.WriteLineEx(true, project.ToString());

        // Capturing versions...
        SemanticVersion debug;
        SemanticVersion local;
        SemanticVersion release;

        if (!project.GetVersion(out debug!))
        {
            Console.WriteLineEx(true, Red, "Cannot get project version.");
            debug = "";
        }
        try
        {
            release = NuGet.GetLatestVersion(project.Name, includePreRelease: true) ?? "";
            Console.WriteEx(true, Cyan, "Latest published NuGet: "); Console.WriteLine(release.ToString(true));
        }
        catch (Exception e)
        {
            Console.WriteLineEx(true, Red, "Cannot get lastest project version.");
            Console.WriteLineEx(true, Red, $"NuGet Error: {e.Message}");
            release = "";
        }        
        local = debug;

        // Editing...
        Console.WriteLineEx(true);
        Console.WriteLineEx(true, Green, Program.SlimSeparator);

        Console.WriteLineEx(true, Green, "Package versions (leave blank to ignore)");
        Console.WriteEx(true, Green, "Project version: ");
        debug = Console.EditLineEx(true, debug.IsEmpty ? "" : debug.ToString(true)).NullWhenEmpty(trim: true) ?? "";
        Console.WriteEx(true, Green, "Local version:   ");
        local = Console.EditLineEx(true, local.IsEmpty ? "" : local.ToString(true)).NullWhenEmpty(trim: true) ?? "";
        Console.WriteEx(true, Green, "Release version: ");
        release = Console.EditLineEx(true, release.IsEmpty ? "" : release.ToString(true)).NullWhenEmpty(trim: true) ?? "";

        if (debug.IsEmpty && local.IsEmpty && release.IsEmpty) return;

        Console.WriteLineEx(true);
        Console.WriteLineEx(true, Green, "Compile and Push (leave blank to ignore)");
        Console.Write(Green, "Local:   "); var complocal = Program.CaptureYesNo(false);
        Console.Write(Green, "Release: "); var comprelease = Program.CaptureYesNo(false);

        Console.WriteLineEx(true);
        Console.WriteEx(true, Green, "Do you want to proceed? ");
        if (!Program.CaptureYesNo(false)) return;

        // Executing...
        var data = new ExecuteData(project, debug, local, release, complocal, comprelease);
        try
        {
            // Proceeding...
            ChangeVersions(data);
            CompileAndPush(data);

            // Saving the modifications...
            foreach (var item in data.ToSave) item.SaveToDisk();
        }
        catch (Exception e)
        {
            Console.WriteLineEx(true);
            Console.WriteLineEx(true, Green, Program.SlimSeparator);
            Console.WriteLineEx(true, Red, e.ToDisplayString());
            data.Errors = true;
        }

        // Reverting if needed...
        if (data.Errors)
        {
            Console.WriteLineEx(true);
            Console.WriteLineEx(true, Green, Program.SlimSeparator);
            Console.WriteLineEx(true, Green, "Reverting to previous state...");
            data.Backup.Restore(display: true);
        }
    }

    // ----------------------------------------------------

    internal record ExecuteData(
        ProjectFile Project,
        SemanticVersion Debug, SemanticVersion Local, SemanticVersion Release,
        bool CompileLocal, bool CompileRelease)
    {
        public BackupMaster Backup { get; } = new();
        public List<ProjectFile> ToSave { get; } = [];
        public bool Errors = false;
    }

    // ----------------------------------------------------

    /// <summary>
    /// Invoked to change the versions according to the captured data.
    /// </summary>
    static void ChangeVersions(ExecuteData data)
    {
        // Saving a backup copy...
        data.Backup.Add(data.Project);

        // Updating project's version...
        if (!data.Debug.IsEmpty)
        {
            Console.WriteLineEx(true);
            Console.WriteLineEx(true, Green, Program.SlimSeparator);
            Console.WriteLineEx(true, Green, "Updating project's version...");

            if (!data.Project.UpdateVersion(data.Debug)) throw new UnExpectedException("Cannot change project version.");
            data.ToSave.Add(data.Project);
        }

        // Updating references in other projects...
        if (data.Local.IsEmpty && data.Release.IsEmpty) return;

        Console.WriteLineEx(true);
        Console.WriteLineEx(true, Green, Program.SlimSeparator);
        Console.WriteLineEx(true, Green, "Updating references...");
        Console.WriteLineEx(true);

        var root = Program.GetSolutionDirectory();
        var items = Program.FindProjects(root);
        var mode = BuildMode.Debug;

        foreach (var item in items)
        {
            if (data.Project.NameAndExtension == item.NameAndExtension) continue;
            data.Backup.Add(item);
            Console.WriteLineEx(true, Yellow, item.Name);

            var done = false;
            foreach (var line in item)
            {
                RollingMode(line, ref mode);
                if (!line.GetNuPackageName(out var name)) continue;
                if (!line.GetNuPackageVersion(out var version)) continue;
                if (string.Compare(data.Project.Name, name, ignoreCase: true) != 0) continue;

                if (mode == BuildMode.Local && !data.Local.IsEmpty)
                {
                    Console.WriteEx(true, Cyan, "Local: ");
                    Console.WriteLineEx(true, $" {version.ToString(true)} => {data.Local.ToString(true)}");
                    line.UpdateNuPackageVersion(data.Local);
                    done = true;
                }
                if (mode == BuildMode.Release && !data.Release.IsEmpty)
                {
                    Console.WriteEx(true, Cyan, "Release: ");
                    Console.WriteLineEx(true, $" {version.ToString(true)} => {data.Release.ToString(true)}");
                    line.UpdateNuPackageVersion(data.Release);
                    done = true;
                }
            }
            if (done) data.ToSave.Add(item);
        }
    }

    /// <summary>
    /// Updates the given build mode reference if such is needed because the contents of the
    /// given line, reverting to debug mode as the default one.
    /// </summary>
    static void RollingMode(string line, ref BuildMode mode)
    {
        if (line.Contains("<ItemGroup") &&
            line.Contains("Condition") &&
            line.Contains(""))
        {
            if (line.Contains("Debug")) { mode = BuildMode.Debug; return; }
            if (line.Contains("Local")) { mode = BuildMode.Local; return; }
            if (line.Contains("Release")) { mode = BuildMode.Release; return; }
        }
        else if (line.Contains("</ItemGroup>"))
        {
            if (mode == BuildMode.Local) { mode = BuildMode.Debug; return; }
            if (mode == BuildMode.Release) { mode = BuildMode.Debug; return; }
        }
    }

    // ----------------------------------------------------

    /// <summary>
    /// Invoked to compile and push the package according to the captured data.
    /// </summary>
    static void CompileAndPush(ExecuteData data)
    {
        if (!data.Project.IsPackable())
        {
            Console.WriteLine(Magenta, $"Project is not a NuGet package.");
            return;
        }
        if (data.CompileLocal) CompileAndPush(data, BuildMode.Local);
        if (data.CompileRelease) CompileAndPush(data, BuildMode.Release);
    }

    /// <summary>
    /// Invoked to compile and push for the given build mode.
    /// </summary>
    /// <param name="data"></param>
    static void CompileAndPush(ExecuteData data, BuildMode mode)
    {
        data.Backup.Add(data.Project);

        // Compiling package...
        Console.WriteLineEx(true);
        Console.WriteLineEx(true, Green, Program.SlimSeparator);
        Console.WriteEx(true, Green, "Compiling: "); Console.WriteEx(true, data.Project.ToString());
        Console.WriteEx(true, Green, " Mode: "); Console.WriteLineEx(true, mode.ToString());

        Console.WriteLineEx(true);
        Console.WriteEx(true, Green, "Cleaning old package files...");
        NuGet.GetFiles(data.Project, out var regulars, out var symbols);
        NuGet.DeleteFiles(regulars);
        NuGet.DeleteFiles(symbols);

        var version = mode == BuildMode.Local ? data.Local : data.Release;
        if (!data.Project.UpdateVersion(version)) throw new UnExpectedException("Cannot update project version.");
        data.Project.SaveToDisk();

        var name = Path.GetFileName(data.Project.FullName);
        var dir = Path.GetDirectoryName(data.Project.FullName);
        var done = SysCommand.Execute("dotnet", $"build -c {mode} {name}", dir);
        if (done != 0) throw new UnExpectedException("Error while compiling package.");

        if (!data.Project.UpdateVersion(data.Debug)) throw new UnExpectedException("Cannot restore project version.");
        data.Project.SaveToDisk();

        // Pushing package...
        Console.WriteLineEx(true);
        Console.WriteLineEx(true, Green, Program.SlimSeparator);
        Console.WriteEx(true, Green, "Pushing: "); Console.WriteEx(true, data.Project.ToString());
        Console.WriteEx(true, Green, " Mode: "); Console.WriteLineEx(true, mode.ToString());

        NuGet.GetFiles(data.Project, out regulars, out symbols);
        if (regulars.Count == 0) throw new UnExpectedException("No package files have been created.");

        var file = regulars[0];
        var cmd = mode switch
        {
            BuildMode.Local => $"nuget push {file} -s {Program.LocalRepoSource}",
            BuildMode.Release => $"nuget push {file} -s https://nuget.org",
            _ => throw new UnExpectedException($"Invalid push mode: {mode}")
        };
        done = SysCommand.Execute("dotnet", cmd, file.DirectoryName);
        if (done != 0) throw new UnExpectedException("Error while pushing package.");
    }
}