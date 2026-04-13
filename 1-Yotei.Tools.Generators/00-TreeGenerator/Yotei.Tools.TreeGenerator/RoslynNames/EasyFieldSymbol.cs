namespace Yotei.Tools.Generators;

// ========================================================
public static partial class RoslynNamesExtensions
{
    /// <summary>
    /// Obtains a c#-alike string representation of the given element, using default options.
    /// </summary>
    /// <param name="source"></param>
    /// <returns></returns>
    public static string EasyName(
        this IFieldSymbol source) => source.EasyName(EasyFieldOptions.Default);

    /// <summary>
    /// Obtains a c#-alike string representation of the given element, using the given options.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="options"></param>
    /// <returns></returns>
    public static string EasyName(this IFieldSymbol source, EasyFieldOptions options)
    {
        ArgumentNullException.ThrowIfNull(source);
        ArgumentNullException.ThrowIfNull(options);

        var sb = new StringBuilder();
        var host = source.ContainingType;
        var iface = host != null && host.IsInterface;
        string? str;

        // Accessibility (not using 'private' for simplicity)...
        if (options.UseAccessibility)
        {
            str = source.DeclaredAccessibility.ToAccessibilityString(iface);
            if (str != null) sb.Append(str).Append(' ');
        }

        // Modifiers...
        if (options.UseModifiers)
        {
            if (source.IsNew) sb.Append("new ");
            if (source.IsStatic) sb.Append("static ");
            if (source.IsConst) sb.Append("const ");
            if (source.IsVolatile) sb.Append("volatile ");
        }

        // Return type...
        if (options.MemberTypeOptions != null)
        {
            // Return type modifiers...
            if (options.UseModifiers)
            {
                str = source.RefKind switch
                {
                    RefKind.Ref => "ref",
                    RefKind.Out => "out",
                    RefKind.In => "ref readonly",
                    _ => null
                };
                if (str != null) sb.Append(str).Append(' ');
            }

            // The type itself...
            var xoptions = options.MemberTypeOptions;
            var type = source.Type;
            str = type.EasyName(xoptions);

            // Nullability...
            while (xoptions.NullableStyle != EasyNullableStyle.None && str.Length > 0)
            {
                if (xoptions.NullableStyle == EasyNullableStyle.KeepWrappers &&
                   type.IsNullableWrapper())
                    break;

                if (str.Length > 0 && str[^1] != '?')
                {
                    if (type.IsNullableAnnotatedOrAttribute()) { str += '?'; break; }
                    if (source.IsNullableAnnotatedOrAttribute()) { str += '?'; break; }
                }

                break;
            }

            // Adding...
            if (str.Length > 0) sb.Append(str).Append(' ');
        }

        // Host type...
        if (options.HostTypeOptions != null && host != null)
        {
            var xoptions = options.HostTypeOptions;
            str = host.EasyName(xoptions);
            if (str.Length > 0) sb.Append(str).Append('.');
        }

        // Name...
        sb.Append(source.Name);

        // Finishing...
        return sb.ToString();
    }
}