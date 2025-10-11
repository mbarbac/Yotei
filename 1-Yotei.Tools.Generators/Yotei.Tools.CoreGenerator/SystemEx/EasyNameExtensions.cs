namespace Yotei.Tools.CoreGenerator;

// ========================================================
/// <summary>
/// Maintains the '<c>EasyName(...)</c>' family of methods, which return a C#-alike name of the
/// given element: types or type members' info instances.
/// </summary>
/// TODO: there seems to be no way to tell if a type is null-annotated, workarounds are weak.
internal static class EasyNameExtensions
{
    /// <summary>
    /// Returns the C#-alike name of this type, using default options.
    /// </summary>
    /// <param name="source"></param>
    /// <returns></returns>
    public static string EasyName(this Type source) => source.EasyName(EasyNameOptions.Default);

    /// <summary>
    /// Returns the C#-alike name of this type, using the given options.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="options"></param>
    /// <returns></returns>
    /// NOTE: Generic arguments must be captured upfront because when getting the type's host,
    /// the bound information is lost.
    public static string EasyName(this Type source, EasyNameOptions options)
    {
        source.ThrowWhenNull();
        options.ThrowWhenNull();

        var types = source.GetGenericArguments();
        return source.EasyName(options, types);
    }

    /// <summary>
    /// Invoked after the actual closed generic arguments are captured.
    /// </summary>
    static string EasyName(this Type source, EasyNameOptions options, Type[] types)
    {
        var sb = new StringBuilder();
        var isgen = source.FullName == null;
        var host = source.DeclaringType;

        // Namespace...
        if (options.TypeUseNamespace && !isgen && host is null)
        {
            var str = source.Namespace;
            if (str is not null && str.Length > 0) sb.Append($"{str}.");
        }

        // Host...
        if ((options.TypeUseHost || options.TypeUseNamespace) && host != null && !isgen)
        {
            var str = host.EasyName(options, types);
            sb.Append($"{str}.");
        }

        // Name...
        var name = string.Empty;
        if (options.TypeUseName || options.TypeUseHost || options.TypeUseNamespace)
        {
            name = source.Name;
            var index = name.IndexOf('`');
            if (index >= 0) name = name[..index];
            sb.Append(name);
        }

        // Generic arguments...
        if (options.TypeGenericArgumentsOptions != null)
        {
            var args = source.GetGenericArguments().Length;
            if (args > 0)
            {
                var used = host == null ? 0 : host.GetGenericArguments().Length;
                var max = args - used;
                if (max > 0)
                {
                    if (name.Length == 0) sb.Append('$');

                    sb.Append('<');
                    for (int i = 0; i < max; i++)
                    {
                        var arg = types[i + used];
                        var str = arg.EasyName(options.TypeGenericArgumentsOptions);

                        if (i > 0) sb.Append(str.Length > 0 ? ", " : ",");
                        sb.Append(str);
                    }
                    sb.Append('>');
                }
            }
        }

        // Finishing...
        return sb.ToString();
    }

    // ----------------------------------------------------

    /// <summary>
    /// Returns the C#-alike name of this method, using default options.
    /// </summary>
    /// <param name="source"></param>
    /// <returns></returns>
    public static string EasyName(this MethodInfo source) => source.EasyName(EasyNameOptions.Default);

    /// <summary>
    /// Returns the C#-alike name of this method, using the given options.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="options"></param>
    /// <returns></returns>
    public static string EasyName(this MethodInfo source, EasyNameOptions options)
    {
        source.ThrowWhenNull();
        options.ThrowWhenNull();

        var sb = new StringBuilder();
        var host = source.DeclaringType;

        // Return type...
        if (options.MemberReturnTypeOptions != null)
        {
            var str = source.ReturnType.EasyName(options.MemberReturnTypeOptions);
            if (str.Length > 0) sb.Append($"{str} ");
        }

        // Host type...
        if (options.MemberHostTypeOptions != null && host != null)
        {
            var str = host.EasyName(options.MemberHostTypeOptions);
            if (str.Length > 0) sb.Append($"{str}.");
        }

        // Name...
        sb.Append(source.Name);

        // Generic arguments...
        if (options.MemberGenericArgumentsOptions != null)
        {
            var args = source.GetGenericArguments();
            if (args.Length > 0)
            {
                sb.Append('<'); for (int i = 0; i < args.Length; i++)
                {
                    var arg = args[i];
                    var str = arg.EasyName(options.MemberGenericArgumentsOptions);
                    if (i > 0) sb.Append(str.Length > 0 ? ", " : ",");
                    sb.Append(str);
                }
                sb.Append('>');
            }
        }

        // Member arguments...
        if (options.MemberArgumentTypesOptions != null || options.MemberArgumentsNames)
        {
            var pars = source.GetParameters();

            sb.Append('('); for (int i = 0; i < pars.Length; i++)
            {
                var par = pars[i];
                var str = new StringBuilder();

                if (options.MemberArgumentTypesOptions != null)
                {
                    var type = par.ParameterType.EasyName(options.MemberArgumentTypesOptions);
                    if (type.Length > 0)
                    {
                        str.Append(type);
                        if (options.MemberArgumentsNames) str.Append(' ');
                    }
                }
                if (options.MemberArgumentsNames) str.Append(par.Name);

                if (i > 0) sb.Append(str.Length > 0 ? ", " : ",");
                if (str.Length > 0) sb.Append(str);
            }
            sb.Append(')');
        }

        // Finishing...
        return sb.ToString();
    }

