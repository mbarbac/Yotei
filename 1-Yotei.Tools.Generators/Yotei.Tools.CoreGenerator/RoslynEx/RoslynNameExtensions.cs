namespace Yotei.Tools.CoreGenerator;

// ========================================================
/// <summary>
/// Maintains the '<c>RoslynName(...)</c>' family of methods, which return a C#-alike name of
/// the given roslyn element.
/// </summary>
internal static class RoslynNameExtensions
{
    /// <summary>
    /// Returns the C#-alike name of this element, using default options.
    /// </summary>
    /// <param name="source"></param>
    /// <returns></returns>
    public static string RoslynName(
        this ITypeSymbol source) => source.RoslynName(RoslynNameOptions.Default);

    /// <summary>
    /// Returns the C#-alike name of this element, using the given options.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="options"></param>
    /// <returns></returns>
    public static string RoslynName(this ITypeSymbol source, RoslynNameOptions options)
    {
        source.ThrowWhenNull();
        options.ThrowWhenNull();

        return source.IsNamespace ? source.RoslynNameSpace() : source.RoslynNameType(options);
    }

    /// <summary>
    /// Invoked when the symbol is a namespace.
    /// </summary>
    /// <param name="source"></param>
    /// <returns></returns>
    static string RoslynNameSpace(this ITypeSymbol source)
    {
        List<string> names = [];

        ISymbol? node = source;
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
    /// Invoked when the symbol is an actual type.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="options"></param>
    /// <returns></returns>
    [SuppressMessage("", "IDE0019")]
    static string RoslynNameType(this ITypeSymbol source, RoslynNameOptions options)
    {
        var sb = new StringBuilder();
        var gen = source.TypeKind == TypeKind.TypeParameter;
        var named = source as INamedTypeSymbol;

        // Namespace and host...
        if (!gen && (options.TypeUseNamespace || options.TypeUseHost))
        {
            var xoptions = options with // Prevents re-entrance and not using nullable...
            {
                TypeUseNamespace = false,
                TypeUseHost = false,
                TypeUseNullable = false,
            };
            List<string> names = [];
            ISymbol? node = source;

            while ((node = node.ContainingSymbol) != null)
            {
                if (node is INamespaceSymbol ns && options.TypeUseNamespace)
                {
                    if (ns.Name.Length > 0) names.Add(ns.Name);
                }
                else if (node is ITypeSymbol tp)
                {
                    var str = tp.RoslynName(xoptions);
                    if (str.Length > 0) names.Add(str);
                }
            }

            if (names.Count > 0)
            {
                names.Reverse();
                sb.Append(string.Join(".", names));
                sb.Append('.');
            }
        }

        // Name...
        var name = source.Name;
        var used =
            options.TypeUseNamespace || options.TypeUseHost ||
            options.TypeUseName || options.TypeUseNullable;

        if (used) sb.Append(name);

        // Generic arguments...
        if (options.TypeGenericArgumentsOptions != null)
        {
            var args = named != null ? named.TypeArguments : [];
            if (args.Length > 0)
            {
                if (!used) // To make syntax sense...
                {
                    sb.Append(name.Length > 0 ? name : "$");
                    used = true;
                }

                sb.Append('<');
                for (int i = 0; i < args.Length; i++)
                {
                    var arg = args[i];
                    var str = arg.RoslynName(options.TypeGenericArgumentsOptions);

                    if (i > 0) sb.Append(str.Length > 0 ? ", " : ",");
                    sb.Append(str);
                }
                sb.Append('>');
            }
        }

        // Nullable annotation...
        if (options.TypeUseNullable)
        {
            bool mark =
                source.NullableAnnotation == NullableAnnotation.Annotated &&
                source.Name != "Nullable";

            if (mark)
            {
                if (!used) sb.Append(name.Length > 0 ? name : "$");
                sb.Append('?');
            }
        }

        // Finishing...
        return sb.ToString();
    }

    // ----------------------------------------------------

    /// <summary>
    /// Returns the C#-alike name of this element, using default options.
    /// </summary>
    /// <param name="source"></param>
    /// <returns></returns>
    public static string RoslynName(
        this IMethodSymbol source) => source.RoslynName(RoslynNameOptions.Default);

    /// <summary>
    /// Returns the C#-alike name of this element, using the given options.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="options"></param>
    /// <returns></returns>
    public static string RoslynName(this IMethodSymbol source, RoslynNameOptions options)
    {
        source.ThrowWhenNull();
        options.ThrowWhenNull();

        var sb = new StringBuilder();

        // Return type...
        if (options.MemberReturnTypeOptions != null)
        {
            var type = source.MethodKind == MethodKind.Constructor
                ? source.ContainingType
                : source.ReturnType;

            var str = type.RoslynName(options.MemberReturnTypeOptions);
            if (str.Length > 0) sb.Append($"{str} ");
        }

        // Host type...
        if (options.MemberHostTypeOptions != null)
        {
            var host = source.ContainingType;
            if (host != null)
            {
                var str = host.RoslynName(options.MemberHostTypeOptions);
                if (str.Length > 0) sb.Append($"{str}.");
            }
        }

        // Name...
        var name = source.Name switch
        {
            ".ctor" => "new",
            ".cctor" => "new",
            _ => source.Name
        };
        sb.Append(name);

        // Generic arguments...
        if (options.MemberGenericArgumentsOptions != null && source.TypeArguments.Length > 0)
        {
            if (name.Length == 0) { name = "$"; sb.Append(name); }

            sb.Append('<'); for (int i = 0; i < source.TypeArguments.Length; i++)
            {
                var arg = source.TypeArguments[i];
                var str = arg.RoslynName(options.MemberGenericArgumentsOptions);

                if (i > 0) sb.Append(str.Length > 0 ? ", " : ",");
                sb.Append(str);
            }
            sb.Append('>');
        }

        // Member arguments...
        if (options.MemberUseArguments ||
            options.MemberArgumentTypesOptions != null || options.MemberArgumentsNames)
        {
            if (name.Length == 0) { name = "$"; sb.Append(name); }

            sb.Append('('); for (int i = 0; i < source.Parameters.Length; i++)
            {
                var str = new StringBuilder();
                if (options.MemberArgumentTypesOptions != null || options.MemberArgumentsNames)
                {
                    var par = source.Parameters[i];
                    var pname = options.MemberArgumentsNames ? par.Name : "";
                    var ptype = options.MemberArgumentTypesOptions == null ? ""
                        : par.Type.RoslynName(options.MemberArgumentTypesOptions);

                    if (ptype.Length > 0)
                    {
                        sb.Append(ptype);
                        if (pname.Length > 0) sb.Append(' ');
                    }
                    sb.Append(pname);
                }

                if (i > 0) sb.Append(str.Length > 0 ? ", " : ",");
                if (str.Length > 0) sb.Append(str);
            }
            sb.Append(')');
        }

        // Finishing...
        return sb.ToString();
    }

    // ----------------------------------------------------

    /// <summary>
    /// Returns the C#-alike name of this element, using default options.
    /// </summary>
    /// <param name="source"></param>
    /// <returns></returns>
    public static string RoslynName(
        this IPropertySymbol source) => source.RoslynName(RoslynNameOptions.Default);

    /// <summary>
    /// Returns the C#-alike name of this element, using the given options.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="options"></param>
    /// <returns></returns>
    public static string RoslynName(this IPropertySymbol source, RoslynNameOptions options)
    {
        source.ThrowWhenNull();
        options.ThrowWhenNull();

        var sb = new StringBuilder();

        // Return type...
        if (options.MemberReturnTypeOptions != null)
        {
            var str = source.Type.RoslynName(options.MemberReturnTypeOptions);
            if (str.Length > 0) sb.Append($"{str} ");
        }

        // Host type...
        if (options.MemberHostTypeOptions != null)
        {
            var host = source.ContainingType;
            if (host != null)
            {
                var str = host.RoslynName(options.MemberHostTypeOptions);
                if (str.Length > 0) sb.Append($"{str}.");
            }
        }

        // Name...
        var name = source.Parameters.Length == 0 ? source.Name : "this";
        sb.Append(name);

        // Member arguments...
        if (source.Parameters.Length > 0 && (
            options.MemberUseArguments ||
            options.MemberArgumentTypesOptions != null || options.MemberArgumentsNames))
        {
            if (name.Length == 0) { name = "$"; sb.Append(name); }

            sb.Append('['); for (int i = 0; i < source.Parameters.Length; i++)
            {
                var str = new StringBuilder();
                if (options.MemberArgumentTypesOptions != null || options.MemberArgumentsNames)
                {
                    var par = source.Parameters[i];
                    var pname = options.MemberArgumentsNames ? par.Name : "";
                    var ptype = options.MemberArgumentTypesOptions == null ? ""
                        : par.Type.RoslynName(options.MemberArgumentTypesOptions);

                    if (ptype.Length > 0)
                    {
                        sb.Append(ptype);
                        if (pname.Length > 0) sb.Append(' ');
                    }
                    sb.Append(pname);
                }

                if (i > 0) sb.Append(str.Length > 0 ? ", " : ",");
                if (str.Length > 0) sb.Append(str);
            }
            sb.Append(']');
        }

        // Finishing...
        return sb.ToString();
    }

    // ----------------------------------------------------

    /// <summary>
    /// Returns the C#-alike name of this element, using default options.
    /// </summary>
    /// <param name="source"></param>
    /// <returns></returns>
    public static string RoslynName(
        this IFieldSymbol source) => source.RoslynName(RoslynNameOptions.Default);

    /// <summary>
    /// Returns the C#-alike name of this element, using the given options.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="options"></param>
    /// <returns></returns>
    public static string RoslynName(this IFieldSymbol source, RoslynNameOptions options)
    {
        source.ThrowWhenNull();
        options.ThrowWhenNull();

        var sb = new StringBuilder();

        // Return type...
        if (options.MemberReturnTypeOptions != null)
        {
            var str = source.Type.RoslynName(options.MemberReturnTypeOptions);
            if (str.Length > 0) sb.Append($"{str} ");
        }

        // Host type...
        if (options.MemberHostTypeOptions != null)
        {
            var host = source.ContainingType;
            if (host != null)
            {
                var str = host.RoslynName(options.MemberHostTypeOptions);
                if (str.Length > 0) sb.Append($"{str}.");
            }
        }

        // Name...
        sb.Append(source.Name);

        // Finishing...
        return sb.ToString();
    }

    // ----------------------------------------------------

    /// <summary>
    /// Returns the C#-alike name of this element, using default options.
    /// </summary>
    /// <param name="source"></param>
    /// <returns></returns>
    public static string RoslynName(
        this AttributeData source) => source.RoslynName(RoslynNameOptions.Default);

    /// <summary>
    /// Returns the C#-alike name of this element, using the given options.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="options"></param>
    /// <returns></returns>
    public static string RoslynName(this AttributeData source, RoslynNameOptions options)
    {
        source.ThrowWhenNull();
        options.ThrowWhenNull();

        var sb = new StringBuilder();

        // Class name...
        if (options.MemberHostTypeOptions != null)
        {
            var name = source.AttributeClass == null
                ? ""
                : source.AttributeClass.RoslynName(options.MemberHostTypeOptions with
                {
                    TypeUseName = true,
                    TypeUseNullable = false,
                });

            sb.Append(name);
        }

        // Generic arguments...
        if (options.MemberGenericArgumentsOptions != null)
        {
            // HIGH: find generic arguments of AttributeData.RoslynName()
            // Something like: source.AttributeConstructor.TypeArguments, etc...
        }

        // Attribute constructor arguments...
        if (options.MemberUseArguments ||
            options.MemberArgumentTypesOptions != null || options.MemberArgumentsNames)
        {
            var done = false;

            sb.Append('(');
            for (int i = 0; i < source.ConstructorArguments.Length; i++) // Only values...
            {
                var arg = source.ConstructorArguments[i];
                var opt = options.MemberArgumentTypesOptions ?? RoslynNameOptions.Empty;
                var str = arg.RoslynName(opt);

                if (done) sb.Append(str.Length == 0 ? "," : ", ");
                sb.Append(str);
            }
            for (int i = 0; i < source.NamedArguments.Length; i++) // Name: value...
            {
                var arg = source.NamedArguments[i];
                var opt = options.MemberArgumentTypesOptions ?? RoslynNameOptions.Empty;
                var str = arg.Value.RoslynName(opt, options.MemberArgumentsNames ? arg.Key : null);

                if (done) sb.Append(str.Length == 0 ? "," : ", ");
                sb.Append(str);
            }
            sb.Append(')');
        }

        // Finishing...
        return sb.ToString();
    }

    // ----------------------------------------------------

    /// <summary>
    /// Returns the C#-alike name of this element, using default options, and the given optional
    /// name, if not null.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="name"></param>
    /// <returns></returns>
    public static string RoslynName(this TypedConstant source, string? name = null)
    {
        return source.RoslynName(RoslynNameOptions.Default, name);
    }

    /// <summary>
    /// Returns the C#-alike name of this element, using the given options, and the given optional
    /// name, if not null.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="options"></param>
    /// <param name="name"></param>
    /// <returns></returns>
    public static string RoslynName(
        this TypedConstant source, RoslynNameOptions options, string? name = null)
    {
        source.ThrowWhenNull();
        options.ThrowWhenNull();

        var sb = new StringBuilder();

        if (name != null)
        {
            sb.Append(name);
            sb.Append(": ");
        }

        if (options.MemberReturnTypeOptions != null && source.Type is not null)
        {
            sb.Append('(');
            var str = source.Type.RoslynName(options); sb.Append(str);
            if (source.Kind == TypedConstantKind.Array) sb.Append("[]");
            sb.Append(") ");
        }

        if (source.IsNull) sb.Append("null");
        else
        {
            if (source.Kind == TypedConstantKind.Array)
            {
                sb.Append('['); for (int i = 0; i < source.Values.Length; i++)
                {
                    if (i > 0) sb.Append(", ");

                    var str = source.Values[i].RoslynName(options, null);
                    sb.Append(str);
                }
                sb.Append(']');
            }
            else
            {
                var str = ObjectValue(source.Value, options);
                sb.Append(str);
            }
        }

        // Finishing...
        return sb.ToString();
    }

    /// <summary>
    /// Invoked to obtain the string representation of the given value-
    /// </summary>
    static string ObjectValue(object? value, RoslynNameOptions options) => value switch
    {
        ITypeSymbol item => item.RoslynName(options),
        IMethodSymbol item => item.RoslynName(options),
        IPropertySymbol item => item.RoslynName(options),
        IFieldSymbol item => item.RoslynName(options),

        Type item => item.EasyName(options.ToEasyNameOptions()),

        null => "null",
        _ => $"{value}"
    };
}