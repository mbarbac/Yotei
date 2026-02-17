namespace Yotei.Tools.CoreGenerator;

// ========================================================
internal static partial class SymbolExtensions
{
    extension(ISymbol source)
    {
        /// <summary>
        /// Gets the symbol of the containing type or of the nearest enclosing namespace, or
        /// null if any.
        /// </summary>
        public ISymbol? ContainingTypeOrNamespace =>
            (ISymbol?)source.ContainingType ??
            (ISymbol?)source.ContainingNamespace;

        // ------------------------------------------------

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
        /// Gets the collection of syntax nodes where this symbol was declared in source code. If
        /// the symbol was declared in metadata or it was implicitly declared, then it may return
        /// an empty array.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<SyntaxNode> GetSyntaxNodes() => source
            .DeclaringSyntaxReferences
            .Select(x => x.GetSyntax());

        // ------------------------------------------------

        /// <summary>
        /// Returns the collection of attributes that decorates the given symbol and whose class is
        /// any of the given ones.
        /// </summary>
        /// <param name="types"></param>
        /// <returns></returns>
        public IEnumerable<AttributeData> GetAttributes(IEnumerable<Type> types)
        {
            ArgumentNullException.ThrowIfNull(source);
            ArgumentNullException.ThrowIfNull(types);

            foreach (var at in source.GetAttributes())
            {
                foreach (var type in types)
                {
                    ArgumentNullException.ThrowIfNull(type);

                    if (at.AttributeClass != null &&
                        at.AttributeClass.Match(type)) yield return at;
                }
            }
        }

        // ------------------------------------------------

        /// <summary>
        /// Tries to determine if the symbol is decorated with the <see langword="new"/> keyword by
        /// finding its declaring syntax references and in each finding the 'new' modifier.
        /// <br/> If no syntax references were available, then it returns false.
        /// </summary>
        public bool IsNew
        {
            get
            {
                var nodes = source.GetSyntaxNodes();
                foreach (var node in nodes)
                {
                    var item = node;
                    
                    // Motivation: the symbol obtained by the tree generator for a FieldEvent syntax
                    // get transformed into a IEventSymbol, but then its declaration syntaxes are
                    // VariableDeclarator ones...
                    if (node is VariableDeclaratorSyntax vardec)
                    {
                        item = vardec.Parent;
                        item = (item as VariableDeclarationSyntax)?.Parent;
                    }
                    switch (item)
                    {
                        case EventDeclarationSyntax temp: if (temp.Modifiers.Any(x => x.IsKind(SyntaxKind.NewKeyword))) return true; break;
                        case EventFieldDeclarationSyntax temp: if (temp.Modifiers.Any(x => x.IsKind(SyntaxKind.NewKeyword))) return true; break;
                        case BaseTypeDeclarationSyntax temp: if (temp.Modifiers.Any(x => x.IsKind(SyntaxKind.NewKeyword))) return true; break;
                        case BasePropertyDeclarationSyntax temp: if (temp.Modifiers.Any(x => x.IsKind(SyntaxKind.NewKeyword))) return true; break;
                        case BaseFieldDeclarationSyntax temp: if (temp.Modifiers.Any(x => x.IsKind(SyntaxKind.NewKeyword))) return true; break;
                        case BaseMethodDeclarationSyntax temp: if (temp.Modifiers.Any(x => x.IsKind(SyntaxKind.NewKeyword))) return true; break;
                    }
                }
                return false;
            }
        }
    }
}