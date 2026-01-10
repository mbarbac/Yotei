namespace Yotei.Tools;

// ========================================================
public static class EasyNameExtensions
{
    /// <summary>
    /// Obtains a C#-alike representation of the given element, using default options.
    /// </summary>
    /// <param name="source"></param>
    /// <returns></returns>
    public static string EasyName(
        this Type source) => source.EasyName(EasyNameOptions.Default);

    /// <summary>
    /// Obtains a C#-alike representation of the given element, using the given options.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="options"></param>
    /// <returns></returns>
    public static string EasyName(this Type source, EasyNameOptions options)
    {
        source.ThrowWhenNull();
        options.ThrowWhenNull();

        var types = source.GetGenericArguments();
        return EasyName(source, types, options);
    }

    /// <summary>
    /// Invoked to obtain a C#-alike representation of the given type element, once the details
    /// of its closed generic arguments have been captured, to prevent loosing them in recursive
    /// calls.
    /// </summary>
    static string EasyName(this Type source, Type[] types, EasyNameOptions options)
    {
        var isgen = source.IsGenericParameter || source.IsGenericTypeParameter || source.IsGenericMethodParameter;
        var host = source.DeclaringType;
        var args = source.GetGenericArguments();
        var used = host is null ? 0 : host.GetGenericArguments().Length;
        var need = args.Length - used;
        string? str;

        // Shortcut hide name...
        if (options.HideTypeName) return string.Empty;

        // Shortcut wrapped nullability...
        if (options.TypeNullableStyle is not EasyNullableStyle.KeepWrappers && (
            (need == 1 && source.Name.StartsWith("Nullable`1")) ||
            (need == 1 && source.Name.StartsWith("IsNullable`1"))))
        {
            var type = types[used];
            str = type.EasyName(options);

            if (options.TypeNullableStyle is EasyNullableStyle.UseAnnotations &&
                !str.EndsWith('?'))
                str += '?';

            return str;
        }

        // Processing...
        var sb = new StringBuilder();

        // Variance mask...
        if (options.UseTypeVariance && source.IsGenericParameter) // or otherwise an error!
        {
            var gpa = source.GenericParameterAttributes;
            var variance = gpa & GenericParameterAttributes.VarianceMask;

            if ((variance & GenericParameterAttributes.Covariant) != 0) sb.Append("out ");
            if ((variance & GenericParameterAttributes.Contravariant) != 0) sb.Append("in ");
        }

        // Namespace...
        if (options.UseTypeNamespace && host == null && !isgen)
        {
            str = source.Namespace;
            if (str is not null && str.Length > 0) { sb.Append(str); sb.Append('.'); }
        }

        // Host...
        if (options.UseHost && host != null && !isgen)
        {
            str = host.EasyName(types, options);
            if (str is not null && str.Length > 0) { sb.Append(str); sb.Append('.'); }
        }

        // Name (if 'name&' is a 'ref' type, such is handled elsewhere)...
        str = source.Name;
        var index = str.IndexOf('`'); if (index >= 0) str = str[..index];
        if (str.Length > 0 && str[^1] == '&') str = str[..^1];
        sb.Append(str);

        // Generic arguments...
        if (options.UseGenericArguments && need > 0)
        {
            sb.Append('<'); for (var i = 0; i < need; i++)
            {
                var arg = types[used + i];
                str = arg.EasyName(options);
                if (i > 0) sb.Append(str.Length > 0 ? ", " : ",");
                sb.Append(str);
            }
            sb.Append('>');
        }

        // Nullability...
        if (options.TypeNullableStyle is not EasyNullableStyle.None &&
            sb.Length > 0 &&
            sb[^1] != '?')
        {
            if (options.TypeNullableStyle is EasyNullableStyle.KeepWrappers && (
                source.Name.StartsWith("Nullable`1") ||
                source.Name.StartsWith("IsNullable`1"))) goto END;

            // Nullable attribute (API)...
            var nullability = source.GetCustomAttribute<NullableAttribute>();
            if (nullability != null && nullability.NullableFlags.Length > 0)
            {
                var flag = nullability.NullableFlags[0];
                if (flag == 2) { sb.Append('?'); goto END; }
            }

            // IsNullable attribute...
            if (source.GetCustomAttribute<IsNullableAttribute>() != null)
            { sb.Append('?'); goto END; }
        }

        // Finishing...
        END:
        return sb.ToString();
    }
}