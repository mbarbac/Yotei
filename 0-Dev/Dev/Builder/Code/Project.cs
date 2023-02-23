namespace Dev.Builder;

// ========================================================
/// <summary>
/// Represents a project.
/// </summary>
public record Project
{
    /// <summary>
    /// An empty instance.
    /// </summary>
    public static Project Empty { get; } = new();
    protected Project() { }

    /// <summary>
    /// Initializes a new instance using the given file.
    /// </summary>
    /// <param name="file"></param>
    public Project(File file)
    {
        File = file;

        if (string.Compare(file.Extension, "csproj", Program.Comparison) != 0)
            throw new ArgumentException($"File extension is not 'csproj': {File}");
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns><inheritdoc/></returns>
    public override string ToString() => File.Name;

    /// <summary>
    /// Implicit conversion operator.
    /// </summary>
    public static implicit operator string(Project project) => project.File.Path;

    /// <summary>
    /// Implicit conversion operator.
    /// </summary>
    public static implicit operator Project(string path) => new(new File(path));

    /// <summary>
    /// The file of this project.
    /// </summary>
    public File File
    {
        get => _Value;
        init => _Value = value.ThrowIfNull();
    }
    File _Value = File.Empty;

    /// <summary>
    /// The name of this project.
    /// </summary>
    public string Name => File.Name;

    /// <summary>
    /// The path of this project.
    /// </summary>
    public string Path => File.Path;
}