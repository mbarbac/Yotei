namespace Yotei.Tools.CoreGenerator;

// ========================================================
internal static class ITypeSymbolExtensions
{
    extension(ITypeSymbol symbol)
    {
        /// <summary>
        /// Determines if this type symbol is an interface, or not.
        /// </summary>
        public bool IsInterface => symbol.TypeKind == TypeKind.Interface;

        /// <summary>
        /// Determines if this type symbol is a partial one, or not.
        /// </summary>
        public bool IsPartial => symbol
            .GetSyntaxNodes()
            .OfType<TypeDeclarationSyntax>()
            .Any(x => x.Modifiers.Any(m => m.IsKind(SyntaxKind.PartialKeyword)));

        /// <summary>
        /// Obtains the collection of base types of this named type symbol, in reverse inheritance
        /// order.
        /// </summary>
        public IEnumerable<INamedTypeSymbol> AllBaseTypes
        {
            get
            {
                INamedTypeSymbol? temp = null;
                while ((temp = (temp ?? symbol).BaseType) != null)
                    if (!temp.IsNamespace) yield return temp;
            }
        }

        /// <summary>
        /// Determines if instances of this type can be assigned to instances of the given target
        /// one, or not. This method uses both the inheritance and interfaces chains, recursively.
        /// </summary>
        /// <param name="target"></param>
        /// <returns></returns>
        public bool IsAssignableTo(ITypeSymbol target)
        {
            var comparer = SymbolEqualityComparer.Default;
            if (comparer.Equals(symbol, target)) return true;

            if (symbol.AllBaseTypes.Any(x => comparer.Equals(x, target))) return true;
            if (symbol.AllInterfaces.Any(x => comparer.Equals(x, target))) return true;
            return false;
        }
    }
}