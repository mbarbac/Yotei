namespace Yotei.Tools;

// ========================================================
public static class EasyNameExtensions
{
    /// <summary>
    /// Returns a C#-alike name of the given type, using default options.
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    public static string EasyName(this Type item) => item.EasyName(EasyNameOptions.Default);

    /// <summary>
    /// Returns a C#-alike name of the given type, using the given options.
    /// </summary>
    /// <param name="item"></param>
    /// <param name="options"></param>
    /// <returns></returns>
    public static string EasyName(this Type item, EasyNameOptions options)
    {
        item.ThrowWhenNull();
        options.ThrowWhenNull();

        var tpargs = item.GetGenericArguments().AsSpan();
        var tpused = 0;
        return item.EasyName(options, tpargs, ref tpused);
    }

    /// <summary>
    /// Invoked after having captured the whole set of generic arguments.
    /// </summary>
    static string EasyName(this Type item, EasyNameOptions options, Span<Type> tpargs, ref int tpused)
    {
        var sb = new StringBuilder();
        var gen = item.FullName == null;
        var host = item.DeclaringType;

        // Namespace...
        if (options.UseTypeNamespace && !gen && host == null)
        {
            var ns = item.Namespace;
            if (ns != null && ns.Length > 0) sb.Append($"{ns}.");
        }

        // Declaring host...
        if (!gen && host != null)
        {
            // Consuming host type arguments...
            var str = host.EasyName(options, tpargs, ref tpused);

            // Using host if requested...
            if ((options.UseTypeHost || options.UseTypeNamespace) && str.Length > 0) sb.Append($"{str}.");
        }

        // Name...
        var name = item.Name;
        var index = name.IndexOf('`');
        if (index >= 0) name = name.Remove(index, name.Length - index);

        var used = options.UseTypeName || options.UseTypeHost || options.UseTypeNamespace;
        if (used) sb.Append(name);

        // Type arguments...
        tpargs = tpargs[tpused..]; if (tpargs.Length > 0)
        {
            var hold = host == null ? [] : host.GetGenericArguments();
            var args = item.GetGenericArguments();
            var num = args.Length - hold.Length;

            if (num > 0)
            {
                // Consuming arguments...
                tpused += num;

                // Using arguments, if requested...
                if (options.UseTypeArguments != null)
                {
                    if (!used) sb.Append(name.Length > 0 ? name : "$"); // To make sense...

                    var xoptions = options with
                    {
                        UseTypeNamespace = options.UseTypeArguments.UseTypeNamespace,
                        UseTypeHost = options.UseTypeArguments.UseTypeHost,
                        UseTypeName = options.UseTypeArguments.UseTypeName,
                    };

                    tpargs = tpargs[..num];

                    sb.Append('<');
                    for (int i = 0; i < tpargs.Length; i++)
                    {
                        var arg = tpargs[i];
                        var str = arg.EasyName(xoptions);

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
    /// Returns a C#-alike name of the given method, using default options.
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    public static string EasyName(this MethodInfo item) => item.EasyName(EasyNameOptions.Default);

    /// <summary>
    /// Returns a C#-alike name of the given method, using the given options.
    /// </summary>
    /// <param name="item"></param>
    /// <param name="options"></param>
    /// <returns></returns>
    public static string EasyName(this MethodInfo item, EasyNameOptions options)
    {
        item.ThrowWhenNull();
        options.ThrowWhenNull();

        var sb = new StringBuilder();
        var host = item.DeclaringType;

        // Return type...
        if (options.UseMemberType != null)
        {
            var str = item.ReturnType.EasyName(options.UseMemberType);
            if (str.Length > 0) sb.Append($"{str} ");
        }

        // Member host...
        if (options.UseMemberHost != null && host != null)
        {
            var str = host.EasyName(options.UseMemberHost);
            if (str.Length > 0) sb.Append($"{str}.");
        }

        // Name...
        sb.Append(item.Name);

        // Type arguments...
        if (options.UseMemberTypeArguments != null)
        {
            var args = item.GetGenericArguments();
            if (args.Length > 0)
            {
                sb.Append('<');
                for (int i = 0; i < args.Length; i++)
                {
                    var arg = args[i];
                    var str = arg.EasyName(options.UseMemberTypeArguments);

                    if (i > 0) sb.Append(str.Length > 0 ? ", " : ",");
                    sb.Append(str);
                }
                sb.Append('>');
            }
        }

        // Member arguments...
        if (options.UseMemberArguments || options.UseMemberArgumentsTypes != null ||
            options.UseMemberArgumentsNames)
        {
            sb.Append('(');
            var pars = item.GetParameters(); for (int i = 0; i < pars.Length; i++)
            {
                var par = pars[i];
                var ptype = options.UseMemberArgumentsTypes != null ? par.ParameterType.EasyName(options.UseMemberArgumentsTypes) : "";
                var pname = options.UseMemberArgumentsNames ? (par.Name ?? "") : "";

                var len = ptype.Length + pname.Length;
                if (i > 0) sb.Append(len > 0 ? ", " : ",");

                if (ptype.Length > 0)
                {
                    sb.Append(ptype);
                    if (pname.Length > 0) sb.Append(' ');
                }
                if (pname.Length > 0) sb.Append(pname);
            }
            sb.Append(')');
        }

        // Finishing...
        return sb.ToString();
    }

    // ----------------------------------------------------

    /// <summary>
    /// Returns a C#-alike name of the given constructor, using default options.
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    public static string EasyName(this ConstructorInfo item) => item.EasyName(EasyNameOptions.Default);

    /// <summary>
    /// Returns a C#-alike name of the given constructor, using the given options.
    /// </summary>
    /// <param name="item"></param>
    /// <param name="options"></param>
    /// <returns></returns>
    public static string EasyName(this ConstructorInfo item, EasyNameOptions options)
    {
        item.ThrowWhenNull();
        options.ThrowWhenNull();

        var sb = new StringBuilder();
        var host = item.DeclaringType;

        // Member host...
        if (options.UseMemberHost != null && host != null)
        {
            var str = host.EasyName(options.UseMemberHost);
            if (str.Length > 0) sb.Append($"{str}.");
        }

        // Name...
        var name = "new";
        sb.Append(name);

        // Member arguments...
        if (options.UseMemberArguments || options.UseMemberArgumentsTypes != null || options.UseMemberArgumentsNames)
        {
            sb.Append('(');
            var pars = item.GetParameters(); for (int i = 0; i < pars.Length; i++)
            {
                var par = pars[i];
                var ptype = options.UseMemberArgumentsTypes != null ? par.ParameterType.EasyName(options.UseMemberArgumentsTypes) : "";
                var pname = options.UseMemberArgumentsNames ? (par.Name ?? "") : "";

                var len = ptype.Length + pname.Length;
                if (i > 0) sb.Append(len > 0 ? ", " : ",");

                if (ptype.Length > 0)
                {
                    sb.Append(ptype);
                    if (pname.Length > 0) sb.Append(' ');
                }
                if (pname.Length > 0) sb.Append(pname);
            }
            sb.Append(')');
        }

        // Finishing...
        return sb.ToString();
    }

    // ----------------------------------------------------

    /// <summary>
    /// Returns a C#-alike name of the given property, using default options.
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    public static string EasyName(this PropertyInfo item) => item.EasyName(EasyNameOptions.Default);

    /// <summary>
    /// Returns a C#-alike name of the given property, using the given options.
    /// </summary>
    /// <param name="item"></param>
    /// <param name="options"></param>
    /// <returns></returns>
    public static string EasyName(this PropertyInfo item, EasyNameOptions options)
    {
        item.ThrowWhenNull();
        options.ThrowWhenNull();

        var sb = new StringBuilder();
        var host = item.DeclaringType;

        // Return type...
        if (options.UseMemberType != null)
        {
            var str = item.PropertyType.EasyName(options.UseMemberType);
            if (str.Length > 0) sb.Append($"{str} ");
        }

        // Member host...
        if (options.UseMemberHost != null && host != null)
        {
            var str = host.EasyName(options.UseMemberHost);
            if (str.Length > 0) sb.Append($"{str}.");
        }

        // Name...
        var pars = item.GetIndexParameters();
        var name = pars.Length == 0 ? item.Name : "this";
        sb.Append(name);

        // Member arguments...
        if ((options.UseMemberArguments || options.UseMemberArgumentsTypes != null || options.UseMemberArgumentsNames)
           && pars.Length > 0)
        {
            sb.Append('[');
            for (int i = 0; i < pars.Length; i++)
            {
                var par = pars[i];
                var ptype = options.UseMemberArgumentsTypes != null ? par.ParameterType.EasyName(options.UseMemberArgumentsTypes) : "";
                var pname = options.UseMemberArgumentsNames ? (par.Name ?? "") : "";

                var len = ptype.Length + pname.Length;
                if (i > 0) sb.Append(len > 0 ? ", " : ",");

                if (ptype.Length > 0)
                {
                    sb.Append(ptype);
                    if (pname.Length > 0) sb.Append(' ');
                }
                if (pname.Length > 0) sb.Append(pname);
            }
            sb.Append(']');
        }

        // Finishing...
        return sb.ToString();
    }

    // ----------------------------------------------------

    /// <summary>
    /// Returns a C#-alike name of the given field, using default options.
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    public static string EasyName(this FieldInfo item) => item.EasyName(EasyNameOptions.Default);

    /// <summary>
    /// Returns a C#-alike name of the given field, using the given options.
    /// </summary>
    /// <param name="item"></param>
    /// <param name="options"></param>
    /// <returns></returns>
    public static string EasyName(this FieldInfo item, EasyNameOptions options)
    {
        item.ThrowWhenNull();
        options.ThrowWhenNull();

        var sb = new StringBuilder();
        var host = item.DeclaringType;

        // Return type...
        if (options.UseMemberType != null)
        {
            var str = item.FieldType.EasyName(options.UseMemberType);
            if (str.Length > 0) sb.Append($"{str} ");
        }

        // Member host...
        if (options.UseMemberHost != null && host != null)
        {
            var str = host.EasyName(options.UseMemberHost);
            if (str.Length > 0) sb.Append($"{str}.");
        }

        // Name...
        sb.Append(item.Name);

        // Finishing...
        return sb.ToString();
    }
}