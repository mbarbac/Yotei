namespace Yotei.Tools.CoreGenerator;

// ========================================================
internal static class SymbolExtensions
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

        // ------------------------------------------------

        /// <summary>
        /// Gets the first known syntax location where this symbol is found. This property might
        /// return '<c>null</c>' if this symbol was declared in metadata or if it was implicitly
        /// declared.
        /// </summary>
        public Location? FirstLocation =>
            symbol.Locations.FirstOrDefault() ??
            symbol.GetSyntaxNodes().FirstOrDefault()?.GetLocation();

        // ------------------------------------------------

        /// <summary>
        /// Gets the collection of syntax nodes where this symbol was declared in source code. If
        /// the symbol was declared in metadata or it was implicitly declared, then it may return
        /// an empty array.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<SyntaxNode> GetSyntaxNodes() => symbol
            .DeclaringSyntaxReferences
            .Select(x => x.GetSyntax());

        // ------------------------------------------------

        /// <summary>
        /// Determines if this symbol has any attribute whose class is the given one.
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public bool HasAttributes(Type type) => symbol.GetAttributes(type).Any();

        /// <summary>
        /// Returns the collection of attributes that decorates the given symbol and whose class is
        /// the given one.
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public IEnumerable<AttributeData> GetAttributes(Type type)
        {
            ArgumentNullException.ThrowIfNull(symbol);
            ArgumentNullException.ThrowIfNull(type);

            foreach (var at in symbol.GetAttributes())
            {
                if (at.AttributeClass is not null &&
                    at.AttributeClass.Match(type)) yield return at;
            }
        }

        /// <summary>
        /// Determines if this symbol has any attribute whose class is among the given ones.
        /// </summary>
        /// <param name="types"></param>
        /// <returns></returns>
        public bool HasAttributes(IEnumerable<Type> types) => symbol.GetAttributes(types).Any();

        /// <summary>
        /// Returns the collection of attributes that decorates the given symbol and whose class is
        /// any of the given ones.
        /// </summary>
        /// <param name="types"></param>
        /// <returns></returns>
        public IEnumerable<AttributeData> GetAttributes(IEnumerable<Type> types)
        {
            ArgumentNullException.ThrowIfNull(symbol);
            ArgumentNullException.ThrowIfNull(types);

            foreach (var at in symbol.GetAttributes())
            {
                foreach (var type in types)
                {
                    if (at.AttributeClass is not null &&
                    at.AttributeClass.Match(type)) yield return at;
                }
            }
        }

        // ------------------------------------------------

        /// <summary>
        /// Tries to determine if the symbol is decorated with the <see langword="new"/> keyword
        /// by finding its declaring syntax references and in each finding the 'new' modifier.
        /// <br/> If no syntax references were available, then it returns false.
        /// </summary>
        public bool IsNew
        {
            get
            {
                var syntaxes = symbol
                    .DeclaringSyntaxReferences
                    .Select(x => x.GetSyntax() as MethodDeclarationSyntax);

                foreach (var syntax in syntaxes)
                    if (syntax?.Modifiers.Any(x => x.IsKind(SyntaxKind.NewKeyword)) ?? false)
                        return true;

                return false;
            }
        }
    }
}