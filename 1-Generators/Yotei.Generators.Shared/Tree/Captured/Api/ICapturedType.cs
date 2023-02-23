namespace Yotei.Generators.Tree;

// ========================================================
/// <summary>
/// Represents a type-alike element captured by a source code generator.
/// </summary>
internal interface ICapturedType : ICaptured
{
    /// <summary>
    /// <inheritdoc cref="ICaptured.Syntax"/>
    /// </summary>
    new TypeDeclarationSyntax Syntax { get; }

    /// <summary>
    /// <inheritdoc cref="ICaptured.Symbol"/>
    /// </summary>
    new INamedTypeSymbol Symbol { get; }

    // ----------------------------------------------------

    /// <summary>
    /// The chain of namespace declaration syntaxes for this instance.
    /// </summary>
    ImmutableArray<BaseNamespaceDeclarationSyntax> NamespaceSyntaxChain { get; }

    /// <summary>
    /// The chain of type declaration syntaxes for this instance.
    /// </summary>
    ImmutableArray<TypeDeclarationSyntax> TypeSyntaxChain { get; }

    /// <summary>
    /// The chain of type symbols for this instance.
    /// </summary>
    ImmutableArray<INamedTypeSymbol> TypeSymbolChain { get; }
}