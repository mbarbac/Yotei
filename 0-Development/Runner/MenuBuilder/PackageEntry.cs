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

    /// <inheritdoc/>
    public override void Execute()
    {
    }
}