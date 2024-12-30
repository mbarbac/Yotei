namespace Runner;

// ========================================================
public class EntryBuildAll : MenuEntry
{
    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    /// <param name="projects"></param>
    public EntryBuildAll(List<Project> projects) => Projects = projects.ThrowWhenNull();

    /// <summary>
    /// The collection of projects to build.
    /// </summary>
    public List<Project> Projects { get; }

    /// <inheritdoc/>
    public override string Header() => "All Packages";

    /// <inheritdoc/>
    public override void Execute() { }
}