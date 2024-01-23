using Microsoft.CodeAnalysis.Diagnostics;

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
    public static string EasyName(this ITypeSymbol symbol, EasyNameOptions options)
    {
        symbol.ThrowWhenNull();
        options.ThrowWhenNull();
        if (symbol.IsNamespace) throw new ArgumentException("Symbol is namespace.").WithData(symbol);

        var sb = new StringBuilder();

        if (options.UseFullTypeName)
        {
            List<string> names = [];
            ISymbol? node = symbol;

            while (node != null)
            {
                switch (node)
                {
                    case INamespaceSymbol item: names.Add(item.Name); break;
                    case ITypeSymbol item:
                        names.Add(item.EasyName(options with
                        {
                            UseNullableAnnotation = false,
                            UseFullTypeName = false,
                        }));
                        break;
                }
                node = node.ContainingSymbol;
            }
            names.Reverse();
            sb.Append(string.Join(".", names));
            sb.Append('.');
        }

        sb.Append(symbol.Name);

        if (options.UseTypeParameters &&
            symbol is INamedTypeSymbol named &&
            named.TypeParameters.Length > 0)
        {
            sb.Append('<'); for (int i = 0; i < named.TypeParameters.Length; i++)
            {
                if (i != 0) sb.Append(", ");

                var item = named.TypeArguments[i];
                var name = item.EasyName(options);
                sb.Append(name);
            }
            sb.Append('>');
        }

        var add =
            options.UseNullableAnnotation &&
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

        if (options.UseMemberType)
        {
            var type = symbol.Type;
            var name = type.EasyName(options);
            sb.Append(name);
            sb.Append(' ');
        }

        if (options.UseHostType)
        {
            var type = symbol.ContainingType;
            var name = type.EasyName(options with { UseNullableAnnotation = false });
            sb.Append(name);
            sb.Append('.');
        }

        sb.Append(symbol.Name);

        if (options.UseArguments && symbol.IsIndexer)
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

        if (options.UseMemberType)
        {
            var type = symbol.Type;
            var name = type.EasyName(options);
            sb.Append(name);
            sb.Append(' ');
        }

        if (options.UseHostType)
        {
            var type = symbol.ContainingType;
            var name = type.EasyName(options with { UseNullableAnnotation = false });
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

        if (options.UseMemberType)
        {
            var type = symbol.ReturnType;
            var name = type.EasyName(options);
            sb.Append(name);
            sb.Append(' ');
        }

        if (options.UseHostType)
        {
            var type = symbol.ContainingType;
            var name = type.EasyName(options with { UseNullableAnnotation = false });
            sb.Append(name);
            sb.Append('.');
        }

        sb.Append(symbol.Name);

        if (options.UseTypeParameters && symbol.TypeParameters.Length > 0)
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

        if (options.UseArguments)
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