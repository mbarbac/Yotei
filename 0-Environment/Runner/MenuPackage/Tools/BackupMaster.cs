using static System.ConsoleColor;
namespace Runner;

// ========================================================
/// <summary>
/// Represents a collection of project file backups that can be restored as a single unit.
/// </summary>
public class BackupMaster
{
    readonly Dictionary<ProjectFile, List<ProjectFileLine>> Items = [];

    /// <summary>
    /// Initializes a new empty instance.
    /// </summary>
    public BackupMaster() { }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    public override string ToString() => $"Count: {Items.Count}";

    // ----------------------------------------------------

    /// <summary>
    /// Determines if this instance contains an entry for the given project
    /// </summary>
    /// <param name="project"></param>
    /// <returns></returns>
    public bool Contains(ProjectFile project) => Contains(project, out _);

    /// <summary>
    /// Determines if this instance contains an entry for the given project and, if so, returns
    /// the saved contents in the out argument.
    /// </summary>
    /// <param name="project"></param>
    /// <param name="lines"></param>
    /// <returns></returns>
    public bool Contains(
        ProjectFile project,
        [NotNullWhen(true)] out List<ProjectFileLine>? lines)
    {
        project.ThrowWhenNull();

        lines = null;
        return Items.TryGetValue(project, out lines);
    }

    /// <summary>
    /// Adds the contents of the given project to this instance, provided it has not been added
    /// yet. If so, then this method is ignored. Returns the saved contents, either the new or
    /// the saved ones.
    /// </summary>
    /// <param name="project"></param>
    /// <returns></returns>
    public List<ProjectFileLine> Add(ProjectFile project)
    {
        project.ThrowWhenNull();

        if (Items.TryGetValue(project, out var lines)) return lines;

        lines = [.. project.Select(x => new ProjectFileLine(x.Value))];
        Items.Add(project, lines);
        return lines;
    }

    /// <summary>
    /// Removes the given project and its contents from this instance.
    /// </summary>
    /// <param name="project"></param>
    /// <returns></returns>
    public bool Remove(ProjectFile project) => Items.Remove(project.ThrowWhenNull());

    /// <summary>
    /// Clears this instance.
    /// </summary>
    public void Clear() => Items.Clear();

    // ----------------------------------------------------

    /// <summary>
    /// Restores to disk all the projects and their contents in this instance.
    /// </summary>
    /// <param name="display"></param>
    public void Restore(bool display)
    {
        if (display) Console.WriteLineEx(true);
        foreach (var (project, lines) in Items)
        {
            if (display)
            {
                Console.WriteEx(true, Cyan, "Restoring: ");
                Console.WriteLineEx(true, project.Name);
            }
            project.Clear();
            project.AddRange(lines);
            project.SaveToDisk();
        }
    }
}