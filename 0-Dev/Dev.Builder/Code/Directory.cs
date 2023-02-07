using System.Xml;

namespace Dev.Builder;

// ========================================================
/// <summary>
/// Represents a directory on disk.
/// </summary>
public record Directory
{
    protected static StringComparison Comparison = StringComparison.OrdinalIgnoreCase;

    /// <summary>
    /// An empty instance.
    /// </summary>
    public static Directory Empty { get; } = new();
    protected Directory() { }

    /// <summary>
    /// Initializes a new instance using the given path, which can either be null, or empty, or
    /// an existing directory or file path.
    /// </summary>
    /// <param name="path"></param>
    public Directory(string path)
    {
        if (path == null || path.Length == 0) return;
        path = path.NotNullNotEmpty();

        if (!_Directory.Exists(path))
        {
            path = Path.GetDirectoryName(path)!;
            if (path == null) throw new ArgumentException($"Cannot find path: {path}");
        }

        if (path.EndsWith('\\')) path = path[..^1];
        Value = path;
    }

    /// <inheritdoc>
    /// </inheritdoc>
    public override string ToString() => Value;

    /// <summary>
    /// Determines if this instance is an empty one.
    /// </summary>
    public bool IsEmpty => Value.Length == 0;

    /// <summary>
    /// Gets a string with the value carried by this instance, or an empty string.
    /// </summary>
    public string Value
    {
        get => _Value;
        init => _Value = value == null || value.Length == 0
            ? string.Empty
            : value.NotNullNotEmpty();
    }
    string _Value = string.Empty;

    /// <summary>
    /// Determines if this directory is a debug one, or not.
    /// </summary>
    public bool IsDebug =>
        Value.EndsWith("\\debug", Comparison) ||
        Value.Contains("\\debug\\", Comparison);

    /// <summary>
    /// Determines if this directory is a release one, or not.
    /// </summary>
    public bool IsRelease =>
        Value.EndsWith("\\release", Comparison) ||
        Value.Contains("\\release\\", Comparison);

    /// <summary>
    /// Determines if this instance is equivalent to the other given one.
    /// </summary>
    /// <param name="other"></param>
    /// <returns></returns>
    public bool EquivalentTo(Directory? other) => other != null && EquivalentTo(other.Value);

    /// <summary>
    /// Determines if this instance is equivalent to the given path.
    /// </summary>
    /// <param name="path"></param>
    /// <returns></returns>
    public bool EquivalentTo(string? path)
    {
        path = path.NullWhenEmpty() ?? string.Empty;
        if (string.Compare(Value, path, Comparison) == 0) return true;

        path = Path.GetDirectoryName(path).NullWhenEmpty() ?? string.Empty;
        if (string.Compare(Value, path, Comparison) == 0) return true;

        return false;
    }

    /// <summary>
    /// Determines if this instance exists on disk, or not.
    /// </summary>
    /// <returns></returns>
    public bool Exists() => !IsEmpty && _Directory.Exists(Value);

    /// <summary>
    /// Deletes this instance from disk.
    /// </summary>
    public void Delete() { if (!IsEmpty) _Directory.Delete(Value); }

    /// <summary>
    /// Returns the parent directory of this instance, or null if it cannot be found.
    /// </summary>
    /// <returns></returns>
    public Directory? GetParent()
    {
        if (!IsEmpty)
        {
            var path = _Directory.GetParent(Value);
            if (path != null) return new Directory(path.FullName);
        }
        return null;
    }

    /// <summary>
    /// Returns the collection of subdirectories beneath this one.
    /// </summary>
    /// <returns></returns>
    public Directory[] GetDirectories()
    {
        var names = IsEmpty ? Array.Empty<string>() : _Directory.GetDirectories(Value);
        return names.Select(x => new Directory(x)).ToArray();
    }

    /// <summary>
    /// Returns the collection of subdirectories beneath this one whose names match the given
    /// search pattern.
    /// </summary>
    /// <param name="search"></param>
    /// <returns></returns>
    public Directory[] GetDirectories(string search)
    {
        search = search.NotNullNotEmpty();

        var names = IsEmpty ? Array.Empty<string>() : _Directory.GetDirectories(Value, search);
        return names.Select(x => new Directory(x)).ToArray();
    }
    
    /// <summary>
    /// Returns the collection of files in this directory.
    /// </summary>
    /// <returns></returns>
    public File[] GetFiles()
    {
        var names = IsEmpty ? Array.Empty<string>() : _Directory.GetFiles(Value);
        return names.Select(x => new File(x)).ToArray();
    }

    /// <summary>
    /// Returns the collection of files in this directory whose names match the given search
    /// pattern.
    /// </summary>
    /// <param name="search"></param>
    /// <returns></returns>
    public File[] GetFiles(string search)
    {
        search = search.NotNullNotEmpty();

        var names = IsEmpty ? Array.Empty<string>() : _Directory.GetFiles(Value, search);
        return names.Select(x => new File(x)).ToArray();
    }
}

// ========================================================
public static class DirectoryExtensions
{
    static StringComparison Comparison = StringComparison.OrdinalIgnoreCase;

    // ----------------------------------------------------

    /// <summary>
    /// Returns the collection of projects found from the given directory, provided their paths
    /// do not start with the <paramref name="exclude"/> one.
    /// </summary>
    /// <param name="directory"></param>
    /// <param name="exclude"></param>
    /// <returns></returns>
    public static ImmutableList<Project> FindProjects(this Directory directory, Directory exclude)
    {
        directory = directory.ThrowIfNull();
        exclude = exclude.ThrowIfNull();

        var list = new List<Project>();
        if (!directory.IsEmpty) Populate(list, directory, exclude);
        return list.ToImmutableList();

        /// <summary> Invoked to populate the list...
        /// </summary>
        static void Populate(List<Project> list, Directory directory, Directory exclude)
        {
            if (exclude.Value.Length > 0 &&
            directory.Value.StartsWith(exclude.Value, Comparison)) return;

            var files = directory.GetFiles("*.csproj");
            foreach (var file in files) list.Add(new Project(file.FullName));

            var dirs = directory.GetDirectories();
            foreach (var dir in dirs) Populate(list, dir, exclude);
        }
    }
}