namespace Yotei.Tools.Generators.Internal;

// ========================================================
/// <summary>
/// Represents a wrapper over a syntax node.
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
    /// The chain of namespaces to this instance.
    /// </summary>
    ImmutableArray<BaseNamespaceDeclarationSyntax> NamespaceSyntaxChain { get; }

    /// <summary>
    /// The chain of type declaration syntaxes to this instance, including itself if it is a
    /// type-alike one.
    /// </summary>
    ImmutableArray<TypeDeclarationSyntax> TypeSyntaxChain { get; }

    /// <summary>
    /// The chain of type symbols to this instance, including itself if it is a type-alike one.
    /// </summary>
    ImmutableArray<INamedTypeSymbol> TypeSymbolChain { get; }
}