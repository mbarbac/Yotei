namespace Runner;

// ========================================================
public class EntryBuildPackage : MenuEntry
{
    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    /// <param name="project"></param>
    public EntryBuildPackage(Project project) => Project = project.ThrowWhenNull();

    /// <summary>
    /// The project this instance refers to.
    /// </summary>
    public Project Project { get; }

    /// <inheritdoc/>
    public override string Header() => Project.NameExtensionVersion;

    /// <inheritdoc/>
    public override void Execute() { }
}