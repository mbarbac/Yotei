﻿#pragma warning disable IDE0019

namespace Yotei.Tools.Generators.Internal;

// ========================================================
internal static partial class EasyNameExtensions
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

        return item.IsNamespace ? item.EasyNamespace() : item.EasyType(options);
    }

    /// <summary>
    /// Invoked when the type symbol IS a namespace.
    /// </summary>
    static string EasyNamespace(this ITypeSymbol item)
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
    /// Invoked when the type symbol IS NOT a namespace.
    /// </summary>
    static string EasyType(this ITypeSymbol item, EasyNameOptions options)
    {
        var sb = new StringBuilder();
        var gen = item.TypeKind == TypeKind.TypeParameter;
        var named = item as INamedTypeSymbol;

        // Namespace and Host...
        if ((options.UseTypeNamespace || options.UseTypeHost) && !gen)
        {
            List<string> names = [];
            ISymbol? node = item;

            var xoptions = options with // Prevents re-entrance and not using nullable...
            {
                UseTypeNamespace = false,
                UseTypeHost = false,
                UseTypeNullable = false,
            };

            while ((node = node.ContainingSymbol) != null)
            {
                if (node is INamespaceSymbol ns && options.UseTypeNamespace)
                {
                    if (ns.Name.Length > 0) names.Add(ns.Name);
                }
                else if (node is ITypeSymbol tp)
                {
                    var str = tp.EasyName(xoptions);
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
        var name = item.Name;
        var used =
            options.UseTypeName || options.UseTypeNullable ||
            options.UseTypeHost || options.UseTypeNamespace;

        if (used) sb.Append(name);

        // Type arguments...
        var args = named != null ? named.TypeArguments : ImmutableArray<ITypeSymbol>.Empty;
        if (args.Length > 0 && options.UseTypeArguments != null)
        {
            if (!used) { sb.Append(name.Length > 0 ? name : "$"); used = true; } // To make sense...

            var xoptions = options with
            {
                UseTypeNamespace = options.UseTypeArguments.UseTypeNamespace,
                UseTypeHost = options.UseTypeArguments.UseTypeHost,
                UseTypeName = options.UseTypeArguments.UseTypeName,
                UseTypeNullable = options.UseTypeArguments.UseTypeNullable,
            };

            sb.Append('<');
            for (int i = 0; i < args.Length; i++)
            {
                var arg = args[i];
                var str = arg.EasyName(xoptions);

                if (i > 0) sb.Append(str.Length > 0 ? ", " : ",");
                sb.Append(str);
            }
            sb.Append('>');
        }

        // Nullable requested...
        bool nullable =
            options.UseTypeNullable &&
            item.NullableAnnotation == NullableAnnotation.Annotated &&
            item.Name != "Nullable";

        if (nullable)
        {
            if (!used) sb.Append(name.Length > 0 ? name : "$");
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

        // Return type...
        if (options.UseMemberType != null)
        {
            var type = item.MethodKind == MethodKind.Constructor
                ? item.ContainingType
                : item.ReturnType;

            var str = type.EasyName(options.UseMemberType);
            if (str.Length > 0) sb.Append($"{str} ");
        }

        // Member host...
        if (options.UseMemberHost != null)
        {
            var host = item.ContainingType;
            if (host != null)
            {
                var str = host.EasyName(options.UseMemberHost);
                if (str.Length > 0) sb.Append($"{str}.");
            }
        }

        // Name...
        var name = item.Name switch
        {
            ".ctor" => "new",
            ".cctor" => "new",
            _ => item.Name
        };
        sb.Append(name);

        // Type arguments...
        if (options.UseMemberTypeArguments != null && item.TypeArguments.Length > 0)
        {
            if (name.Length == 0) { name = "$"; sb.Append(name); };

            var xoptions = options.UseMemberTypeArguments with
            {
                UseTypeNullable = false,
            };

            sb.Append('<');
            for (int i = 0; i < item.TypeArguments.Length; i++)
            {
                var arg = item.TypeArguments[i];
                var str = arg.EasyName(xoptions);

                if (i > 0) sb.Append(str.Length > 0 ? ", " : ",");
                sb.Append(str);
            }
            sb.Append('>');
        }

        // Member arguments...
        if (options.UseMemberArguments || options.UseMemberArgumentsTypes != null ||
            options.UseMemberArgumentsNames)
        {
            if (name.Length == 0) { name = "$"; sb.Append(name); };

            sb.Append('(');
            var pars = item.Parameters; for (int i = 0; i < pars.Length; i++)
            {
                var par = pars[i];
                var ptype = options.UseMemberArgumentsTypes != null ? par.Type.EasyName(options.UseMemberArgumentsTypes) : "";
                var pname = options.UseMemberArgumentsNames ? (par.Name ?? "") : "";

                var len = ptype.Length + pname.Length;
                if (i > 0) sb.Append(len > 0 ? ", " : ",");

                if (ptype.Length > 0)
                {
                    sb.Append(ptype);
                    if (pname.Length > 0) sb.Append(' ');
                }
                if (pname.Length > 0) sb.Append(pname);
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

        // Return type...
        if (options.UseMemberType != null)
        {
            var str = item.Type.EasyName(options.UseMemberType);
            if (str.Length > 0) sb.Append($"{str} ");
        }

        // Member host...
        if (options.UseMemberHost != null)
        {
            var host = item.ContainingType;
            if (host != null)
            {
                var str = host.EasyName(options.UseMemberHost);
                if (str.Length > 0) sb.Append($"{str}.");
            }
        }

        // Name...
        var name = item.Parameters.Length == 0 ? item.Name : "this";
        sb.Append(name);

        // Member arguments...
        if (item.Parameters.Length > 0 && (
            options.UseMemberArguments || options.UseMemberArgumentsTypes != null ||
            options.UseMemberArgumentsNames))
        {
            if (name.Length == 0) { name = "$"; sb.Append(name); };

            sb.Append('[');
            var pars = item.Parameters; for (int i = 0; i < pars.Length; i++)
            {
                var par = pars[i];
                var ptype = options.UseMemberArgumentsTypes != null ? par.Type.EasyName(options.UseMemberArgumentsTypes) : "";
                var pname = options.UseMemberArgumentsNames ? (par.Name ?? "") : "";

                var len = ptype.Length + pname.Length;
                if (i > 0) sb.Append(len > 0 ? ", " : ",");

                if (ptype.Length > 0)
                {
                    sb.Append(ptype);
                    if (pname.Length > 0) sb.Append(' ');
                }
                if (pname.Length > 0) sb.Append(pname);
            }
            sb.Append(']');
        }

        // Finishing...
        return sb.ToString();
    }

    // ----------------------------------------------------

    /// <summary>
    /// Returns a C#-alike name of the given field, using default options.
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    public static string EasyName(this IFieldSymbol item) => item.EasyName(EasyNameOptions.Default);

    /// <summary>
    /// Returns a C#-alike name of the given field, using the given options.
    /// </summary>
    /// <param name="item"></param>
    /// <param name="options"></param>
    /// <returns></returns>
    public static string EasyName(this IFieldSymbol item, EasyNameOptions options)
    {
        item.ThrowWhenNull();
        options.ThrowWhenNull();

        var sb = new StringBuilder();

        // Return type...
        if (options.UseMemberType != null)
        {
            var str = item.Type.EasyName(options.UseMemberType);
            if (str.Length > 0) sb.Append($"{str} ");
        }

        // Member host...
        if (options.UseMemberHost != null)
        {
            var host = item.ContainingType;
            if (host != null)
            {
                var str = host.EasyName(options.UseMemberHost);
                if (str.Length > 0) sb.Append($"{str}.");
            }
        }

        // Name...
        sb.Append(item.Name);

        // Finishing...
        return sb.ToString();
    }

    // ----------------------------------------------------

    /// <summary>
    /// Returns a C#-alike representation of the given attribute instance, using default options.
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    public static string EasyName(this AttributeData item) => item.EasyName(EasyNameOptions.Default);

    /// <summary>
    /// Returns a C#-alike representation of the given attribute instance, using the given options.
    /// </summary>
    /// <param name="item"></param>
    /// <param name="options"></param>
    /// <returns></returns>
    public static string EasyName(this AttributeData item, EasyNameOptions options)
    {
        item.ThrowWhenNull();
        options.ThrowWhenNull();

        var sb = new StringBuilder();

        // Attribute class name (including its type attributes, if any)...
        if (options.UseMemberHost != null)
        {
            var xoptions = options.UseMemberHost with { UseTypeName = true, UseTypeNullable = false };
            var name = item.AttributeClass == null ? "" : item.AttributeClass.EasyName(xoptions);
            sb.Append(name);
        }

        // Attribute arguments...
        if (options.UseMemberTypeArguments != null && (
            item.ConstructorArguments.Length != 0 || item.NamedArguments.Length != 0))
        {
            var done = false;
            sb.Append('(');

            foreach (var arg in item.ConstructorArguments)
            {
                if (done) sb.Append(", ");
                done = true;

                var temp = arg.EasyName(options, null);
                sb.Append(temp);
            }

            foreach (var named in item.NamedArguments)
            {
                if (done) sb.Append(", ");
                done = true;

                var temp = named.Value.EasyName(options, named.Key);
                sb.Append(temp);
            }

            sb.Append(')');
        }

        // Finishing...
        return sb.ToString();
    }

    // ----------------------------------------------------

    /// <summary>
    /// Returns a C#-alike name of the constant, using default options, and the optional name.
    /// <br/> Constants typically are constructor arguments or named ones.
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    public static string EasyName(
        this TypedConstant item, string? name = null) => item.EasyName(EasyNameOptions.Default, name);

    /// <summary>
    /// Returns a C#-alike name of the constant, using given options, and the optional name.
    /// <br/> Constants typically are constructor arguments or named ones.
    /// </summary>
    /// <param name="item"></param>
    /// <param name="options"></param>
    /// <returns></returns>
    public static string EasyName(
        this TypedConstant item, EasyNameOptions options, string? name = null)
    {
        item.ThrowWhenNull();
        options.ThrowWhenNull();

        var sb = new StringBuilder();
        string str;

        if (options.UseMemberType != null && item.Type != null)
        {
            if (item.Type != null)
            {
                sb.Append('(');
                str = item.Type.EasyName(options); sb.Append(str);
                if (item.Kind == TypedConstantKind.Array) sb.Append("[]");
                sb.Append(')');
                sb.Append(' ');
            }
        }

        if (name != null)
        {
            sb.Append(name);
            sb.Append(": ");
        }

        if (item.IsNull) sb.Append("null");
        else
        {
            if (item.Kind == TypedConstantKind.Array)
            {
                sb.Append('['); for (int i = 0; i < item.Values.Length; i++)
                {
                    if (i > 0) sb.Append(", ");

                    var value = item.Values[i];
                    str = value.EasyName(options, null);
                    sb.Append(str);
                }
                sb.Append(']');
            }
            else
            {
                str = ObjectValue(item.Value, options);
                sb.Append(str);
            }
        }

        // Finishing...
        return sb.ToString();

        /// <summary>
        /// Invoked to obtain the value representation.
        /// </summary>
        static string ObjectValue(object? value, EasyNameOptions options) => value switch
        {
            ITypeSymbol item => item.EasyName(options),
            IMethodSymbol item => item.EasyName(options),
            IPropertySymbol item => item.EasyName(options),
            IFieldSymbol item => item.EasyName(options),

            Type type => type.EasyName(options),

            null => "null",
            _ => $"{value}"
        };
    }
}