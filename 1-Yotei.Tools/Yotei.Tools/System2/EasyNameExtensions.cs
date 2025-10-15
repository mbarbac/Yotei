namespace Yotei.Tools;

// ========================================================
public static class EasyNameExtensions
{
    /// <summary>
    /// Returns the C#-alike name of the source type, using default options.
    /// </summary>
    /// <param name="source"></param>
    /// <returns></returns>
    public static string EasyName(this Type source) => source.EasyName(EasyNameOptions.Default);

    /// <summary>
    /// Returns the C#-alike name of the source type, using the given options.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="options"></param>
    /// <returns></returns>
    public static string EasyName(this Type source, EasyNameOptions options)
    {
        source.ThrowWhenNull();
        options.ThrowWhenNull();

        var types = source.GetGenericArguments();
        return source.EasyName(options, types);
    }

    /// <summary>
    /// Invoked after the actual closed generic arguments are captured.
    /// <br/> We need to capture the generic arguments before invoking this method because, from
    /// now on, when getting the source's host, the closed-bound information is lost.
    /// </summary>
    [SuppressMessage("", "IDE0057")]
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
        if ((options.TypeUseHost || options.TypeUseNamespace) && !isgen && host is not null)
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
            if (index >= 0) name = name.Remove(index, name.Length - index);
            sb.Append(name);
        }

        // Generic arguments...
        if (options.TypeGenericArgumentOptions is not null)
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
                        var str = arg.EasyName(options.TypeGenericArgumentOptions);

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
    /// Returns the C#-alike name of the source method, using default options.
    /// </summary>
    /// <param name="source"></param>
    /// <returns></returns>
    public static string EasyName(
        this MethodInfo source) => source.EasyName(EasyNameOptions.Default);

    /// <summary>
    /// Returns the C#-alike name of the source method, using the given options.
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
        if (options.MemberReturnTypeOptions is not null)
        {
            var str = source.ReturnType.EasyName(options.MemberReturnTypeOptions);
            if (str.Length > 0) sb.Append($"{str} ");
        }

        // Host type...
        if (options.MemberHostTypeOptions is not null && host is not null)
        {
            var str = host.EasyName(options.MemberHostTypeOptions);
            if (str.Length > 0) sb.Append($"{str}.");
        }

        // Name...
        sb.Append(source.Name);

        // Generic arguments...
        if (options.MemberGenericArgumentOptions is not null)
        {
            var args = source.GetGenericArguments();
            if (args.Length > 0)
            {
                sb.Append('<'); for (int i = 0; i < args.Length; i++)
                {
                    var arg = args[i];
                    var str = arg.EasyName(options.MemberGenericArgumentOptions);
                    if (i > 0) sb.Append(str.Length > 0 ? ", " : ",");
                    sb.Append(str);
                }
                sb.Append('>');
            }
        }

        // Member arguments...
        if (options.MemberArgumentTypeOptions is not null || options.MemberUseArgumentNames)
        {
            var pars = source.GetParameters();

            sb.Append('('); for (int i = 0; i < pars.Length; i++)
            {
                var par = pars[i];
                var str = new StringBuilder();

                if (options.MemberArgumentTypeOptions is not null)
                {
                    var type = par.ParameterType.EasyName(options.MemberArgumentTypeOptions);
                    if (type.Length > 0) str.Append(type);
                }
                if (options.MemberUseArgumentNames)
                {
                    if (str.Length > 0) str.Append(' ');
                    str.Append(par.Name);
                }

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
    /// Returns the C#-alike name of the source constructor, using default options.
    /// </summary>
    /// <param name="source"></param>
    /// <returns></returns>
    public static string EasyName(
        this ConstructorInfo source) => source.EasyName(EasyNameOptions.Default);

    /// <summary>
    /// Returns the C#-alike name of the source constructor, using the given options.
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
        if (options.MemberHostTypeOptions is not null && host is not null)
        {
            var str = host.EasyName(options.MemberHostTypeOptions);
            if (str.Length > 0) sb.Append($"{str}.");
        }

        // Name...
        var name = options.ConstructorName == "$" ? source.Name : options.ConstructorName;
        if (name[0] == '.' && sb.Length > 0 && sb[^1] == '.') name = name[1..];
        if (name.Length == 0) name = "$";
        sb.Append(name);

        // Member arguments...
        if (options.MemberArgumentTypeOptions is not null || options.MemberUseArgumentNames)
        {
            var pars = source.GetParameters();

            sb.Append('('); for (int i = 0; i < pars.Length; i++)
            {
                var par = pars[i];
                var str = new StringBuilder();

                if (options.MemberArgumentTypeOptions is not null)
                {
                    var type = par.ParameterType.EasyName(options.MemberArgumentTypeOptions);
                    if (type.Length > 0) str.Append(type);
                }
                if (options.MemberUseArgumentNames)
                {
                    if (str.Length > 0) str.Append(' ');
                    str.Append(par.Name);
                }

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
    /// Returns the C#-alike name of the source property, using default options.
    /// </summary>
    /// <param name="source"></param>
    /// <returns></returns>
    public static string EasyName(
        this PropertyInfo source) => source.EasyName(EasyNameOptions.Default);

    /// <summary>
    /// Returns the C#-alike name of the source property, using the given options.
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
        if (options.MemberReturnTypeOptions is not null)
        {
            var str = source.PropertyType.EasyName(options.MemberReturnTypeOptions);
            if (str.Length > 0) sb.Append($"{str} ");
        }

        // Host type...
        if (options.MemberHostTypeOptions is not null && host is not null)
        {
            var str = host.EasyName(options.MemberHostTypeOptions);
            if (str.Length > 0) sb.Append($"{str}.");
        }

        // Name...
        var pars = source.GetIndexParameters();
        var name = pars.Length == 0 ? source.Name : options.IndexerName;
        if (name == "$") name = source.Name;
        sb.Append(name);

        // Member arguments...
        if (pars.Length > 0 && (
            (options.MemberArgumentTypeOptions is not null || options.MemberUseArgumentNames)))
        {
            sb.Append('['); for (int i = 0; i < pars.Length; i++)
            {
                var par = pars[i];
                var str = new StringBuilder();

                if (options.MemberArgumentTypeOptions is not null)
                {
                    var type = par.ParameterType.EasyName(options.MemberArgumentTypeOptions);
                    if (type.Length > 0) str.Append(type);
                }
                if (options.MemberUseArgumentNames)
                {
                    if (str.Length > 0) str.Append(' ');
                    str.Append(par.Name);
                }

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
    /// Returns the C#-alike name of the source field, using default options.
    /// </summary>
    /// <param name="source"></param>
    /// <returns></returns>
    public static string EasyName(
        this FieldInfo source) => source.EasyName(EasyNameOptions.Default);

    /// <summary>
    /// Returns the C#-alike name of the source field, using the given options.
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
        if (options.MemberReturnTypeOptions is not null)
        {
            var str = source.FieldType.EasyName(options.MemberReturnTypeOptions);
            if (str.Length > 0) sb.Append($"{str} ");
        }

        // Host type...
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