namespace Yotei.Tools;

// ========================================================
public static class EasyNameExtensions
{
    /// <summary>
    /// Returns the C#-alike name of the given type, using the default options.
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    public static string EasyName(this Type item) => item.EasyName(EasyNameOptions.Default);

    /// <summary>
    /// Returns the C#-alike name of the given type, using the given options.
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
    /// Invoked after captured the whole set of generic arguments
    /// </summary>
    static string EasyName(this Type item, EasyNameOptions options, Span<Type> tpargs, ref int tpused)
    {
        var sb = new StringBuilder();
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
            // Consuming host generic arguments...
            var str = host.EasyName(options, tpargs, ref tpused);

            // Using host if requested...
            if ((options.UseTypeHost || options.UseTypeNamespace) &&
                str.Length > 0)
                sb.Append($"{str}.");
        }

        // Name...
        var name = item.Name;
        var index = name.IndexOf('`');
        if (index >= 0) name = name.Remove(index, name.Length - index);

        var used = options.UseTypeName || options.UseTypeHost || options.UseTypeNamespace;
        if (used) sb.Append(name);

        // Generic arguments...
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
                if (options.UseTypeGenericArguments != null)
                {
                    if (!used) sb.Append(name.Length > 0 ? name : "$"); // To make sense...

                    var xoptions = options with
                    {
                        UseTypeNamespace = options.UseTypeGenericArguments.UseTypeNamespace,
                        UseTypeHost = options.UseTypeGenericArguments.UseTypeHost,
                        UseTypeName = options.UseTypeGenericArguments.UseTypeName,
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
}
