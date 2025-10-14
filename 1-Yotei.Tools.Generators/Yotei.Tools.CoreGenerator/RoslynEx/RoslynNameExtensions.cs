#pragma warning disable IDE0019

namespace Yotei.Tools.CoreGenerator;

// ========================================================
internal static class RoslynNameExtensions
{
    /// <summary>
    /// Returns the C#-alike name of the source type symbol, using default options.
    /// </summary>
    /// <param name="source"></param>
    /// <returns></returns>
    public static string EasyName(
        this ITypeSymbol source) => source.EasyName(EasyNameOptions.Default);

    /// <summary>
    /// Returns the C#-alike name of the source type symbol, using the given options.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="options"></param>
    /// <returns></returns>
    public static string EasyName(this ITypeSymbol source, EasyNameOptions options)
    {
        source.ThrowWhenNull();
        options.ThrowWhenNull();

        return source.IsNamespace ? source.EasyNameNamespace() : source.EasyNameType(options);
    }

    // ----------------------------------------------------

    /// <summary>
    /// Invoked when the symbol is a namespace... and when the options say so!, so we need to do
    /// this recursively. Also note that 'source' typically is a 'type'-alike instance, so the
    /// first iteration of the while loop just grabs its containing namespace, if any.
    /// </summary>
    static string EasyNameNamespace(this ITypeSymbol source)
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

    // ----------------------------------------------------

