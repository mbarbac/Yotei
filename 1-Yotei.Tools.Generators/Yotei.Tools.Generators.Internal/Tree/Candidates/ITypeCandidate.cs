namespace Yotei.Tools.Generators.Internal;

// ========================================================
/// <summary>
/// Represents a wrapper over a type-alike syntax node identified for source generation.
/// </summary>
/// <param name="model"></param>
/// <param name="node"></param>
/// <param name="symbol"></param>
internal interface ITypeCandidate : ICandidate, INode
{
    /// <summary>
    /// The syntax node this instance wraps over.
    /// </summary>
    new TypeDeclarationSyntax Syntax { get; }

    /// <summary>
    /// The symbol this instance is associated with.
    /// </summary>
    new INamedTypeSymbol Symbol { get; }
}