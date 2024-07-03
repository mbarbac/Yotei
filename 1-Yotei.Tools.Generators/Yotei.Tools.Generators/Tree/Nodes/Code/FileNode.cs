namespace Yotei.Tools.Generators.Internal;

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
    /// The name of this file, without extensions.
    /// </summary>
    public string FileName { get; }

    // -----------------------------------------------------

    /// <summary>
    /// The collection of child namespaces registered into this node.
    /// </summary>
    public ChildNamespaces ChildNamespaces { get; } = [];

    // -----------------------------------------------------

    /// <summary>
    /// Invoked before generation to validate this node, and its child ones, if any.
    /// </summary>
    /// <param name="context"></param>
    /// <returns></returns>
    public bool Validate(SourceProductionContext context) => throw null;

    // -----------------------------------------------------

    /// <summary>
    /// Invoked to emit the source code for this node, and its child ones, if any.
    /// </summary>
    /// <param name="context"></param>
    /// <param name="cb"></param>
    public void Emit(SourceProductionContext context, CodeBuilder cb) => throw null;
}