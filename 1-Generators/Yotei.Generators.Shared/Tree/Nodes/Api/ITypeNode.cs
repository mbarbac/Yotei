namespace Yotei.Generators.Tree;

// ========================================================
/// <summary>
/// Represents a type-alike node in the source code generation hierarchy.
/// </summary>
internal interface ITypeNode : INode
{
    /// <summary>
    /// <inheritdoc cref="INode.Parent"/>
    /// This property is a not null one, an it is either a namespace or a parent type.
    /// </summary>
    new INode Parent { get; }

    /// <summary>
    /// The syntax node of this type.
    /// </summary>
    TypeDeclarationSyntax Syntax { get; }

    /// <summary>
    /// The symbol of this type.
    /// </summary>
    INamedTypeSymbol Symbol { get; }

    /// <summary>
    /// The semantic model of this type.
    /// </summary>
    SemanticModel SemanticModel { get; }

    /// <summary>
    /// Determines if this instance represents an interface, or not.
    /// </summary>
    bool IsInterface { get; }

    // ----------------------------------------------------

    /// <summary>
    /// The collection of child types in this instance.
    /// </summary>
    IEnumerable<ITypeNode> ChildTypes { get; }

    /// <summary>
    /// The collection of child properties in this instance.
    /// </summary>
    IEnumerable<IPropertyNode> ChildProperties { get; }

    /// <summary>
    /// The collection of child files in this instance.
    /// </summary>
    IEnumerable<IFieldNode> ChildFields { get; }

    /// <summary>
    /// Invoked to register into this instance the hierarchy for the given captured element,
    /// at the given type index.
    /// </summary>
    /// <param name="captured"></param>
    /// <param name="tpIndex"></param>
    void Register(ICaptured captured, int tpIndex);

    /// <summary>
    /// Adds the given property into this instance. Throws an exception if there is an existing
    /// one with the same name.
    /// </summary>
    /// <param name="captured"></param>
    void Add(ICapturedProperty captured);

    /// <summary>
    /// Adds the given field into this instance. Throws an exception if there is an existing
    /// one with the same name.
    /// </summary>
    /// <param name="captured"></param>
    void Add(ICapturedField captured);
}