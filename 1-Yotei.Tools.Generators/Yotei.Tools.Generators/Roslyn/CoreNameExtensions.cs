namespace Yotei.Tools.Generators.Internal;

// ========================================================
internal static class CoreNameExtensions
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

        return item.IsNamespace ? item.EasyNamespace(options) : item.EasyType(options);
    }

    /// <summary>
    /// Invoked when the type symbol is a namespace.
    /// </summary>
    static string EasyNamespace(this ITypeSymbol item, EasyNameOptions options)
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
    static string EasyType(this ITypeSymbol item, EasyNameOptions options)
    {
        var sb = new StringBuilder();
        var gen = item.TypeKind == TypeKind.TypeParameter;
        var named = item as INamedTypeSymbol;

        // Namespace or type host requested...
        if (!gen && (options.UseTypeNamespace || options.UseTypeHost))
        {
            List<string> names = [];
            ISymbol? node = item;

            var xoptions = options;
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

            var xoptions = options;
            sb.Append('<');
            for (int i = 0; i < args.Length; i++)
            {
                var arg = args[i];
                var temp = arg.EasyType(xoptions);

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
}
/*

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
}*/