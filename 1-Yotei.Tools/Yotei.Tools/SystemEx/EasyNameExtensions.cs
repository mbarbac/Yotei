using System.Transactions;

namespace Yotei.Tools;

// ========================================================
public static class EasyNameExtensions
{
    /// <summary>
    /// Returns a C#-alike name of the given type, using default options.
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    public static string EasyName(this Type item) => item.EasyName(EasyTypeOptions.Default);

    /// <summary>
    /// Returns a C#-alike name of the given type, using the given options.
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

    /// <summary>
    /// Invoked after having captured the whole set of generic arguments.
    /// </summary>
    static string EasyName(this Type item, EasyTypeOptions options, Span<Type> tpargs, ref int tpused)
    {
        var sb = new StringBuilder();
        var gen = item.FullName == null;
        var host = item.DeclaringType;

        // Namespace...
        if (options.UseNamespace && !gen && host == null)
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
            if ((options.UseHost || options.UseNamespace) && str.Length > 0) sb.Append($"{str}.");
        }

        // Name...
        var name = item.Name;
        var index = name.IndexOf('`');
        if (index >= 0) name = name.Remove(index, name.Length - index);

        sb.Append(gen && !options.UseName ? "" : name);

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
                    if (name.Length == 0) sb.Append(name); // To make sense...

                    var xoptions = options with
                    {
                        UseNamespace = options.UseTypeArguments.UseNamespace,
                        UseHost = options.UseTypeArguments.UseHost,
                        UseName = options.UseTypeArguments.UseName,
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