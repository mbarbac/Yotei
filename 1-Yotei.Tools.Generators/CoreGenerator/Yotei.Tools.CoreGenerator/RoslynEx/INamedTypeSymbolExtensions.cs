namespace Yotei.Tools.CoreGenerator;

// ========================================================
internal static class INamedTypeSymbolExtensions
{
    extension(INamedTypeSymbol symbol)
    {
        /// <summary>
        /// Tries to find a copy constructor defined in this named type.
        /// <br/> In the default strict mode, the sole constructor parameter must be of the same
        /// type as this one. In not-strict mode, it is enough if if the parameter can be assigned
        /// to this type.
        /// <br/> Parameter nullability is not taken into consideration.
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

        // ------------------------------------------------

        /// <summary>
        /// Determines if this type is a nullable one, or not.
        /// <br/> This property also takes into consideration '<see cref="IsNullable{T}"/>' ones.
        /// </summary>
        public bool IsNullable
        {
            get
            {
                if (symbol.OriginalDefinition.SpecialType == SpecialType.System_Nullable_T)
                    return true;

                if (symbol.IsReferenceType && symbol.NullableAnnotation == NullableAnnotation.Annotated)
                    return true;

                if (symbol.Name == "IsNullable" && symbol.TypeArguments.Length == 1)
                    return true;

                return false;
            }
        }

        /// <summary>
        /// Obtains the underlying type from this one, provided it is a nullable one, or returns
        /// this type otherwise.
        /// <br/> This method also takes into consideration '<see cref="IsNullable{T}"/>' ones.
        /// </summary>
        /// <param name="isnullable"></param>
        /// <returns></returns>
        public INamedTypeSymbol UnwrapNullable(out bool isnullable)
        {
            if (symbol.OriginalDefinition.SpecialType == SpecialType.System_Nullable_T)
            { isnullable = true; return (INamedTypeSymbol)symbol.TypeArguments[0]; }

            if (symbol.IsReferenceType && symbol.NullableAnnotation == NullableAnnotation.Annotated)
            { isnullable = true; return symbol; }

            if (symbol.Name == "IsNullable" && symbol.TypeArguments.Length == 1)
            { isnullable = true; return (INamedTypeSymbol)symbol.TypeArguments[0]; }

            isnullable = false;
            return symbol;
        }
    }
}