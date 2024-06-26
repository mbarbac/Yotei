namespace Yotei.Tools;

// ========================================================
public static class EasyNameExtensions
{
    /// <summary>
    /// Returns the C#-alike name of the given type, using the default settings.
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    public static string EasyName(this Type type) => type.EasyName(EasyTypeOptions.Default);

    /// <summary>
    /// Returns the C#-alike name of the given type, using the given arguments.
    /// </summary>
    /// <param name="item"></param>
    /// <param name="options"></param>
    /// <returns></returns>
    public static string EasyName(this Type item, EasyTypeOptions options)
    {
        item.ThrowWhenNull();
        options.ThrowWhenNull();

        var tpargs = item.GetGenericArguments().AsSpan();
        var tpused = 0;
        return item.EasyName(options, tpargs, ref tpused);
    }
    static string EasyName(this Type item, EasyTypeOptions options, Span<Type> tpargs, ref int tpused)
    {
        item.ThrowWhenNull();
        options.ThrowWhenNull();

        var sb = new StringBuilder();
        var isgen = item.FullName == null;
        var ns = item.Namespace;
        var host = item.DeclaringType;

        // Namespace...
        if (ns != null && ns.Length > 0 && host == null)
        {
            // Namespace requested...
            if (!isgen && options.UseNamespace) sb.Append($"{ns}.");
        }

        // Host type...
        if (!isgen && host != null)
        {
            // Consuming type arguments as needed...
            var s = host.EasyName(options, tpargs, ref tpused);            
            if (s.Length > 0 && !isgen)
            {
                // Declaring host requested...
                if (options.UseTypeHost || options.UseNamespace) sb.Append($"{s}.");
            }
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
                tpused += num;

                if (options.UseArguments || options.UseArgumentNames ||
                    options.UseArgumentsHosts || options.UseArgumentsNamespaces)
                {
                    if (!options.UseTypeName && !isgen) sb.Append(name);

                    var xoptions = options with // Order matters!
                    {
                        UseNamespace = options.UseArgumentsNamespaces,
                        UseTypeHost = options.UseArgumentsHosts,
                        UseTypeName = options.UseArgumentNames,
                    };

                    tpargs = tpargs[..num];
                    sb.Append('<');
                    for (int i = 0; i < num; i++)
                    {
                        var temp = tpargs[i];
                        var s = temp.EasyName(xoptions);

                        if (i > 0) sb.Append(s.Length > 0 ? ", " : ",");
                        sb.Append(s);
                    }
                    sb.Append('>');
                }
            }
        }

        // Finishing...
        return sb.ToString();
    }
}