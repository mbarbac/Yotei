namespace Dev.Builder;

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
    /// The path of this file, including its directory, name and extension.
    /// </summary>
    public string Path => Directory.Path.Length == 0
        ? NameAndExtension
        : $"{Directory}\\{NameAndExtension}";

    /// <summary>
    /// The name and extension of this file.
    /// </summary>
    public string NameAndExtension => Name.Length == 0
        ? Extension
        : Name + (Extension.Length == 0 ? string.Empty : $".{Extension}");

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
    public virtual string Extension
    {
        get => _Extension;
        init => _Extension = value.ThrowIfNull().Trim();
    }
    string _Extension = string.Empty;

    /// <summary>
    /// Determines if this file is a debug one, or not.
    /// </summary>
    public bool IsDebug =>
        Path.EndsWith("\\debug", Program.Comparison) ||
        Path.Contains("\\debug\\", Program.Comparison);

    /// <summary>
    /// Determines if this file is a release one, or not.
    /// </summary>
    public bool IsRelease =>
        Path.EndsWith("\\release", Program.Comparison) ||
        Path.Contains("\\release\\", Program.Comparison);

    /// <summary>
    /// Determines if this instance is equivalent to the given other one, or not.
    /// </summary>
    /// <param name="other"></param>
    /// <returns></returns>
    public bool EquivalentTo(File other)
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
    public bool Exists() => _File.Exists(Path);

    /// <summary>
    /// Deletes from disk this directory, provided is an empty one with no files or child
    /// directories.
    /// </summary>
    public void Delete() => _File.Delete(Path);
}

// ========================================================
public static class FileExtensions
{
    /// <summary>
    /// Deletes from disk the given collection of files.
    /// </summary>
    /// <param name="files"></param>
    public static void DeleteMany(this IEnumerable<File> files)
    {
        files = files.ThrowIfNull();
        foreach (var file in files)
        {
            Write(Color.Cyan, "Deleting file: "); WriteLine(file.Path);
            file.Delete();
        }
    }
}