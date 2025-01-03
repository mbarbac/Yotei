﻿using static Yotei.Tools.Diagnostics.ConsoleEx;
using static System.ConsoleColor;

namespace Runner;

// ========================================================
/// <summary>
/// Represents a collection of build backups.
/// </summary>
public class BuildBackups
{
    readonly Dictionary<Project, List<ProjectLine>> Items = [];

    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    public BuildBackups() { }

    /// <inheritdoc/>
    public override string ToString() => $"Count: {Items.Count}";

    // ----------------------------------------------------

    /// <summary>
    /// Restores all the projects in this collection using their respective backups, and saves
    /// the files.
    /// </summary>
    public void Restore()
    {
        WriteLine(true);
        foreach (var (project, lines) in Items)
        {
            Write(true, Red, "Restoring project: ");
            WriteLine(project.Name);

            project.RestoreLines(lines);
            project.SaveContents();
        }
    }

    // ----------------------------------------------------

    /// <summary>
    /// Adds a backup of the given project to this collection. If the project was already added,
    /// then an exception is thrown.
    /// </summary>
    /// <param name="project"></param>
    public void Add(Project project)
    {
        project.ThrowWhenNull();

        if (Items.ContainsKey(project)) throw new DuplicateException(
            "This collection already contains an entry for the given project.")
            .WithData(project);

        var lines = project.CopyLines();
        Items.Add(project, lines);
    }

    /// <summary>
    /// Adds a backup of the given project to this collection, or updates the existing one.
    /// </summary>
    /// <param name="project"></param>
    public void AddOrUpdate(Project project)
    {
        project.ThrowWhenNull();

        var lines = project.CopyLines();
        Items[project] = lines;
    }

    /// <summary>
    /// Adds a backup of the given project to this collection, but only if no previous backup
    /// exist. Otherwise, this method is ignored.
    /// </summary>
    /// <param name="project"></param>
    public void AddOrIgnore(Project project)
    {
        project.ThrowWhenNull();

        if (Items.ContainsKey(project)) return;

        var lines = project.CopyLines();
        Items.Add(project, lines);
    }

    /// <summary>
    /// Removes the backup of the given project from this collection.
    /// </summary>
    /// <param name="project"></param>
    public void Remove(Project project)
    {
        project.ThrowWhenNull();
        Items.Remove(project);
    }

    /// <summary>
    /// Clears this collection.
    /// </summary>
    public void Clear() => Items.Clear();
}