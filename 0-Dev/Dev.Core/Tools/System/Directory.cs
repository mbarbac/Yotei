namespace Dev.Tools;

// ========================================================
/// <summary>
/// Represents a directory on disk.
/// </summary>
public record Directory
{
    /// <summary>
    /// An empty instance.
    /// </summary>
    public static Directory Empty { get; } = new();
    protected Directory() { }

    /// <summary>
    /// Initializes a new instance using a valid directory or file path on disk.
    /// </summary>
    /// <param name="path"></param>
    public Directory(string path)
    {
        path = path.ThrowIfNull().Trim();

        if (!_Directory.Exists(path))
        {
            path = _Path.GetDirectoryName(path)!;
            if (path == null)
                throw new ArgumentException($"Cannot find path on disk: {path}");
        }

        if (path.EndsWith('\\')) path = path[..^1];
        Path = path;
    }

    /// <inheritdoc>
    /// </inheritdoc>
    public override string ToString() => Path;

    /// <summary>
    /// Implicit conversion operator.
    /// </summary>
    public static implicit operator string(Directory directory) => directory.Path;

    /// <summary>
    /// Implicit conversion operator.
    /// </summary>
    public static implicit operator Directory(string path) => new(path);

    /// <summary>
    /// The path of this directory.
    /// </summary>
    public string Path
    {
        get => _Value;
        init => _Value = value.ThrowIfNull().Trim();
    }
    string _Value = string.Empty;
}

// ========================================================
public static class DirectoryExtensions
{
    public static StringComparison Comparison = StringComparison.OrdinalIgnoreCase;

    /// <summary>
    /// Determines if this directory is a debug one, or not.
    /// </summary>
    /// <param name="directory"></param>
    /// <returns></returns>
    public static bool IsDebug(this Directory directory)
    {
        directory = directory.ThrowIfNull();

        return
            directory.Path.EndsWith("\\debug", Comparison) ||
            directory.Path.Contains("\\debug\\", Comparison);
    }

    /// <summary>
    /// Determines if this directory is a release one, or not.
    /// </summary>
    /// <param name="directory"></param>
    /// <returns></returns>
    public static bool IsRelease(this Directory directory)
    {
        directory = directory.ThrowIfNull();

        return
            directory.Path.EndsWith("\\release", Comparison) ||
            directory.Path.Contains("\\release\\", Comparison);
    }

    /// <summary>
    /// Determines if this directory is equivalent to the other given one, or not.
    /// </summary>
    /// <param name="directory"></param>
    /// <param name="other"></param>
    /// <returns></returns>
    public static bool EquivalentTo(this Directory directory, Directory other)
    {
        directory = directory.ThrowIfNull();
        other = other.ThrowIfNull();

        return directory.EquivalentTo(other.Path);
    }

    /// <summary>
    /// Determines if this directory is equivalent to the given path, or not.
    /// </summary>
    /// <param name="directory"></param>
    /// <param name="other"></param>
    /// <returns></returns>
    public static bool EquivalentTo(this Directory directory, string path)
    {
        directory = directory.ThrowIfNull();
        path = path.ThrowIfNull();

        path = path.Trim();
        return string.Compare(path, directory.Path, Comparison) == 0;
    }

    /// <summary>
    /// Determines if this directory exists on disk, or not.
    /// </summary>
    /// <param name="directory"></param>
    /// <returns></returns>
    public static bool Exists(this Directory directory)
    {
        directory = directory.ThrowIfNull();
        return _Directory.Exists(directory.Path);
    }

    /// <summary>
    /// Deletes this directory from disk, provided it is an empty one.
    /// </summary>
    /// <param name="directory"></param>
    /// <returns></returns>
    public static bool Delete(this Directory directory)
    {
        directory = directory.ThrowIfNull();

        try { _Directory.Delete(directory.Path); return true; }
        catch { }
        return false;
    }

    /// <summary>
    /// Returns the parent of this directory, or null if it is a root one.
    /// </summary>
    /// <param name="directory"></param>
    /// <returns></returns>
    public static Directory? GetParent(this Directory directory)
    {
        directory = directory.ThrowIfNull();

        try
        {
            var path = _Directory.GetParent(directory.Path);
            if (path != null) return path.FullName;
        }
        catch { }
        return null;
    }

    /// <summary>
    /// Gets the collection of subdirectories of this one.
    /// </summary>
    /// <param name="directory"></param>
    /// <returns></returns>
    public static ImmutableArray<Directory> GetDirectories(this Directory directory)
    {
        directory = directory.ThrowIfNull();

        try
        {
            var paths = _Directory.GetDirectories(directory.Path);
            return paths.Select(x => new Directory(x)).ToImmutableArray();
        }
        catch { }
        return ImmutableArray<Directory>.Empty;
    }

    /// <summary>
    /// Gets the collection of subdirectories of this one, whose names match the given search
    /// pattern.
    /// </summary>
    /// <param name="directory"></param>
    /// <returns></returns>
    public static ImmutableArray<Directory> GetDirectories(this Directory directory, string search)
    {
        directory = directory.ThrowIfNull();
        search = search.NotNullNotEmpty();

        try
        {
            var paths = _Directory.GetDirectories(directory.Path, search);
            return paths.Select(x => new Directory(x)).ToImmutableArray();
        }
        catch { }
        return ImmutableArray<Directory>.Empty;
    }

    /// <summary>
    /// Gets the collection of files in this directory.
    /// </summary>
    /// <param name="directory"></param>
    /// <returns></returns>
    public static ImmutableArray<File> GetFiles(this Directory directory)
    {
        directory = directory.ThrowIfNull();

        try
        {
            var items = _Directory.GetFiles(directory.Path);
            return items.Select(x => new File(x)).ToImmutableArray();
        }
        catch { }
        return ImmutableArray<File>.Empty;
    }

    /// <summary>
    /// Gets the collection of files in this directory whose names match the given search
    /// pattern.
    /// </summary>
    /// <param name="directory"></param>
    /// <param name="search"></param>
    /// <returns></returns>
    public static ImmutableArray<File> GetFiles(this Directory directory, string search)
    {
        directory = directory.ThrowIfNull();
        search = search.NotNullNotEmpty();

        try
        {
            var items = _Directory.GetFiles(directory.Path, search);
            return items.Select(x => new File(x)).ToImmutableArray();
        }
        catch { }
        return ImmutableArray<File>.Empty;
    }
}