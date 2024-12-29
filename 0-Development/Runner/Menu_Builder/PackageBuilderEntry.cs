using static Yotei.Tools.Diagnostics.ConsoleEx;
using static System.ConsoleColor;

namespace Runner.Builder;

// ========================================================
/// <summary>
/// Represents a builder for a given package.
/// </summary>
public class PackageBuilderEntry : MenuEntry
{
    public Project Project { get; }
    public SemanticVersion Version { get; }
    BuildMode BuildMode { get; }

    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    /// <param name="project"></param>
    /// <param name="mode"></param>
    public PackageBuilderEntry(Project project, BuildMode mode)
    {
        if (!project.ThrowWhenNull().IsPackable()) throw new ArgumentException(
            "Project is not a packable one.")
            .WithData(project);

        Project = project;

        if (!project.GetVersion(out var version)) throw new ArgumentException(
            "Cannot obtain project version.")
            .WithData(project);

        Version = version;
        BuildMode = mode;
    }

    /// <inheritdoc/>
    public override string Header() => $"{Project.Name} v:{Version}";

    /// <inheritdoc/>
    public override void Execute()
    {
        var backup = Project.SaveBackup();
        var done = Project.Build(BuildMode, fatSeparator: false);

        if (!done) Project.RestoreBackup(backup);
        return;
    }
}