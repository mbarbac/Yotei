namespace Yotei.Tools.Generators.Internal;

// ========================================================
/// <inheritdoc cref="INode"/>
internal interface INodeEx : INode
{
    /// <summary>
    /// Gets the name with no extensions of this file where the source code of this node shall
    /// be emitted.
    /// </summary>
    /// <returns></returns>
    string GetFileName();

    // ----------------------------------------------------

    /// <summary>
    /// The chain of namespace declarations this node belongs to.
    /// </summary>
    ImmutableArray<BaseNamespaceDeclarationSyntax> NamespaceSyntaxChain { get; }

    /// <summary>
    /// The chain of type declarations this node belongs to, including itself if it is a
    /// type-alike one.
    /// </summary>
    ImmutableArray<TypeDeclarationSyntax> TypeSyntaxChain { get; }

    /// <summary>
    /// The chain of type symbols this node belongs to, including itself if it is a type-alike
    /// one.
    /// </summary>
    ImmutableArray<INamedTypeSymbol> TypeSymbolChain { get; }
}