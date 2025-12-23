namespace Yotei.Tools.CoreGenerator;

// ========================================================
internal static class ISymbolExtensions
{
    extension(ISymbol symbol)
    {
        /// <summary>
        /// Gets the symbol of the containing type or of the nearest enclosing namespace, or
        /// null if any.
        /// </summary>
        public ISymbol? ContainingTypeOrNamespace =>
            (ISymbol?)symbol.ContainingType ??
            (ISymbol?)symbol.ContainingNamespace;

        /// <summary>
        /// Gets the collection of syntax nodes where this symbol was declared in source code. If
        /// the symbol was declared in metadata or it was implicitly declared, then it may return
        /// an empty array.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<SyntaxNode> GetSyntaxNodes() => symbol
            .DeclaringSyntaxReferences
            .Select(x => x.GetSyntax());

        /// <summary>
        /// Gets the first known syntax location where this symbol is found. This property might
        /// return '<c>null</c>' if this symbol was declared in metadata or if it was implicitly
        /// declared.
        /// </summary>
        public Location? FirstLocation =>
            symbol.Locations.FirstOrDefault() ??
            symbol.GetSyntaxNodes().FirstOrDefault()?.GetLocation();
    }
}