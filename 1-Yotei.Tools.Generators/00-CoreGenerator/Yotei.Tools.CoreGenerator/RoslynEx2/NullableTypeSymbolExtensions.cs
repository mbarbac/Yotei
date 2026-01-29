namespace Yotei.Tools.CoreGenerator;

// ========================================================
internal static class NullableTypeSymbolExtensions
{
    extension (INamedTypeSymbol symbol)
    {
        /// <summary>
        /// Determines if this named type symbol is a nullable wrapper or not.
        /// </summary>
        public bool IsNullableWrapper
        {
            get =>
                symbol.OriginalDefinition.SpecialType == SpecialType.System_Nullable_T ||
                symbol.OriginalDefinition.Name == nameof(IsNullable<>);
        }

        // ------------------------------------------------

        /// <summary>
        /// Determines if this type is a nullable one or not.
        /// This property also takes in consideration the <see cref="IsNullable{T}"/> wrapper and
        /// the <see cref="IsNullableAttribute"/> attribute, as needed.
        /// </summary>
        public bool IsNullable
        {
            get
            {
                if (symbol.OriginalDefinition.SpecialType == SpecialType.System_Nullable_T)
                    return true;

                if (symbol.IsReferenceType && symbol.NullableAnnotation == NullableAnnotation.Annotated)
                    return true;

                if (symbol.GetAttributes(typeof(IsNullableAttribute)).Any())
                    return true;

                if (symbol.Name == nameof(IsNullable<>) && symbol.TypeArguments.Length == 1)
                    return true;

                return false;
            }
        }

        // ------------------------------------------------

        /// <summary>
        /// Returns the underlying type from this one, provided it is a nullable one, or the original
        /// type otherwise. This method also takes in consideration the <see cref="IsNullable{T}"/>
        /// wrapper and the <see cref="IsNullableAttribute"/> attribute, as needed.
        /// </summary>
        /// <param name="isNullable"></param>
        /// <returns></returns>
        public INamedTypeSymbol UnwrapNullable(out bool isNullable)
        {
            isNullable = true; // For simplicity...

            if (symbol.OriginalDefinition.SpecialType == SpecialType.System_Nullable_T)
                return (INamedTypeSymbol)symbol.TypeArguments[0];

            if (symbol.IsReferenceType && symbol.NullableAnnotation == NullableAnnotation.Annotated)
                return symbol;

            if (symbol.GetAttributes(typeof(IsNullableAttribute)).Any())
                return symbol;

            if (symbol.Name == nameof(IsNullable<>) && symbol.TypeArguments.Length == 1)
                return (INamedTypeSymbol)symbol.TypeArguments[0];

            isNullable = false; // Recovering default...
            return symbol;
        }
    }
}