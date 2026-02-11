namespace Yotei.Tools.CoreGenerator;

// ========================================================
internal static partial class TypeSymbolExtensions
{
    extension(ITypeSymbol source)
    {
        /// <summary>
        /// Determines if this type symbol is an interface, or not.
        /// </summary>
        public bool IsInterface => source.TypeKind == TypeKind.Interface;

        // ------------------------------------------------

        /// <summary>
        /// Determines if this type symbol is a partial one, or not.
        /// </summary>
        public bool IsPartial => source
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
                while ((temp = (temp ?? source).BaseType) != null)
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
            if (comparer.Equals(source, target)) return true;

            if (source.AllBaseTypes.Any(x => comparer.Equals(x, target))) return true;
            if (source.AllInterfaces.Any(x => comparer.Equals(x, target))) return true;
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
            ArgumentNullException.ThrowIfNull(source);
            ArgumentNullException.ThrowIfNull(type);

            // Trivial cases...
            if (source.IsNamespace) return false;
            if (source.Kind == SymbolKind.TypeParameter) return true;
            if (type.IsGenericParameter) return true;

            // Capturing...
            var sargs = (source as INamedTypeSymbol)?.TypeArguments ?? [];
            var targs = type.GenericTypeArguments.Length != 0
            ? type.GenericTypeArguments
            : type is System.Reflection.TypeInfo info ? info.GenericTypeParameters : [];

            if (sargs.Length != targs.Length) return false; // shortcut...

            // Names...
            var sname = source.Name;
            var tname = type.Name;
            var index = tname.IndexOf('`');
            if (index >= 0) tname = tname[..index];
            if (sname != tname) return false;

            // Hierarchy...
            var shost = source.ContainingType;
            var thost = type.DeclaringType;

            if (shost is null && thost is null) // Namespaces...
            {
                var sspace = source.ContainingNamespace?.ToString() ?? string.Empty;
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
            ArgumentNullException.ThrowIfNull(source);
            ArgumentNullException.ThrowIfNull(types);

            return types.Any(x => source.Match(x));
        }
    }
}