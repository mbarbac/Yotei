using static Yotei.Tools.Diagnostics.ConsoleEx;
using static System.ConsoleColor;

namespace Runner.Builder;

// ========================================================
public static class NuBuilder
{
    /// <summary>
    /// Invoked to build the given project for the given build mode. Returns whether the build
    /// process has succeeded or not. This method does not save nor restore a backup in case of
    /// errors.
    /// </summary>
    /// <param name="project"></param>
    /// <param name="mode"></param>
    /// <param name="fatSeparator"></param>
    /// <returns></returns>
    public static bool Build(this Project project, BuildMode mode, bool fatSeparator)
    {
        project.ThrowWhenNull();

        var separator = fatSeparator ? Program.FatSeparator : Program.SlimSeparator;
        WriteLine(true);
        WriteLine(true, Green, separator);
        Write(true, Green, "Compiling: "); Write(true, $"{project.Name}");

        if (!project.IsPackable())
        {
            WriteLine(true);
            Write(true, Red, "Project is not a packable one: ");
            WriteLine(project.FullName);
            return false;
        }

        if (!project.GetVersion(out var oldversion))
        {
            WriteLine(true);
            Write(true, Red, "Cannot obtain current version of project: ");
            WriteLine(project.FullName);
            return false;
        }
                
        Write(true, $" v:{oldversion}");
        Write(true, Green, " for mode: "); WriteLine(mode.ToString());

        WriteLine(true);
        WriteLine(true, Green, "Clearing files...");
        if (!DeleteFiles(project, mode)) return false;

        var newversion = IncreaseVersion(oldversion, mode);
        WriteLine(true);
        Write(true, Green, "Increasing version to: "); WriteLine(newversion);
        if (!SetVersion(project, newversion)) return false;

        WriteLine(true);
        WriteLine(true, Green, "Compiling project...");
        if (!CompileProject(project, mode)) return false;

        WriteLine(true);
        WriteLine(true, Green, "Pushing package files...");
        if (!PushPackage(project, mode)) return false;

        return false;
    }

    // ----------------------------------------------------

    /// <summary>
    /// Invoked to push the package files.
    /// </summary>
    static bool PushPackage(Project project, BuildMode mode)
    {
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

        //var file = files[0];
        //var cmd = mode switch
        //{
        //    BuildMode.Debug => $"nuget push {file} -s {Program.NuGetRepoSource}",
        //    BuildMode.Local => $"nuget push {file} -s {Program.LocalRepoSource}",
        //    BuildMode.Release => $"nuget push {file} -s {Program.NuGetRepoSource}",
        //    _ => throw new ArgumentException("Unknown build mode.").WithData(mode)
        //};

        //var done = Command.Execute(
        //    "dotnet",
        //    cmd,
        //    file.DirectoryName);

        //if (done != 0) WriteLine(true, Red, "Cannot push package files!");
        //return done == 0;

        return true;
    }

    // ----------------------------------------------------

    /// <summary>
    /// Invoked to compile the given project for the given mode.
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
    /// Invoked to set the project version to the given value, and save the file.
    /// </summary>
    /// <param name="project"></param>
    /// <param name="newversion"></param>
    /// <returns></returns>
    static bool SetVersion(Project project, SemanticVersion newversion)
    {
        if (!project.SetVersion(newversion, out _))
        {
            Write(true, Red, "Cannot set the new version to: ");
            WriteLine(newversion);
            return false;
        }

        try { project.SaveContents(); }
        catch (Exception e)
        {
            Write(true, Red, "Cannot save the projet with a new version because: ");
            WriteLine(true, Red, $"- {e.Message}");
            return false;
        }

        return true;
    }

    // ----------------------------------------------------

    /// <summary>
    /// Invoked to clear the appropriate repository.
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
    /// Returns an increased semantic version using the given mode.
    /// </summary>
    /// <param name="oldversion"></param>
    /// <param name="mode"></param>
    /// <returns></returns>
    public static SemanticVersion IncreaseVersion(SemanticVersion oldversion, BuildMode mode)
    {
        oldversion.ThrowWhenNull();

        if (mode == BuildMode.Debug)
        {
            var increased = false;
            var newversion = oldversion.PreRelease.IsEmpty
                ? oldversion.IncreasePatch()
                : oldversion.IncreasePreRelease(out increased);

            if (!increased) newversion = newversion with { PreRelease = "v001" };
            return newversion;
        }
        else if (mode == BuildMode.Local)
        {
            var newversion = oldversion.PreRelease.IsEmpty
                ? oldversion with { PreRelease = "v001" }
                : oldversion;

            return newversion;
        }
        else if (mode == BuildMode.Release)
        {
            var newversion = oldversion.PreRelease.IsEmpty
                ? oldversion.IncreasePatch()
                : oldversion with { PreRelease = "" };

            return newversion;
        }
        else throw new UnExpectedException("Unknown build mode.").WithData(mode);
    }

    // ----------------------------------------------------

    /// <summary>
    /// Invoked to save a backup of the current project file contents.
    /// </summary>
    /// <param name="project"></param>
    /// <returns></returns>
    public static List<ProjectLine> SaveBackup(this Project project)
    {
        return project.ThrowWhenNull().SaveLines();
    }

    /// <summary>
    /// Invoked to restore the backup.
    /// </summary>
    public static void RestoreBackup(this Project project, List<ProjectLine> backup)
    {
        project.ThrowWhenNull();
        backup.ThrowWhenNull();

        Write(true, Green, "Recovering backup for project: "); WriteLine(project.Name);
        project.RestoreLines(backup);
        project.SaveContents();
    }
}