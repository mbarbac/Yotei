namespace Yotei.Tools.Generators;

// ========================================================
/// <summary>
/// Represents a captured source code hierarchycal generation node.
/// </summary>
public interface ITreeNode : INode
{
    /// <summary>
    /// The collection of syntax nodes captured by the generator for this instance, if any.
    /// </summary>
    CustomList<BaseTypeDeclarationSyntax> SyntaxNodes { get; }

    /// <summary>
    /// The symbol represented by this instance.
    /// </summary>
    ISymbol Symbol { get; }

    /// <summary>
    /// The collection of attributes by which the element carried by this instance was found by
    /// the generator, if any.
    /// </summary>
    CustomList<AttributeData> Attributes { get; }
}