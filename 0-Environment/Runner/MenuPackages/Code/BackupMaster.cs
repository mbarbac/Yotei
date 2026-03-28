using static Yotei.Tools.ConsoleExtensions;
using static System.ConsoleColor;

namespace Runner;

// ========================================================
/// <summary>
/// Represents a collection of project backups that, if needed, can be restored as a unit.
/// </summary>
public class BackupMaster
{
    readonly Dictionary<Project, List<ProjectLine>> Items = [];

    /// <summary>
    /// Initializes a new empty instance.
    /// </summary>
    public BackupMaster() { }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns><inheritdoc/></returns>
    public override string ToString() => $"Count: {Items.Count}";

    // ----------------------------------------------------

    /// <summary>
    /// Determines if this collection contains an entry for the given project and, if so, returns
    /// in the out argument the collection of lines currently kept for it.
    /// </summary>
    /// <param name="project"></param>
    /// <param name="lines"></param>
    /// <returns></returns>
    public bool Contains(
        Project project,
        [NotNullWhen(true)] out IEnumerable<ProjectLine>? lines)
    {
        ArgumentNullException.ThrowIfNull(project);

        var done = Items.TryGetValue(project, out var temps);
        lines = done ? temps : null;
        return done;
    }

    /// <summary>
    /// Adds the given project to this collection, provided it has not been already added. If
    /// so, then it is ignored. Either case, returns the actual list of saved lines.
    /// </summary>
    /// <param name="project"></param>
    /// <returns></returns>
    public List<ProjectLine> Add(Project project)
    {
        ArgumentNullException.ThrowIfNull(project);

        if (!Items.ContainsKey(project)) Items.Add(project, []);
        return Items[project];
    }

    /// <summary>
    /// Removes the given project from this collection.
    /// </summary>
    /// <param name="project"></param>
    public bool Remove(Project project)
    {
        ArgumentNullException.ThrowIfNull(project);
        return Items.Remove(project);
    }

    /// <summary>
    /// Clears this collection.
    /// </summary>
    public void Clear() => Items.Clear();

    // ----------------------------------------------------

    /// <summary>
    /// Restores all projects in this collection using the saved lines for each, and then saving
    /// each of them to disk. If requested, progress is displayed in the console after a new line
    /// separator.
    /// </summary>
    /// <param name="display"></param>
    /// <returns></returns>
    public void Restore(bool display)
    {
        if (display) WriteLineEx(true);

        foreach (var (project, lines) in Items)
        {
            if (display)
            {
                WriteEx(true, Cyan, "Restoring project: ");
                WriteLineEx(true, project.Name);
            }

            project.FromLines(lines);
            project.SaveContents();
        }
    }
}