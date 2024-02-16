namespace Yotei.Tools.Generators.Internal;

// ========================================================
/// <summary>
/// Represents a wrapper over a property-alike syntax node identified for source generation.
/// </summary>
/// <param name="model"></param>
/// <param name="node"></param>
/// <param name="symbol"></param>
internal interface IPropertyCandidate : ICandidate, INode
{
    /// <summary>
    /// The syntax node this instance wraps over.
    /// </summary>
    new PropertyDeclarationSyntax Syntax { get; }

    /// <summary>
    /// The symbol this instance is associated with.
    /// </summary>
    new IPropertySymbol Symbol { get; }
}