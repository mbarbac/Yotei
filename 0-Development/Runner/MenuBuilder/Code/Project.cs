namespace Runner;

// ========================================================
/// <summary>
/// Represents a project file, by maintaining its path and its collection of mutable project
/// lines, which are also mutable themselves.
/// </summary>
public class Project : IEnumerable<ProjectLine>
{
    /// <summary>
    /// Initializes a new instance using the file found at the given path.
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
    public override string ToString() => NameExtension;

    // ----------------------------------------------------

    /// <summary>
    /// The directory this project file belongs to, without any tail separator character.
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

    /// <summary>
    /// The full name of this project file, including its directory and extension.
    /// </summary>
    public string FullName => _FullName ??= (Directory.Length > 0 ? $"{Directory}\\" : string.Empty) + NameExtension;
    string? _FullName;

    /// <summary>
    /// The name and extension of this project file.
    /// </summary>
    public string NameExtension => _NameExtension ??= Name + (Extension.Length > 0 ? $".{Extension}" : string.Empty);
    string? _NameExtension;

    /// <summary>
    /// The name and version, if any, of this project.
    /// </summary>
    public string NameVersion
    {
        get
        {
            var sb = new StringBuilder(Name);
            if (GetVersion(out var version)) sb.Append($" v:{version}");
            return sb.ToString();
        }
    }

    /// <summary>
    /// The name, extension and version, if any, of this project.
    /// </summary>
    public string NameExtensionVersion
    {
        get
        {
            var sb = new StringBuilder(NameExtension);
            if (GetVersion(out var version)) sb.Append($" v:{version}");
            return sb.ToString();
        }
    }

    // ----------------------------------------------------

    readonly List<ProjectLine> Lines = [];

    /// <inheritdoc/>
    public IEnumerator<ProjectLine> GetEnumerator() => Lines.GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    /// <summary>
    /// The number of lines in this project file.
    /// </summary>
    public int Count => Lines.Count;

    /// <summary>
    /// Gets or sets the project line at the given index.
    /// </summary>
    /// <param name="index"></param>
    /// <returns></returns>
    public ProjectLine this[int index]
    {
        get => Lines[index];
        set => Lines[index] = value.ThrowWhenNull();
    }

    /// <summary>
    /// Adds the given line to this instance, provided it is not duplicated.
    /// </summary>
    /// <param name="line"></param>
    public void Add(ProjectLine line)
    {
        line.ThrowWhenNull();

        if (Lines.Contains(line)) throw new DuplicateException("Duplicated line.").WithData(line);
        Lines.Add(line);
    }

    /// <summary>
    /// Adds to this instance the lines of the given range, provided any is duplicated.
    /// </summary>
    /// <param name="range"></param>
    public void AddRange(IEnumerable<ProjectLine> range)
    {
        range.ThrowWhenNull();
        foreach (var item in range) Add(item);
    }

    /// <summary>
    /// Inserts the given line into this instance, at the given index, provided it is not
    /// duplicated.
    /// </summary>
    /// <param name="index"></param>
    /// <param name="line"></param>
    public void Insert(int index, ProjectLine line)
    {
        line.ThrowWhenNull();

        if (Lines.Contains(line)) throw new DuplicateException("Duplicated line.").WithData(line);
        Lines.Insert(index, line);
    }

    /// <summary>
    /// Inserts into this instance the lines of the given range, starting at the given index,
    /// provided any is duplicated.
    /// </summary>
    /// <param name="index"></param>
    /// <param name="range"></param>
    public void InsertRange(int index, IEnumerable<ProjectLine> range)
    {
        range.ThrowWhenNull();
        foreach (var item in range) Insert(index++, item);
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
    /// <returns></returns>
    public bool Remove(ProjectLine line) => Lines.Remove(line.ThrowWhenNull());

    /// <summary>
    /// Clears the collection of lines in this instance.
    /// </summary>
    public void Clear() => Lines.Clear();

    /// <summary>
    /// Gets a list with the current lines in this instance.
    /// </summary>
    /// <returns></returns>
    public List<ProjectLine> ToList() => [.. Lines];

    /// <summary>
    /// Restores the contents of this file using the given range of lines, removing any previous
    /// ones.
    /// </summary>
    /// <param name="items"></param>
    public void FromLines(IEnumerable<ProjectLine> items)
    {
        items.ThrowWhenNull();

        Clear();
        AddRange(items);
    }

    // ----------------------------------------------------

    /// <summary>
    /// Loads from disk the contents of this project file, clearing any previous ones.
    /// </summary>
    public void LoadContents()
    {
        Clear();

        var items = File.ReadAllLines(FullName);
        AddRange(items.Select(x => new ProjectLine(x)));
    }

    /// <summary>
    /// Saves to disk the contents of this project file.
    /// </summary>
    [SuppressMessage("", "IDE0305")]
    public void SaveContents()
    {
        File.WriteAllLines(FullName, Lines.Select(x => x.Value).ToArray());
    }

    // ----------------------------------------------------

    const string VERSION = "Version";

    /// <summary>
    /// Tries to obtain the semantic version specified in this project file, using the current
    /// collection of lines.
    /// </summary>
    /// <param name="version"></param>
    /// <returns></returns>
    public bool GetVersion([NotNullWhen(true)] out SemanticVersion? version)
    {
        foreach (var line in Lines)
        {
            if (line.GetXMLWrappedValue(VERSION, out var value))
            {
                version = new(value);
                return true;
            }
        }

        version = null;
        return false;
    }

    /// <summary>
    /// Tries to set the semantic version of this project file, using the current collection of
    /// lines. If so, the old value is returned in the out argument.
    /// </summary>
    /// <param name="version"></param>
    /// <param name="old"></param>
    /// <returns></returns>
    public bool SetVersion(SemanticVersion version, [NotNullWhen(true)] out SemanticVersion? old)
    {
        version.ThrowWhenNull();

        foreach (var line in Lines)
        {
            if (!line.GetXMLWrappedValue(VERSION, out var temp)) continue;
            if (line.SetXMLWrappedValue(VERSION, version))
            {
                old = temp;
                return true;
            }
        }

        old = null;
        return false;
    }

    // ----------------------------------------------------

    const string ISPACKABLE = "IsPackable";
    const string TRUE = "true";

    /// <summary>
    /// Determines if this project is a packable one, or not.
    /// </summary>
    /// <returns></returns>
    public bool IsPackable()
    {
        foreach (var line in Lines)
        {
            if (line.GetXMLWrappedValue(ISPACKABLE, out var value) &&
                string.Compare(TRUE, value, ignoreCase: true) == 0)
                return true;
        }

        return false;
    }
}