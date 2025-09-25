using static Yotei.Tools.ConsoleEx;
using static System.ConsoleColor;

namespace Runner;

// ========================================================
public class PackageEntry : ConsoleMenuEntry
{
    /// <summary>
    /// The project this instance refers to.
    /// </summary>
    public Project Project { get; }

    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    /// <param name="project"></param>
    public PackageEntry(Project project) => Project = project.ThrowWhenNull();

    /// <inheritdoc/>
    public override string Header() => Project.NameVersion;

    // ----------------------------------------------------

    /// <inheritdoc/>
    public override void Execute()
    {
        WriteLine(true);
        WriteLine(true, Green, Program.FatSeparator);
        Write(true, Green, "Package: "); WriteLine(true, Header());

        // Capturing the desired build mode...
        var bmode = BuildMode.Local;
        WriteLine(true);
        WriteLine(true, Green, "Select Build Mode:");
        if (!CaptureBuildMode(ref bmode)) return;

        // Capturing the desired increase mode, if needed...
        var imode = IncreaseMode.PreRelease;
        if (bmode == BuildMode.Release)
        {
            WriteLine(true);
            WriteLine(true, Green, "Select Increase Mode:");
            if (!CaptureIncreaseMode(ref imode)) return;
        }

        // Executing under a backups' umbrella...
        var backups = new BuildBackups();
        var saved = backups.Add(Project);
        saved.AddRange(Project);

        try
        {
            // Cleaning directory...
            WriteLine(true);
            WriteLine(true, Green, "Cleaning old package files...");
            GetNuFiles(Project, out var regulars, out var symbols);
            DeleteFiles(regulars);
            DeleteFiles(symbols);

            // Compling project...
            WriteLine(true);
            WriteLine(true, Green, "Compiling project...");
            if (!CompileProject(Project, bmode)) return;

            // Executing...
            if (bmode == BuildMode.Local) ExecuteLocal();
            if (bmode == BuildMode.Release) ExecuteRelease(imode, backups);
        }
        catch (Exception ex)
        {
            WriteLine(true);
            WriteLine(true, Red, "Exception intercepted:");
            WriteLine(true, ex.ToDisplayString());

            WriteLine(true);
            WriteLine(true, Red, "Reverting to previous state...");
            backups.Restore();

            WriteLine(true);
            Write(true, Green, "Press [Enter] to continue...");
            Console.ReadLine();
        }
    }

    // ----------------------------------------------------

    /// <summary>
    /// LOCAL mode: just push to local repo.
    /// </summary>
    void ExecuteLocal() => PushPackage(Project, BuildMode.Local);

    // ----------------------------------------------------

    /// <summary>
    /// RELEASE mode: push to nuget, increase version, and update references.
    /// </summary>
    void ExecuteRelease(IncreaseMode imode, BuildBackups backups)
    {
        // Push may fail because it is already in NuGet...
        if (!PushPackage(Project, BuildMode.Release))
        {
            WriteLine(true);
            Write(true, Green, "Please enter [YES] to continue... ");

            if (!EditLine(true, Program.Timeout, null, out var value) ||
                value is null ||
                string.Compare("YES", value, ignoreCase: true) != 0)
                return;
        }

        // Capturing and increasing version...
        if (!Project.GetVersion(out var original)) return;
        if (!IncreaseVersion(original, imode, out var increased)) return;
        var reduced = increased.PreRelease.IsEmpty ? increased : increased with { PreRelease = "" };
        var enlarged = increased.PreRelease.IsEmpty ? increased with { PreRelease = "v0001" } : increased;

        // Setting project's version...
        WriteLine(true);
        Write(true, Green, "Setting new version to: "); WriteLine(true, enlarged);
        if (!Project.SetVersion(enlarged))
        {
            Write(true, Red, "Cannot set new version for: ");
            WriteLine(true, Project.Name);
            return;
        }
        Project.SaveContents();

        // Updating references, need to throw exceptions if fail...
        WriteLine(true);
        WriteLine(true, Green, "Updating references... ");

        var root = Program.GetSolutionDirectory();
        var items = MenuBuilder.FindProjects(root);
        foreach (var item in items)
        {
            // Pre-saving actual state...
            var saved = item.Select(x => new ProjectLine(x)).ToList();
            var modified = false;

            // Iterating though its lines...
            var ulines = item.Where(x => x.IsNuReference()).ToList();
            foreach (var uline in ulines)
            {
                if (!uline.GetNuName(out var uname)) continue;
                if (!uline.GetNuVersion(out var uversion)) continue;
                if (string.Compare(Project.Name, uname, ignoreCase: true) != 0) continue;

                if (!modified) // First-time modification...
                {
                    WriteLine(true);
                    Write(true, Green, "Modifying: "); WriteLine(true, item.NameExtension);

                    var temps = backups.Add(item);
                    temps.Clear();
                    temps.AddRange(saved);
                    modified = true;
                }

                var done = uversion.PreRelease.IsEmpty
                        ? uline.SetNuVersion(reduced)
                        : uline.SetNuVersion(enlarged);

                if (!done) throw new Exception($"Cannot modify line: {uline}");
            }

            // If item is modified, save its new contents...
            if (modified) item.SaveContents();
        }
    }

