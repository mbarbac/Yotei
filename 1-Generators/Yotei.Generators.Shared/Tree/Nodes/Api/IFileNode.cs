namespace Yotei.Generators.Tree;

// ========================================================
/// <summary>
/// Represents a file-alike node in the source code generation hierarchy.
/// </summary>
internal interface IFileNode : INode
{
    /// <summary>
    /// <inheritdoc cref="INode.Parent"/>
    /// The value of this property is always null for file nodes.
    /// </summary>
    new INode? Parent { get; }

    // ----------------------------------------------------

    /// <summary>
    /// The collection of child namespaces in this instance.
    /// </summary>
    IEnumerable<INamespaceNode> ChildNamespaces { get; }

    /// <summary>
    /// Invoked to register into this instance the hierarchy for the given captured element.
    /// </summary>
    /// <param name="captured"></param>
    void Register(ICaptured captured);
}