namespace Yotei.Tools.Generators.Internal;

// ========================================================
internal static class EasyNameExtensions
{
    /// <summary>
    /// Returns the C#-alike name of the given type.
    /// </summary>
    public static string EasyName(
        this ITypeSymbol symbol,
        bool fullyQualifiedName = false,
        bool addTypeParameters = true,
        bool addNullableAnnotation = true)
    {
        if (symbol.IsNamespace) throw new ArgumentException("Symbol is namespace.").WithData(symbol);

        var sb = new StringBuilder();

        if (fullyQualifiedName)
        {
            List<string> items = [];
            ISymbol? node = symbol;

            while (node != null)
            {
                switch (node)
                {
                    case INamespaceSymbol item: items.Add(item.Name); break;
                    case INamedTypeSymbol item: items.Add(item.EasyName(addNullableAnnotation: false)); break;
                }
                node = node.ContainingSymbol;
            }
            items.Reverse();
            sb.Append(string.Join(".", items));
            sb.Append('.');
        }

        sb.Append(symbol.Name);

        if (symbol is INamedTypeSymbol named &&
            named.TypeParameters.Length > 0 &&
            addTypeParameters)
        {
            sb.Append('<'); for (int i = 0; i < named.TypeParameters.Length; i++)
            {
                if (i != 0) sb.Append(", ");

                var temp = named.TypeArguments[i];
                var name = temp.EasyName();
                sb.Append(name);
            }
            sb.Append('>');
        }

        var add = addNullableAnnotation &&
            symbol.NullableAnnotation == NullableAnnotation.Annotated &&
            symbol.Name != "Nullable";        
        
        if (add) sb.Append('?');

        return sb.ToString();
    }

    // ----------------------------------------------------

    /// <summary>
    /// Returns the C#-alike name of the given property.
    /// </summary>
    public static string EasyName(
        this IPropertySymbol symbol,
        bool useSymbolType = true,
        bool useHostType = true,
        bool useIndexer = true)
    {
        var sb = new StringBuilder();

        if (useSymbolType)
        {
            var type = symbol.Type;
            var name = type.EasyName();
            sb.Append(name);
            sb.Append(' ');
        }

        if (useHostType)
        {
            var type = symbol.ContainingType;
            var name = type.EasyName();
            sb.Append(name);
            sb.Append('.');
        }

        sb.Append(symbol.Name);

        if (useIndexer && symbol.IsIndexer)
        {
            sb.Append('['); for (int i = 0; i < symbol.Parameters.Length; i++)
            {
                if (i > 0) sb.Append(", ");

                var item = symbol.Parameters[i];
                var itemName = item.Name;
                var itemType = item.Type.EasyName();

                sb.Append($"{itemType} {itemName}");
            }
            sb.Append(']');
        }

        return sb.ToString();
    }

    // ----------------------------------------------------

    /// <summary>
    /// Returns the C#-alike name of the given field.
    /// </summary>
    public static string EasyName(
        this IFieldSymbol symbol,
        bool useSymbolType = true,
        bool useHostType = true)
    {
        var sb = new StringBuilder();

        if (useSymbolType)
        {
            var type = symbol.Type;
            var name = type.EasyName();
            sb.Append(name);
            sb.Append(' ');
        }

        if (useHostType)
        {
            var type = symbol.ContainingType;
            var name = type.EasyName();
            sb.Append(name);
            sb.Append('.');
        }

        sb.Append(symbol.Name);

        return sb.ToString();
    }

    // ----------------------------------------------------

    /// <summary>
    /// Returns the C#-alike name of the given method.
    /// </summary>
    public static string EasyName(
        this IMethodSymbol symbol,
        bool useReturnType = true,
        bool useHostType = true,
        bool useTypeArguments = true,
        bool useArguments = true)
    {
        var sb = new StringBuilder();

        if (useReturnType)
        {
            var type = symbol.ReturnType;
            var name = type.EasyName();
            sb.Append(name);
            sb.Append(' ');
        }

        if (useHostType)
        {
            var type = symbol.ContainingType;
            var name = type.EasyName();
            sb.Append(name);
            sb.Append('.');
        }

        sb.Append(symbol.Name);

        if (useTypeArguments && symbol.TypeParameters.Length > 0)
        {
            sb.Append('<'); for (int i = 0; i < symbol.TypeArguments.Length; i++)
            {
                if (i != 0) sb.Append(", ");

                var temp = symbol.TypeArguments[i];
                var name = temp.EasyName();
                sb.Append(name);
            }
            sb.Append('>');
        }

        if (useArguments)
        {
            sb.Append('('); for (int i = 0; i < symbol.Parameters.Length; i++)
            {
                if (i > 0) sb.Append(", ");

                var item = symbol.Parameters[i];
                var itemName = item.Name;
                var itemType = item.Type.EasyName();

                sb.Append($"{itemType} {itemName}");
            }
            sb.Append(')');
        }

        return sb.ToString();
    }
}