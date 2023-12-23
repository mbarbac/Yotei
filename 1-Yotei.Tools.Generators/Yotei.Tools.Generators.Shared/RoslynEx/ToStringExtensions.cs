namespace Yotei.Tools.Generators.Shared;

// ========================================================
internal static class ToStringExtensions
{
    /// <summary>
    /// Returns a C#-alike string representation of the given property, including its containing
    /// type, and its indexed parameters, if any.
    /// Optionally, the string is preceded by the member type.
    /// </summary>
    /// <param name="symbol"></param>
    /// <param name="useSymbolType"></param>
    /// <returns></returns>
    public static string ToStringEx(this IPropertySymbol symbol, bool useSymbolType)
    {
        var sb = new StringBuilder(); if (useSymbolType)
        {
            var type = symbol.Type;
            var ret = type.Name;
            if (type.NullableAnnotation == NullableAnnotation.Annotated) ret += "?";
            sb.Append(ret);
            sb.Append(' ');
        }

        sb.Append($"{symbol.ContainingType.Name}.{symbol.Name}");
        if (symbol.IsIndexer)
        {
            sb.Append('['); for (int i = 0; i < symbol.Parameters.Length; i++)
            {
                if (i > 0) sb.Append(", ");

                var item = symbol.Parameters[i];
                var itemName = item.Name;
                var itemType = item.Type;
                var typeName = itemType.Name;
                if (itemType.NullableAnnotation == NullableAnnotation.Annotated) typeName += "?";

                sb.Append($"{typeName} {itemName}");
            }
            sb.Append(']');
        }

        return sb.ToString();
    }

    /// <summary>
    /// Returns a C#-alike string representation of the given field, including its containing
    /// type. Optionally, the string is preceded by the member type.
    /// </summary>
    /// <param name="symbol"></param>
    /// <param name="useSymbolType"></param>
    /// <returns></returns>
    public static string ToStringEx(this IFieldSymbol symbol, bool useSymbolType)
    {
        var sb = new StringBuilder(); if (useSymbolType)
        {
            var type = symbol.Type;
            var ret = type.Name;
            if (type.NullableAnnotation == NullableAnnotation.Annotated) ret += "?";
            sb.Append(ret);
            sb.Append(' ');
        }

        sb.Append($"{symbol.ContainingType.Name}.{symbol.Name}");
        return sb.ToString();
    }

    /// <summary>
    /// Returns a C#-alike string representation of the given method, including its containing
    /// type and its parameters, if any. Optionally, the string is preceded by its return type.
    /// </summary>
    /// <param name="symbol"></param>
    /// <param name="useReturnType"></param>
    /// <returns></returns>
    public static string ToStringEx(this IMethodSymbol symbol, bool useReturnType)
    {
        var sb = new StringBuilder(); if (useReturnType)
        {
            var type = symbol.ReturnType;
            var ret = type.Name;
            if (type.NullableAnnotation == NullableAnnotation.Annotated) ret += "?";
            sb.Append(ret);
            sb.Append(' ');
        }

        sb.Append($"{symbol.ContainingType.Name}.{symbol.Name}");
        sb.Append('(');

        for (int i = 0; i < symbol.Parameters.Length; i++)
        {
            if (i > 0) sb.Append(", ");

            var item = symbol.Parameters[i];
            var itemName = item.Name;
            var itemType = item.Type;
            var typeName = itemType.Name;
            if (itemType.NullableAnnotation == NullableAnnotation.Annotated) typeName += "?";

            sb.Append($"{typeName} {itemName}");
        }

        sb.Append(')');
        return sb.ToString();
    }
}