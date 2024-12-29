namespace Runner.Builder;

// ========================================================
/// <summary>
/// Represents a project file.
/// </summary>
public class Project
{
    /// <summary>
    /// Initializes a new instance using the one found at the given path.
    /// </summary>
    /// <param name="path"></param>
    public Project(string path)
    {
        path.ThrowWhenNull();

        Directory = Path.GetDirectoryName(path) ?? string.Empty;
        if (Directory.Length > 0 && Directory.EndsWith('\\')) Directory = Directory[..^1];

        Name = Path.GetFileNameWithoutExtension(path);

        Extension = Path.GetExtension(path) ?? string.Empty;
        if (Extension.Length > 0 && Extension.StartsWith('.')) Extension = Extension[1..];

        if (string.Compare(Extension, "csproj", ignoreCase: true) != 0)
            throw new ArgumentException(
                "File has not a project file valid extension.")
                .WithData(path);

        var file = new FileInfo(FullName);
        if (!file.Exists)
            throw new ArgumentException(
                "Cannot find the given project file.")
                .WithData(path);

        LoadContents();
    }

    /// <inheritdoc/>
    public override string ToString()
    {
        var sb = new StringBuilder(NameAndExtension);
        if (GetVersion(out var version)) sb.Append($" v:{version}");
        return sb.ToString(); ;
    }

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
    public List<ProjectLine> SaveLines()
    {
        var list = Lines.Select(x => new ProjectLine(x)).ToList();
        return list;
    }

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

    const string ISPACKABLE = "IsPackable";
    const string TRUE = "true";
    const string VERSION = "Version";

    /// <summary>
    /// Determines if this project file represents a packable one, or not, using the current
    /// collection of lines.
    /// </summary>
    /// <returns></returns>
    public bool IsPackable()
    {
        foreach (var line in Lines)
        {
            if (line.GetXMLValue(ISPACKABLE, out var value) &&
                string.Compare(TRUE, value, ignoreCase: true) == 0)
                return true;
        }

        return false;
    }

    /// <summary>
    /// Tries to obtain the semantic version value specified in this project file, using the
    /// current collection of lines.
    /// </summary>
    /// <param name="version"></param>
    /// <returns></returns>
    public bool GetVersion([NotNullWhen(true)] out SemanticVersion? version)
    {
        foreach (var line in Lines)
        {
            if (line.GetXMLValue(VERSION, out var value))
            {
                version = new(value);
                return true;
            }
        }

        version = null;
        return false;
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
        version.ThrowWhenNull();

        foreach (var line in Lines)
        {
            if (!line.GetXMLValue(VERSION, out var temp)) continue;

            if (line.SetXMLValue(VERSION, version))
            {
                old = temp;
                return true;
            }
        }

        old = null;
        return false;
    }

    // ----------------------------------------------------

    /// <summary>
    /// Gets the list of NuGet package references contained in the project file.
    /// </summary>
    /// <returns></returns>
    public List<NuPackageReference> GetNuPackageReferences()
    {
        var list = new List<NuPackageReference>();

        foreach (var line in Lines)
        {
            if (NuPackageReference.Validate(line, out _, out _)) list.Add(new(line));
        }

        return list;
    }
}