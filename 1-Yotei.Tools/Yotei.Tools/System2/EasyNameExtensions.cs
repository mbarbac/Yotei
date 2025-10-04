namespace Yotei.Tools;

// ========================================================
public static class EasyNameExtensions
{
    /// <summary>
    /// Returns the C#-alike name of the given element, using default options.
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    public static string EasyName(this Type item) => item.EasyName(EasyNameOptions.Default);

    /// <summary>
    /// Returns the C#-alike name of the given element, using the given options.
    /// </summary>
    /// <param name="item"></param>
    /// <param name="options"></param>
    /// <returns></returns>
    public static string EasyName(this Type item, EasyNameOptions options)
    {
        item.ThrowWhenNull();
        options.ThrowWhenNull();

        var under = item.UnderlyingSystemType;

        var types = item.GetGenericArguments().AsSpan();
        var usedtypes = 0;
        return item.EasyName(options, types, ref usedtypes);
    }

    /// <summary>
    /// Invoke after the whole set of generic arguments has been captured.
    /// </summary>
    static string EasyName(
        this Type item, EasyNameOptions options, Span<Type> types, ref int usedtypes)
    {
        var sb = StringBuilder.Pool.Rent();
        var isgen = item.FullName == null;
        var host = item.DeclaringType;

        // Namespace...
        if (options.UseTypeNamespace && !isgen && host == null)
        {
            var ns = item.Namespace;
            if (ns != null && ns.Length > 0) sb.Append($"{ns}.");
        }

        // Declaring host...
        if (!isgen && host != null)
        {
            // Always consuming host generic arguments...
            var str = host.EasyName(options, types, ref usedtypes);

            // Using host if requested or needed...
            if ((options.UseTypeHost || options.UseTypeNamespace) &&
                str.Length > 0)
                sb.Append($"{str}.");
        }

        // Name...
        var name = string.Empty;
        if (options.UseTypeName || options.UseTypeHost || options.UseTypeNamespace)
        {
            name = item.Name;
            var index = name.IndexOf('`');
            if (index >= 0) name = name.Remove(index, name.Length - index);
            sb.Append(name);
        }

        // Generic arguments...
        types = types[usedtypes..]; if (types.Length > 0)
        {
            var hold = host == null ? [] : host.GetGenericArguments();
            var args = item.GetGenericArguments();
            var num = args.Length - hold.Length;

            if (num > 0)
            {
                // Consuming arguments...
                usedtypes += num;

                // Using arguments, if requested...
                if (options.UseTypeGenericArguments != null)
                {
                    if (name.Length == 0) sb.Append('$'); // To make sense...

                    var xoptions = options with
                    {
                        UseTypeNamespace = options.UseTypeGenericArguments.UseTypeNamespace,
                        UseTypeHost = options.UseTypeGenericArguments.UseTypeHost,
                        UseTypeName = options.UseTypeGenericArguments.UseTypeName,
                    };

                    types = types[..num];

                    sb.Append('<');
                    for (int i = 0; i < types.Length; i++)
                    {
                        var arg = types[i];
                        var str = arg.EasyName(xoptions);

                        if (i > 0) sb.Append(str.Length > 0 ? ", " : ",");
                        sb.Append(str);
                    }
                    sb.Append('>');
                }
            }
        }

        // Finishing...
        return StringBuilder.Pool.Return(sb);
    }

    // ----------------------------------------------------

    /// <summary>
    /// Returns the C#-alike name of the given element, using default options.
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    public static string EasyName(this MethodInfo item) => item.EasyName(EasyNameOptions.Default);

    /// <summary>
    /// Returns the C#-alike name of the given element, using the given options.
    /// </summary>
    /// <param name="item"></param>
    /// <param name="options"></param>
    /// <returns></returns>
    public static string EasyName(this MethodInfo item, EasyNameOptions options)
    {
        item.ThrowWhenNull();
        options.ThrowWhenNull();

        var sb = StringBuilder.Pool.Rent();
        var host = item.DeclaringType;

        // Return type...
        if (options.UseMemberReturnType != null)
        {
            var str = item.ReturnType.EasyName(options.UseMemberReturnType);
            if (str.Length > 0) sb.Append($"{str} ");
        }

        // Member host...
        if (options.UseMemberHostType != null && host != null)
        {
            var str = host.EasyName(options.UseMemberHostType);
            if (str.Length > 0) sb.Append($"{str}.");
        }

        // Name...
        if (options.UseMemberName) sb.Append(item.Name);

        // Generic arguments...
        if (options.UseMemberGenericArguments != null)
        {
            var args = item.GetGenericArguments();

            if (args.Length > 0)
            {
                sb.Append('<');
                for (int i = 0; i < args.Length; i++)
                {
                    var arg = args[i];
                    var str = arg.EasyName(options.UseMemberGenericArguments);

                    if (i > 0) sb.Append(str.Length > 0 ? ", " : ",");
                    sb.Append(str);
                }
                sb.Append('>');
            }
        }

        // Member arguments...
        if (options.UseMemberArguments ||
            options.UseMemberArgumentNames || options.UseMemberArgumentTypes != null)
        {
            sb.Append('(');

            var pars = item.GetParameters();
            for (int i = 0; i < pars.Length; i++)
            {
                var par = pars[i];
                var str = StringBuilder.Pool.Rent();

                if (options.UseMemberArgumentTypes != null)
                {
                    var ptype = par.ParameterType.EasyName(options.UseMemberArgumentTypes);
                    if (ptype.Length > 0)
                    {
                        str.Append(ptype);
                        if (options.UseMemberArgumentNames) str.Append(' ');
                    }
                }
                if (options.UseMemberArgumentNames) str.Append(par.Name);

                if (i > 0) sb.Append(str.Length > 0 ? ", " : ",");
                sb.Append(StringBuilder.Pool.Return(str));
            }

            sb.Append(')');
        }

        // Finishing...
        return StringBuilder.Pool.Return(sb);
    }

