namespace Yotei.Tools.CoreGenerator;

// ========================================================
/// <summary>
/// Represents a file-alike node in the source code generation hierarchy, because the type-alike
/// element it represents was captured by itself, or because any of its childs was captured.
/// </summary>
internal class FileNode : IBaseNode
{
    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    /// <param name="node"></param>
    [SuppressMessage("", "IDE0290")]
    public FileNode(TypeNode node) => Node = node.ThrowWhenNull();

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    public override string ToString() => $"File: {Node.Symbol.Name}";

    /// <summary>
    /// The type node wrapped by this instance.
    /// </summary>
    public TypeNode Node { get; }

    // ----------------------------------------------------

    /// <summary>
    /// Returns the file name where this instance will emit its source code. Inheritors must
    /// guarantee it is unique among all the ones created by a given generator. The actual file
    /// name finally used will be this one plus '<c>.g.cs</c>'.
    /// </summary>
    /// <returns></returns>
    public virtual string FileName() => throw null;

    // ----------------------------------------------------

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="context"></param>
    /// <returns></returns>
    public virtual bool Validate(SourceProductionContext context) => throw null;

    // ----------------------------------------------------

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="context"></param>
    /// <param name="cb"></param>
    public virtual void Emit(SourceProductionContext context, CodeBuilder cb) => throw null;
}