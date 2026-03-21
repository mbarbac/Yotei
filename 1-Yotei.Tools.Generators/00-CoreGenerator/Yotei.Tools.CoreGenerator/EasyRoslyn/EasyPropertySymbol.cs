namespace Yotei.Tools.CoreGenerator;

// ========================================================
internal static partial class EasyNameExtensions
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

        // Accesibility (not using 'private' for simplicity)...
        if (options.UseAccessibility)
        {
            str = source.DeclaredAccessibility.ToAccessibilityString(iface);
            if (str != null) sb.Append(str).Append(' ');
        }

        // Modifiers...
        if (options.UseModifiers)
        {
            if (source.IsNew) sb.Append("new ");
            if (source.IsPartialDefinition) sb.Append("partial ");
            if (source.IsSealed) sb.Append("sealed ");
            if (source.IsStatic) sb.Append("static ");

            if (!iface && source.IsAbstract) sb.Append("abstract ");
            if (source.IsOverride) sb.Append("override ");
            else if (source.IsVirtual) sb.Append("virtual ");
        }

        // Member type...
        if (options.MemberTypeOptions != null)
        {
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

            var xoptions = options.MemberTypeOptions.NoHideName();
            str = source.Type.EasyName(xoptions);

            while (str.Length > 0 && str[^1] != '?' && source.IsNullableByAnnotationOrAttribute())
            {
                if (xoptions.NullableStyle == EasyNullableStyle.KeepWrappers &&
                    source.Type.IsNullableWrapper())
                    break;

                if (xoptions.NullableStyle != EasyNullableStyle.None) str += '?';
                break;
            }
            sb.Append(str).Append(' ');
        }

        // Host type...
        if (options.HostTypeOptions != null && host != null)
        {
            var xoptions = options.HostTypeOptions.NoHideName();
            str = host.EasyName(xoptions);
            if (str.Length > 0) sb.Append(str).Append('.');
        }

        // Name...
        var name = source.Name;
        var args = source.Parameters;
        if (args.Length > 0) name = options.UseTechName ? source.MetadataName : "this";
        sb.Append(name);

        // Parameters...
        if (args.Length > 0 && (options.UseBrackets || options.ParameterOptions != null))
        {
            sb.Append('['); if (options.ParameterOptions != null)
            {
                for (int i = 0; i < args.Length; i++)
                {
                    var arg = args[i];
                    str = arg.EasyName(options.ParameterOptions);
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