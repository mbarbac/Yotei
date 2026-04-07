namespace Yotei.Tools.Generators;

// ========================================================
public static class TypeSymbolExtensions
{
    extension(ITypeSymbol source)
    {


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