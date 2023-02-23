namespace Yotei.Generators.Tree;

// ========================================================
/// <summary>
/// Represents a property-alike node in the source code generation hierarchy.
/// </summary>
internal interface IPropertyNode : INode
{
    /// <summary>
    /// <inheritdoc cref="INode.Parent"/>
    /// This property is a not null type-alike one.
    /// </summary>
    new ITypeNode Parent { get; }

    /// <summary>
    /// The syntax node of this type.
    /// </summary>
    PropertyDeclarationSyntax Syntax { get; }

    /// <summary>
    /// The symbol of this type.
    /// </summary>
    IPropertySymbol Symbol { get; }

    /// <summary>
    /// The semantic model of this type.
    /// </summary>
    SemanticModel SemanticModel { get; }
}