    // ----------------------------------------------------

    /// <summary>
    /// Invoked to capture the desired build mode, if any.
    /// </summary>
    /// <param name="mode"></param>
    /// <returns></returns>
    static bool CaptureBuildMode(ref BuildMode mode)
    {
        var option = mode switch
        {
            BuildMode.Local => 1,
            BuildMode.Release => 2,
            _ => 0,
        };
        option = new ConsoleMenu
        {
            new("Exit"),
            new("Local (Build package and Push to local repo)"),
            new("Release (Build package, push to Nuget, and Increase version)"),
        }
        .Run(true, Green, Program.Timeout, option);

        switch (option)
        {
            case 1: mode = BuildMode.Local; return true;
            case 2: mode = BuildMode.Release; return true;
        }
        return false;
    }

    // ----------------------------------------------------

    /// <summary>
    /// Invoked to capture the desired increase mode, if any.
    /// </summary>
    /// <param name="mode"></param>
    /// <returns></returns>
    static bool CaptureIncreaseMode(ref IncreaseMode mode)
    {
        var option = mode switch
        {
            IncreaseMode.Major => 1,
            IncreaseMode.Minor => 2,
            IncreaseMode.Patch => 3,
            IncreaseMode.PreRelease => 4,
            _ => 0,
        };
        option = new ConsoleMenu
        {
            new("Exit"),
            new("Major"), new("Minor"), new("Patch"), new("PreRelease"),
        }
        .Run(true, Green, Program.Timeout, option);

        switch (option)
        {
            case 1: mode = IncreaseMode.Major; return true;
            case 2: mode = IncreaseMode.Minor; return true;
            case 3: mode = IncreaseMode.Patch; return true;
            case 4: mode = IncreaseMode.PreRelease; return true;
        }
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
    /// Compiles the given project for the given mode.
    /// </summary>
    static bool CompileProject(Project project, BuildMode mode)
    {
        var name = Path.GetFileName(project.FullName);
        var dir = Path.GetDirectoryName(project.FullName);

        var code = SysCommand.Execute("dotnet", $"build -c {mode} {name}", dir);
        return code == 0;
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

    // ----------------------------------------------------

    /// <summary>
    /// Returns a new version increased according to the given increase mode.
    /// </summary>
    static bool IncreaseVersion(
        SemanticVersion version, IncreaseMode imode,
        [NotNullWhen(true)] out SemanticVersion? result)
    {
        switch (imode)
        {
            case IncreaseMode.Major: result = version.IncreaseMajor(); return true;
            case IncreaseMode.Minor: result = version.IncreaseMinor(); return true;
            case IncreaseMode.Patch: result = version.IncreasePatch(); return true;
            case IncreaseMode.PreRelease:
                result = version.IncreasePreRelease(out var done, "v0001");
                return done;
        }
        throw new ArgumentException("Invalid increase mode.").WithData(imode);
    }
}