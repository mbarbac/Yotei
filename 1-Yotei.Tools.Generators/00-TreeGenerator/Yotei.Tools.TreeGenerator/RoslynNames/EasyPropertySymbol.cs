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
        this IPropertySymbol source) => source.EasyName(EasyPropertyOptions.Default);

    /// <summary>
    /// Obtains a c#-alike string representation of the given element, using the given options.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="options"></param>
    /// <returns></returns>
    public static string EasyName(this IPropertySymbol source, EasyPropertyOptions options)
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
            if (source.IsPartialDefinition) sb.Append("partial ");
            if (source.IsSealed) sb.Append("sealed ");

            if (!iface && source.IsAbstract) sb.Append("abstract ");
            if (source.IsOverride) sb.Append("override ");
            else if (source.IsVirtual && !iface && !source.IsAbstract) sb.Append("virtual ");
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
        var name = source.Name;
        var args = source.Parameters;
        if (args.Length > 0) name = options.UseTechName ? source.MetadataName : "this";
        sb.Append(name);

        // Parameters...
        if (args.Length > 0 && (
            options.UseBrackets || options.ParameterOptions != null))
        {
            sb.Append('['); if (options.ParameterOptions != null)
            {
                var xoptions = options.ParameterOptions;

                for (int i = 0; i < args.Length; i++)
                {
                    var arg = args[i];
                    str = arg.EasyName(xoptions);

                    if (i > 0) sb.Append(str.Length > 0 ? ", " : ",");
                    sb.Append(str);
                }
            }
            sb.Append(']');
        }

        // Finishing...
        return sb.ToString();
    }
}