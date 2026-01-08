namespace Yotei.Tools;

// ========================================================
public static class EasyNameExtensions
{
    /// <summary>
    /// Invoked to get the C#-alike name of the given element, using default options.
    /// </summary>
    /// <param name="source"></param>
    /// <returns></returns>
    public static string EasyName(
        this Type source) => source.EasyName(EasyNameOptions.Default);

    /// <summary>
    /// Invoked to get the C#-alike name of the given element, using the given options.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="options"></param>
    /// <returns></returns>
    public static string EasyName(this Type source, EasyNameOptions options)
    {
        source.ThrowWhenNull();
        options.ThrowWhenNull();

        var types = source.GetGenericArguments();
        return source.EasyName(types, options);
    }

    /// <summary>
    /// Invoked to obtain the C#-alike name if the given type, once its closed generic arguments
    /// have been captured to prevent loosing their detailed information in recursive calls.
    /// </summary>
    static string EasyName(this Type source, Type[] types, EasyNameOptions options)
    {
        var isgen = source.FullName == null;
        var host = source.DeclaringType;
        var args = source.GetGenericArguments();
        var used = host is null ? 0 : host.GetGenericArguments().Length;
        var need = args.Length - used;

        // Shortcut hide name...
        if (options.TypeHideName) return string.Empty;

        // Shortcut removing nullability wrappers (*see decorated nullability*)...
        if (options.TypeUseNullability
            && !options.TypeKeepNullableWrappers
            && ((need == 1 && source.Name.StartsWith("Nullable`1"))
            || (need == 1 && source.Name.StartsWith("IsNullable`1"))))
        {
            var type = types[used];
            var str = EasyName(type, options); if (!str.EndsWith('?')) str += '?';
            return str;
        }

        // Processing...
        var sb = new StringBuilder();

        // Variance mask...
        if (options.TypeUseVarianceMask && source.IsGenericParameter)
        {
            var gpa = source.GenericParameterAttributes;
            var variance = gpa & GenericParameterAttributes.VarianceMask;

            if ((variance & GenericParameterAttributes.Covariant) != 0) sb.Append("out ");
            if ((variance & GenericParameterAttributes.Contravariant) != 0) sb.Append("in ");
        }

        // Namespace...
        if (options.TypeUseNamespace && host == null && !isgen)
        {
            var str = source.Namespace;
            if (str is not null && str.Length > 0) { sb.Append(str); sb.Append('.'); }
        }

        // Host...
        if (options.TypeUseHost && host != null && !isgen)
        {
            if (options.TypeHideName) options = options with { TypeHideName = false };
            var str = host.EasyName(types, options);
            if (str is not null && str.Length > 0) { sb.Append(str); sb.Append('.'); }
        }

        // Name (name& is a ref type, but 'ref' is handled elsewhere)...
        var name = source.Name;
        var index = name.IndexOf('`'); if (index >= 0) name = name[..index];
        if (name.Length > 0 && name[^1] == '&') name = name[..^1];
        sb.Append(name);

        // Generic arguments...
        if (options.TypeUseGenericArguments && need > 0)
        {
            sb.Append('<'); for (var i = 0; i < need; i++)
            {
                var arg = types[used + i];
                var str = arg.EasyName(options);
                if (i > 0) sb.Append(str.Length > 0 ? ", " : ",");
                sb.Append(str);
            }
            sb.Append('>');
        }

        // Decorated nullability...
        while (options.TypeUseNullability && sb.Length > 0 && sb[^1] != '?')
        {
#if PREVENT_NULLABILITY_API
            // Using nullability API...
            var nullability = source.GetCustomAttribute<NullableAttribute>();
            if (nullability != null && nullability.NullableFlags.Length > 0)
            {
                var flag = nullability.NullableFlags[0];
                if (flag == 2) { sb.Append('?'); break; }
            }
#endif

            // Using 'IsNullable' forced attribute...
            if (source.GetCustomAttribute<IsNullableAttribute>() != null
                && !source.Name.StartsWith("Nullable`1")
                && !source.Name.StartsWith("IsNullable`1")) { sb.Append('?'); break; }

            break;
        }

        // Finishing...
        return sb.ToString();
    }

    // ----------------------------------------------------

    /// <summary>
    /// Invoked to get the C#-alike name of the given element, using default options.
    /// </summary>
    /// <param name="source"></param>
    /// <returns></returns>
    public static string EasyName(
        this MethodInfo source) => source.EasyName(EasyNameOptions.Default);

    /// <summary>
    /// Invoked to get the C#-alike name of the given element, using the given options.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="options"></param>
    /// <returns></returns>
    public static string EasyName(this MethodInfo source, EasyNameOptions options)
    {
        throw null;
    }

    // ----------------------------------------------------

    /// <summary>
    /// Invoked to get the C#-alike name of the given element, using default options.
    /// </summary>
    /// <param name="source"></param>
    /// <returns></returns>
    public static string EasyName(
        this ParameterInfo source) => source.EasyName(EasyNameOptions.Default);

    /// <summary>
    /// Invoked to get the C#-alike name of the given element, using the given options.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="options"></param>
    /// <returns></returns>
    public static string EasyName(this ParameterInfo source, EasyNameOptions options)
    {
        throw null;
    }

    // ----------------------------------------------------

    /// <summary>
    /// Invoked to get the C#-alike name of the given element, using default options.
    /// </summary>
    /// <param name="source"></param>
    /// <returns></returns>
    public static string EasyName(
        this ConstructorInfo source) => source.EasyName(EasyNameOptions.Default);

    /// <summary>
    /// Invoked to get the C#-alike name of the given element, using the given options.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="options"></param>
    /// <returns></returns>
    public static string EasyName(this ConstructorInfo source, EasyNameOptions options)
    {
        throw null;
    }

    // ----------------------------------------------------

    /// <summary>
    /// Invoked to get the C#-alike name of the given element, using default options.
    /// </summary>
    /// <param name="source"></param>
    /// <returns></returns>
    public static string EasyName(
        this PropertyInfo source) => source.EasyName(EasyNameOptions.Default);

    /// <summary>
    /// Invoked to get the C#-alike name of the given element, using the given options.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="options"></param>
    /// <returns></returns>
    public static string EasyName(this PropertyInfo source, EasyNameOptions options)
    {
        throw null;
    }

    // ----------------------------------------------------

    /// <summary>
    /// Invoked to get the C#-alike name of the given element, using default options.
    /// </summary>
    /// <param name="source"></param>
    /// <returns></returns>
    public static string EasyName(
        this FieldInfo source) => source.EasyName(EasyNameOptions.Default);

    /// <summary>
    /// Invoked to get the C#-alike name of the given element, using the given options.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="options"></param>
    /// <returns></returns>
    public static string EasyName(this FieldInfo source, EasyNameOptions options)
    {
        throw null;
    }
}