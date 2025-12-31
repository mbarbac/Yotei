namespace Yotei.Tools.CoreGenerator;

// ========================================================
internal static class EasyNameSymbolExtensions
{
    /// <summary>
    /// Returns the C#-alike name of the given element, using default options.
    /// <br/> This method accepts either namespaces or named type symbols.
    /// <para>
    /// </summary>
    /// <param name="source"></param>
    /// <returns></returns>
    public static string EasyName(
        this ITypeSymbol source) => source.EasyName(EasyNameOptions.Default);

    /// <summary>
    /// Returns the C#-alike name of the given element, using the given options.
    /// <br/> This method accepts either namespaces or named type symbols.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="options"></param>
    /// <returns></returns>
    public static string EasyName(this ITypeSymbol source, EasyNameOptions options)
    {
        source.ThrowWhenNull();
        options.ThrowWhenNull();

        return source.IsNamespace
            ? ((INamespaceSymbol)source).EasyNameSpace(options)
            : source.EasyNameType(options);
    }

    // ----------------------------------------------------

    /// <summary>
    /// Invoked when the symbol represents a namespace.
    /// </summary>
    static string EasyNameSpace(this INamespaceSymbol source, EasyNameOptions options)
    {
        List<string> names = [];

        ISymbol? node = source;
        while (node != null)
        {
            if (node is INamespaceSymbol ns &&
                ns.Name != null &&
                ns.Name.Length > 0) names.Add(ns.Name);

            if (!options.TypeUseNamespace) break; // At least done once!
            node = node.ContainingSymbol;
        }

        if (names.Count == 0) return string.Empty;
        names.Reverse();
        return string.Join(".", names);
    }

    // ----------------------------------------------------

    /// <summary>
    /// Invoked when the symbol represents an actual type.
    /// </summary>
    [SuppressMessage("", "IDE0019")]
    static string EasyNameType(this ITypeSymbol source, EasyNameOptions options)
    {
        var isgen = source.TypeKind == TypeKind.TypeParameter;
        var host = source.ContainingType;
        var named = source as INamedTypeSymbol;
        var args = named is null ? [] : named.TypeArguments;

        // Shortcut hide name...
        var hide = options.TypeHideName;
        if (options.TypeUseNamespace || options.TypeUseHost ||
            (args.Length > 0 && options.TypeArgumentsOptions is not null))
            hide = false;

        if (hide) return string.Empty;

        // Shortcut (some) nullable types...
        if (options.TypeUseNullability)
        {
            if (args.Length == 1 && (source.Name == "Nullable" || source.Name == "IsNullable"))
            {
                var str = args[0].EasyName(options);
                if (!str.EndsWith('?')) str += '?';
                return str;
            }
        }

        // Other cases...
        var sb = new StringBuilder();

        // Namespace...
        if (options.TypeUseNamespace && !isgen && host is null)
        {
            var ns = source.ContainingNamespace;
            var str = ns.EasyNameSpace(options);
            if (str is not null && str.Length > 0) sb.Append($"{str}.");
        }

        // Host...
        if ((options.TypeUseHost || options.TypeUseNamespace) &&
            !isgen &&
            host is not null)
        {
            var str = host.EasyNameType(options);
            sb.Append($"{str}.");
        }

        // Name...
        var name = source.Name;
        sb.Append(name);

        // Generic arguments...
        if (args.Length > 0 && options.TypeArgumentsOptions is not null)
        {
            var xoptions = options.TypeArgumentsOptions;
            if (xoptions.TypeArgumentsOptions is null)
                xoptions = xoptions with { TypeArgumentsOptions = xoptions };

            sb.Append('<'); for (int i = 0; i < args.Length; i++)
            {
                var arg = args[i];
                var str = arg.EasyName(xoptions);
                if (i > 0) sb.Append(str.Length > 0 ? ", " : ",");
                sb.Append(str);
            }
            sb.Append('>');
        }

        // Nullable annotations (when not intecepted before)...
        if (options.TypeUseNullability &&
            source.NullableAnnotation == NullableAnnotation.Annotated) sb.Append('?');

        // Finishing...
        return sb.ToString();
    }

    // ----------------------------------------------------

    /// <summary>
    /// Returns the C#-alike name of the given element, using default options.
    /// <para>
    /// </summary>
    /// <param name="source"></param>
    /// <returns></returns>
    public static string EasyName(
        this IParameterSymbol source) => source.EasyName(EasyNameOptions.Default);

