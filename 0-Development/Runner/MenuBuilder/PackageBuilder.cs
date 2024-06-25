using static Yotei.Tools.Diagnostics.ConsoleWrapper;
using static System.ConsoleColor;

namespace Runner.Builder;

// ========================================================
/// <summary>
/// Provides the ability of building packages.
/// </summary>
public static class PackageBuilder
{
    /// <summary>
    /// Builds the given packable project for RELEASE mode.
    /// </summary>
    /// <param name="packable"></param>
    /// <returns></returns>
    public static bool BuildRelease(Project packable)
    {
        if (!IsPackable(packable, out var oldversion)) return false;

        WriteLine(true);
        WriteLine(Green, Program.SlimSeparator);
        WriteLine(true);
        Write(true, Magenta, "Build Release Package: ");
        WriteLine($"{packable.Name} v:{oldversion}");

        var backups = new List<Project>();
        AddBackup(backups, packable);

        var done = Execute();
        if (!done) RestoreBackups(backups);
        return done;

        // Executor...
        bool Execute()
        {
            // Release...
            if (!DeleteNuGetFiles(packable)) return false;

            var newversion = oldversion with { PreRelease = "" };
            if (!SetVersion(packable, newversion)) return false;

            if (!CompileProject(packable, BuildMode.Release)) return false;
            if (!PushPackage(packable, BuildMode.Release)) return false;
            if (!UpdateReferences(packable, newversion, backups)) return false;

            // Debug...
            if (!DeleteNuGetFiles(packable)) return false;

            newversion = newversion.IncreasePatch();
            newversion = newversion.IncreasePreRelease("v0000");
            if (!SetVersion(packable, newversion)) return false;

            if (!CompileProject(packable, BuildMode.Debug)) return false;
            if (!PushPackage(packable, BuildMode.Debug)) return false;
            if (!UpdateReferences(packable, newversion, backups)) return false;

            return true;
        }
    }

    /// <summary>
    /// Builds the given packable project for DEBUG mode.
    /// </summary>
    /// <param name="packable"></param>
    /// <returns></returns>
    public static bool BuildDebug(Project packable)
    {
        if (!IsPackable(packable, out var oldversion)) return false;

        WriteLine(true);
        WriteLine(Green, Program.SlimSeparator);
        WriteLine(true);
        Write(true, Magenta, "Build Debug Package: ");
        WriteLine($"{packable.Name} v:{oldversion}");

        var backups = new List<Project>();
        AddBackup(backups, packable);

        var done = Execute();
        if (!done) RestoreBackups(backups);
        return done;

        // Executor...
        bool Execute()
        {
            if (!DeleteNuGetFiles(packable)) return false;

            var newversion = oldversion.IncreasePreRelease("v0000");
            if (!SetVersion(packable, newversion)) return false;

            if (!CompileProject(packable, BuildMode.Debug)) return false;
            if (!PushPackage(packable, BuildMode.Debug)) return false;
            if (!UpdateReferences(packable, newversion, backups)) return false;

            return true;
        }
    }

    /// <summary>
    /// Rebuilds the given packable project for the LOCAL REPO, without updating its version, the
    /// references to it in any other project, and not publishin it to the public repository.
    /// </summary>
    /// <param name="packable"></param>
    /// <returns></returns>
    public static bool UpdateLocalRepo(Project packable)
    {
        if (!IsPackable(packable, out var oldversion)) return false;

        WriteLine(true);
        WriteLine(Green, Program.SlimSeparator);
        WriteLine(true);
        Write(true, Magenta, "Updating local repo for: ");
        WriteLine($"{packable.Name} v:{oldversion}");

        var backups = new List<Project>();
        AddBackup(backups, packable);

        var done = Execute();
        if (!done) RestoreBackups(backups);
        return done;

        // Executor...
        bool Execute()
        {
            if (!DeleteNuGetFiles(packable)) return false;
            if (!CompileProject(packable, BuildMode.Debug)) return false;
            if (!PushPackage(packable, BuildMode.Debug)) return false;

            return true;
        }
    }

