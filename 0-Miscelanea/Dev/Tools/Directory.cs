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

    /// <summary>
    /// Determines if this directory is a debug one, or not.
    /// </summary>
    public bool IsDebug =>
        Path.EndsWith("\\debug", Program.Comparison) ||
        Path.Contains("\\debug\\", Program.Comparison);

    /// <summary>
    /// Determines if this directory is a release one, or not.
    /// </summary>
    public bool IsRelease =>
        Path.EndsWith("\\release", Program.Comparison) ||
        Path.Contains("\\release\\", Program.Comparison);

    /// <summary>
    /// Determines if this instance is equivalent to the given other one, or not.
    /// </summary>
    /// <param name="other"></param>
    /// <returns></returns>
    public bool EquivalentTo(Directory other)
    {
        other = other.ThrowIfNull();
        return EquivalentTo(other.Path);
    }

    /// <summary>
    /// Determines if this instance is equivalent to the given path, or not.
    /// </summary>
    /// <param name="path"></param>
    /// <returns></returns>
    public bool EquivalentTo(string path)
    {
        path = path.ThrowIfNull();
        return string.Compare(path, Path, Program.Comparison) == 0;
    }

    /// <summary>
    /// Determines if this instance exists on disk, or not.
    /// </summary>
    /// <returns></returns>
    public bool Exists() => _Directory.Exists(Path);

    /// <summary>
    /// Deletes from disk this directory, provided is an empty one with no files or child
    /// directories.
    /// </summary>
    public void Delete() => _Directory.Delete(Path);

    /// <summary>
    /// Deletes from disk this directory and, if requested, and files and subdirectories in it.
    /// </summary>
    /// <param name="recursive"></param>
    public void Delete(bool recursive) => _Directory.Delete(Path, recursive);

    /// <summary>
    /// Gets the parent directory of this instance, or null if it is a root one.
    /// </summary>
    /// <returns></returns>
    public Directory? GetParent()
    {
        try
        {
            var path = _Directory.GetParent(Path);
            if (path != null) return path.FullName;
        }
        catch { }
        return null;
    }

    /// <summary>
    /// Gets the collection of subdirectories of this one.
    /// </summary>
    /// <returns></returns>
    public ImmutableArray<Directory> GetDirectories()
    {
        try
        {
            var paths = _Directory.GetDirectories(Path);
            return paths.Select(x => new Directory(x)).ToImmutableArray();
        }
        catch { }
        return ImmutableArray<Directory>.Empty;
    }

    /// <summary>
    /// Gets the collection of subdirectories of this one, whose names match the given search
    /// pattern.
    /// </summary>
    /// <param name="search"></param>
    /// <returns></returns>
    public ImmutableArray<Directory> GetDirectories(string search)
    {
        search = search.NotNullNotEmpty();

        try
        {
            var paths = _Directory.GetDirectories(Path, search);
            return paths.Select(x => new Directory(x)).ToImmutableArray();
        }
        catch { }
        return ImmutableArray<Directory>.Empty;
    }

    /// <summary>
    /// Gets the collection of files in this directory.
    /// </summary>
    /// <returns></returns>
    public ImmutableArray<File> GetFiles() => GetFiles(false);

    /// <summary>
    /// Gets the collection of files in this directory, and subdirectories if requested.
    /// </summary>
    /// <param name="recursive"></param>
    /// <returns></returns>
    public ImmutableArray<File> GetFiles(bool recursive)
    {
        var list = new List<File>(); Populate(this);
        return list.ToImmutableArray();

        /// <summary> Invoked to populate the list.
        /// </summary>
        void Populate(Directory directory)
        {
            try
            {
                var items = _Directory.GetFiles(directory.Path);
                list.AddRange(items.Select(x => new File(x)));
            }
            catch { }

            if (recursive)
                foreach (var dir in GetDirectories()) Populate(dir);
        }
    }

    /// <summary>
    /// Gets the collection of files in this directory, whose names match the given search
    /// pattern.
    /// </summary>
    /// <param name="search"></param>
    /// <returns></returns>
    public ImmutableArray<File> GetFiles(string search) => GetFiles(search, false);

    /// <summary>
    /// Gets the collection of files in this directory, and subdirectories if requested, whose
    /// names match the given search pattern.
    /// </summary>
    /// <param name="search"></param>
    /// <param name="recursive"></param>
    /// <returns></returns>
    public ImmutableArray<File> GetFiles(string search, bool recursive)
    {
        var list = new List<File>(); Populate(this);
        return list.ToImmutableArray();

        /// <summary> Invoked to populate the list.
        /// </summary>
        void Populate(Directory directory)
        {
            try
            {
                var items = _Directory.GetFiles(directory.Path, search);
                list.AddRange(items.Select(x => new File(x)));
            }
            catch { }

            if (recursive)
                foreach (var dir in GetDirectories()) Populate(dir);
        }
    }
}