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

    /// <inheritdoc/>
    public override string ToString() => $"Count: {Items.Count}";

    // ----------------------------------------------------

    /// <summary>
    /// Adds the given project to this collection. If it has been already added, an exception is
    /// thrown.
    /// </summary>
    /// <param name="project"></param>
    public void Add(Project project)
    {
        project.ThrowWhenNull();

        if (Items.ContainsKey(project)) throw new DuplicateException(
            "This collection already contains the given project.")
            .WithData(project);

        var lines = project.ToList();
        Items.Add(project, lines);
    }

    /// <summary>
    /// Tries to add the given project to this collection or, if it was already added, then just
    /// ignore it.
    /// </summary>
    /// <param name="project"></param>
    public void AddOrIgnore(Project project)
    {
        project.ThrowWhenNull();

        if (Items.ContainsKey(project)) return;

        var lines = project.ToList();
        Items.Add(project, lines);
    }

    /// <summary>
    /// Tries to add the given project to this collection. If it was already added, then updates
    /// the saved collection of lines with the new ones from the project.
    /// </summary>
    /// <param name="project"></param>
    public void AddOrUpdate(Project project)
    {
        project.ThrowWhenNull();

        var lines = project.ToList();
        Items[project] = lines;
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
    public void Restore()
    {
        WriteLine(true);
        foreach (var (project, lines) in Items)
        {
            Write(true, Red, "Restoring project: ");
            WriteLine(project.Name);

            project.FromLines(lines);
            project.SaveContents();
        }
    }
}