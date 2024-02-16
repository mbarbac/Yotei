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

    /// <summary>
    /// The name of the file this instance represents, without extensions.
    /// </summary>
    public string FileName { get; } = fileName.ThrowWhenNull();

    /// <summary>
    /// The list of child namespaces.
    /// <br/> Default equality: long namespace name, with its dot separated parts.
    /// </summary>
    public List<NamespaceNode> ChildNamespaces { get; } = [];

    // ----------------------------------------------------

    /// <inheritdoc/>
    public bool Validate(SourceProductionContext context)
    {
        foreach (var node in ChildNamespaces) if (!node.Validate(context)) return false;
        return true;
    }

    // ----------------------------------------------------

    /// <inheritdoc/>
    public void Emit(SourceProductionContext context, CodeBuilder cb) => throw null;
}