namespace Yotei.Tools.Generators;

// ========================================================
public static partial class RoslynNamesExtensions
{
    /// <summary>
    /// Obtains a c#-alike string representation of the given element, using default options.
    /// </summary>
    /// <param name="source"></param>
    /// <returns></returns>
    public static string EasyName(
        this ITypeSymbol source) => source.EasyName(EasyTypeOptions.Default);

    /// <summary>
    /// Obtains a c#-alike string representation of the given element, using the given options.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="options"></param>
    /// <returns></returns>
    public static string EasyName(this ITypeSymbol source, EasyTypeOptions options)
    {
        ArgumentNullException.ThrowIfNull(source);
        ArgumentNullException.ThrowIfNull(options);

        // - When using generic type arguments (ie: 'typeof(Whatever<>)') to print the '<T>,
        //   for whatever reasons we get an 'IErrorTypeSymbol'. As it happens it implements
        //   'INamedTypeSymbol' we can just continue.
        // - 'IErrorTypeSymbol': I have no use case for this case yet.
        // Shortcuts...
        switch (source)
        {
            case IArrayTypeSymbol item: return EasyNameTypeArray(item, options);
            case IPointerTypeSymbol item: return EasyNameTypePointer(item, options);
            default: break;
        }

        // Processing...
        var sb = new StringBuilder();
        var named = source as INamedTypeSymbol;
        var isgen = source.IsGenericAlike();
        var args = named is null ? [] : named.TypeArguments;
        var xname = options.UseSpecialNames ? source.ToSpecialName() : null;
        var host = source.ContainingType;

        // Intercepting nullable wrappers...
        if (options.NullableStyle == EasyNullableStyle.UseAnnotations &&
            source.IsNullableWrapper())
        {
            var arg = args[0];
            var str = arg.EasyName(options);
            if (str.Length > 0 && !str.EndsWith('?')) str += '?';
            return str;
        }

        // Variance...
        if (options.UseVariance && source is ITypeParameterSymbol par)
        {
            switch (par.Variance)
            {
                case VarianceKind.Out: sb.Append("out "); break;
                case VarianceKind.In: sb.Append("in "); break;
            }
        }

        // Namespace...
        if (options.NamespaceStyle != EasyNamespaceStyle.None && !isgen &&
            host == null &&
            xname == null)
        {
            var ns = source.ContainingNamespace;
            if (ns != null)
            {
                var str = EasyNamespace(ns, options);
                if (str != null && str.Length > 0) sb.Append(str).Append('.');
            }
        }

        // Host...
        if ((options.UseHost || options.NamespaceStyle != EasyNamespaceStyle.None) && !isgen &&
            host != null &&
            xname == null)
        {
            var str = host.EasyName(options);
            if (str.Length > 0) sb.Append(str).Append('.');
        }

        // Name...
        if (xname != null) sb.Append(xname);
        else
        {
            var misc = default(SymbolDisplayMiscellaneousOptions);
            if (options.UseSpecialNames) misc |= SymbolDisplayMiscellaneousOptions.UseSpecialTypes;
            if (options.RemoveAttributeSuffix) misc |= SymbolDisplayMiscellaneousOptions.RemoveAttributeSuffix;

            var format = new SymbolDisplayFormat(miscellaneousOptions: misc);
            var name = source.ToDisplayString(format);
            sb.Append(name);
        }

        // Generic arguments...
        if (options.GenericListStyle != EasyGenericListStyle.None && args.Length > 0)
        {
            sb.Append('<'); for (int i = 0; i < args.Length; i++)
            {
                var arg = args[i];
                var str = options.GenericListStyle == EasyGenericListStyle.PlaceHolders
                    ? string.Empty
                    : arg.EasyName(options);

                if (i > 0) sb.Append(str.Length > 0 ? ", " : ",");
                sb.Append(str);
            }
            sb.Append('>');
        }

        // Nullable annotations...
        while (options.NullableStyle != EasyNullableStyle.None &&
            sb.Length > 0 &&
            sb[^1] != '?')
        {
            if (options.NullableStyle == EasyNullableStyle.KeepWrappers &&
                source.IsNullableWrapper())
                break;

            if (source.IsNullableAnnotatedOrAttribute()) sb.Append('?');
            break;
        }

        // Finishing...
        return sb.ToString();
    }

    // ----------------------------------------------------

    /// <summary>
    /// Invoked when the symbol is a namespace.
    /// </summary>
    static string? EasyNamespace(INamespaceSymbol source, EasyTypeOptions options)
    {
        List<string> names = [];
        ISymbol? node = source;

        // Using the given one as the first element...
        while (node != null)
        {
            if (node is INamespaceSymbol ns &&
                ns.Name != null &&
                ns.Name.Length > 0) names.Add(ns.Name);

            node = node.ContainingSymbol;
        }

        // Returning...
        if (names.Count == 0) return null;
        else
        {
            names.Reverse();
            var str = string.Join(".", names);

            if (options.NamespaceStyle == EasyNamespaceStyle.UseGlobal) str = $"global::{str}";
            return str;
        }
    }

    // ----------------------------------------------------

    /// <summary>
    /// Invoked when the symbol is an array (ie: string?[]?)
    /// </summary>
    static string EasyNameTypeArray(IArrayTypeSymbol source, EasyTypeOptions options)
    {
        var type = source.ElementType;
        var name = EasyName(type, options);

        name = $"{name}[{new string(',', source.Rank - 1)}]";

        if (options.NullableStyle != EasyNullableStyle.None &&
            !name.EndsWith('?'))
        {
            if (source.NullableAnnotation == NullableAnnotation.Annotated)
                return name + '?';

            if (source.GetAttributes().Any(x => x.AttributeClass?.Name
                is (nameof(IsNullableAttribute))
                or (nameof(NullableAttribute))))
                return name + '?';
        }

        return name;
    }

    // ----------------------------------------------------

    /// <summary>
    /// Invoked when the symbol is pointer (ie: int*)
    /// </summary>
    static string EasyNameTypePointer(IPointerTypeSymbol source, EasyTypeOptions options)
    {
        var type = source.PointedAtType;
        var name = EasyName(type, options);

        if (options.NullableStyle != EasyNullableStyle.None &&
            !name.EndsWith('?'))
        {
            if (source.NullableAnnotation == NullableAnnotation.Annotated)
                return name + '?';

            if (source.GetAttributes().Any(x => x.AttributeClass?.Name
                is (nameof(IsNullableAttribute))
                or (nameof(NullableAttribute))))
                return name + '?';
        }

        return name;
    }
}