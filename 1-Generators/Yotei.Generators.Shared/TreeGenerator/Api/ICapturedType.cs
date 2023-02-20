namespace Yotei.Generators.Tree;

// ========================================================
/// <summary>
/// Represents a type-alike captured element for code generation purposes.
/// </summary>
internal interface ICapturedType : ICaptured
{
    /// <summary>
    /// The type declaration syntax captured by this instance.
    /// </summary>
    TypeDeclarationSyntax TypeSyntax { get; }

    /// <summary>
    /// The named type symbol captured by this instance.
    /// </summary>
    INamedTypeSymbol TypeSymbol { get; }

    /// <summary>
    /// Determines if this type refers to an interface, or not.
    /// </summary>
    bool IsInterface { get; }

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
    /// The chain of type symbols 
    /// for this instance.
    /// </summary>
    ImmutableArray<INamedTypeSymbol> TypeSymbolChain { get; }
}