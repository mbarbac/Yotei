namespace Yotei.Generators.Tree;

// ========================================================
/// <summary>
/// Represents a field-alike node in the source code generation hierarchy.
/// </summary>
internal interface IFieldNode : INode
{
    /// <summary>
    /// <inheritdoc cref="INode.Parent"/>
    /// This property is a not null type-alike one.
    /// </summary>
    new ITypeNode Parent { get; }

    /// <summary>
    /// The syntax node of this type.
    /// </summary>
    FieldDeclarationSyntax Syntax { get; }

    /// <summary>
    /// The symbol of this type.
    /// </summary>
    IFieldSymbol Symbol { get; }

    /// <summary>
    /// The semantic model of this type.
    /// </summary>
    SemanticModel SemanticModel { get; }
}