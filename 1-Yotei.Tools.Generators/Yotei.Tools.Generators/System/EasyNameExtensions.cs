namespace Yotei.Tools.Generators.Internal;

// ========================================================
internal static class EasyNameExtensions
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
        var ns = item.Namespace;
        var host = item.DeclaringType;

        // Type namespace...
        if (!gen && ns != null && ns.Length > 0 && host == null && options.UseTypeNamespace)
            sb.Append($"{ns}.");

        // Type host...
        if (!gen && host != null)
        {
            // Consuming type arguments if needed...
            var str = host.EasyName(options, tpargs, ref tpused);

            // Using the host if needed...
            if (str.Length > 0 && (options.UseTypeHost || options.UseTypeNamespace))
                sb.Append($"{str}.");
        }

        // Type name...
        var name = item.Name;
        var index = name.IndexOf('`');
        if (index >= 0) name = name.Remove(index, name.Length - index);
        if (options.UseTypeName) sb.Append(name);

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

                // Using arguments, if needed...
                if (options.UseTypeArguments)
                {
                    if (!options.UseTypeName) sb.Append(name); // Otherwise makes no sense...
                    tpargs = tpargs[..num];

                    sb.Append('<');
                    for (int i = 0; i < num; i++)
                    {
                        var temp = tpargs[i];
                        var str = temp.EasyName(options);

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

    // ---------------------------------------------------

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

        // Member type or return type...
        if (options.UseMemberType)
        {
            var str = item.ReturnType.EasyName(options);
            if (str.Length > 0) sb.Append($"{str} ");
        }

        // Member host declaring type...
        if (options.UseMemberHost && host != null)
        {
            var str = host.EasyName(options);
            if (str.Length > 0) sb.Append($"{str}.");
        }

        // Member name...
        var used = options.UseMemberName;
        if (used) sb.Append(item.Name);

        // Member type arguments...
        if (options.UseMemberTypeArguments)
        {
            if (!used) { sb.Append(item.Name); used = true; }

            var args = item.GetGenericArguments();
            if (args.Length > 0)
            {
                sb.Append('<'); for (int i = 0; i < args.Length; i++)
                {
                    var arg = args[i];
                    var str = arg.EasyName(options);

                    if (i > 0) sb.Append(str.Length > 0 ? ", " : ",");
                    sb.Append(str);
                }
                sb.Append('>');
            }
        }

        // Member arguments...
        if (options.UseMemberArguments ||
            options.UseMemberArgumentsTypes || options.UseMemberArgumentsNames)
        {
            if (!used) sb.Append(item.Name);

            sb.Append('(');
            var pars = item.GetParameters(); for (int i = 0; i < pars.Length; i++)
            {
                var par = pars[i];
                var stype = options.UseMemberArgumentsTypes ? par.ParameterType.EasyName(options) : "";
                var sname = options.UseMemberArgumentsNames ? (par.Name ?? "") : "";

                if (i > 0) sb.Append((stype.Length > 0 || sname.Length > 0) ? ", " : ",");
                if (stype.Length > 0)
                {
                    sb.Append(stype);
                    if (sname.Length > 0) sb.Append(' ');
                }
                if (sname.Length > 0) sb.Append(sname);
            }
            sb.Append(')');
        }

        // Finishing...
        return sb.ToString();
    }

    // ---------------------------------------------------

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

        // Member type and/or host type...
        if (host != null && (options.UseMemberType || options.UseMemberHost))
        {
            var str = host.EasyName(options);
            if (str.Length > 0)
            {
                if (options.UseMemberType) sb.Append($"{str} ");
                if (options.UseMemberHost) sb.Append($"{str}.");
            }
        }

        // Member name...
        var name = item.Name;
        if (name == ".ctor") name = "new";

        var used = options.UseMemberName;
        if (used) sb.Append(name);

        // Member arguments...
        if (options.UseMemberArguments ||
            options.UseMemberArgumentsTypes || options.UseMemberArgumentsNames)
        {
            if (!used) sb.Append(name);

            sb.Append('(');
            var pars = item.GetParameters(); for (int i = 0; i < pars.Length; i++)
            {
                var par = pars[i];
                var stype = options.UseMemberArgumentsTypes ? par.ParameterType.EasyName(options) : "";
                var sname = options.UseMemberArgumentsNames ? (par.Name ?? "") : "";

                if (i > 0) sb.Append((stype.Length > 0 || sname.Length > 0) ? ", " : ",");
                if (stype.Length > 0)
                {
                    sb.Append(stype);
                    if (sname.Length > 0) sb.Append(' ');
                }
                if (sname.Length > 0) sb.Append(sname);
            }
            sb.Append(')');
        }

        // Finishing...
        return sb.ToString();
    }

    // ---------------------------------------------------

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
        var pars = item.GetIndexParameters();

        // Member type or return type...
        if (options.UseMemberType)
        {
            var str = item.PropertyType.EasyName(options);
            if (str.Length > 0) sb.Append($"{str} ");
        }

        // Member host declaring type...
        if (options.UseMemberHost && host != null)
        {
            var str = host.EasyName(options);
            if (str.Length > 0) sb.Append($"{str}.");
        }

        // Member name...
        var name = pars.Length == 0 ? item.Name : "this";
        var used = options.UseMemberName;
        if (used) sb.Append(name);

        // Member arguments...
        if (pars.Length > 0 &&
            (options.UseMemberArguments ||
            options.UseMemberArgumentsTypes || options.UseMemberArgumentsNames))
        {
            if (!used) sb.Append(name);

            sb.Append('[');
            for (int i = 0; i < pars.Length; i++)
            {
                var par = pars[i];
                var stype = options.UseMemberArgumentsTypes ? par.ParameterType.EasyName(options) : "";
                var sname = options.UseMemberArgumentsNames ? (par.Name ?? "") : "";

                if (i > 0) sb.Append((stype.Length > 0 || sname.Length > 0) ? ", " : ",");
                if (stype.Length > 0)
                {
                    sb.Append(stype);
                    if (sname.Length > 0) sb.Append(' ');
                }
                if (sname.Length > 0) sb.Append(sname);
            }
            sb.Append(']');
        }

        // Finishing...
        return sb.ToString();
    }

    // ---------------------------------------------------

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

        // Member type or return type...
        if (options.UseMemberType)
        {
            var str = item.FieldType.EasyName(options);
            if (str.Length > 0) sb.Append($"{str} ");
        }

        // Member host declaring type...
        if (options.UseMemberHost && host != null)
        {
            var str = host.EasyName(options);
            if (str.Length > 0) sb.Append($"{str}.");
        }

        // Member name...
        if (options.UseMemberName) sb.Append(item.Name);

        // Finishing...
        return sb.ToString();
    }
}