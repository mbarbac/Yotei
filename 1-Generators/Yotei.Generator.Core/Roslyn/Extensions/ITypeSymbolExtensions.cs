namespace Yotei.Generator
{
    // ====================================================
    public static class ITypeSymbolExtensions
    {
        /// <summary>
        /// Determines if instances of the given type can be assigned to the target one, or not.
        /// </summary>
        /// <param name="type"></param>
        /// <param name="target"></param>
        /// <returns></returns>
        public static bool IsAssignableTo(this ITypeSymbol type, ITypeSymbol target)
        {
            type = type.ThrowIfNull(nameof(type));
            target = target.ThrowIfNull(nameof(target));

            if (type.IsNamespace) throw new ArgumentException($"Type cannot be a namespace: {type}");
            if (target.IsNamespace) throw new ArgumentException($"Target cannot be a namespace: {target}");

            return Helper(type, target, true);

            /// <summary> Recursive helper for the base type and interfaces of the given one.
            /// </summary>
            static bool Helper(ITypeSymbol type, ITypeSymbol target, bool useInterfaces)
            {
                var comparer = SymbolEqualityComparer.Default;
                if (comparer.Equals(type, target)) return true;

                ISymbol? node = type.BaseType;
                if (node != null &&
                    node is ITypeSymbol temp &&
                    !temp.IsNamespace)
                {
                    if (Helper(temp, target, false)) return true;
                }

                if (useInterfaces)
                {
                    if (type.AllInterfaces.Any(x => comparer.Equals(target, x))) return true;
                }

                return false;
            }
        }
    }
}