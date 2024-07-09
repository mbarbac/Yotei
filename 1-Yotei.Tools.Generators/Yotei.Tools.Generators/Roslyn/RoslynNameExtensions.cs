namespace Yotei.Tools.Generators.Internal;

// ========================================================
internal static class RoslynNameExtensions
{
    /// <summary>
    /// Returns a C#-alike name of the given type, using default options.
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    public static string EasyName(this ITypeSymbol item) => item.EasyName(EasyNameOptions.Default);

    /// <summary>
    /// Returns a C#-alike name of the given type, using the given options.
    /// </summary>
    /// <param name="item"></param>
    /// <param name="options"></param>
    /// <returns></returns>
    public static string EasyName(this ITypeSymbol item, EasyNameOptions options)
    {
        item.ThrowWhenNull();
        options.ThrowWhenNull();

        return item.IsNamespace ? EasyNameNamespace(item, options) : EasyNameType(item, options);
    }

    /// <summary>
    /// Invoked when the type symbol is a namespace.
    /// </summary>
    static string EasyNameNamespace(ITypeSymbol item, EasyNameOptions options)
    {
        List<string> names = [];
        ISymbol? node = item;

        while (node != null)
        {
            if (node is INamespaceSymbol ns)
            {
                if (ns.Name.Length > 0) names.Add(ns.Name);
            }
            node = node.ContainingSymbol;
        }

        if (names.Count > 0)
        {
            names.Reverse();
            return string.Join(".", names);
        }

        return string.Empty;
    }

