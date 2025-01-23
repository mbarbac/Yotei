namespace Yotei.Tools.BaseGenerator;

// ========================================================
/// <summary>
/// Represents a file-alike top-most node in the source code generation hierarchy.
/// </summary>
internal sealed class FileNode : INode
{
    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    /// <param name="fileName"></param>
    public FileNode(string fileName) => FileName = fileName.NotNullNotEmpty();

    /// <inheritdoc/>
    public override string ToString() => $"File: {FileName}";

    /// <summary>
    /// The name of this file, without extension parts.
    /// </summary>
    public string FileName { get; }

    // ----------------------------------------------------

    /// <inheritdoc/>
    public bool Validate(SourceProductionContext context) => true;

    // ----------------------------------------------------

    /// <inheritdoc/>
    public void Emit(SourceProductionContext context, CodeBuilder cb) { }
}