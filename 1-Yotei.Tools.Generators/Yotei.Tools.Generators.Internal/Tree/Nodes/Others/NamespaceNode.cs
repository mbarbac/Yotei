namespace Yotei.Tools.Generators.Internal;

// ========================================================
/// <summary>
/// Represents a namespace in the source code generation hierarchy.
/// </summary>
/// <param name="node"></param>
internal sealed class NamespaceNode(BaseNamespaceDeclarationSyntax node) : INode
{
    /// <inheritdoc/>
    public override string ToString() => $"Namespace: {Name}";

    /// <summary>
    /// The syntax this instance represents.
    /// </summary>
    public BaseNamespaceDeclarationSyntax Syntax { get; } = node.ThrowWhenNull();

    /// <summary>
    /// The name of this namespace, including dot separated parts if they are explicitly used.
    /// </summary>
    public string Name => Syntax.Name.ToString();

    /// <summary>
    /// The list of child namespaces.
    /// <br/> Default equality: long namespace name, with its dot separated parts.
    /// </summary>
    public List<NamespaceNode> ChildNamespaces { get; } = [];

    /// <summary>
    /// The list of child types.
    /// <br/> Default equality: symbol comparison.
    /// </summary>
    public List<TypeNode> ChildTypes { get; } = [];

    // ----------------------------------------------------

    /// <inheritdoc/>
    public bool Validate(SourceProductionContext context) => throw null;

    // ----------------------------------------------------

    /// <inheritdoc/>
    public void Emit(SourceProductionContext context, CodeBuilder cb) => throw null;
}