namespace Yotei.Tools.CoreGenerator;

// ========================================================
internal static class TypeSymbolExtensions
{
    extension(ITypeSymbol symbol)
    {
        /// <summary>
        /// Determines if the type is an interface, or not.
        /// </summary>
        public bool IsInterface => symbol.TypeKind == TypeKind.Interface;

        /// <summary>
        /// Determines if the type is a partial one, or not.
        /// </summary>
        public bool IsPartial => symbol.GetSyntaxNodes().Any(static x => x.IsPartial);

        /// <summary>
        /// The base types of this one, in inheritance order.
        /// </summary>
        public IEnumerable<INamedTypeSymbol> AllBaseTypes
        {
            get
            {
                INamedTypeSymbol? temp = null;

                while ((temp = (temp ?? symbol).BaseType) != null)
                {
                    if (!temp.IsNamespace) yield return temp;
                    else yield break;
                }
            }
        }

        /// <summary>
        /// Determines if this type can be assigned to the given target one, or not.
        /// <br/> This method uses both the inheritance and interfaces chains, recursively.
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

        /// <summary>
        /// Returns the copy constructor defined by this type.
        /// <br/> In the default strict mode, the sole constructor parameter must be of the same
        /// type. In non-strict mode, a match is granted is the parameter can be assigned to the
        /// type.
        /// </summary>
        /// <param name="strict"></param>
        /// <returns></returns>
        public IMethodSymbol? FindCopyConstructor(bool strict = true)
        {
            var comparer = SymbolEqualityComparer.Default;
            var methods = symbol.GetMembers().OfType<IMethodSymbol>().Where(static x =>
                x.MethodKind == MethodKind.Constructor &&
                x.IsStatic == false &&
                x.Parameters.Length == 1)
                .ToDebugArray();

            var method = methods.FirstOrDefault(x => comparer.Equals(symbol, x.Parameters[0].Type));
            if (method == null && !strict)
                method = methods.FirstOrDefault(x => symbol.IsAssignableTo(x.Parameters[0].Type));

            return method;
        }
    }
}