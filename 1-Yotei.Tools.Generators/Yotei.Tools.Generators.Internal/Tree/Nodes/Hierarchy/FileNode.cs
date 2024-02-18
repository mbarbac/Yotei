namespace Yotei.Tools.Generators.Internal;

// ========================================================
/// <summary>
/// Represents a file in the source code generation hierarchy.
/// </summary>
/// <param name="fileName"></param>
internal sealed class FileNode(string fileName) : INode
{
    /// <inheritdoc/>
    public override string ToString() => $"File: {FileName}";

    /// <inheritdoc/>
    public INode? ParentNode => null;
    INode? INode.ParentNode
    {
        get => ParentNode;
        set => throw new InvalidOperationException("File nodes cannot have a parent one.");
    }

    /// <summary>
    /// The file name of this instance, with no extensions.
    /// </summary>
    public string FileName { get; } = fileName.NotNullNotEmpty();

    // ----------------------------------------------------

    /// <inheritdoc/>
    public bool Validate(SourceProductionContext context) => throw null;

    // ----------------------------------------------------

    /// <inheritdoc/>
    public void Emit(SourceProductionContext context, CodeBuilder cb) => throw null;
}