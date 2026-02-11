namespace Yotei.Tools.CoreGenerator;

// ========================================================
internal static partial class SymbolExtensions
{
    extension(ISymbol source)
    {
        /// <summary>
        /// Determines if the given symbol is either decorated with a nullable annotation, or with
        /// the <see cref="IsNullableAttribute"/>.
        /// </summary>
        public bool IsNullableDecorated()
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