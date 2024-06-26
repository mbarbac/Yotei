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

        var tpargs = item.GetGenericArguments();
        var tpused = 0;
        return item.EasyName(options, tpargs, ref tpused);
    }
    static string EasyName(this Type item, EasyTypeOptions options, Type[] tpargs, ref int tpused)
    {
        item.ThrowWhenNull();
        options.ThrowWhenNull();

        var sb = new StringBuilder();
        var isgen = item.FullName == null; // || item.IsGenericTypeParameter || item.IsGenericMethodParameter

        // Namespace requested...
        if (!isgen && options.UseNamespace)
        {
            var s = item.Namespace;
            if (s != null && s.Length > 0) sb.Append($"{s}.");
        }

        // Declaring host requested...
        var host = item.DeclaringType;
        if (!isgen && host != null && options.UseHostType)
        {
            var xoptions = options with { UseNamespace = false };

            var s = host.EasyName(xoptions, tpargs, ref tpused);
            if (s != null && s.Length > 0) sb.Append($"{s}.");
        }

        // Type name...
        var name = item.Name;
        if (!options.UseTypeName) name = string.Empty;

        var index = name.IndexOf('`');
        if (index >= 0) name = name.Remove(index, name.Length - index);
        sb.Append(name);

        // Type arguments...
        if (tpargs.Length > 0)
        {
            var hargs = host == null ? [] : host.GetGenericArguments();
            var args = item.GetGenericArguments().AsSpan(0, hargs.Length);
            if (args.Length > 0)
            {
                var temps = tpargs.AsSpan(tpused, args.Length);
                if (temps.Length > 0)
                {
                    var xoptions = options with // Order of assignation matters!
                    {
                        UseNamespace = isgen && options.UseTypeArgumentNamespaces,
                        UseHostType = isgen && options.UseTypeArgumentHostTypes,
                        UseTypeName = options.UseTypeName
                    };

                    sb.Append('<'); for (int i = 0; i < temps.Length; i++)
                    {
                        var temp = temps[i];
                        var s = temp.EasyName(xoptions);

                        if (i > 0) sb.Append(s.Length > 0 ? ", " : ",");
                        sb.Append(s);
                    }
                    sb.Append('>');
                }
            }

            // Consuming this level type arguments...
            tpused += args.Length;
        }

        // Finishing...
        return sb.ToString();
    }
}