namespace Yotei.Tools.CoreGenerator;

// ========================================================
internal static partial class SymbolExtensions
{
    extension(ISymbol source)
    {
        /// <summary>
        /// Determines if the given symbol is nullable one because it either has been denoted as
        /// such with a '<see langword="?"/>' nullable annotation, or because it is decorared with
        /// a <see cref="NullableAttribute"/> or a  <see cref="IsNullableAttribute"/> attribute.
        /// <para>
        /// This method does not take into consideration the case that the symbol is a nullable
        /// wrapper type-alike one (such as value-type related <see cref="Nullable{T}"/> ones, or
        /// the <see cref="IsNullable{T}"/> workaround). For these cases, you can check the type
        /// using the <see cref="TypeSymbolExtensions.IsNullableWrapper(ITypeSymbol)"/> method.
        /// </para>
        /// </summary>
        public bool IsNullableByAnnotationOrAttribute()
        {
            // Using nullable annotations...
            var annotated = source switch
            {
                ITypeSymbol item => item.NullableAnnotation == NullableAnnotation.Annotated,
                IPropertySymbol item => item.NullableAnnotation == NullableAnnotation.Annotated,
                IFieldSymbol item => item.NullableAnnotation == NullableAnnotation.Annotated,
                IEventSymbol item => item.NullableAnnotation == NullableAnnotation.Annotated,
                _ => false
            };
            if (annotated) return true;

            // Using custom metadata attribute...
            var name = "System.Runtime.CompilerServices.NullableAttribute";
            var ats = source.GetAttributes().Where(x => x.AttributeClass?.ToDisplayString() == name);
            foreach (var at in ats)
            {
                var items = at.GetType().GetField("NullableFlags");
                if (items != null)
                {
                    var value = items.GetValue(at);

                    if ((value is byte b && b == 2) ||
                        (value is byte[] bs && bs.Length > 0 && bs[0] == 2))
                        return true;
                }
            }

            // Using the wrapper workaround...
            if (ats.Any(x => x.AttributeClass?.Name == nameof(IsNullableAttribute))) return true;

            // Not nullable...
            return false;
        }
    }
}