    /// <summary>
    /// Invoked when the type symbol is NOT a namespace.
    /// </summary>
    static string EasyNameType(ITypeSymbol item, EasyNameOptions options)
    {
        var sb = new StringBuilder();
        var gen = item.TypeKind == TypeKind.TypeParameter;
        var named = item as INamedTypeSymbol;

        // Namespace or type host requested...
        if (!gen && (options.UseTypeNamespace || options.UseTypeHost))
        {
            List<string> names = [];
            ISymbol? node = item;

            var xoptions = options with
            { AddNullable = false, UseTypeNamespace = false, UseTypeHost = false, UseTypeName = true };

            while ((node = node.ContainingSymbol) != null)
            {
                if (node is INamespaceSymbol ns && options.UseTypeName)
                {
                    if (ns.Name.Length > 0) names.Add(ns.Name);
                }
                if (node is ITypeSymbol tp && options.UseTypeHost)
                {
                    var temp = tp.EasyName(xoptions);
                    if (temp.Length > 0) names.Add(temp);
                }
            }

            if (names.Count > 0)
            {
                names.Reverse();
                sb.Append(string.Join(".", names));
                sb.Append('.');
            }
        }

        // The type name...
        var name = item.Name;
        var used = options.UseTypeName;

        if (used) sb.Append(name);

        // Type arguments...
        var args = named != null ? named.TypeArguments : ImmutableArray<ITypeSymbol>.Empty;
        if (args.Length > 0 && options.UseTypeArguments)
        {
            if (!used) { sb.Append(name.Length == 0 ? "$" : name); used = true; }

            var xoptions = options with { AddNullable = options.AddNullableToTypeArguments };

            sb.Append('<');
            for (int i = 0; i < args.Length; i++)
            {
                var arg = args[i];
                var temp = arg.EasyName(xoptions);

                if (i > 0) sb.Append(temp.Length > 0 ? ", " : ",");
                sb.Append(temp);
            }
            sb.Append('>');
        }

        // Nullable requested...
        bool nullable =
            options.AddNullable &&
            item.NullableAnnotation == NullableAnnotation.Annotated &&
            item.Name != "Nullable";

        if (nullable)
        {
            if (!used) sb.Append(name.Length == 0 ? "$" : name);
            sb.Append('?');
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
    public static string EasyName(this IMethodSymbol item) => item.EasyName(EasyNameOptions.Default);

    /// <summary>
    /// Returns a C#-alike name of the given method, using the given options.
    /// </summary>
    /// <param name="item"></param>
    /// <param name="options"></param>
    /// <returns></returns>
    public static string EasyName(this IMethodSymbol item, EasyNameOptions options)
    {
        item.ThrowWhenNull();
        options.ThrowWhenNull();

        var sb = new StringBuilder();

        // Return type requested...
        if (options.UseMemberType)
        {
            var str = item.ReturnType.EasyName(options);
            if (str != null) sb.Append($"{str} ");
        }

        // Declaring host requested...
        if (options.UseMemberHost)
        {
            var host = item.ContainingType;
            if (host != null)
            {
                var str = host.EasyName(options);
                if (str != null) sb.Append($"{str}.");
            }
        }

        // Name...
        var used = options.UseMemberName;
        if (used) sb.Append(item.Name);

        var xname = item.Name.Length > 0 ? item.Name : "$";

        // Type arguments...
        if (options.UseMemberTypeArguments && item.TypeArguments.Length > 0)
        {
            if (!used) { sb.Append(xname); used = true; }

            var xoptions = options with
            { AddNullable = false, AddNullableToTypeArguments = false };

            sb.Append('<'); for (int i = 0; i < item.TypeArguments.Length; i++)
            {
                var arg = item.TypeArguments[i];
                var str = arg.EasyName(options);

                if (i > 0) sb.Append(str.Length > 0 ? ", " : ",");
                sb.Append(str);
            }
            sb.Append('>');
        }

        // Arguments...
        if (options.UseMemberArguments ||
            options.UseMemberArgumentsTypes || options.UseMemberArgumentsNames)
        {
            if (!used) sb.Append(xname);

            sb.Append('('); for (int i = 0; i < item.Parameters.Length; i++)
            {
                var par = item.Parameters[i];
                var ptype = options.UseMemberArgumentsTypes ? par.Type.EasyName(options) : "";
                var pname = options.UseMemberArgumentsNames ? par.Name : "";

                var len = ptype.Length + pname.Length;
                if (i > 0) sb.Append(len > 0 ? ", " : ",");

                if (ptype.Length > 0)
                {
                    sb.Append(ptype);
                    if (pname.Length > 0) sb.Append(' ');
                }
                if (pname.Length > 0)
                {
                    sb.Append(pname);
                }
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
    public static string EasyName(this IPropertySymbol item) => item.EasyName(EasyNameOptions.Default);

    /// <summary>
    /// Returns a C#-alike name of the given property, using the given options.
    /// </summary>
    /// <param name="item"></param>
    /// <param name="options"></param>
    /// <returns></returns>
    public static string EasyName(this IPropertySymbol item, EasyNameOptions options)
    {
        item.ThrowWhenNull();
        options.ThrowWhenNull();

        var sb = new StringBuilder();

        // Return type requested...
        if (options.UseMemberType)
        {
            var str = item.Type.EasyName(options);
            if (str != null) sb.Append($"{str} ");
        }

        // Declaring host requested...
        if (options.UseMemberHost)
        {
            var host = item.ContainingType;
            if (host != null)
            {
                var str = host.EasyName(options);
                if (str != null) sb.Append($"{str}.");
            }
        }

        // Name...
        var name = item.Parameters.Length == 0 ? item.Name : "this";
        if (name.Length == 0) name = "$";

        var used = options.UseMemberName;
        if (used) sb.Append(name);

        // Arguments...
        if (item.Parameters.Length > 0 &&
            (options.UseMemberArguments ||
            options.UseMemberArgumentsTypes || options.UseMemberArgumentsNames))
        {
            if (!used) sb.Append(name);

            sb.Append('['); for (int i = 0; i < item.Parameters.Length; i++)
            {
                var par = item.Parameters[i];
                var ptype = options.UseMemberArgumentsTypes ? par.Type.EasyName(options) : "";
                var pname = options.UseMemberArgumentsNames ? par.Name : "";

                var len = ptype.Length + pname.Length;
                if (i > 0) sb.Append(len > 0 ? ", " : ",");

                if (ptype.Length > 0)
                {
                    sb.Append(ptype);
                    if (pname.Length > 0) sb.Append(' ');
                }
                if (pname.Length > 0)
                {
                    sb.Append(pname);
                }
            }
            sb.Append(']');
        }

        // Finishing...
        return sb.ToString();
    }
}