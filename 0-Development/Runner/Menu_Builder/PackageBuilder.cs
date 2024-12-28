using static Yotei.Tools.Diagnostics.ConsoleEx;
using static System.ConsoleColor;

namespace Runner.Builder;

// ========================================================
/// <summary>
/// Represents a builder for a given package.
/// </summary>
public class PackageBuilder : MenuEntry
{
    public Project Project { get; }
    public SemanticVersion Version { get; }
    BuildMode BuildMode { get; }

    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    /// <param name="project"></param>
    /// <param name="mode"></param>
    public PackageBuilder(Project project, BuildMode mode)
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
        WriteLine(true);
        WriteLine(true, Green, Program.SlimSeparator);
        Write(true, Green, "Compiling: "); Write(Header());
        Write(true, Green, " for mode: "); WriteLine(BuildMode.ToString());
        WriteLine(true);
    }
}