using Microsoft.CodeAnalysis.Diagnostics;
using System.Data;

namespace Yotei.Tools.Generators.Internal;

// ========================================================
internal static class EasyNameExtensions
{
    /// <summary>
    /// Returns the C#-alike name of the given element, with default options.
    /// </summary>
    /// <param name="symbol"></param>
    /// <returns></returns>
    public static string EasyName(
        this ITypeSymbol symbol) => symbol.EasyName(EasyNameOptions.Default);

    /// <summary>
    /// Returns the C#-alike name of the given element, with the given options.
    /// </summary>
    /// <param name="symbol"></param>
    /// <param name="options"></param>
    /// <returns></returns>
    [SuppressMessage("", "IDE0019")]
    public static string EasyName(this ITypeSymbol symbol, EasyNameOptions options)
    {
        symbol.ThrowWhenNull();
        options.ThrowWhenNull();
        if (symbol.IsNamespace) throw new ArgumentException("Symbol is namespace.").WithData(symbol);

        var sb = new StringBuilder();
        var named = symbol as INamedTypeSymbol;

        if (options.TypeFullName && symbol is not ITypeParameterSymbol)
        {
            List<string> names = [];
            string? name;

            ISymbol? node = symbol.ContainingSymbol;
            while (node != null)
            {
                switch (node)
                {
                    case INamespaceSymbol item:
                        name = item.Name.NullWhenEmpty();
                        if (name != null) names.Add(name);
                        break;

                    case ITypeSymbol item:
                        name = item.EasyName(options with
                        {
                            TypeFullName = false,
                            TypeNullable = false
                        });
                        names.Add(name);
                        break;
                }
                node = node.ContainingSymbol;
            }
            if (names.Count > 0)
            {
                names.Reverse();
                sb.Append(string.Join(".", names));
                sb.Append('.');
            }
        }

        sb.Append(symbol.Name);

        if (options.TypeGenerics && named != null && named.TypeParameters.Length > 0)
        {
            sb.Append('<');
            var temp = options with { TypeNullable = options.TypeNullableGenerics };
            
            for (int i = 0; i < named.TypeParameters.Length; i++)
            {
                if (i != 0) sb.Append(", ");

                var item = named.TypeArguments[i];
                var name = item.EasyName(temp);
                sb.Append(name);
            }
            sb.Append('>');
        }

        var add =
            options.TypeNullable &&
            symbol.NullableAnnotation == NullableAnnotation.Annotated &&
            symbol.Name != "Nullable";

        if (add) sb.Append('?');

        return sb.ToString();
    }

    // ----------------------------------------------------

    /// <summary>
    /// Returns the C#-alike name of the given element, with default options.
    /// </summary>
    /// <param name="symbol"></param>
    /// <returns></returns>
    public static string EasyName(
        this IPropertySymbol symbol) => symbol.EasyName(EasyNameOptions.Default);

    /// <summary>
    /// Returns the C#-alike name of the given element, with the given options.
    /// </summary>
    /// <param name="symbol"></param>
    /// <param name="options"></param>
    /// <returns></returns>
    public static string EasyName(this IPropertySymbol symbol, EasyNameOptions options)
    {
        var sb = new StringBuilder();

        if (options.MemberType)
        {
            var type = symbol.Type;
            var name = type.EasyName(options);
            sb.Append(name);
            sb.Append(' ');
        }

        if (options.MemberHostType)
        {
            var type = symbol.ContainingType;
            var name = type.EasyName(options with { TypeNullable = false });
            sb.Append(name);
            sb.Append('.');
        }

        sb.Append(symbol.Name);

        if (options.MemberArguments && symbol.IsIndexer)
        {
            sb.Append('['); for (int i = 0; i < symbol.Parameters.Length; i++)
            {
                if (i != 0) sb.Append(", ");

                var item = symbol.Parameters[i];
                var itemName = item.Name;
                var itemType = item.Type.EasyName(options);

                sb.Append($"{itemType} {itemName}");
            }
            sb.Append(']');
        }

        return sb.ToString();
    }

    // ----------------------------------------------------

    /// <summary>
    /// Returns the C#-alike name of the given element, with default options.
    /// </summary>
    /// <param name="symbol"></param>
    /// <returns></returns>
    public static string EasyName(
        this IFieldSymbol symbol) => symbol.EasyName(EasyNameOptions.Default);

    /// <summary>
    /// Returns the C#-alike name of the given element, with the given options.
    /// </summary>
    /// <param name="symbol"></param>
    /// <param name="options"></param>
    /// <returns></returns>
    public static string EasyName(this IFieldSymbol symbol, EasyNameOptions options)
    {
        var sb = new StringBuilder();

        if (options.MemberType)
        {
            var type = symbol.Type;
            var name = type.EasyName(options);
            sb.Append(name);
            sb.Append(' ');
        }

        if (options.MemberHostType)
        {
            var type = symbol.ContainingType;
            var name = type.EasyName(options with { TypeNullable = false });
            sb.Append(name);
            sb.Append('.');
        }

        sb.Append(symbol.Name);

        return sb.ToString();
    }

    // ----------------------------------------------------

    /// <summary>
    /// Returns the C#-alike name of the given element, with default options.
    /// </summary>
    /// <param name="symbol"></param>
    /// <returns></returns>
    public static string EasyName(
        this IMethodSymbol symbol) => symbol.EasyName(EasyNameOptions.Default);

    /// <summary>
    /// Returns the C#-alike name of the given element, with the given options.
    /// </summary>
    /// <param name="symbol"></param>
    /// <param name="options"></param>
    /// <returns></returns>
    public static string EasyName(this IMethodSymbol symbol, EasyNameOptions options)
    {
        var sb = new StringBuilder();

        if (options.MemberType)
        {
            var type = symbol.ReturnType;
            var name = type.EasyName(options);
            sb.Append(name);
            sb.Append(' ');
        }

        if (options.MemberHostType)
        {
            var type = symbol.ContainingType;
            var name = type.EasyName(options with { TypeNullable = false });
            sb.Append(name);
            sb.Append('.');
        }

        sb.Append(symbol.Name);

        if (options.TypeGenerics && symbol.TypeParameters.Length > 0)
        {
            sb.Append('<'); for (int i = 0; i < symbol.TypeArguments.Length; i++)
            {
                if (i != 0) sb.Append(", ");

                var temp = symbol.TypeArguments[i];
                var name = temp.EasyName(options);
                sb.Append(name);
            }
            sb.Append('>');
        }

        if (options.MemberArguments)
        {
            sb.Append('('); for (int i = 0; i < symbol.Parameters.Length; i++)
            {
                if (i > 0) sb.Append(", ");

                var item = symbol.Parameters[i];
                var itemName = item.Name;
                var itemType = item.Type.EasyName(options);

                sb.Append($"{itemType} {itemName}");
            }
            sb.Append(')');
        }

        return sb.ToString();
    }
}