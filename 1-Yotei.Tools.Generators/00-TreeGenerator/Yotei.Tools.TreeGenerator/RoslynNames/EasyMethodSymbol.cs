namespace Yotei.Tools.TreeGenerator;

// ========================================================
internal static partial class RoslynNames
{
    /// <summary>
    /// Obtains a c#-alike string representation of the given element, using default options.
    /// </summary>
    /// <param name="source"></param>
    /// <returns></returns>
    public static string EasyName(
        this IMethodSymbol source) => source.EasyName(EasyMethodOptions.Default);

    /// <summary>
    /// Obtains a c#-alike string representation of the given element, using the given options.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="options"></param>
    /// <returns></returns>
    public static string EasyName(this IMethodSymbol source, EasyMethodOptions options)
    {
        ArgumentNullException.ThrowIfNull(source);
        ArgumentNullException.ThrowIfNull(options);

        var sb = new StringBuilder();
        var host = source.ContainingType;
        var iface = host != null && host.IsInterface;
        string? str;

        // Accessibility...
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

        // Return type...
        if (options.ReturnTypeOptions != null && (
            source.MethodKind is not MethodKind.Constructor and not MethodKind.StaticConstructor))
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

            var xoptions = options.ReturnTypeOptions.WithHideNameFalse();
            str = source.ReturnType.EasyName(xoptions);

            while (str.Length > 0 && str[^1] != '?' && source.IsNullableByAnnotationOrAttribute())
            {
                if (xoptions.NullableStyle == EasyNullableStyle.KeepWrappers &&
                    source.ReturnType.IsNullableWrapper())
                    break;

                if (xoptions.NullableStyle != EasyNullableStyle.None) str += '?';
                break;
            }
            sb.Append(str).Append(' ');
        }

        // Host type...
        if (options.HostTypeOptions != null &&
            host != null && (
            source.MethodKind is not MethodKind.Constructor and not MethodKind.StaticConstructor))
        {
            var xoptions = options.HostTypeOptions.WithHideNameFalse();
            str = host.EasyName(xoptions);
            if (str.Length > 0) sb.Append(str).Append('.');
        }

        // Name...
        if (source.MethodKind is MethodKind.Constructor or MethodKind.StaticConstructor)
        {
            str = options.UseTechName
                ? source.Name
                : host?.EasyName(EasyTypeOptions.Empty) ?? "new";

            sb.Append(str);
        }
        else sb.Append(source.Name);

        // Generic arguments...
        if (options.GenericOptions != null)
        {
            var args = source.TypeArguments;
            if (args.Length > 0)
            {
                sb.Append('<'); for (int i = 0; i < args.Length; i++)
                {
                    var arg = args[i];
                    str = arg.EasyName(options.GenericOptions);
                    if (i > 0) sb.Append(str.Length > 0 ? ", " : ",");
                    sb.Append(str);
                }
                sb.Append('>');
            }
        }

        // Parameters...
        if (options.UseBrackets || options.ParameterOptions != null)
        {
            sb.Append('('); if (options.ParameterOptions != null)
            {
                var args = source.Parameters;
                for (int i = 0; i < args.Length; i++)
                {
                    var arg = args[i];
                    str = arg.EasyName(options.ParameterOptions);
                    if (i > 0) sb.Append(str.Length > 0 ? ", " : ",");
                    sb.Append(str);
                }
            }
            sb.Append(')');
        }

        // Finishing...
        return sb.ToString();
    }
}