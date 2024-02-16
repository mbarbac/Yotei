namespace Yotei.Tools.Generators.Internal;

// ========================================================
/// <summary>
/// Represents a wrapper over a method-alike syntax node identified for source generation.
/// </summary>
/// <param name="model"></param>
/// <param name="node"></param>
/// <param name="symbol"></param>
internal interface IMethodCandidate : ICandidate, INode
{
    /// <summary>
    /// The syntax node this instance wraps over.
    /// </summary>
    new MethodDeclarationSyntax Syntax { get; }

    /// <summary>
    /// The symbol this instance is associated with.
    /// </summary>
    new IMethodSymbol Symbol { get; }
}