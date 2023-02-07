namespace Dev.Builder;

// ========================================================
/// <summary>
/// Represents a file on disk.
/// </summary>
public record File
{
    protected static StringComparison Comparison = StringComparison.OrdinalIgnoreCase;

    /// <summary>
    /// An empty instance.
    /// </summary>
    public static File Empty { get; } = new();
    protected File() { }

    /// <summary>
    /// Initializes a new instance using the given path, which can either be null, or empty, or
    /// an existing file path.
    /// </summary>
    /// <param name="path"></param>
    public File(string path)
    {
        if (path == null || path.Length == 0) return;
        path = path.NotNullNotEmpty();

        if (!_File.Exists(path)) throw new ArgumentException($"Cannot find file: {path}");

        Directory = new Directory(path);
        path = path.Replace($"{Directory.Value}\\", string.Empty);

        Name = Path.GetFileNameWithoutExtension(path) ?? string.Empty;
        Extension = path.Replace($"{Name}.", string.Empty);
    }

    /// <summary>
    /// Initializes a new instance using its directory, name and extension.
    /// </summary>
    /// <param name="directory"></param>
    /// <param name="name"></param>
    /// <param name="extension"></param>
    public File(Directory directory, string name, string extension)
    {
        Directory = directory;
        Name = name;
        Extension = extension;
    }

    /// <inheritdoc>
    /// </inheritdoc>
    public override string ToString() => NameAndExtension;

    /// <summary>
    /// Determines if this instance is an empty one.
    /// </summary>
    public virtual bool IsEmpty =>
        Directory.IsEmpty &&
        Name.Length == 0 &&
        Extension.Length == 0;

    /// <summary>
    /// The full name of this file, including its directory, name and extension, if any.
    /// </summary>
    public string FullName => IsEmpty
        ? string.Empty
        : Directory.IsEmpty ? NameAndExtension : $"{Directory}\\{NameAndExtension}";

    /// <summary>
    /// The name and extension of this file.
    /// </summary>
    public string NameAndExtension => IsEmpty
        ? string.Empty
        : Extension.Length == 0 ? Name : $"{Name}.{Extension}";

    /// <summary>
    /// The directory where to find this file.
    /// </summary>
    public Directory Directory
    {
        get => _Directory;
        init => _Directory = value.ThrowIfNull(nameof(Directory));
    }
    Directory _Directory = Directory.Empty;

    /// <summary>
    /// The actual name of this file, without its extension.
    /// </summary>
    public string Name
    {
        get => _Name;
        init => _Name = value.NotNullNotEmpty(valueName: nameof(Name));
    }
    string _Name = string.Empty;

    /// <summary>
    /// The extension of this file, or an empty string.
    /// </summary>
    public virtual string Extension
    {
        get => _Extension;
        init => _Extension = value == null
            ? string.Empty
            : value.NullWhenEmpty() ?? string.Empty;
    }
    string _Extension = string.Empty;

    /// <summary>
    /// Determines if this file is a debug one, or not.
    /// </summary>
    public bool IsDebug => FullName.Contains("\\debug\\", Comparison);

    /// <summary>
    /// Determines if this file is a release one, or not.
    /// </summary>
    public bool IsRelease => FullName.Contains("\\release\\", Comparison);

    /// <summary>
    /// Determines if this instance is equivalent to the other given one.
    /// </summary>
    /// <param name="other"></param>
    /// <returns></returns>
    public bool EquivalentTo(File? other) => other != null && EquivalentTo(other.FullName);

    /// <summary>
    /// Determines if this instance is equivalent to the given path.
    /// </summary>
    /// <param name="path"></param>
    /// <returns></returns>
    public bool EquivalentTo(string? path)
    {
        path = path.NullWhenEmpty() ?? string.Empty;
        return string.Compare(FullName, path, Comparison) == 0;
    }

    /// <summary>
    /// Determines if this instance exists on disk, or not.
    /// </summary>
    /// <returns></returns>
    public bool Exists() => !IsEmpty && _File.Exists(FullName);

    /// <summary>
    /// Deletes this instance from disk.
    /// </summary>
    public void Delete() { if (!IsEmpty) _File.Delete(FullName); }
}

// ========================================================
public static class FileExtensions
{
    static StringComparison Comparison = StringComparison.OrdinalIgnoreCase;

    // ----------------------------------------------------

    /// <summary>
    /// Deletes the given collection of files.
    /// </summary>
    /// <param name="files"></param>
    /// <param name="head"></param>
    public static void Delete(this IEnumerable<File> files, bool head)
    {
        if (head)
        {
            WriteLine();
            WriteLine(Color.Green, "Deleting files:");
        }

        foreach (var file in files)
        {
            Write(Color.Cyan, $"- Deleting: "); WriteLine(file.FullName);
            file.Delete();
        }
    }
}