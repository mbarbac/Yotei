using System.ComponentModel;

namespace Yotei.Tools.Generators.Internal;

// ========================================================
internal static class EasyNameExtensions
{
    /// <summary>
    /// Returns the C#-alike name of the given symbol, using default options.
    /// </summary>
    /// <param name="symbol"></param>
    /// <returns></returns>
    public static string EasyName(
        this ITypeSymbol symbol) => symbol.EasyName(EasyNameOptions.Default);

    /// <summary>
    /// Returns the C#-alike name of the given symbol, using the given options.
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

        if (options.UseFullName && symbol is not ITypeParameterSymbol and not IErrorTypeSymbol)
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
                        name = item.EasyName(options with { UseFullName = false, AddNullable = true });
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

        if (options.UseGenerics && named != null && named.Arity > 0)
        {
            var temp = options with { AddNullable = true };
            
            sb.Append('<'); for (int i = 0; i < named.TypeArguments.Length; i++)
            {
                if (i != 0) sb.Append(", ");
                
                var item = named.TypeArguments[i];
                var name = item.EasyName(temp);
                sb.Append(name);
            }
            sb.Append('>');
        }

        if (options.AddNullable &&
            symbol.NullableAnnotation == NullableAnnotation.Annotated &&
            symbol.Name != "Nullable")
            sb.Append('?');

        return sb.ToString();
    }

    // ----------------------------------------------------

    /// <summary>
    /// Returns the C#-alike name of the given property, using default options.
    /// </summary>
    /// <param name="symbol"></param>
    /// <returns></returns>
    public static string EasyName(
        this IPropertySymbol symbol) => symbol.EasyName(EasyNameOptions.Default);

    /// <summary>
    /// Returns the C#-alike name of the given property, using the given options.
    /// </summary>
    /// <param name="symbol"></param>
    /// <param name="options"></param>
    /// <returns></returns>
    public static string EasyName(this IPropertySymbol symbol, EasyNameOptions options)
    {
        symbol.ThrowWhenNull();
        options.ThrowWhenNull();

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
            var name = type.EasyName(options with { AddNullable = false });
            sb.Append(name);
            sb.Append('.');
        }

        sb.Append(symbol.Name);

        if (options.UseMemberArguments && symbol.IsIndexer)
        {
            sb.Append('['); for (int i = 0; i < symbol.Parameters.Length; i++)
            {
                if (i != 0) sb.Append(", ");

                var item = symbol.Parameters[i];
                var itemType = item.Type.EasyName(options with { AddNullable = true });
                var itemName = item.Name;

                sb.Append($"{itemType} {itemName}");
            }
            sb.Append(']');
        }

        return sb.ToString();
    }

    // ----------------------------------------------------

    /// <summary>
    /// Returns the C#-alike name of the given field, using default options.
    /// </summary>
    /// <param name="symbol"></param>
    /// <returns></returns>
    public static string EasyName(
        this IFieldSymbol symbol) => symbol.EasyName(EasyNameOptions.Default);

    /// <summary>
    /// Returns the C#-alike name of the given field, using the given options.
    /// </summary>
    /// <param name="symbol"></param>
    /// <param name="options"></param>
    /// <returns></returns>
    public static string EasyName(this IFieldSymbol symbol, EasyNameOptions options)
    {
        symbol.ThrowWhenNull();
        options.ThrowWhenNull();

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
            var name = type.EasyName(options with { AddNullable = false });
            sb.Append(name);
            sb.Append('.');
        }

        sb.Append(symbol.Name);

        return sb.ToString();
    }

    // ----------------------------------------------------

    /// <summary>
    /// Returns the C#-alike name of the given method, using default options.
    /// </summary>
    /// <param name="symbol"></param>
    /// <returns></returns>
    public static string EasyName(
        this IMethodSymbol symbol) => symbol.EasyName(EasyNameOptions.Default);

    /// <summary>
    /// Returns the C#-alike name of the given method, using the given options.
    /// </summary>
    /// <param name="symbol"></param>
    /// <param name="options"></param>
    /// <returns></returns>
    public static string EasyName(this IMethodSymbol symbol, EasyNameOptions options)
    {
        symbol.ThrowWhenNull();
        options.ThrowWhenNull();

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
            var name = type.EasyName(options with { AddNullable = false });
            sb.Append(name);
            sb.Append('.');
        }

        sb.Append(symbol.Name);

        if (options.UseGenerics && symbol.TypeParameters.Length > 0)
        {
            var temp = options with { AddNullable = true };

            sb.Append('<'); for (int i = 0; i < symbol.TypeArguments.Length; i++)
            {
                if (i != 0) sb.Append(", ");

                var item = symbol.TypeArguments[i];
                var name = item.EasyName(temp);
                sb.Append(name);
            }
            sb.Append('>');
        }

        if (options.UseMemberArguments && symbol.Parameters.Length > 0)
        {
            sb.Append('('); for (int i = 0; i < symbol.Parameters.Length; i++)
            {
                if (i != 0) sb.Append(", ");

                var item = symbol.Parameters[i];
                var itemType = item.Type.EasyName(options with { AddNullable = true });
                var itemName = item.Name;

                sb.Append($"{itemType} {itemName}");
            }
            sb.Append(')');
        }

        return sb.ToString();
    }
}