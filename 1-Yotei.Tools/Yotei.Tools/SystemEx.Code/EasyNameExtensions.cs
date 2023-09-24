namespace Yotei.Tools;

// ========================================================
public static class EasyNameExtensions
{
    /// <summary>
    /// Returns the C#-alike name of the given type.
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    public static string EasyName(this Type type)
    {
        return type.EasyName(EasyNameOptions.Default);
    }

    /// <summary>
    /// Returns the C#-alike name of the given type.
    /// </summary>
    /// <param name="type"></param>
    /// <param name="options"></param>
    /// <returns></returns>
    public static string EasyName(this Type type, EasyNameOptions options)
    {
        ArgumentNullException.ThrowIfNull(type);

        var gens = type.GetGenericArguments();
        return EasyName(type, options, gens);
    }

    static string EasyName(Type type, EasyNameOptions options, Type[] gens)
    {
        // Generic types...
        if (type.IsGenericParameter)
        {
            var str = options.PreventGenericTypeNames ? string.Empty : type.Name;
            return str;
        }

        // Others...
        else
        {
            var sb = new StringBuilder();

            // Nested types or namespace...
            if (type.DeclaringType != null && (options.UseFullTypeName || options.UseNameSpace))
            {
                var host = type.DeclaringType;
                var str = EasyName(host, options, gens);
                sb.Append($"{str}.");
            }
            else if (options.UseNameSpace)
            {
                var str = type.Namespace!;
                if (str != null) sb.Append($"{str}.");
            }

            // Generic arguments...
            var temps = type.GetGenericArguments();
            if (temps.Length > 0)
            {
                var i = type.Name.IndexOf('`');
                sb.Append(i >= 0 ? type.Name.AsSpan(0, i) : type.Name);

                var used = 0;
                var host = type.DeclaringType; while (host != null)
                {
                    var items = host.GetGenericArguments(); used += items.Length;
                    host = host.DeclaringType;
                }

                temps = gens.AsSpan(used, temps.Length - used).ToArray();
                if (temps.Length > 0)
                {
                    sb.Append('<'); for (i = 0; i < temps.Length; i++)
                    {
                        var temp = temps[i];
                        var str = EasyName(temp, options, gens);

                        if (i > 0) sb.Append(str.Length > 0 ? ", " : ",");
                        sb.Append(str);
                    }
                    sb.Append('>');
                }
            }
            else
            {
                sb.Append(type.Name);
            }

            // Finishing...
            return sb.ToString();
        }
    }

    // ----------------------------------------------------

    /// <summary>
    /// Returns the C#-alike name of the given constructor.
    /// </summary>
    /// <param name="info"></param>
    /// <returns></returns>
    public static string EasyName(this ConstructorInfo info)
    {
        return info.EasyName(EasyNameOptions.Default);
    }

    /// <summary>
    /// Returns the C#-alike name of the given constructor.
    /// </summary>
    /// <param name="info"></param>
    /// <param name="options"></param>
    /// <returns></returns>
    public static string EasyName(this ConstructorInfo info, EasyNameOptions options)
    {
        ArgumentNullException.ThrowIfNull(info);

        var sb = new StringBuilder();
        string str;

        if (options.UseTypeName)
        {
            var type = info.DeclaringType;
            if (type != null)
            {
                str = type.EasyName(options);
                sb.Append(str);
                sb.Append('.');
            }
        }

        sb.Append(info.Name);

        if (!options.PreventArguments)
        {
            var pars = info.GetParameters();
            sb.Append('('); for (int i = 0; i < pars.Length; i++)
            {
                var par = pars[i];
                var type = par.ParameterType;
                var temp = type.EasyName(options);

                if (i > 0) sb.Append(temp.Length > 0 ? ", " : ",");
                sb.Append(temp);

                if (temp.Length > 0) sb.Append(' ');
                sb.Append(par.Name);
            }
            sb.Append(')');
        }

        return sb.ToString();
    }

    // ----------------------------------------------------

    /// <summary>
    /// Returns the C#-alike name of the given method.
    /// </summary>
    /// <param name="info"></param>
    /// <returns></returns>
    public static string EasyName(this MethodInfo info)
    {
        return info.EasyName(EasyNameOptions.Default);
    }

