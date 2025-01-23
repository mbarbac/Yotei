namespace Yotei.Tools.BaseGenerator;

// ========================================================
/// <summary>
/// Represents a namespace-alike node in the source code generation hierarchy.
/// </summary>
internal sealed class NamespaceNode : IChildNode
{
    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    /// <param name="parent"></param>
    /// <param name="syntax"></param>
    public NamespaceNode(INode parent, BaseNamespaceDeclarationSyntax syntax)
    {
        ParentNode = parent.ThrowWhenNull();
        Syntax = syntax.ThrowWhenNull();

        if (ParentNode is not FileNode and not NamespaceNode) throw new ArgumentException(
            "Parent node is not a file nor a namespace.")
            .WithData(parent);
    }

    /// <inheritdoc/>
    public override string ToString() => $"Namespace: {Name}";

    /// <summary>
    /// <inheritdoc/>
    /// <br/> The value of this property is either a parent file, or a parent namespace.
    /// </summary>
    public INode ParentNode { get; }

    /// <summary>
    /// The full name of this namespace, including its dot-separated parts, if any.
    /// </summary>
    public string Name => Syntax.Name.ToString();

    /// <summary>
    /// The syntax associated to this namespace.
    /// </summary>
    public BaseNamespaceDeclarationSyntax Syntax { get; }

    // ----------------------------------------------------

    /// <inheritdoc/>
    public bool Validate(SourceProductionContext context) => true;

    // ----------------------------------------------------

    /// <inheritdoc/>
    public void Emit(SourceProductionContext context, CodeBuilder cb) { }
}