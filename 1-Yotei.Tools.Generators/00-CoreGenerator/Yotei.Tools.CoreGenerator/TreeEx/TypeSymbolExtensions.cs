namespace Yotei.Tools.CoreGenerator;

// ========================================================
internal static partial class TypeSymbolExtensions
{
    extension(ITypeSymbol source)
    {
        /// <summary>
        /// Determines if the type is a nullable wrapper or not.
        /// </summary>
        public bool IsNullableWrapper() =>
            source is INamedTypeSymbol named &&
            named.Arity == 1 && (
            source.OriginalDefinition.SpecialType == SpecialType.System_Nullable_T ||
            source.OriginalDefinition.Name == nameof(IsNullable<>));
    }
}