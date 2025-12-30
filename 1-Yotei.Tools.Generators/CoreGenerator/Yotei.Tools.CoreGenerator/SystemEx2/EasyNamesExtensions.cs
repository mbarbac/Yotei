namespace Yotei.Tools.CoreGenerator;

// ========================================================
internal static class EasyNameExtensions
{
    /// <summary>
    /// Returns the C#-alike name of the given element, using default options.
    /// <para>
    /// </summary>
    /// <param name="source"></param>
    /// <returns></returns>
    public static string EasyName(this Type source) => source.EasyName(EasyNameOptions.Default);

    /// <summary>
    /// Returns the C#-alike name of the given element, using the given options.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="options"></param>
    /// <returns></returns>
    public static string EasyName(this Type source, EasyNameOptions options)
    {
        source.ThrowWhenNull();
        options.ThrowWhenNull();

        var types = source.GetGenericArguments();
        return EasyName(source, options, types);
    }

    // ----------------------------------------------------

    /// <summary>
    /// Determines if the given type is a value nullable one, or not.
    /// </summary>
    static bool IsNullableValueType(this Type type)
        => type.Name.StartsWith("Nullable`1") && type.GetGenericArguments().Length == 1;

    /// <summary>
    /// Determines if the given type is a faked nullable one, or not.
    /// </summary>
    static bool IsNullableFakedType(this Type type)
        => type.Name.StartsWith("IsNullable`1") && type.GetGenericArguments().Length == 1;

    /// <summary>
    /// Invoked to return the C#-alike name of the given type element, after its closed generic
    /// type arguments have been obtained. Otherwise, this information is lost if asking for it
    /// in a recursive fashion.
    /// </summary>
    static string EasyName(this Type source, EasyNameOptions options, Type[] types)
    {
        var isgen = source.FullName == null;
        var host = source.DeclaringType;

        var args = source.GetGenericArguments();
        var used = host == null ? 0 : host.GetGenericArguments().Length;
        var need = args.Length - used;

        // Shortcut hide name...
        var hide = options.TypeHideName;
        if (options.TypeUseNamespace || options.TypeUseHost ||
            (need > 0 && options.TypeArgumentsOptions is not null))
            hide = false;

        if (hide) return string.Empty;

        // Shortcut nullable types...
        if (options.TypeUseAnnotation)
        {
            var isnullablevalue = IsNullableValueType(source);
            var isnullablefaked = IsNullableFakedType(source);

            if (isnullablevalue || isnullablefaked)
            {
                var str = args[0].EasyName(options);
                if (!str.EndsWith('?')) str += '?';
                return str;
            }
        }

        // Other cases...
        var sb = new StringBuilder();

        // Namespace...
        if (options.TypeUseNamespace && !isgen && host is null)
        {
            var str = source.Namespace;
            if (str is not null && str.Length > 0) sb.Append($"{str}.");
        }

        // Host...
        if ((options.TypeUseHost || options.TypeUseNamespace) &&
            !isgen &&
            host is not null)
        {
            var str = host.EasyName(options, types);
            sb.Append($"{str}.");
        }

        // Name...
        var name = source.Name;
        var index = name.IndexOf('`'); if (index >= 0) name = name[..index];
        if (name.EndsWith('&')) name = name[..^1];
        sb.Append(name);

        // Generic arguments...
        if (need > 0 && options.TypeArgumentsOptions is not null)
        {
            var xoptions = options.TypeArgumentsOptions;
            if (xoptions.TypeArgumentsOptions is null)
                xoptions = xoptions with { TypeArgumentsOptions = xoptions };

            sb.Append('<'); for (int i = 0; i < need; i++)
            {
                var arg = types[i + used];
                var str = arg.EasyName(xoptions);
                if (i > 0) sb.Append(str.Length > 0 ? ", " : ",");
                sb.Append(str);
            }
            sb.Append('>');
        }

        // Finishing...
        return sb.ToString();
    }

    // ----------------------------------------------------

    /// <summary>
    /// Returns the C#-alike name of the given element, using default options.
    /// </summary>
    /// <param name="source"></param>
    /// <returns></returns>
    public static string EasyName(this ParameterInfo source) => source.EasyName(EasyNameOptions.Default);

