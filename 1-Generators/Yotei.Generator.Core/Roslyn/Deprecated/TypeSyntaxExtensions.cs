namespace Yotei.Generator.Deprecated
{
    // ====================================================
    internal static class TypeSyntaxExtensions
    {
        /// <summary>
        /// Gets the property declaration syntax of the given property symbol on the given type
        /// declaration syntax, or null if such cannot be found.
        /// </summary>
        /// <param name="typeSyntax"></param>
        /// <param name="propSymbol"></param>
        /// <returns></returns>
        public static PropertyDeclarationSyntax? FindPropertyDeclarationSyntax(
            this TypeDeclarationSyntax typeSyntax,
            IPropertySymbol propSymbol)
        {
            typeSyntax = typeSyntax.ThrowIfNull(nameof(typeSyntax));
            propSymbol = propSymbol.ThrowIfNull(nameof(propSymbol));

            var propName = propSymbol.Name;
            var propType = propSymbol.Type.ToString().Replace("?", string.Empty);

            var members = typeSyntax.Members.OfType<PropertyDeclarationSyntax>().ToDebugArray();
            foreach (var member in members)
            {
                var memberName = member.Identifier.Text;
                var memberType = member.Type.ToString().Replace("?", "");

                if (propType == memberType && propName == memberName) return member;
            }

            return null;
        }

        /// <summary>
        /// Gets the field declaration syntax of the given field symbol on the given type
        /// declaration syntax, or null if such cannot be found.
        /// </summary>
        /// <param name="typeSyntax"></param>
        /// <param name="fieldSymbol"></param>
        /// <returns></returns>
        public static FieldDeclarationSyntax? FindFieldDeclarationSyntax(
           this TypeDeclarationSyntax typeSyntax,
           IFieldSymbol fieldSymbol)
        {
            typeSyntax = typeSyntax.ThrowIfNull(nameof(typeSyntax));
            fieldSymbol = fieldSymbol.ThrowIfNull(nameof(fieldSymbol));

            var fieldName = fieldSymbol.Name;
            var fieldType = fieldSymbol.Type.ToString().Replace("?", string.Empty);

            var members = typeSyntax.Members.OfType<FieldDeclarationSyntax>().ToDebugArray();
            foreach (var member in members)
            {
                var temp = member.Declaration.Variables.FirstOrDefault();
                if (temp == null) continue;

                var memberName = temp.Identifier.Text;
                var memberType = member.Declaration.Type.ToString().Replace("?", "");

                if (fieldType == memberType && fieldName == memberName) return member;
            }

            return null;
        }

        // ----------------------------------------------------

        /// <summary>
        /// Gets the syntax declaration syntax of the base type of the one given, or null if it
        /// cannot be found.
        /// </summary>
        /// <param name="typeSyntax"></param>
        /// <returns></returns>
        /// <remarks>
        /// This method is quite fragile: it we try to obtain the full name of the given type,
        /// as that parent is declared in the base list of types, the original type syntax appears
        /// in the path. So, we have to find in the compilation unit a class with the same short
        /// name, which is far from ideal as we are assuming that this parent type is visible to
        /// that compilation unit, which very probably is not the case.
        /// <para>
        /// An alternate approach would be obtaining the symbol of the given symbol, via a
        /// semantic model, and from it obtain its base type, although this will return a symbol
        /// and not a syntax node.
        /// </para>
        /// </remarks>
        public static TypeDeclarationSyntax? BaseType(this TypeDeclarationSyntax typeSyntax)
        {
            typeSyntax = typeSyntax.ThrowIfNull(nameof(typeSyntax));

            var baselist = typeSyntax.BaseList;
            if (baselist != null)
            {
                var parent = baselist.Types.Count > 0 ? baselist.Types[0] : null;
                if (parent != null)
                {
                    var compilation = typeSyntax.SyntaxTree.GetCompilationUnitRoot();
                    var name = parent.ToString();
                    return FindBase(compilation.Members, name);
                }
            }
            return null;
        }

        static TypeDeclarationSyntax? FindBase(IEnumerable<SyntaxNode> members, string parentName)
        {
            foreach (var member in members)
            {
                if (member is TypeDeclarationSyntax syntax)
                {
                    var name = syntax.Identifier.Text;
                    if (name == parentName) return syntax;
                }

                var temp = FindBase(member.DescendantNodes(), parentName);
                if (temp != null) return temp;
            }

            return null;
        }
    }
}