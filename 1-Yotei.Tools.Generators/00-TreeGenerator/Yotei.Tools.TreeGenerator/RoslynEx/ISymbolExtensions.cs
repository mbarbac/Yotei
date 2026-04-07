namespace Yotei.Tools.Generators;

// ========================================================
public static class ISymbolExtensions
{
    extension(ISymbol source)
    {
        /// <summary>
        /// Gets the collection of syntax nodes where this symbol was declared in source code. If
        /// the symbol was declared in metadata or it was implicitly declared, then it may return
        /// an empty array.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<SyntaxNode> GetSyntaxNodes() => source
            .DeclaringSyntaxReferences
            .Select(x => x.GetSyntax());

        /// <summary>
        /// Gets the first known syntax location where this symbol is found. This property might
        /// return '<c>null</c>' if this symbol was declared in metadata or if it was implicitly
        /// declared.
        /// </summary>
        public Location? FirstLocation =>
            source.Locations.FirstOrDefault() ??
            source.GetSyntaxNodes().FirstOrDefault()?.GetLocation();

        // ------------------------------------------------

        /// <summary>
        /// Gets the symbol of the containing type or of the nearest enclosing namespace, or
        /// null if any.
        /// </summary>
        public ISymbol? ContainingTypeOrNamespace =>
            (ISymbol?)source.ContainingType ??
            (ISymbol?)source.ContainingNamespace;

        // ------------------------------------------------

        /// <summary>
        /// Returns the collection of attributes that decorates this symbol, and whose classes
        /// match any of the given ones.
        /// </summary>
        /// <param name="types"></param>
        /// <returns></returns>
        public IEnumerable<AttributeData> GetAttributes(IEnumerable<Type> types)
        {
            ArgumentNullException.ThrowIfNull(source);
            ArgumentNullException.ThrowIfNull(types);

            var ats = source.GetAttributes();
            foreach (var at in ats)
            {
                foreach (var type in types)
                {
                    ArgumentNullException.ThrowIfNull(type);

                    if (at.AttributeClass != null &&
                        at.AttributeClass.Match(type)) yield return at;
                }
            }
        }
    }
}