    /// <summary>
    /// Invoked when the symbol is an actual type.
    /// </summary>
    static string EasyNameType(this ITypeSymbol source, EasyNameOptions options)
    {
        var sb = new StringBuilder();
        var isgen = source.TypeKind == TypeKind.TypeParameter;
        var named = source as INamedTypeSymbol;
        var host = source.ContainingType;

        // Namespace...
        if (options.TypeUseNamespace && !isgen && host is null)
        {
            var str = EasyNameNamespace(source);
            if (str.Length > 0) sb.Append($"{str}.");
        }

        // Special case, at least one host level...
        var forced =
            host is not null &&
            host.Name == "Nullable" &&
            host.TypeArguments.Length > 0;

        // Host...
        if (!isgen && host is not null && (
            forced || options.TypeUseHost || options.TypeUseNamespace))
        {
            var str = host.EasyNameType(options);
            sb.Append($"{str}.");
        }

        // Name...
        var name = source.Name;
        var used =
            options.TypeUseNamespace || options.TypeUseHost ||
            options.TypeUseName || options.TypeUseNullable;

        if (used) sb.Append(name);

        // Generic arguments...
        if (options.TypeGenericArgumentOptions is not null)
        {
            var args = named == null ? [] : named.TypeArguments;
            if (args.Length > 0)
            {
                if (!used) // Validating it makes sense, and having an anchor for nullable '?'
                {
                    sb.Append(name.Length > 0 ? name : "$");
                    used = true;
                }

                sb.Append('<'); for (int i = 0; i < args.Length; i++)
                {
                    var arg = args[i];
                    var str = arg.EasyName(options.TypeGenericArgumentOptions);

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
    /// Returns the C#-alike name of the source method symbol, using default options.
    /// </summary>
    /// <param name="source"></param>
    /// <returns></returns>
    public static string EasyName(
        this IMethodSymbol source) => source.EasyName(EasyNameOptions.Default);

    /// <summary>
    /// Returns the C#-alike name of the source method symbol, using the given options.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="options"></param>
    /// <returns></returns>
    public static string EasyName(this IMethodSymbol source, EasyNameOptions options)
    {
        source.ThrowWhenNull();
        options.ThrowWhenNull();

        var sb = new StringBuilder();
        var host = source.ContainingType;
        var isctor = source.MethodKind == MethodKind.Constructor;

        // Return type...
        if (options.MemberReturnTypeOptions is not null && !isctor)
        {
            var type = source.ReturnType;
            var str = type.EasyName(options.MemberReturnTypeOptions);
            if (str.Length > 0) sb.Append($"{str} ");
        }

        // Host type...
        if (host is not null)
        {
            if (options.MemberHostTypeOptions is not null)
            {
                var str = host.EasyName(options.MemberHostTypeOptions);
                if (str.Length > 0) sb.Append($"{str}.");
            }
            else if (options.MemberReturnTypeOptions is not null && isctor)
            {
                var type = source.ContainingType;
                var str = type.EasyName(options.MemberReturnTypeOptions);
                if (str.Length > 0) sb.Append($"{str}.");
            }
        }

        // Name...
        var name = source.Name;
        if (isctor)
        {
            name = options.ConstructorName[0] == '.' && sb.Length > 0 && sb[^1] == '.'
                ? options.ConstructorName[1..]
                : options.ConstructorName;
        }
        sb.Append(name);

        // Generic arguments...
        if (options.MemberGenericArgumentOptions is not null && source.TypeArguments.Length > 0)
        {
            if (name.Length == 0) { name = "$"; sb.Append(name); }

            sb.Append('<'); for (int i = 0; i < source.TypeArguments.Length; i++)
            {
                var arg = source.TypeArguments[i];
                var str = arg.EasyName(options.MemberGenericArgumentOptions);

                if (i > 0) sb.Append(str.Length > 0 ? ", " : ",");
                sb.Append(str);
            }
            sb.Append('>');
        }

        // Member arguments...
        if (source.Parameters.Length > 0 && (
            options.MemberArgumentTypeOptions is not null || options.MemberUseArgumentNames))
        {
            if (name.Length == 0) { name = "$"; sb.Append(name); }

            sb.Append('('); for (int i = 0; i < source.Parameters.Length; i++)
            {
                var par = source.Parameters[i];
                var str = new StringBuilder();

                if (options.MemberArgumentTypeOptions is not null)
                {
                    var type = par.Type.EasyName(options.MemberArgumentTypeOptions);
                    if (type.Length > 0) str.Append(type);
                }
                if (options.MemberUseArgumentNames)
                {
                    if (str.Length > 0) str.Append(' ');
                    str.Append(par.Name);
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
    /// Returns the C#-alike name of the source property symbol, using default options.
    /// </summary>
    /// <param name="source"></param>
    /// <returns></returns>
    public static string EasyName(
        this IPropertySymbol source) => source.EasyName(EasyNameOptions.Default);

    /// <summary>
    /// Returns the C#-alike name of the source property symbol, using the given options.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="options"></param>
    /// <returns></returns>
    public static string EasyName(this IPropertySymbol source, EasyNameOptions options)
    {
        source.ThrowWhenNull();
        options.ThrowWhenNull();

        var sb = new StringBuilder();
        var host = source.ContainingType;

        // Return type...
        if (options.MemberReturnTypeOptions is not null)
        {
            var str = source.Type.EasyName(options.MemberReturnTypeOptions);
            if (str.Length > 0) sb.Append($"{str} ");
        }

        // Host type...
        if (options.MemberHostTypeOptions is not null && host != null)
        {
            var str = host.EasyName(options.MemberHostTypeOptions);
            if (str.Length > 0) sb.Append($"{str}.");
        }

        // Name...
        var pars = source.Parameters;
        var name = source.Name;
        if (pars.Length != 0)
        {
            // HIGH: Intercept IndexerName '$', maybe up the tree for custom attributes?
            // Something like: ISymbol.GetAttributes => AttributeData[] to investigate
            name = options.IndexerName;
        }
        sb.Append(name);

        // Member arguments...
        if (pars.Length > 0 && (
            options.MemberArgumentTypeOptions is not null || options.MemberUseArgumentNames))
        {
            if (name.Length == 0) { name = "$"; sb.Append(name); }

            sb.Append('('); for (int i = 0; i < pars.Length; i++)
            {
                var par = source.Parameters[i];
                var str = new StringBuilder();

                if (options.MemberArgumentTypeOptions is not null)
                {
                    var type = par.Type.EasyName(options.MemberArgumentTypeOptions);
                    if (type.Length > 0) str.Append(type);
                }
                if (options.MemberUseArgumentNames)
                {
                    if (str.Length > 0) str.Append(' ');
                    str.Append(par.Name);
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
    /// Returns the C#-alike name of the source field symbol, using default options.
    /// </summary>
    /// <param name="source"></param>
    /// <returns></returns>
    public static string EasyName(
        this IFieldSymbol source) => source.EasyName(EasyNameOptions.Default);

    /// <summary>
    /// Returns the C#-alike name of the source field symbol, using the given options.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="options"></param>
    /// <returns></returns>
    public static string EasyName(this IFieldSymbol source, EasyNameOptions options)
    {
        source.ThrowWhenNull();
        options.ThrowWhenNull();

        var sb = new StringBuilder();
        var host = source.ContainingType;

        // Return type...
        if (options.MemberReturnTypeOptions is not null)
        {
            var str = source.Type.EasyName(options.MemberReturnTypeOptions);
            if (str.Length > 0) sb.Append($"{str} ");
        }

        // Host type...
        if (options.MemberHostTypeOptions is not null && host != null)
        {
            var str = host.EasyName(options.MemberHostTypeOptions);
            if (str.Length > 0) sb.Append($"{str}.");
        }

        // Name...
        sb.Append(source.Name);

        // Finishing...
        return sb.ToString();
    }

    // ----------------------------------------------------

    /// <summary>
    /// Returns the C#-alike name of the source attribute data element, using default options.
    /// </summary>
    /// <param name="source"></param>
    /// <returns></returns>
    public static string EasyName(
        this AttributeData source) => source.EasyName(EasyNameOptions.Default);

    /// <summary>
    /// Returns the C#-alike name of the source attribute data element, using the given options.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="options"></param>
    /// <returns></returns>
    public static string EasyName(this AttributeData source, EasyNameOptions options)
    {
        source.ThrowWhenNull();
        options.ThrowWhenNull();

        var sb = new StringBuilder();

        // Class name...
        var name = string.Empty;
        if (options.MemberHostTypeOptions != null)
        {
            name = source.AttributeClass == null
                ? ""
                : source.AttributeClass.EasyName(options.MemberHostTypeOptions with
                {
                    TypeUseName = true,
                    TypeUseNullable = false,
                });

            sb.Append(name);
        }

        // Generic arguments...
        // HIGH: Find generic arguments of AttributeData.EasyName()
        // Something like: ISymbol.GetAttributes => AttributeData[] to investigate
        /*if (options.MemberGenericArgumentOptions is not null && source.TypeArguments.Length > 0)
        {
            if (name.Length == 0) { name = "$"; sb.Append(name); }

            sb.Append('<'); for (int i = 0; i < source.TypeArguments.Length; i++)
            {
                var arg = source.TypeArguments[i];
                var str = arg.EasyName(options.MemberGenericArgumentOptions);

                if (i > 0) sb.Append(str.Length > 0 ? ", " : ",");
                sb.Append(str);
            }
            sb.Append('>');
        }*/

        // Member arguments...
        if (options.MemberArgumentTypeOptions is not null || options.MemberUseArgumentNames)
        {
            if (name.Length == 0) { name = "$"; sb.Append(name); }
            sb.Append('(');
            var done = false;

            for (int i = 0; i < source.ConstructorArguments.Length; i++) // Only values...
            {
                var arg = source.ConstructorArguments[i];
                var opt = options with { MemberReturnTypeOptions = options.MemberArgumentTypeOptions };
                var str = arg.EasyName(opt);

                if (done) sb.Append(str.Length > 0 ? ", " : ",");
                sb.Append(str);
                done = true;
            }

            for (int i = 0; i < source.NamedArguments.Length; i++) // Names and values...
            {
                var arg = source.NamedArguments[i];
                var opt = options with { MemberReturnTypeOptions = options.MemberArgumentTypeOptions };
                var argname = options.MemberUseArgumentNames ? arg.Key : null;
                var str = arg.Value.EasyName(opt, argname);

                if (done) sb.Append(str.Length > 0 ? ", " : ",");
                sb.Append(str);
                done = true;
            }

            sb.Append(')');
        }

        // Finishing...
        return sb.ToString();
    }

    // ----------------------------------------------------

    /// <summary>
    /// Returns the C#-alike name of the source typed constant element, using default options,
    /// and the given optional name if not null.
    /// </summary>
    /// <param name="source"></param>
    /// <returns></returns>
    public static string EasyName(
        this TypedConstant source, string? name = null)
        => source.EasyName(EasyNameOptions.Default, name);

    /// <summary>
    /// Returns the C#-alike name of the source typed constant element, using the given options,
    /// and the given optional name if not null.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="options"></param>
    /// <param name="name"></param>
    /// <returns></returns>
    public static string EasyName(
        this TypedConstant source, EasyNameOptions options, string? name = null)
    {
        source.ThrowWhenNull();
        options.ThrowWhenNull();

        var sb = new StringBuilder();

        // The name if requested...
        if (name != null)
        {
            sb.Append(name);
            sb.Append(": ");
        }

        // The 'return type' of the constant...
        if (options.MemberReturnTypeOptions != null && source.Type is not null)
        {
            sb.Append('(');
            var str = source.Type.EasyName(options); sb.Append(str);
            if (source.Kind == TypedConstantKind.Array) sb.Append("[]");
            sb.Append(") ");
        }

        // The actual value...
        if (source.IsNull) sb.Append("null");
        else
        {
            if (source.Kind == TypedConstantKind.Array)
            {
                sb.Append('['); for (int i = 0; i < source.Values.Length; i++)
                {
                    if (i > 0) sb.Append(", ");

                    var str = source.Values[i].EasyName(options, null);
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

    // ----------------------------------------------------

    /// <summary>
    /// Invoked to obtain the C#-alike representation of the given value, using the given optios,
    /// when such is needed for a typed constant.
    /// </summary>
    static string ObjectValue(object? value, EasyNameOptions options) => value switch
    {
        ITypeSymbol item => item.EasyName(options),
        IMethodSymbol item => item.EasyName(options),
        IPropertySymbol item => item.EasyName(options),
        IFieldSymbol item => item.EasyName(options),

        Type item => item.EasyName(options),

        null => "null",
        _ => $"{value}"
    };
}