namespace Runner.Builder;

// ========================================================
/// <summary>
/// Represents a project file.
/// </summary>
public class ProjectFile
{
    /// <summary>
    /// Initializes a new instance using the one found at the given path.
    /// </summary>
    /// <param name="path"></param>
    public ProjectFile(string path)
    {
        throw null;
    }

    /// <inheritdoc/>
    public override string ToString() => NameAndExtension;

    /// <summary>
    /// The directory this project file belongs to, without any tailing separator character.
    /// </summary>
    public string Directory { get; } = default!;

    /// <summary>
    /// The name of this project file, without the directory or extensions.
    /// </summary>
    public string Name { get; } = default!;

    /// <summary>
    /// The extension of this project file, without any leading dot character.
    /// </summary>
    public string Extension { get; } = default!;

    // ----------------------------------------------------

    /// <summary>
    /// The full name of this project file, including its directory and extension.
    /// </summary>
    public string FullName =>
        (Directory.Length > 0 ? $"{Directory}\\" : string.Empty) +
        NameAndExtension;

    /// <summary>
    /// The name and extension of this project file.
    /// </summary>
    public string NameAndExtension =>
        Name +
        (Extension.Length > 0 ? $".{Extension}" : string.Empty);

    // ----------------------------------------------------

    /// <summary>
    /// The collecion of lines in this instance.
    /// </summary>
    public List<ProjectLine> Lines { get; } = [];

    /// <summary>
    /// Saves the current contents of this project file in a new collection of lines.
    /// </summary>
    /// <returns></returns>
    public List<ProjectLine> SaveLines() => new(Lines);

    /// <summary>
    /// Restores the contents of this project file from the given collection of lines.
    /// </summary>
    /// <param name="lines"></param>
    public void RestoreLines(List<ProjectLine> lines)
    {
        lines.ThrowWhenNull();

        Lines.Clear();
        Lines.AddRange(lines);
    }

    // ----------------------------------------------------

    /// <summary>
    /// Loads the contents of this project file from disk.
    /// </summary>
    public void LoadContents()
    {
        Lines.Clear();

        var items = File.ReadAllLines(FullName);
        Lines.AddRange(items.Select(x => new ProjectLine(x)));
    }

    /// <summary>
    /// Save the current contents of this project file to disk.
    /// </summary>
    public void SaveContents()
    {
        File.WriteAllLines(FullName, Lines.Select(x => x.Value).ToArray());
    }

    // ----------------------------------------------------

    /// <summary>
    /// Determines if this project file represents a packable one, or not.
    /// </summary>
    /// <returns></returns>
    public bool IsPackable()
    {
        throw null;
    }

    /// <summary>
    /// Tries to obtain the semantic version value specified in this project file.
    /// </summary>
    /// <param name="version"></param>
    /// <returns></returns>
    public bool GetVersion([NotNullWhen(true)] out SemanticVersion version)
    {
        throw null;
    }

    /// <summary>
    /// Tries to set the semantic version value specified in this project file, assuming a previous
    /// value exists. If so, the old value is returned in the out argument, which is set to null
    /// otherwise.
    /// </summary>
    /// <param name="version"></param>
    /// <param name="old"></param>
    /// <returns></returns>
    public bool SetVersion(SemanticVersion version, [NotNullWhen(true)] out SemanticVersion? old)
    {
        throw null;
    }
}