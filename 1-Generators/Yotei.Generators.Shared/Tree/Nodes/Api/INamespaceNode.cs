namespace Yotei.Generators.Tree;

// ========================================================
/// <summary>
/// Represents a namespace-alike node in the source code generation hierarchy.
/// </summary>
internal interface INamespaceNode : INode
{
    /// <summary>
    /// <inheritdoc cref="INode.Parent"/>
    /// This property is a not null one, an it is either a file or a parent namespace.
    /// </summary>
    new INode Parent { get; }

    /// <summary>
    /// The syntax node of this namespace.
    /// </summary>
    BaseNamespaceDeclarationSyntax Syntax { get; }

    // ----------------------------------------------------

    /// <summary>
    /// The collection of child namespaces in this instance.
    /// </summary>
    IEnumerable<INamespaceNode> ChildNamespaces { get; }

    /// <summary>
    /// The collection of child types in this instance.
    /// </summary>
    IEnumerable<ITypeNode> ChildTypes { get; }

    /// <summary>
    /// Invoked to register into this instance the hierarchy for the given captured element,
    /// at the given namespace and type indexes.
    /// </summary>
    /// <param name="captured"></param>
    /// <param name="nsIndex"></param>
    /// <param name="tpIndex"></param>
    void Register(ICaptured captured, int nsIndex, int tpIndex);
}