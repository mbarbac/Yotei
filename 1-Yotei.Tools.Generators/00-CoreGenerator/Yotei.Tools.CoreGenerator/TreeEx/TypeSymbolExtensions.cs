namespace Yotei.Tools.CoreGenerator;

// ========================================================
internal static partial class TypeSymbolExtensions
{
    extension(ITypeSymbol source)
    {
        /// <summary>
        /// Determines if the type is a nullable wrapper or not (so it either is a value-type
        /// <see cref="Nullable{}"/> instance, or a <see cref="IsNullable{T}"/> one).
        /// </summary>
        public bool IsNullableWrapper() =>
            source is INamedTypeSymbol named &&
            named.Arity == 1 && (
            source.OriginalDefinition.SpecialType == SpecialType.System_Nullable_T ||
            source.OriginalDefinition.Name == nameof(IsNullable<>));
    }
}