    /// <summary>
    /// Returns the C#-alike name of the given method.
    /// </summary>
    /// <param name="info"></param>
    /// <param name="options"></param>
    /// <returns></returns>
    public static string EasyName(this MethodInfo info, EasyNameOptions options)
    {
        ArgumentNullException.ThrowIfNull(info);

        var sb = new StringBuilder();
        string str;

        if (!options.PreventReturnType)
        {
            str = info.ReturnType.EasyName(options);
            sb.Append(str);
            sb.Append(' ');
        }

        if (options.UseTypeName)
        {
            var type = info.DeclaringType;
            if (type != null)
            {
                str = type.EasyName(options);
                sb.Append(str);
                sb.Append('.');
            }
        }

        sb.Append(info.Name);

        var gens = info.GetGenericArguments();
        if (gens.Length > 0)
        {
            sb.Append('<'); for (int i = 0; i < gens.Length; i++)
            {
                var type = gens[i];
                str = type.EasyName(options);

                if (i > 0) sb.Append(str.Length > 0 ? ", " : ",");
                sb.Append(str);
            }
            sb.Append('>');
        }

        if (!options.PreventArguments)
        {
            var pars = info.GetParameters();
            sb.Append('('); for (int i = 0; i < pars.Length; i++)
            {
                var par = pars[i];
                var type = par.ParameterType;
                var temp = type.EasyName(options);

                if (i > 0) sb.Append(temp.Length > 0 ? ", " : ",");
                sb.Append(temp);

                if (temp.Length > 0) sb.Append(' ');
                sb.Append(par.Name);
            }
            sb.Append(')');
        }

        return sb.ToString();
    }

    // ----------------------------------------------------

    /// <summary>
    /// Returns the C#-alike name of the given property.
    /// </summary>
    /// <param name="info"></param>
    /// <returns></returns>
    public static string EasyName(this PropertyInfo info)
    {
        return info.EasyName(EasyNameOptions.Default);
    }

    /// <summary>
    /// Returns the C#-alike name of the given property.
    /// </summary>
    /// <param name="info"></param>
    /// <param name="options"></param>
    /// <returns></returns>
    public static string EasyName(this PropertyInfo info, EasyNameOptions options)
    {
        ArgumentNullException.ThrowIfNull(info);

        var sb = new StringBuilder();
        string str;

        if (!options.PreventReturnType)
        {
            str = info.PropertyType.EasyName(options);
            sb.Append(str);
            sb.Append(' ');
        }

        if (options.UseTypeName)
        {
            var type = info.DeclaringType;
            if (type != null)
            {
                str = type.EasyName(options);
                sb.Append(str);
                sb.Append('.');
            }
        }

        // Indexed property...
        var pars = info.GetIndexParameters();
        if (pars.Length > 0)
        {
            sb.Append("this");

            if (!options.PreventArguments)
            {
                sb.Append('['); for (int i = 0; i < pars.Length; i++)
                {
                    var par = pars[i];
                    var type = par.ParameterType;
                    var temp = type.EasyName(options);

                    if (i > 0) sb.Append(temp.Length > 0 ? ", " : ",");
                    sb.Append(temp);

                    if (temp.Length > 0) sb.Append(' ');
                    sb.Append(par.Name);
                }
                sb.Append(']');
            }
        }

        // Regular property...
        else
        {
            sb.Append(info.Name);
        }

        return sb.ToString();
    }

    // ----------------------------------------------------

    /// <summary>
    /// Returns the C#-alike name of the given field.
    /// </summary>
    /// <param name="info"></param>
    /// <returns></returns>
    public static string EasyName(this FieldInfo info)
    {
        return info.EasyName(EasyNameOptions.Default);
    }

    /// <summary>
    /// Returns the C#-alike name of the given field.
    /// </summary>
    /// <param name="info"></param>
    /// <param name="options"></param>
    /// <returns></returns>
    public static string EasyName(this FieldInfo info, EasyNameOptions options)
    {
        ArgumentNullException.ThrowIfNull(info);

        var sb = new StringBuilder();
        string str;

        if (!options.PreventReturnType)
        {
            str = info.FieldType.EasyName(options);
            sb.Append(str);
            sb.Append(' ');
        }

        if (options.UseTypeName)
        {
            var type = info.DeclaringType;
            if (type != null)
            {
                str = type.EasyName(options);
                sb.Append(str);
                sb.Append('.');
            }
        }

        sb.Append(info.Name);

        return sb.ToString();
    }
}