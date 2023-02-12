namespace Yotei.Generators.Cloneable;

// ========================================================
public static class FieldExtensions
{
    /// <summary>
    /// Determines if this field is marked to ignore cloning, or not.
    /// </summary>
    /// <param name="symbol"></param>
    /// <returns></returns>
    public static bool IgnoreClone(this IFieldSymbol symbol)
    {
        symbol = symbol.ThrowIfNull(nameof(symbol));

        if (symbol.HasAttribute(CloneableMemberSource.Name, out var data))
        {
            var arg = data.GetNamedArgument(CloneableMemberSource.Ignore);
            if (arg != null &&
                arg.Value.Value is bool value && value) return true;
        }

        return false;
    }

    /// <summary>
    /// Determines if this field is marked for deep clone, or not.
    /// </summary>
    /// <param name="symbol"></param>
    /// <returns></returns>
    public static bool DeepClone(this IFieldSymbol symbol)
    {
        symbol = symbol.ThrowIfNull(nameof(symbol));

        if (symbol.HasAttribute(CloneableMemberSource.Name, out var data))
        {
            var arg = data.GetNamedArgument(CloneableMemberSource.Deep);
            if (arg != null &&
                arg.Value.Value is bool value && value) return true;
        }

        return false;
    }

    /// <summary>
    /// Determines if this field is a nullable one, or not.
    /// </summary>
    /// <param name="symbol"></param>
    /// <returns></returns>
    public static bool IsNullable(this IFieldSymbol symbol)
    {
        symbol = symbol.ThrowIfNull(nameof(symbol));

        return
            symbol.NullableAnnotation == NullableAnnotation.Annotated ||
            symbol.Type.IsReferenceType;
    }

    /// <summary>
    /// Gets a string with the appropriate cloning code for this field.
    /// </summary>
    /// <returns></returns>
    public static string GetCode(this IFieldSymbol symbol)
    {
        symbol = symbol.ThrowIfNull(nameof(symbol));

        var name = symbol.Name;

        if (symbol.DeepClone())
        {
            var nstr = symbol.IsNullable() ? "?" : string.Empty;

            var temp = symbol.Type.ToString().EndsWith("?");
            var type = symbol.Type.FullyQualifiedName();
            if (temp) type += "?";

            return $"({type})this.{name}{nstr}.Clone()";
        }
        else return $"this.{name}";
    }
}