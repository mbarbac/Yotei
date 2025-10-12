using static Yotei.Tools.ConsoleEx;
using static System.ConsoleColor;

namespace Runner;

// ========================================================
/// <summary>
/// Represents the collection of project lines saved for the registered projects, so that they all
/// can be restored as a unit if needed.
/// </summary>
public class BuildBackups
{
    readonly Dictionary<Project, List<ProjectLine>> Items = [];

    /// <summary>
    /// Initializes an empty instance.
    /// </summary>
    public BuildBackups() { }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns><inheritdoc/></returns>
    public override string ToString() => $"Count: {Items.Count}";

    // ----------------------------------------------------

    /// <summary>
    /// Determines if this collection contains the given project and, if so, returns the list
    /// of saved lines.
    /// </summary>
    /// <param name="project"></param>
    /// <param name="lines"></param>
    /// <returns></returns>
    public bool Contains(Project project, [NotNullWhen(true)] out List<ProjectLine>? lines)
    {
        project.ThrowWhenNull();

        var done = Items.TryGetValue(project, out lines);
        return done;
    }

    /// <summary>
    /// Adds the given project to this collection, provided it has not been added yet, and
    /// returns the list of saved lines.
    /// </summary>
    /// <param name="project"></param>
    /// <returns></returns>
    public List<ProjectLine> Add(Project project)
    {
        project.ThrowWhenNull();

        if (!Items.ContainsKey(project)) Items.Add(project, []);
        return Items[project];
    }

    /// <summary>
    /// Removes the given project from this collection.
    /// </summary>
    /// <param name="project"></param>
    public bool Remove(Project project) => Items.Remove(project.ThrowWhenNull());

    /// <summary>
    /// Clears this collection.
    /// </summary>
    public void Clear() => Items.Clear();

    // ----------------------------------------------------

    /// <summary>
    /// Restores, for all projects registered into this instance, their saved lines.
    /// </summary>
    /// <param name="display"></param>
    public void Restore(bool display)
    {
        if (display) WriteLine(true);

        foreach (var (project, lines) in Items)
        {
            if (display)
            {
                Write(true, Cyan, "Restoring project: ");
                WriteLine(project.Name);
            }
            project.FromLines(lines);
            project.SaveContents();
        }
    }
}