namespace Yotei.Tools.CoreGenerator;

// ========================================================
internal static class TypeSymbolExtensions
{
    extension(ITypeSymbol symbol)
    {
        /// <summary>
        /// Determines if this type symbol is an interface, or not.
        /// </summary>
        public bool IsInterface => symbol.TypeKind == TypeKind.Interface;

        // ------------------------------------------------

        /// <summary>
        /// Determines if this type symbol is a partial one, or not.
        /// </summary>
        public bool IsPartial => symbol
            .GetSyntaxNodes()
            .OfType<TypeDeclarationSyntax>()
            .Any(x => x.Modifiers.Any(m => m.IsKind(SyntaxKind.PartialKeyword)));

        // ------------------------------------------------

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

        // ------------------------------------------------

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

        // ------------------------------------------------

        /// <summary>
        /// Determines if this type symbol matches the given regular type, or not.
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public bool Match(Type type)
        {
            ArgumentNullException.ThrowIfNull(symbol);
            ArgumentNullException.ThrowIfNull(type);

            // Trivial cases...
            if (symbol.IsNamespace) return false;
            if (symbol.Kind == SymbolKind.TypeParameter) return true;
            if (type.IsGenericParameter) return true;

            // Capturing...
            var sargs = (symbol as INamedTypeSymbol)?.TypeArguments ?? [];
            var targs = type.GenericTypeArguments.Length != 0
            ? type.GenericTypeArguments
            : type is System.Reflection.TypeInfo info ? info.GenericTypeParameters : [];

            if (sargs.Length != targs.Length) return false; // shortcut...

            // Names...
            var sname = symbol.Name;
            var tname = type.Name;
            var index = tname.IndexOf('`');
            if (index >= 0) tname = tname[..index];
            if (sname != tname) return false;

            // Hierarchy...
            var shost = symbol.ContainingType;
            var thost = type.DeclaringType;

            if (shost is null && thost is null) // Namespaces...
            {
                var sspace = symbol.ContainingNamespace?.ToString() ?? string.Empty;
                var tspace = type.Namespace ?? string.Empty;
                if (sspace != tspace) return false;
            }
            else if (shost is null || thost is null) return false;

            if (shost is not null && thost is not null) // Nested types...
            {
                if (!shost.Match(thost)) return false;
            }
            else if (shost is not null || thost is not null) return false;

            // Type arguments...
            var count = sargs.Length;
            for (int i = 0; i < count; i++)
            {
                var sarg = sargs[i];
                var targ = targs[i];
                if (!sarg.Match(targ)) return false;
            }

            // Finishing...
            return true;
        }

        /// <summary>
        /// Determines if this type symbol matches any of the given regular type, or not.
        /// </summary>
        /// <param name="types"></param>
        /// <returns></returns>
        public bool MatchAny(IEnumerable<Type> types)
        {
            ArgumentNullException.ThrowIfNull(symbol);
            ArgumentNullException.ThrowIfNull(types);

            return types.Any(x => symbol.Match(x));
        }
    }
}