    // ----------------------------------------------------

    /// <summary>
    /// Returns the C#-alike name of the given element, using default options.
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    public static string EasyName(this ConstructorInfo item) => item.EasyName(EasyNameOptions.Default);

    /// <summary>
    /// Returns the C#-alike name of the given element, using the given options.
    /// </summary>
    /// <param name="item"></param>
    /// <param name="options"></param>
    /// <returns></returns>
    public static string EasyName(this ConstructorInfo item, EasyNameOptions options)
    {
        item.ThrowWhenNull();
        options.ThrowWhenNull();

        var sb = StringBuilder.Pool.Rent();
        var host = item.DeclaringType;

        // Member host...
        if (options.UseMemberHostType != null && host != null)
        {
            var str = host.EasyName(options.UseMemberHostType);
            if (str.Length > 0) sb.Append($"{str}.");
        }

        // Name...
        if (options.UseMemberName) sb.Append("new");

        // Member arguments...
        if (options.UseMemberArguments || options.UseMemberName ||
            options.UseMemberArgumentNames || options.UseMemberArgumentTypes != null)
        {
            sb.Append('(');

            var pars = item.GetParameters();
            for (int i = 0; i < pars.Length; i++)
            {
                var par = pars[i];
                var str = StringBuilder.Pool.Rent();

                if (options.UseMemberArgumentTypes != null)
                {
                    var ptype = par.ParameterType.EasyName(options.UseMemberArgumentTypes);
                    if (ptype.Length > 0)
                    {
                        str.Append(ptype);
                        if (options.UseMemberArgumentNames) str.Append(' ');
                    }
                }
                if (options.UseMemberArgumentNames) str.Append(par.Name);

                if (i > 0) sb.Append(str.Length > 0 ? ", " : ",");
                sb.Append(StringBuilder.Pool.Return(str));
            }

            sb.Append(')');
        }

        // Finishing...
        return StringBuilder.Pool.Return(sb);
    }

    // ----------------------------------------------------

    /// <summary>
    /// Returns the C#-alike name of the given element, using default options.
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    public static string EasyName(this PropertyInfo item) => item.EasyName(EasyNameOptions.Default);

    /// <summary>
    /// Returns the C#-alike name of the given element, using the given options.
    /// </summary>
    /// <param name="item"></param>
    /// <param name="options"></param>
    /// <returns></returns>
    public static string EasyName(this PropertyInfo item, EasyNameOptions options)
    {
        item.ThrowWhenNull();
        options.ThrowWhenNull();

        var sb = StringBuilder.Pool.Rent();
        var host = item.DeclaringType;

        // Return type...
        if (options.UseMemberReturnType != null)
        {
            var str = item.PropertyType.EasyName(options.UseMemberReturnType);
            if (str.Length > 0) sb.Append($"{str} ");
        }

        // Member host...
        if (options.UseMemberHostType != null && host != null)
        {
            var str = host.EasyName(options.UseMemberHostType);
            if (str.Length > 0) sb.Append($"{str}.");
        }

        // Name...
        var pars = item.GetIndexParameters();
        if (options.UseMemberName)
        {
            var name = pars.Length == 0 ? item.Name : "this";
            sb.Append(name);
        }

        // Member arguments...
        if (options.UseMemberArguments ||
            options.UseMemberArgumentNames || options.UseMemberArgumentTypes != null)
        {
            sb.Append('[');

            for (int i = 0; i < pars.Length; i++)
            {
                var par = pars[i];
                var str = StringBuilder.Pool.Rent();

                if (options.UseMemberArgumentTypes != null)
                {
                    var ptype = par.ParameterType.EasyName(options.UseMemberArgumentTypes);
                    if (ptype.Length > 0)
                    {
                        str.Append(ptype);
                        if (options.UseMemberArgumentNames) str.Append(' ');
                    }
                }
                if (options.UseMemberArgumentNames) str.Append(par.Name);

                if (i > 0) sb.Append(str.Length > 0 ? ", " : ",");
                sb.Append(StringBuilder.Pool.Return(str));
            }

            sb.Append(']');
        }
        
        // Finishing...
        return StringBuilder.Pool.Return(sb);
    }

    // ----------------------------------------------------

    /// <summary>
    /// Returns the C#-alike name of the given element, using default options.
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    public static string EasyName(this FieldInfo item) => item.EasyName(EasyNameOptions.Default);

    /// <summary>
    /// Returns the C#-alike name of the given element, using the given options.
    /// </summary>
    /// <param name="item"></param>
    /// <param name="options"></param>
    /// <returns></returns>
    public static string EasyName(this FieldInfo item, EasyNameOptions options)
    {
        item.ThrowWhenNull();
        options.ThrowWhenNull();

        var sb = StringBuilder.Pool.Rent();
        var host = item.DeclaringType;

        // Return type...
        if (options.UseMemberReturnType != null)
        {
            var str = item.FieldType.EasyName(options.UseMemberReturnType);
            if (str.Length > 0) sb.Append($"{str} ");
        }

        // Member host...
        if (options.UseMemberHostType != null && host != null)
        {
            var str = host.EasyName(options.UseMemberHostType);
            if (str.Length > 0) sb.Append($"{str}.");
        }

        // Name...
        if (options.UseMemberName) sb.Append(item.Name);

        // Finishing...
        return StringBuilder.Pool.Return(sb);
    }
}