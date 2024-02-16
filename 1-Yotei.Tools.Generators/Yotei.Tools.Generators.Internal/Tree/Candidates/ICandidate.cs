namespace Yotei.Tools.Generators.Internal;

// ========================================================
/// <summary>
/// Represents a wrapper over a syntax node identified for source generation.
/// </summary>
internal interface ICandidate
{
    /// <summary>
    /// Allow asking semantic questions about a tree of syntax nodes in a compilation.
    /// </summary>
    SemanticModel SemanticModel { get; }

    /// <summary>
    /// The syntax node this instance wraps over.
    /// </summary>
    SyntaxNode Syntax { get; }

    /// <summary>
    /// The symbol this instance is associated with.
    /// </summary>
    ISymbol Symbol { get; }

    // ----------------------------------------------------

    /// <summary>
    /// The name of the file where this candidate will emit its source code.
    /// </summary>
    /// <returns></returns>
    string GetFileName();

    // ----------------------------------------------------

    /// <summary>
    /// The chain of namespace declarations this candidate belongs to.
    /// </summary>
    ImmutableArray<BaseNamespaceDeclarationSyntax> NamespaceSyntaxChain { get; }

    /// <summary>
    /// The chain of type declarations this candidate belongs to, including itself if it is
    /// a type-alike one.
    /// </summary>
    ImmutableArray<TypeDeclarationSyntax> TypeSyntaxChain { get; }

    /// <summary>
    /// The chain of type symbols this candidate belongs to, including itself if it is a
    /// type-alike one.
    /// </summary>
    ImmutableArray<INamedTypeSymbol> TypeSymbolChain { get; }
}