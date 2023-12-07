namespace Yotei.Tools.Generators.Shared;

// ========================================================
/// <summary>
/// Represents a file into which generated source code can be emitted.
/// </summary>
internal class FileBuilder : CodeBuilder
{
    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    /// <param name="fileName"></param>
    [SuppressMessage("", "IDE0290")]
    public FileBuilder(string fileName)
    {
        FileName = fileName.NotNullNotEmpty(nameof(fileName));
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    public override string ToString() => $"File: {FileName}";

    /// <summary>
    /// The file name of this instance, without extensions.
    /// </summary>
    public string FileName { get; }

    /// <summary>
    /// The collection of candidates registered into this instance.
    /// </summary>
    public List<Candidate> Candidates { get; } = [];
}