    /// <summary>
    /// Returns the C#-alike name of the given element, using the given options.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="options"></param>
    /// <returns></returns>
    public static string EasyName(this ParameterInfo source, EasyNameOptions options)
    {
        var sb = new StringBuilder();

        if (options.MemberUseArgumentTypes)
        {
            var str = source.ParameterType.EasyName(options);
            if (str.Length > 0)
            {
                if (source.IsIn) sb.Append("in ");
                else if (source.IsOut) sb.Append("out ");
                else if (source.ParameterType.IsByRef) sb.Append("ref ");
                sb.Append(str);
            }

            /* Net Standard 2.0 does not support nullability APIs...
            if (options.TypeUseAnnotation && sb[^1] != '?')
            {
                // Nullability API not reliable for generic types...
                if (source.ParameterType.FullName == null)
                {
                    var at = source.GetCustomAttribute<NullableAttribute>();
                    if (at is not null &&
                        at.NullableFlags.Length > 0 &&
                        at.NullableFlags[0] == 2)
                        sb.Append('?');
                }
                // Standard case using nullability API...
                else
                {
                    var nic = new NullabilityInfoContext();
                    var info = nic.Create(source);

                    if (info.ReadState == NullabilityState.Nullable ||
                        info.WriteState == NullabilityState.Nullable)
                        sb.Append('?');
                }
            }*/
        }

        if (options.MemberUseArgumentNames)
        {
            if (sb.Length > 0) sb.Append(' ');
            sb.Append(source.Name);
        }

        return sb.ToString();
    }

    // ----------------------------------------------------

    /// <summary>
    /// Returns the C#-alike name of the given element, using default options.
    /// </summary>
    /// <param name="source"></param>
    /// <returns></returns>
    public static string EasyName(this MethodInfo source) => source.EasyName(EasyNameOptions.Default);

    /// <summary>
    /// Returns the C#-alike name of the given element, using the given options.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="options"></param>
    /// <returns></returns>
    public static string EasyName(this MethodInfo source, EasyNameOptions options)
    {
        source.ThrowWhenNull();
        options.ThrowWhenNull();

        var host = source.DeclaringType;
        var sb = new StringBuilder();

        // Return type...
        if (options.MemberReturnTypeOptions is not null)
        {
            var str = source.ReturnType.EasyName(options.MemberReturnTypeOptions);
            if (str.Length > 0) sb.Append($"{str} ");
        }

        // Host...
        if (options.MemberHostTypeOptions is not null && host is not null)
        {
            var str = host.EasyName(options.MemberHostTypeOptions);
            if (str.Length > 0) sb.Append($"{str}.");
        }

        // Name...
        sb.Append(source.Name);

        // Generic arguments...
        if (options.MemberGenericArgumentsOptions is not null)
        {
            var args = source.GetGenericArguments();
            if (args.Length > 0)
            {
                var xoptions = options.MemberGenericArgumentsOptions;
                if (xoptions.MemberGenericArgumentsOptions is null)
                    xoptions = xoptions with { MemberGenericArgumentsOptions = xoptions };

                sb.Append('<'); for (int i = 0; i < args.Length; i++)
                {
                    var arg = args[i];
                    var str = arg.EasyName(xoptions);
                    if (i > 0) sb.Append(str.Length > 0 ? ", " : ",");
                    sb.Append(str);
                }
                sb.Append('>');
            }
        }

        // Member arguments...
        if (options.MemberUseArgumentTypes || options.MemberUseArgumentNames)
        {
            sb.Append('(');
            var args = source.GetParameters(); for (int i = 0; i < args.Length; i++)
            {
                var arg = args[i];
                var str = arg.EasyName(options);
                if (i > 0) sb.Append(str.Length > 0 ? ", " : ",");
                sb.Append(str);
            }
            sb.Append(')');
        }

        // Finishing...
        return sb.ToString();
    }

    // ----------------------------------------------------

    /// <summary>
    /// Returns the C#-alike name of the given element, using default options.
    /// </summary>
    /// <param name="source"></param>
    /// <returns></returns>
    public static string EasyName(
        this ConstructorInfo source) => source.EasyName(EasyNameOptions.Default);

