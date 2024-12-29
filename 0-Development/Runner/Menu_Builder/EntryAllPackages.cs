using static Yotei.Tools.Diagnostics.ConsoleEx;
using static System.ConsoleColor;
using Microsoft.VisualStudio.TestPlatform.PlatformAbstractions;
using System.Security.Cryptography;

namespace Runner.Builder;

// ========================================================
/// <summary>
/// Builds all the given packages.
/// </summary>
public class EntryAllPackages : MenuEntry
{
    List<Project> Projects { get; }
    List<Backup> Backups { get; }
    BuildMode BuildMode { get; }

    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    /// <param name="projects"></param>
    /// <param name="mode"></param>
    public EntryAllPackages(List<Project> projects, BuildMode mode)
    {
        Projects = projects.ThrowWhenNull();
        Backups = [];
        BuildMode = mode;
    }

    /// <inheritdoc/>
    public override string Header() => "All Projects";

    /// <inheritdoc/>
    public override void Execute()
    {
        var done = true;
        foreach (var project in Projects)
        {
            var lines = NuBuilder.SaveBackup(project);
            var backup = new Backup(project, lines);
            Backups.Add(backup);

            done = project.Build(BuildMode, fatSeparator: true);
            if (!done)
            {
                Write(Red, "Cannot compile project: ");
                WriteLine(project.FullName);
                break;
            }
        }

        if (!done)
        {
            WriteLine(true);
            WriteLine(true, Green, Program.FatSeparator);
            WriteLine(true);

            foreach (var backup in Backups)
                NuBuilder.RestoreBackup(backup.Project, backup.Lines);
        }
    }

    // ----------------------------------------------------

    public class Backup(Project project, List<ProjectLine> lines)
    {
        public Project Project { get; } = project.ThrowWhenNull();
        public List<ProjectLine> Lines { get; } = lines.ThrowWhenNull();
    }
}