using System.Net;
using System.Net.Mail;

namespace Dev.Tools;

// ========================================================
public record Directory
{
    /// <summary>
    /// An empty instance.
    /// </summary>
    public static Directory Empty { get; } = new();
    protected Directory() { }

    /// <summary>
    /// Initializes a new instance, using a valid path, which can either be an actual directory
    /// one, or a file path.
    /// </summary>
    /// <param name="path"></param>
    public Directory(string path)
    {
        path = path.NotNullNotEmpty();

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
        get => _PathValue;
        init => _PathValue = value.ThrowIfNull().Trim();
    }
    string _PathValue = string.Empty;

    /// <summary>
    /// Determines if this directory exists on disk, or not.
    /// </summary>
    /// <returns></returns>
    public bool Exists() => _Directory.Exists(Path);

    /// <summary>
    /// Deletes this directory from disk, providing it is not an empty one. Returns whether
    /// it has been deleted or not.
    /// </summary>
    /// <returns></returns>
    public bool Delete()
    {
        try { _Directory.Delete(Path); return true; }
        catch { }
        return false;
    }

    /// <summary>
    /// Returns the parent directory of this one, or null if it does not exist, or if it is
    /// a root one.
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
    /// Gets the collection of subdirectories of this one whose names match the given search
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
    public ImmutableArray<File> GetFiles()
    {
        try
        {
            var paths = _Directory.GetFiles(Path);
            return paths.Select(x => new File(x)).ToImmutableArray();
        }
        catch { }
        return ImmutableArray<File>.Empty;
    }

    /// <summary>
    /// Gets the collection of files in this directory whose names match the given search
    /// pattern.
    /// </summary>
    /// <param name="search"></param>
    /// <returns></returns>
    public ImmutableArray<File> GetFiles(string search)
    {
        search = search.NotNullNotEmpty();

        try
        {
            var paths = _Directory.GetFiles(Path, search);
            return paths.Select(x => new File(x)).ToImmutableArray();
        }
        catch { }
        return ImmutableArray<File>.Empty;
    }

    /// <summary>
    /// Gets the collection of files in this directory, optionally only those whose names
    /// match the given search pattern, if it is not null, and optionally recursing through
    /// its subdirectories.
    /// </summary>
    /// <param name="recursive"></param>
    /// <param name="search"></param>
    /// <returns></returns>
    public ImmutableArray<File> GetFiles(bool recursive, string? search = null)
    {
        search = search?.NotNullNotEmpty();

        var list = new List<File>(); Populate(list, this);
        return list.ToImmutableArray();

        void Populate(List<File> list, Directory directory)
        {
            var paths = search == null
                ? _Directory.GetFiles(directory.Path)
                : _Directory.GetFiles(directory.Path, search);

            list.AddRange(paths.Select(x => new File(x)));

            if (recursive)
                foreach (var dir in directory.GetDirectories()) Populate(list, dir);
        }
    }
}

// ========================================================
public static class DirectoryExtensions
{
    /// <summary>
    /// Determines if the path of this directory is equivalent to the other given one.
    /// </summary>
    /// <param name="directory"></param>
    /// <param name="path"></param>
    /// <returns></returns>
    public static bool EquivalentTo(this Directory directory, string path)
    {
        directory = directory.ThrowIfNull();
        path = path.ThrowIfNull().Trim();

        return string.Compare(path, directory, StringComparison.OrdinalIgnoreCase) == 0;
    }
}