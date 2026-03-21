namespace Yotei.Tools.CoreGenerator;

// ========================================================
internal static partial class EasyNameExtensions
{
    /// <summary>
    /// Obtains a c#-alike string representation of the given element, using default options.
    /// </summary>
    /// <param name="source"></param>
    /// <returns></returns>
    public static string EasyName(this ITypeSymbol source) => source.EasyName(EasyTypeOptions.Default);

    /// <summary>
    /// Obtains a c#-alike string representation of the given element, using the given options.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="options"></param>
    /// <returns></returns>
    public static string EasyName(this ITypeSymbol source, EasyTypeOptions options)
    {
        ArgumentNullException.ThrowIfNull(source);
        ArgumentNullException.ThrowIfNull(options);

        // Shortcuts...
        if (options.HideName) return string.Empty;
        switch (source)
        {
            case IArrayTypeSymbol item: return EasyNameTypeArray(item, options);
            case IPointerTypeSymbol item: return EasyNameTypePointer(item, options);

            // When using generic type arguments (ie: 'typeof(Whatever<>)') to print the '<T>',
            // for whatever reasons we get an 'IErrorTypeSymbol' object. As it happens that it
            // implements 'INamedTypeSymbol' we can just continue without custom processing,
            // case IErrorTypeSymbol:

            // I have not yet a use case for this case, so I'm not going to maintain it for now.
            // case IFunctionPointerTypeSymbol:

            default: break;
        }

        // Processing...
        var sb = new StringBuilder();
        var named = source as INamedTypeSymbol;
        var isgen = source.IsGenericAlike();
        var args = named is null ? [] : named.TypeArguments;
        var host = source.ContainingType;

        

        // Finishing...
        return sb.ToString();
    }
    /*

        // Processing...
        
        var isgen = source.IsGenericAlike();
        var host = source.DeclaringType;

        // Shortcut nullable wrappers...
        if (options.NullableStyle == EasyNullableStyle.UseAnnotations &&
            source.IsNullableWrapper())
        {
            var arg = source.GetGenericArguments()[0];
            var str = arg.EasyName(options);
            if (str.Length > 0 && !str.EndsWith('?')) str += '?';
            return str;
        }

        // Variance...
        if (options.UseVariance && source.IsGenericParameter)
        {
            var gpa = source.GenericParameterAttributes;
            var variance = gpa & GenericParameterAttributes.VarianceMask;

            if ((variance & GenericParameterAttributes.Covariant) != 0) sb.Append("out ");
            if ((variance & GenericParameterAttributes.Contravariant) != 0) sb.Append("in ");
        }

        // Special names...
        var xname = options.UseSpecialNames ? source.ToSpecialName() : null;

        // Namespace...
        if (options.NamespaceStyle != EasyNamespaceStyle.None &&
            host == null && !isgen &&
            xname == null)
        {
            var str = source.Namespace;
            if (str != null && str.Length > 0)
            {
                if (options.NamespaceStyle == EasyNamespaceStyle.UseGlobal) sb.Append("global:");
                sb.Append(str).Append('.');
            }
        }

        // Host...
        if ((options.UseHost || options.NamespaceStyle != EasyNamespaceStyle.None) &&
            host != null && !isgen &&
            xname == null)
        {
            var xoptions = options.NoHideName();
            var str = host.EasyName(types, xoptions);
            if (str.Length > 0) sb.Append(str).Append('.');
        }

        // Name...
        string? name = xname; if (name == null)
        {
            name = source.Name;
            var index = name.IndexOf('`'); if (index >= 0) name = name[..index];
            if (name.Length > 0 && name[^1] == '&') name = name[..^1];

            if (options.RemoveAttributeSuffix &&
                name != "Attribute" &&
                name.EndsWith("Attribute"))
                name = name.RemoveLast("Attribute").ToString();
        }
        sb.Append(name);

        // Generic arguments...
        if (options.GenericStyle != EasyGenericStyle.None)
        {
            var args = source.GetGenericArguments();
            var used = host == null ? 0 : host.GetGenericArguments().Length;
            var need = args.Length - used;

            if (need > 0)
            {
                var xoptions = options.GenericStyle == EasyGenericStyle.PlaceHolders
                    ? options.YesHideName()
                    : options.NoHideName();

                sb.Append('<'); for (int i = 0; i < need; i++)
                {
                    var arg = types[used + i];
                    var str = arg.EasyName(xoptions);
                    if (i > 0) sb.Append(str.Length > 0 ? ", " : ",");
                    sb.Append(str);
                }
                sb.Append('>');
            }
        }

        // Nullable annotations...
        if (options.NullableStyle != EasyNullableStyle.None)
        {
            if (options.NullableStyle == EasyNullableStyle.KeepWrappers &&
                source.IsNullableWrapper())
                goto ENDNULLABLE;

            if (source.HasNullableEnabledAttribute())
            {
                if (sb.Length > 0 && sb[^1] != '?') sb.Append('?');
                goto ENDNULLABLE;
            }   
        }
        ENDNULLABLE:;

        // Finishing...
        return sb.ToString();
    }
     */

    // ----------------------------------------------------

    /// <summary>
    /// Invoked when the type is an array one (ie: string?[]?)
    /// </summary>
    static string EasyNameTypeArray(IArrayTypeSymbol source, EasyTypeOptions options)
    {
        throw null;
    }

    // ----------------------------------------------------

    /// <summary>
    /// Invoked when the type represents a pointer (ie: int*)
    /// </summary>
    static string EasyNameTypePointer(IPointerTypeSymbol source, EasyTypeOptions options)
    {
        throw null;
    }
}