namespace Yotei.Generators
{
    // ====================================================
    public static class ChainExtensions
    {
        /// <summary>
        /// Gets the chain of namespace declaration syntax for the given item.
        /// </summary>
        /// <param name="syntax"></param>
        /// <returns></returns>
        public static ImmutableArray<BaseNamespaceDeclarationSyntax> NamespaceSyntaxChain(
           this MemberDeclarationSyntax syntax)
        {
            syntax = syntax.ThrowIfNull(nameof(syntax));

            List<BaseNamespaceDeclarationSyntax> list = new();
            SyntaxNode? node = syntax.Parent;

            while (node != null)
            {
                switch (node)
                {
                    case NamespaceDeclarationSyntax item: list.Add(item); break;
                    case FileScopedNamespaceDeclarationSyntax item: list.Add(item); break;
                }
                node = node.Parent;
            }

            list.Reverse();
            return list.ToImmutableArray();
        }

        /// <summary>
        /// Gets the chain of type declaration syntax for the given item, including itself by default
        /// if it is a type one.
        /// </summary>
        /// <param name="syntax"></param>
        /// <param name="tryIncludeSelf"></param>
        /// <returns></returns>
        public static ImmutableArray<TypeDeclarationSyntax> TypeSyntaxChain(
           this MemberDeclarationSyntax syntax,
           bool tryIncludeSelf = true)
        {
            syntax = syntax.ThrowIfNull(nameof(syntax));

            List<TypeDeclarationSyntax> list = new();
            SyntaxNode? node = tryIncludeSelf ? syntax : syntax.Parent;

            while (node != null)
            {
                if (node is TypeDeclarationSyntax item) list.Add(item);
                node = node.Parent;
            }

            list.Reverse();
            return list.ToImmutableArray();
        }

        // ----------------------------------------------------

        /// <summary>
        /// Gets the chain of type symbols for the given item, including itself by default if it is
        /// a type one.
        /// </summary>
        /// <param name="symbol"></param>
        /// <param name="tryIncludeSelf"></param>
        /// <returns></returns>
        public static ImmutableArray<INamedTypeSymbol> TypeSymbolChain(
            this ISymbol symbol,
            bool tryIncludeSelf = true)
        {
            symbol = symbol.ThrowIfNull(nameof(symbol));

            List<INamedTypeSymbol> list = new();
            ISymbol? node = tryIncludeSelf ? symbol : symbol.ContainingType;

            while (node != null)
            {
                if (node is INamedTypeSymbol item) list.Add(item);
                node = node.ContainingType;
            }

            list.Reverse();
            return list.ToImmutableArray();
        }
    }
}