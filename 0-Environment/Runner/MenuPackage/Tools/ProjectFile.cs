namespace Runner;

// ========================================================
/// <summary>
/// Represents a project configuration file.
/// </summary>
public class ProjectFile : IEnumerable<ProjectFileLine>
{
    /// <summary>
    /// Initialize a new instance, validating that the given path exists, and loads from disk
    /// its contents.
    /// </summary>
    /// <param name="path"></param>
    public ProjectFile(string path)
    {
        path = path.NotNullNotEmpty(trim: true);

        Directory = Path.GetDirectoryName(path) ?? string.Empty;
        if (Directory.Length > 0 && Directory.EndsWith('\\')) Directory = Directory[..^1];

        Name = Path.GetFileNameWithoutExtension(path);

        Extension = Path.GetExtension(path);
        if (Extension is not null &&
            Extension.Length > 0 &&
            Extension.StartsWith('.')) Extension = Extension[1..];

        if (string.Compare(Extension, "csproj", ignoreCase: true) != 0)
            throw new ArgumentException(
                "File has not a project file valid extension.")
                .WithData(path);

        var file = new FileInfo(FullName);
        if (!file.Exists)
            throw new ArgumentException(
                "Cannot find the given project file.")
                .WithData(path);

        LoadFromDisk();
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    public override string ToString()
    {
        var sb = new StringBuilder(NameAndExtension);
        if (GetVersion(out var version)) sb.Append($" {version.ToString(true)}");
        return sb.ToString();
    }

    // ----------------------------------------------------

    /// <summary>
    /// The directory of this project file (without any separator character).
    /// </summary>
    public string Directory { get; }

    /// <summary>
    /// The actual name of this project file (without any separator or extension).
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// The file's extension (without any separator), or null if any.
    /// </summary>
    public string? Extension { get; }

    /// <summary>
    /// The name and extension of this file.
    /// </summary>
    public string NameAndExtension => _NameAndExtension ??=  Extension is null
        ? Name
        : $"{Name}.{Extension}";

    string? _NameAndExtension;

    /// <summary>
    /// The full name of this file.
    /// </summary>
    public string FullName => _FullName ??= Directory.Length > 0
        ? $"{Directory}\\{NameAndExtension}"
        : NameAndExtension;

    string? _FullName;

    // ----------------------------------------------------

    /// <summary>
    /// Clear any previous lines from this instance and loads its contents from disk.
    /// </summary>
    public void LoadFromDisk()
    {
        Lines.Clear();

        var lines = File.ReadAllLines(FullName);
        AddRange(lines.Select(x => new ProjectFileLine(x)));
    }

    /// <summary>
    /// Saves the contents of this file to disk, overwriting any previous ones.
    /// </summary>
    public void SaveToDisk()
    {
        File.WriteAllLines(FullName, [.. Lines.Select(x => x.Value)]);
    }

    // ----------------------------------------------------

    const string SEMVERSION = "Version";

    /// <summary>
    /// Tries to obtain the semantic version specified in the project file, using the current
    /// collection of lines.
    /// </summary>
    /// <param name="version"></param>
    /// <returns></returns>
    public bool GetVersion([NotNullWhen(true)] out SemanticVersion? version)
    {
        foreach (var line in Lines)
        {
            if (line.ExtractXML(SEMVERSION, out var value))
            {
                version = new(value);
                return true;
            }
        }
        version = null;
        return false;
    }

    /// <summary>
    /// Tries to update the semantic version in the current collection of lines with the new given
    /// one
    /// </summary>
    /// <param name="version"></param>
    /// <returns></returns>
    public bool UpdateVersion(SemanticVersion version) => UpdateVersion(version, out _);

    /// <summary>
    /// Tries to update the semantic version in the current collection of lines with the new given
    /// one, and returns the old one in the out argument.
    /// </summary>
    /// <param name="version"></param>
    /// <param name="oldversion"></param>
    /// <returns></returns>
    public bool UpdateVersion(
        SemanticVersion version,
        [NotNullWhen(true)] out SemanticVersion? oldversion)
    {
        ArgumentNullException.ThrowIfNull(version);

        foreach (var line in Lines)
        {
            if (!line.ExtractXML(SEMVERSION, out var value)) continue;
            if (line.UpdateXML(SEMVERSION, version))
            {
                oldversion = value;
                return true;
            }
        }
        oldversion = null;
        return false;
    }

    // ----------------------------------------------------

    const string ISPACKABLE = "IsPackable";

    /// <summary>
    /// Determines if this project file represents a packable one, or not.
    /// </summary>
    /// <returns></returns>
    public bool IsPackable()
    {
        foreach (var line in Lines)
        {
            if (line.ExtractXML(ISPACKABLE, out var value))
                return string.Compare("true", value, ignoreCase: true) == 0;
        }
        return false;
    }

    // ----------------------------------------------------

    readonly List<ProjectFileLine> Lines = [];

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    public IEnumerator<ProjectFileLine> GetEnumerator() => Lines.GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    /// <summary>
    /// The number of lines in this file.
    /// </summary>
    public int Count => Lines.Count;

    /// <summary>
    /// Gets or sets the line at the given index.
    /// </summary>
    /// <param name="index"></param>
    /// <returns></returns>
    public ProjectFileLine this[int index]
    {
        get => Lines[index];
        set => Lines[index] = value.ThrowWhenNull();
    }

    /// <summary>
    /// Returns an array with the lines in this instance.
    /// </summary>
    /// <returns></returns>
    public ProjectFileLine[] ToArray() => [.. Lines];

    /// <summary>
    /// Returns a list with the lines in this instance.
    /// </summary>
    /// <returns></returns>
    public List<ProjectFileLine> ToList() => [.. Lines];

    /// <summary>
    /// Adds the given line to this instance.
    /// </summary>
    /// <param name="line"></param>
    public void Add(ProjectFileLine line)
    {
        ArgumentNullException.ThrowIfNull(line);
        if (Lines.Contains(line)) throw new DuplicateException("Duplicated line.").WithData(line);
        Lines.Add(line);
    }

    /// <summary>
    /// Adds the lines from the given range to this instance.
    /// </summary>
    /// <param name="range"></param>
    public void AddRange(IEnumerable<ProjectFileLine> range)
    {
        ArgumentNullException.ThrowIfNull(range);
        foreach (var line in range) Add(line);
    }

    /// <summary>
    /// Inserts the given line into this instance at the given index.
    /// </summary>
    /// <param name="index"></param>
    /// <param name="line"></param>
    public void Insert(int index, ProjectFileLine line)
    {
        ArgumentNullException.ThrowIfNull(line);
        if (Lines.Contains(line)) throw new DuplicateException("Duplicated line.").WithData(line);
        Lines.Insert(index, line);
    }

    /// <summary>
    /// Inserts the lines from the given range into this instance starting at the given index.
    /// </summary>
    /// <param name="index"></param>
    /// <param name="range"></param>
    public void InsertRange(int index, IEnumerable<ProjectFileLine> range)
    {
        ArgumentNullException.ThrowIfNull(range);
        foreach (var line in range) Insert(index++, line);
    }

    /// <summary>
    /// Removes from this instance the line at the given index.
    /// </summary>
    /// <param name="index"></param>
    public void RemoveAt(int index) => Lines.RemoveAt(index);

    /// <summary>
    /// Removes from this instance the given line.
    /// </summary>
    /// <param name="line"></param>
    public void Remove(ProjectFileLine line) => Lines.Remove(line.ThrowWhenNull());

    /// <summary>
    /// Removes all lines from this instance.
    /// </summary>
    public void Clear() => Lines.Clear();
}