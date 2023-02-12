namespace Dev.Tools;

// ========================================================
/// <summary>
/// Represents a file on disk.
/// </summary>
public record File
{
    /// <summary>
    /// An empty instance.
    /// </summary>
    public static File Empty { get; } = new();
    protected File() { }

    /// <summary>
    /// Initializes a new instance using a valid file path on disk.
    /// </summary>
    /// <param name="path"></param>
    public File(string path)
    {
        path = path.ThrowIfNull().Trim();

        if (!_File.Exists(path))
            throw new ArgumentException($"Cannot find file on disk: {path}");

        Directory = path;
        path = path.Replace($"{Directory}\\", string.Empty);

        Name = _Path.GetFileNameWithoutExtension(path) ?? string.Empty;
        Extension = _Path.GetExtension(path) ?? string.Empty;
        if (Extension.StartsWith('.')) Extension = Extension[1..];
    }

    /// <inheritdoc>
    /// </inheritdoc>
    public override string ToString() => NameAndExtension;

    /// <summary>
    /// Implicit conversion operator.
    /// </summary>
    public static implicit operator string(File directory) => directory.Path;

    /// <summary>
    /// Implicit conversion operator.
    /// </summary>
    public static implicit operator File(string path) => new(path);

    /// <summary>
    /// The directory where to find this file.
    /// </summary>
    public Directory Directory
    {
        get => _Directory;
        init => _Directory = value.ThrowIfNull();
    }
    Directory _Directory = Directory.Empty;

    /// <summary>
    /// The actual name of this file, without its extension.
    /// </summary>
    public string Name
    {
        get => _Name;
        init => _Name = value.ThrowIfNull().Trim();
    }
    string _Name = string.Empty;

    /// <summary>
    /// The extension of this file, or an empty string.
    /// </summary>
    public string Extension
    {
        get => _Extension;
        init => _Extension = value.ThrowIfNull().Trim();
    }
    string _Extension = string.Empty;

    /// <summary>
    /// The name and extension of this file.
    /// </summary>
    public string NameAndExtension => $"{Name}.{Extension}";

    /// <summary>
    /// The path of this file.
    /// </summary>
    public string Path => Directory.Path.Length == 0
        ? NameAndExtension
        : $"{Directory.Path}\\{NameAndExtension}";
}

// ========================================================
public static class FileExtensions
{
    public static StringComparison Comparison = StringComparison.OrdinalIgnoreCase;

    /// <summary>
    /// Determines if this file is a debug one, or not.
    /// </summary>
    /// <param name="file"></param>
    /// <returns></returns>
    public static bool IsDebug(this File file)
    {
        file = file.ThrowIfNull();

        return
            file.Path.EndsWith("\\debug", Comparison) ||
            file.Path.Contains("\\debug\\", Comparison);
    }

    /// <summary>
    /// Determines if this file is a release one, or not.
    /// </summary>
    /// <param name="file"></param>
    /// <returns></returns>
    public static bool IsRelease(this File file)
    {
        file = file.ThrowIfNull();

        return
            file.Path.EndsWith("\\release", Comparison) ||
            file.Path.Contains("\\release\\", Comparison);
    }

    /// <summary>
    /// Determines if this file is equivalent to the other given one, or not.
    /// </summary>
    /// <param name="file"></param>
    /// <param name="other"></param>
    /// <returns></returns>
    public static bool EquivalentTo(this File file, File other)
    {
        file = file.ThrowIfNull();
        other = other.ThrowIfNull();

        return file.EquivalentTo(other.Path);
    }

    /// <summary>
    /// Determines if this file is equivalent to the given path, or not.
    /// </summary>
    /// <param name="file"></param>
    /// <param name="other"></param>
    /// <returns></returns>
    public static bool EquivalentTo(this File file, string path)
    {
        file = file.ThrowIfNull();
        path = path.ThrowIfNull();

        path = path.Trim();
        return string.Compare(path, file.Path, Comparison) == 0;
    }

    /// <summary>
    /// Determines if this file exists on disk, or not.
    /// </summary>
    /// <param name="file"></param>
    /// <returns></returns>
    public static bool Exists(this File file)
    {
        file = file.ThrowIfNull();
        return _File.Exists(file.Path);
    }

    /// <summary>
    /// Deletes this file from disk.
    /// </summary>
    /// <param name="file"></param>
    /// <returns></returns>
    public static bool Delete(this File file)
    {
        file = file.ThrowIfNull();

        try { _File.Delete(file.Path); return true; }
        catch { }
        return false;
    }

    /// <summary>
    /// Returns the parent of this directory, or null if it is a root one.
    /// </summary>
    /// <param name="directory"></param>
    /// <returns></returns>
    public static Directory? GetParent(this File directory)
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
    public static ImmutableArray<Directory> GetDirectories(this File directory)
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
    public static ImmutableArray<Directory> GetDirectories(this File directory, string search)
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
}