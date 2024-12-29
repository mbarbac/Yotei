using static Yotei.Tools.Diagnostics.ConsoleEx;
using static System.ConsoleColor;
using System.Runtime.InteropServices.Marshalling;

namespace Runner.Builder;

// ========================================================
public static class Builder
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

        if (!project.IsPackable()) throw new ArgumentException(
            "Project is not a packable one.")
            .WithData(project);

        if (!project.GetVersion(out var oldversion)) throw new InvalidOperationException(
            "Cannot obtain current version of packable project.")
            .WithData(project);

        var separator = fatSeparator ? Program.FatSeparator : Program.SlimSeparator;
        WriteLine(true);
        WriteLine(true, Green, separator);
        Write(true, Green, "Compiling: "); Write(true, $"{project.Name} v:{oldversion}");
        Write(true, Green, " for mode: "); WriteLine(mode.ToString());

        WriteLine(true);
        WriteLine(true, Green, "Clearing files...");
        if (!DeleteFiles(project, mode)) return false;

        var newversion = IncreaseVersion(oldversion, mode);

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