namespace Yotei.Tools.Generators;

// ========================================================
public static partial class RoslynNamesExtensions
{
    /// <summary>
    /// Obtains a C#-alike representation for a given type-alike element, using default options.
    /// </summary>
    /// <param name="source"></param>
    /// <returns></returns>
    public static string EasyName(
        this ITypeSymbol source) => source.EasyName(new EasyTypeOptions());

    /// <summary>
    /// Obtains a C#-alike representation for a given type-alike element, using the given options.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="options"></param>
    /// <returns></returns>
    public static string EasyName(this ITypeSymbol source, EasyTypeOptions options)
    {
        ArgumentNullException.ThrowIfNull(source);
        ArgumentNullException.ThrowIfNull(options);

        // Intercepting placeholders...
        if (options.UsePlaceHolder) return string.Empty;

        throw null;
    }
    /*
    {
        

        // Intercepting arrays...
        if (source.IsArray)
        {
            var arg = source.GetElementType()!;
            var str = arg.EasyName(options);
            if (str.Length > 0)
            {
                var rank = source.GetArrayRank();
                str = $"{str}[{new string(',', rank - 1)}]";
            }
            return str;
        }

        // Intercepting wrappers...
        if (source.IsNullableWrapper() &&
            options.NullableStyle == EasyNullableStyle.UseAnnotations)
        {
            var arg = source.GetGenericArguments()[0];
            var str = arg.EasyName(options);
            if (str.Length > 0 && str[^1] != '?') str += '?';
            return str;
        }

        // Capturing...
        var sb = new StringBuilder();
        var isgen = source.IsGenericAlike();
        var host = source.DeclaringType;
        var xname = options.UseSpecialNames ? source.ToSpecialName() : null;

        // Variance...
        if (options.UseVariance && source.IsGenericParameter)
        {
            var gpa = source.GenericParameterAttributes;
            var variance = gpa & GenericParameterAttributes.VarianceMask;

            if ((variance & GenericParameterAttributes.Covariant) != 0) sb.Append("out ");
            if ((variance & GenericParameterAttributes.Contravariant) != 0) sb.Append("in ");
        }

        // Namespace...
        if (options.NamespaceStyle != EasyNamespaceStyle.None &&
            xname == null &&
            host == null && !isgen)
        {
            var str = source.Namespace;
            if (str != null && str.Length > 0)
            {
                if (options.NamespaceStyle == EasyNamespaceStyle.UseGlobal) sb.Append("global::");
                sb.Append(str).Append('.');
            }
        }

        // Host...
        if ((options.UseHost || options.NamespaceStyle != EasyNamespaceStyle.None) &&
            xname == null &&
            host != null && !isgen)
        {
            var str = host.EasyName(types, options); // Using captured types...
            if (str.Length > 0) sb.Append(str).Append('.');
        }

        // Name...
        if (xname != null) sb.Append(xname);
        else
        {
            var name = source.Name;
            var index = name.IndexOf('`'); if (index >= 0) name = name[..index];
            if (name.Length > 0 && name[^1] == '&') name = name[..^1];

            if (options.RemoveAttributeSuffix &&
                name != ATTRIBUTE &&
                name.EndsWith(ATTRIBUTE))
                name = name.RemoveLast(ATTRIBUTE).ToString();

            sb.Append(name);
        }

        // Generic parameters...
        if (xname == null && options.GenericListOptions != null)
        {
            var xoptions = options.GenericListOptions;
            var args = source.GetGenericArguments();
            var used = host == null ? 0 : host.GetGenericArguments().Length;
            var need = args.Length - used;

            if (need > 0)
            {
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

        // Nullability...
        while (options.NullableStyle != EasyNullableStyle.None &&
            sb.Length > 0 &&
            sb[^1] != '?')
        {
            if (source.IsNullableWrapper()) // Special case...
            {
                if (options.NullableStyle == EasyNullableStyle.KeepWrappers)
                { if (xname != null) { sb.Append('?'); break; } }
            }
            if (source.IsNullableAnnotated()) { sb.Append('?'); break; }
            break;
        }

        // Finishing...
        return sb.ToString();
    }
     
     */

    // ----------------------------------------------------

    /// <summary>
    /// Invoked when the symbol is a namespace.
    /// </summary>
    static string EasyNamespace(this INamespaceSymbol source, EasyTypeOptions options)
    {
        throw null;
    }

    // ----------------------------------------------------

    /// <summary>
    /// Invoked when the symbol is an array (ie: string?[]?).
    /// </summary>
    static string EasyNameTypeArray(this IArrayTypeSymbol source, EasyTypeOptions options)
    {
        throw null;
    }

    // ----------------------------------------------------

    /// <summary>
    /// Invoked when the symbol is a pointer (ie: int*).
    /// </summary>
    static string EasyNameTypePointer(this IArrayTypeSymbol source, EasyTypeOptions options)
    {
        throw null;
    }
}