    // ----------------------------------------------------

    /// <summary>
    /// Adds to the given list a backup of the given project.
    /// </summary>
    /// <param name="backups"></param>
    /// <param name="project"></param>
    /// <returns></returns>
    public static void AddBackup(List<Project> backups, Project project)
    {
        var temp = new Project(project.FullName);
        temp.Lines.Clear();
        temp.Lines.AddRange(project.CopyLines());

        backups.Add(temp);
    }

    /// <summary>
    /// Restores the given backups.
    /// </summary>
    /// <param name="backups"></param>
    public static void RestoreBackups(List<Project> backups)
    {
        WriteLine(true);
        WriteLine(true, Green, Program.SlimSeparator);
        WriteLine(true, Red, "Recovering project backups!");
        WriteLine(true);

        foreach (var item in backups)
        {
            WriteLine(true, Path.GetFileName(item.FullName));
            item.SaveFile();
        }
    }

    // ----------------------------------------------------

    /// <summary>
    /// Asks for the build mode to use.
    /// </summary>
    /// <param name="mode"></param>
    /// <returns></returns>
    public static bool AskForMode(out BuildMode mode)
    {
        WriteLine(true);
        WriteLine(true, Green, "Select the build mode to use:");
        WriteLine(true);

        return Program.CaptureBuildMode(out mode);
    }

    /// <summary>
    /// Determines if the project is a packable one or not.
    /// </summary>
    /// <param name="packable"></param>
    /// <param name="oldversion"></param>
    /// <returns></returns>
    public static bool IsPackable(
        Project packable,
        [NotNullWhen(true)] out SemanticVersion? oldversion)
    {
        if (!packable.IsPackable())
        {
            Write(Red, "Project is not packable: ");
            WriteLine(packable.NameAndExtension);
            oldversion = null;
            return false;
        }
        if (!packable.GetVersion(out oldversion))
        {
            Write(Red, "Cannot obtain package version of: ");
            WriteLine(packable.NameAndExtension);
            return false;
        }
        return true;
    }

    // ----------------------------------------------------

    /// <summary>
    /// Deletes the NuGet package and symbol files found for the given project.
    /// </summary>
    /// <param name="packable"></param>
    /// <returns></returns>
    public static bool DeleteNuGetFiles(Project packable)
    {
        WriteLine(true);
        WriteLine(true, Green, Program.SlimSeparator);

        packable.GetPackageFiles(out var regulars, out var symbols);
        if (!DeleteFiles(regulars, "Deleting package files...")) return false;
        if (!DeleteFiles(symbols, "Deleting package symbols...")) return false;
        return true;

        // Invoked to delete the given collection of files...
        static bool DeleteFiles(List<string> files, string description)
        {
            WriteLine(true);
            WriteLine(true, Green, description);

            foreach (var file in files)
            {
                try
                {
                    var temp = new FileInfo(file);
                    temp.Delete();
                }
                catch (Exception e)
                {
                    Write(Red, "Cannot delete file: ");
                    Write(Path.GetFileName(file));
                    WriteLine(Red, $" - {e.Message}");
                    return false;
                }
            }
            return true;
        }
    }

    // ----------------------------------------------------

    /// <summary>
    /// Sets the given version on the project file, and saves it.
    /// </summary>
    /// <param name="packable"></param>
    /// <param name="version"></param>
    /// <returns></returns>
    public static bool SetVersion(Project packable, SemanticVersion version)
    {
        WriteLine(true);
        WriteLine(true, Green, Program.SlimSeparator);
        Write(true, Green, "Setting version to: ");
        WriteLine(true, version);

        if (!packable.SetVersion(version, out _))
        {
            WriteLine(true);
            WriteLine(true, Red, $"Cannot set package version to: {version}");
            return false;
        }
        packable.SaveFile();
        return true;
    }