    /// <summary>
    /// Returns the C#-alike name of the given element, using the given options.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="options"></param>
    /// <returns></returns>
    public static string EasyName(this IParameterSymbol source, EasyNameOptions options)
    {
        var sb = new StringBuilder();

        if (options.MemberUseArgumentTypes)
        {
            var str = source.Type.EasyName(options);
            if (str.Length > 0)
            {
                if (options.MemberModifiers)
                {
                    var prefix = source.RefKind switch
                    {
                        RefKind.Ref => "ref",
                        RefKind.Out => "out",
                        RefKind.In => "in",
                        RefKind.RefReadOnlyParameter => "ref readonly",
                        _ => null
                    };
                    if (prefix is not null) sb.Append($"{prefix} ");
                }
                sb.Append(str);
            }

            if (options.TypeUseNullability && sb[^1] != '?')
            {
                var need = source.NullableAnnotation == NullableAnnotation.Annotated;
                if (need) sb.Append('?');
            }
        }

        if (options.MemberUseArgumentNames)
        {
            if (sb.Length > 0) sb.Append(' ');
            sb.Append(source.Name);
        }

        return sb.ToString();
    }

    // ----------------------------------------------------

    /// <summary>
    /// Returns the C#-alike name of the given element, using default options.
    /// <para>
    /// </summary>
    /// <param name="source"></param>
    /// <returns></returns>
    public static string EasyName(
        this IMethodSymbol source) => source.EasyName(EasyNameOptions.Default);

    /// <summary>
    /// Returns the C#-alike name of the given element, using the given options.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="options"></param>
    /// <returns></returns>
    public static string EasyName(this IMethodSymbol source, EasyNameOptions options)
    {
        source.ThrowWhenNull();
        options.ThrowWhenNull();

        var isctor = source.MethodKind is MethodKind.Constructor or MethodKind.StaticConstructor;
        var host = source.ContainingType;
        var sb = new StringBuilder();

        // Return type...
        if (options.MemberReturnTypeOptions is not null && !isctor)
        {
            var str = source.ReturnType.EasyName(options.MemberReturnTypeOptions);
            if (str.Length > 0)
            {
                if (options.MemberModifiers)
                {
                    var prefix = source.RefKind switch
                    {
                        RefKind.Ref => "ref",
                        RefKind.Out => "out",
                        RefKind.RefReadOnly => "ref readonly",
                        _ => null
                    };
                    if (prefix is not null) sb.Append($"{prefix} ");
                }
                sb.Append($"{str} ");
            }
        }

        // Host...
        if (options.MemberHostTypeOptions is not null && host is not null)
        {
            var str = host.EasyName(options.MemberHostTypeOptions);
            if (str.Length > 0) sb.Append($"{str}.");
        }

        // Name...
        if (!isctor) sb.Append(source.Name);
        else
        {
            // The default name can either be '.ctor' or '..ctor' for static ones.
            if (source.MethodKind is MethodKind.Constructor)
            {
                sb.Append(options.ConstructorTechName ? source.Name : "new");
            }
            else
            {
                if (options.ConstructorTechName) sb.Append(source.Name);
                else
                {
                    if (options.MemberModifiers) sb.Insert(0, "static ");
                    var name = source.ContainingType.EasyName(options);
                    sb.Append(name);
                }
            }
        }

        // Generic arguments...
        if (options.MemberGenericArgumentsOptions is not null)
        {
            var args = source.TypeArguments;
            if (args.Length > 0)
            {
                var xoptions = options.MemberGenericArgumentsOptions;
                if (xoptions.MemberGenericArgumentsOptions is null)
                    xoptions = xoptions with { MemberGenericArgumentsOptions = xoptions };

                sb.Append('<'); for (int i = 0; i < args.Length; i++)
                {
                    var arg = args[i];
                    var str = arg.EasyName(xoptions);
                    if (i > 0) sb.Append(str.Length > 0 ? ", " : ",");
                    sb.Append(str);
                }
                sb.Append('>');
            }
        }

        // Member arguments...
        if (options.MemberUseArgumentTypes || options.MemberUseArgumentNames)
        {
            var pars = source.Parameters;
            if (pars.Length > 0)
            {
                sb.Append('('); for (int i = 0; i < pars.Length; i++)
                {
                    var par  = pars[i];
                    var str = par.EasyName(options);
                    if (i > 0) sb.Append(str.Length > 0 ? ", " : ",");
                    sb.Append(str);                    
                }
                sb.Append(')');
            }
        }

        // Finishing...
        return sb.ToString();
    }