    /// <summary>
    /// Returns the C#-alike name of the given element, using the given options.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="options"></param>
    /// <returns></returns>
    public static string EasyName(this ConstructorInfo source, EasyNameOptions options)
    {
        source.ThrowWhenNull();
        options.ThrowWhenNull();

        var host = source.DeclaringType;
        var sb = new StringBuilder();

        // Host...
        if (options.MemberHostTypeOptions is not null && host is not null)
        {
            var str = host.EasyName(options.MemberHostTypeOptions);
            if (str.Length > 0) sb.Append($"{str}.");
        }

        // Name...
        var name = !options.ConstructorTechName ? "new" : source.Name;
        sb.Append(name);

        // Member arguments...
        if (options.MemberUseArgumentTypes || options.MemberUseArgumentNames)
        {
            sb.Append('(');
            var args = source.GetParameters(); for (int i = 0; i < args.Length; i++)
            {
                var arg = args[i];
                var str = arg.EasyName(options);
                if (i > 0) sb.Append(str.Length > 0 ? ", " : ",");
                sb.Append(str);
            }
            sb.Append(')');
        }

        // Finishing...
        return sb.ToString();
    }

    // ----------------------------------------------------

    /// <summary>
    /// Returns the C#-alike name of the given element, using default options.
    /// </summary>
    /// <param name="source"></param>
    /// <returns></returns>
    public static string EasyName(this PropertyInfo source) => source.EasyName(EasyNameOptions.Default);

    /// <summary>
    /// Returns the C#-alike name of the given element, using the given options.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="options"></param>
    /// <returns></returns>
    public static string EasyName(this PropertyInfo source, EasyNameOptions options)
    {
        source.ThrowWhenNull();
        options.ThrowWhenNull();

        var host = source.DeclaringType;
        var sb = new StringBuilder();

        // Return type...
        if (options.MemberReturnTypeOptions is not null)
        {
            var str = source.PropertyType.EasyName(options.MemberReturnTypeOptions);
            if (str.Length > 0) sb.Append($"{str} ");
        }

        // Host...
        if (options.MemberHostTypeOptions is not null && host is not null)
        {
            var str = host.EasyName(options.MemberHostTypeOptions);
            if (str.Length > 0) sb.Append($"{str}.");
        }

        // Name...
        var name = source.Name;
        var args = source.GetIndexParameters();
        if (args.Length > 0 && !options.IndexerTechName) name = "this";
        sb.Append(name);

        // Member arguments...
        if (args.Length > 0 && (options.MemberUseArgumentTypes || options.MemberUseArgumentNames))
        {
            sb.Append('['); for (int i = 0; i < args.Length; i++)
            {
                var arg = args[i];
                var str = arg.EasyName(options);
                if (i > 0) sb.Append(str.Length > 0 ? ", " : ",");
                sb.Append(str);
            }
            sb.Append(']');
        }

        // Finishing...
        return sb.ToString();
    }

    // ----------------------------------------------------

    /// <summary>
    /// Returns the C#-alike name of the given element, using default options.
    /// </summary>
    /// <param name="source"></param>
    /// <returns></returns>
    public static string EasyName(this FieldInfo source) => source.EasyName(EasyNameOptions.Default);

    /// <summary>
    /// Returns the C#-alike name of the given element, using the given options.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="options"></param>
    /// <returns></returns>
    public static string EasyName(this FieldInfo source, EasyNameOptions options)
    {
        source.ThrowWhenNull();
        options.ThrowWhenNull();

        var host = source.DeclaringType;
        var sb = new StringBuilder();

        // Return type...
        if (options.MemberReturnTypeOptions is not null)
        {
            var str = source.FieldType.EasyName(options.MemberReturnTypeOptions);
            if (str.Length > 0) sb.Append($"{str} ");
        }

        // Host...
        if (options.MemberHostTypeOptions is not null && host is not null)
        {
            var str = host.EasyName(options.MemberHostTypeOptions);
            if (str.Length > 0) sb.Append($"{str}.");
        }

        // Name...
        sb.Append(source.Name);

        // Finishing...
        return sb.ToString();
    }
}