    /// <summary>
    /// Compiles the given project for the given build mode.
    /// </summary>
    /// <param name="packable"></param>
    /// <param name="mode"></param>
    /// <returns></returns>
    public static bool CompileProject(Project packable, BuildMode mode)
    {
        WriteLine(true);
        WriteLine(true, Green, Program.SlimSeparator);
        Write(true, Green, "Compiling project for mode: ");
        WriteLine(true, mode.ToString());

        WriteLine(true);
        var code = Command.Execute(
            "dotnet",
            $"build -c {mode} {Path.GetFileName(packable.FullName)}",
            Path.GetDirectoryName(packable.FullName));

        return code == 0;
    }

    /// <summary>
    /// Pushes the package NuGet files to the appropriate repo for the given build mode.
    /// </summary>
    /// <param name="packable"></param>
    /// <param name="mode"></param>
    /// <returns></returns>
    public static bool PushPackage(Project packable, BuildMode mode)
    {
        WriteLine(true);
        WriteLine(true, Green, Program.SlimSeparator);
        Write(true, Green, "Pushing package files for mode: ");
        WriteLine(true, mode.ToString());

        packable.GetPackageFiles(out var files, out _);
        if (files.Count == 0) { WriteLine(true, Red, "No files found!"); return false; }
        if (files.Count > 1) { WriteLine(true, Red, "Too many files found!"); return false; }

        var file = files[0];
        WriteLine(true);
        Write(true, Green, "Pushing package file: ");
        WriteLine(true, Path.GetFileNameWithoutExtension(file));

        var cmd = mode == BuildMode.Debug
            ? $"nuget push {file} -s {Program.LocalRepoSource}"
            : $"nuget push {file} -s {Program.NuGetRepoSource}";

        WriteLine(true);
        var done = Command.Execute("dotnet", cmd, Path.GetDirectoryName(file));
        if (done != 0) { WriteLine(true, Red, "Cannot push file!"); return false; }
        return true;
    }

    /// <summary>
    /// Updates for the other projects in the solution the references to this packable one,
    /// using the given newversion, which determines if the debug or release references are
    /// updated.
    /// </summary>
    /// <param name="packable"></param>
    /// <param name="newversion"></param>
    /// <param name="backups"></param>
    /// <returns></returns>
    public static bool UpdateReferences(
        Project packable, SemanticVersion newversion, List<Project> backups)
    {
        WriteLine(true);
        WriteLine(true, Green, Program.SlimSeparator);
        Write(true, Green, "Updating package references for mode: ");

        var name = packable.Name;
        var mode = newversion.PreRelease.IsEmpty ? BuildMode.Release : BuildMode.Debug;

        var projects = FindProjects();
        foreach (var project in projects)
        {
            AddIntoBackups(project);

            var nrefs = project.GetNuReferences();
            foreach (var nref in nrefs)
            {
                var nname = nref.Name;
                if (nname == null) continue;
                if (string.Compare(nname, name, ignoreCase: true) != 0) continue;

                var nversion = nref.Version;
                if (nversion == null) continue;

                var nmode = nversion.PreRelease.IsEmpty ? BuildMode.Release : BuildMode.Debug;
                if (nmode != mode) continue;

                Write(true, Green, "Updating package version on: ");
                WriteLine(true, Path.GetFileName(project));

                try { nref.Version = newversion; }
                catch
                {
                    WriteLine(true, Red, "Cannot set the reference version.");
                    return false;
                }
                try { project.SaveFile(); }
                catch
                {
                    WriteLine(true, Red, "Cannot save the references project.");
                    return false;
                }
            }
        }

        return true;

        // Adds the project into the list of backups, if not added yet.
        void AddIntoBackups(Project project)
        {
            foreach (var item in backups)
                if (string.Compare(item.FullName, packable.FullName, ignoreCase: true) == 0)
                    return;

            AddBackup(backups, project);
        }

        // Gets a list with the projects to update...
        List<Project> FindProjects()
        {
            var root = Program.GetSolutionDirectory();
            var projects = root.FindProjects(exclusion: null);

            foreach (var project in projects.ToList())
            {
                if (string.Compare(project.FullName, packable.FullName, ignoreCase: true) == 0)
                    projects.Remove(project);
            }
            return projects;
        }
    }
}