    // ----------------------------------------------------

    /// <summary>
    /// Returns the C#-alike name of the given element, using default options.
    /// <para>
    /// </summary>
    /// <param name="source"></param>
    /// <returns></returns>
    public static string EasyName(
        this IPropertySymbol source) => source.EasyName(EasyNameOptions.Default);

    /// <summary>
    /// Returns the C#-alike name of the given element, using the given options.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="options"></param>
    /// <returns></returns>
    public static string EasyName(this IPropertySymbol source, EasyNameOptions options)
    {
        source.ThrowWhenNull();
        options.ThrowWhenNull();

        var host = source.ContainingType;
        var sb = new StringBuilder();

        // Return type...
        if (options.MemberReturnTypeOptions is not null)
        {
            var str = source.Type.EasyName(options.MemberReturnTypeOptions);
            if (str.Length > 0)
            {
                if (options.MemberModifiers)
                {
                    var prefix = source.RefKind switch
                    {
                        RefKind.Ref => "ref",
                        RefKind.Out => "out",
                        RefKind.RefReadOnly => "ref readonly",
                        _ => null
                    };
                    if (prefix is not null) sb.Append($"{prefix} ");
                }
                sb.Append($"{str} ");
            }
        }

        // Host...
        if (options.MemberHostTypeOptions is not null && host is not null)
        {
            var str = host.EasyName(options.MemberHostTypeOptions);
            if (str.Length > 0) sb.Append($"{str}.");
        }

        // Name...
        var name = source.Name;
        var pars = source.Parameters;
        if (pars.Length > 0)
        {
            if (options.IndexerTechName) name = source.MetadataName;
            else
            {
                var index = name.IndexOf('[');
                if (index >= 0) name = name[..index];
            }
        }
        sb.Append(name);

        // Member arguments...
        if (options.MemberUseArgumentTypes || options.MemberUseArgumentNames)
        {
            if (pars.Length > 0)
            {
                sb.Append('['); for (int i = 0; i < pars.Length; i++)
                {
                    var par = pars[i];
                    var str = par.EasyName(options);
                    if (i > 0) sb.Append(str.Length > 0 ? ", " : ",");
                    sb.Append(str);
                }
                sb.Append(']');
            }
        }

        // Finishing...
        return sb.ToString();
    }

    // ----------------------------------------------------

    /// <summary>
    /// Returns the C#-alike name of the given element, using default options.
    /// <para>
    /// </summary>
    /// <param name="source"></param>
    /// <returns></returns>
    public static string EasyName(
        this IFieldSymbol source) => source.EasyName(EasyNameOptions.Default);

    /// <summary>
    /// Returns the C#-alike name of the given element, using the given options.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="options"></param>
    /// <returns></returns>
    public static string EasyName(this IFieldSymbol source, EasyNameOptions options)
    {
        // TODO: field EasyName...
        // Recordar usar custom modifiers...
        throw null;
    }

    // ----------------------------------------------------

    /// <summary>
    /// Returns the C#-alike name of the given element, using default options.
    /// <para>
    /// </summary>
    /// <param name="source"></param>
    /// <returns></returns>
    public static string EasyName(
        this AttributeData source) => source.EasyName(EasyNameOptions.Default);

    /// <summary>
    /// Returns the C#-alike name of the given element, using the given options.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="options"></param>
    /// <returns></returns>
    public static string EasyName(this AttributeData source, EasyNameOptions options)
    {
        // TODO: field EasyName...
        // Recordar usar custom modifiers...
        throw null;
    }

    // ----------------------------------------------------

    /// <summary>
    /// Returns the C#-alike name of the given element, using default options.
    /// <para>
    /// </summary>
    /// <param name="source"></param>
    /// <returns></returns>
    public static string EasyName(
        this TypedConstant source) => source.EasyName(EasyNameOptions.Default);

    /// <summary>
    /// Returns the C#-alike name of the given element, using the given options.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="options"></param>
    /// <returns></returns>
    public static string EasyName(this TypedConstant source, EasyNameOptions options)
    {
        // TODO: field EasyName...
        // Recordar usar custom modifiers...
        throw null;
    }
}