    // ----------------------------------------------------

    /// <summary>
    /// Returns the C#-alike name of this constructor, using default options.
    /// </summary>
    /// <param name="source"></param>
    /// <returns></returns>
    public static string EasyName(this ConstructorInfo source) => source.EasyName(EasyNameOptions.Default);

    /// <summary>
    /// Returns the C#-alike name of this constructor, using the given options.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="options"></param>
    /// <returns></returns>
    public static string EasyName(this ConstructorInfo source, EasyNameOptions options)
    {
        source.ThrowWhenNull();
        options.ThrowWhenNull();

        var sb = new StringBuilder();
        var host = source.DeclaringType;

        // Host type...
        if (options.MemberHostTypeOptions != null && host != null)
        {
            var str = host.EasyName(options.MemberHostTypeOptions);
            if (str.Length > 0) sb.Append($"{str}.");
        }

        // Name...
        var name = options.MemberConstructorNew ? "new" : (sb.Length > 0 ? "ctor" : ".ctor");
        sb.Append(name);

        // Member arguments...
        if (options.MemberArgumentTypesOptions != null || options.MemberArgumentsNames)
        {
            var pars = source.GetParameters();

            sb.Append('('); for (int i = 0; i < pars.Length; i++)
            {
                var par = pars[i];
                var str = new StringBuilder();

                if (options.MemberArgumentTypesOptions != null)
                {
                    var type = par.ParameterType.EasyName(options.MemberArgumentTypesOptions);
                    if (type.Length > 0)
                    {
                        str.Append(type);
                        if (options.MemberArgumentsNames) str.Append(' ');
                    }
                }
                if (options.MemberArgumentsNames) str.Append(par.Name);

                if (i > 0) sb.Append(str.Length > 0 ? ", " : ",");
                if (str.Length > 0) sb.Append(str);
            }
            sb.Append(')');
        }

        // Finishing...
        return sb.ToString();
    }

    // ----------------------------------------------------

    /// <summary>
    /// Returns the C#-alike name of this property, using default options.
    /// </summary>
    /// <param name="source"></param>
    /// <returns></returns>
    public static string EasyName(this PropertyInfo source) => source.EasyName(EasyNameOptions.Default);

    /// <summary>
    /// Returns the C#-alike name of this property, using the given options.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="options"></param>
    /// <returns></returns>
    public static string EasyName(this PropertyInfo source, EasyNameOptions options)
    {
        source.ThrowWhenNull();
        options.ThrowWhenNull();

        var sb = new StringBuilder();
        var host = source.DeclaringType;

        // Return type...
        if (options.MemberReturnTypeOptions != null)
        {
            var str = source.PropertyType.EasyName(options.MemberReturnTypeOptions);
            if (str.Length > 0) sb.Append($"{str} ");
        }

        // Host type...
        if (options.MemberHostTypeOptions != null && host != null)
        {
            var str = host.EasyName(options.MemberHostTypeOptions);
            if (str.Length > 0) sb.Append($"{str}.");
        }

        // Name...
        var pars = source.GetIndexParameters();
        var name = pars.Length == 0 || !options.MemberIndexerThis ? source.Name : "this";
        sb.Append(name);

        // Member arguments...
        if (pars.Length > 0 && (
            options.MemberArgumentTypesOptions != null || options.MemberArgumentsNames))
        {
            sb.Append('['); for (int i = 0; i < pars.Length; i++)
            {
                var par = pars[i];
                var str = new StringBuilder();

                if (options.MemberArgumentTypesOptions != null)
                {
                    var type = par.ParameterType.EasyName(options.MemberArgumentTypesOptions);
                    if (type.Length > 0)
                    {
                        str.Append(type);
                        if (options.MemberArgumentsNames) str.Append(' ');
                    }
                }
                if (options.MemberArgumentsNames) str.Append(par.Name);

                if (i > 0) sb.Append(str.Length > 0 ? ", " : ",");
                if (str.Length > 0) sb.Append(str);
            }
            sb.Append(']');
        }

        // Finishing...
        return sb.ToString();
    }

    // ----------------------------------------------------

    /// <summary>
    /// Returns the C#-alike name of this field, using default options.
    /// </summary>
    /// <param name="source"></param>
    /// <returns></returns>
    public static string EasyName(this FieldInfo source) => source.EasyName(EasyNameOptions.Default);

    /// <summary>
    /// Returns the C#-alike name of this field, using the given options.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="options"></param>
    /// <returns></returns>
    public static string EasyName(this FieldInfo source, EasyNameOptions options)
    {
        source.ThrowWhenNull();
        options.ThrowWhenNull();

        var sb = new StringBuilder();
        var host = source.DeclaringType;

        // Return type...
        if (options.MemberReturnTypeOptions != null)
        {
            var str = source.FieldType.EasyName(options.MemberReturnTypeOptions);
            if (str.Length > 0) sb.Append($"{str} ");
        }

        // Host type...
        if (options.MemberHostTypeOptions != null && host != null)
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