namespace Runner.Builder;

// ========================================================
/// <summary>
/// Represents a project file.
/// </summary>
public class Project
{
    /// <summary>
    /// Initializes a new instance using the given source specification.
    /// </summary>
    /// <param name="source"></param>
    public Project(string source)
    {
        ArgumentNullException.ThrowIfNull(source);

        Directory = Path.GetDirectoryName(source) ?? string.Empty;
        if (Directory.Length > 0 && Directory.EndsWith('\\')) Directory = Directory[..^1];

        Name = Path.GetFileNameWithoutExtension(source);

        Extension = Path.GetExtension(source) ?? string.Empty;
        if (Extension.Length > 0 && Extension.StartsWith('.')) Extension = Extension[1..];

        var file = new FileInfo(FullName);
        if (!file.Exists) throw new ArgumentException(
            "File does not exist.")
            .WithData(FullName);

        if (string.Compare(Extension, "csproj", ignoreCase: true) != 0)
            throw new ArgumentException(
                "File is not a project file.")
                .WithData(FullName);

        LoadFile();
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns><inheritdoc/></returns>
    public override string ToString() => $"{Name}.{Extension}";

    /// <summary>
    /// Conversion operator.
    /// </summary>
    public static implicit operator string(Project item)
    {
        ArgumentNullException.ThrowIfNull(item);
        return item.FullName;
    }

    /// <summary>
    /// Conversion operator.
    /// </summary>
    public static implicit operator Project(string source) => new(source);

    /// <summary>
    /// The full name of this project file, including its directory path and extension.
    /// </summary>
    public string FullName =>
        (Directory.Length > 0 ? $"{Directory}\\" : string.Empty) +
        NameAndExtension;

    /// <summary>
    /// Returns the name and extension of this project.
    /// </summary>
    public string NameAndExtension =>
        (Name) +
        (Extension.Length > 0 ? $".{Extension}" : string.Empty);

    /// <summary>
    /// The directory of this project file, without the trailing '\' character.
    /// </summary>
    public string Directory { get; private set; } = default!;

    /// <summary>
    /// The short name of this project file, without its directory or extension.
    /// </summary>
    public string Name { get; private set; } = default!;

    /// <summary>
    /// The extension of this project file, without its leading '.' character.
    /// </summary>
    public string Extension { get; private set; } = default!;

    // ----------------------------------------------------

    /// <summary>
    /// The collection of lines in this instance.
    /// </summary>
    public List<ProjectLine> Lines { get; } = new();

    /// <summary>
    /// Saves a copy of the lines in this project file.
    /// </summary>
    /// <returns></returns>
    public List<ProjectLine> CopyLines()
    {
        return Lines.Select(x => new ProjectLine(x)).ToList();
    }

    /// <summary>
    /// Restores into this instance the given set of lines.
    /// </summary>
    /// <param name="lines"></param>
    public void RestoreLines(List<ProjectLine> lines)
    {
        ArgumentNullException.ThrowIfNull(lines);
        Lines.Clear();
        Lines.AddRange(lines);
    }

    // ----------------------------------------------------

    /// <summary>
    /// Reloads the contents of this file from disk.
    /// </summary>
    public void LoadFile()
    {
        Lines.Clear();

        var items = File.ReadAllLines(FullName);
        Lines.AddRange(items.Select(x => new ProjectLine(x)));
    }

    /// <summary>
    /// Saves the contents maintained by this instance to disk.
    /// </summary>
    public void SaveFile()
    {
        var items = Lines.Select(x => x.Value).ToArray();
        File.WriteAllLines(FullName, items);
    }

    // ----------------------------------------------------

    /// <summary>
    /// Determines if this project is a packable one, or not.
    /// </summary>
    /// <returns></returns>
    public bool IsPackable()
    {
        foreach (var line in Lines)
        {
            if (line.GetXMLValue("IsPackable", out var value) &&
                string.Compare("true", value, ignoreCase: true) == 0)
                return true;
        }

        return false;
    }

    /// <summary>
    /// Tries to obtain the version specified by this project file.
    /// </summary>
    /// <param name="version"></param>
    /// <returns></returns>
    public bool GetVersion([NotNullWhen(true)] out SemanticVersion? version)
    {
        foreach (var line in Lines)
        {
            if (line.GetXMLValue("Version", out var value))
            {
                version = new(value);
                return true;
            }
        }

        version = null;
        return false;
    }

    /// <summary>
    /// Tries to set the version specified by this project file. If so, returns the old one in
    /// the out argument.
    /// </summary>
    /// <param name="version"></param>
    /// <param name="old"></param>
    /// <returns></returns>
    public bool SetVersion(SemanticVersion version, [NotNullWhen(true)] out SemanticVersion? old)
    {
        ArgumentNullException.ThrowIfNull(version);

        foreach (var line in Lines)
        {
            if (!line.GetXMLValue("Version", out var temp)) continue;

            if (line.SetXMLValue("Version", version))
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
    /// Returns the list with the NuGet package references contained in this project file.
    /// </summary>
    /// <returns></returns>
    public List<NuReference> GetNuReferences()
    {
        var list = new List<NuReference>();
        foreach (var line in Lines)
        {
            if (NuReference.IsValid(line, out _, out _)) list.Add(new(line));
        }
        return list;
    }
}