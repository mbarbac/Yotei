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
    /// Initializes a new instance, using a file valid path.
    /// </summary>
    /// <param name="path"></param>
    public File(string path)
    {
        path = path.NotNullNotEmpty();

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

    /// <summary>
    /// Determines if this directory exists on disk, or not.
    /// </summary>
    /// <returns></returns>
    public bool Exists() => _File.Exists(Path);

    /// <summary>
    /// Deletes this directory from disk, providing it is not an empty one. Returns whether
    /// it has been deleted or not.
    /// </summary>
    /// <returns></returns>
    public bool Delete()
    {
        try { _File.Delete(Path); return true; }
        catch { }
        return false;
    }
}

// ========================================================
public static class FileExtensions
{
    /// <summary>
    /// Determines if the path of this directory is equivalent to the other given one.
    /// </summary>
    /// <param name="file"></param>
    /// <param name="path"></param>
    /// <returns></returns>
    public static bool EquivalentTo(this File file, string path)
    {
        file = file.ThrowIfNull();
        path = path.ThrowIfNull().Trim();

        return string.Compare(path, file, StringComparison.OrdinalIgnoreCase) == 0;
    }
}