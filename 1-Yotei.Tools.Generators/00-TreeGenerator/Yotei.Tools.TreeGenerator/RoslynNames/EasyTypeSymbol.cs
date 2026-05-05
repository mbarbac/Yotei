using System.Reflection.Metadata;

namespace Yotei.Tools.Generators;

// ========================================================
public static partial class RoslynNamesExtensions
{
    /// <summary>
    /// Obtains a C#-alike representation for a given type-alike element, using default options.
    /// </summary>
    /// <param name="source"></param>
    /// <returns></returns>
    public static string EasyName(
        this ITypeSymbol source) => source.EasyName(new EasyTypeOptions());

    /// <summary>
    /// Obtains a C#-alike representation for a given type-alike element, using the given options.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="options"></param>
    /// <returns></returns>
    [SuppressMessage("", "IDE0019")]
    public static string EasyName(this ITypeSymbol source, EasyTypeOptions options)
    {
        ArgumentNullException.ThrowIfNull(source);
        ArgumentNullException.ThrowIfNull(options);

        // Intercepting placeholders...
        if (options.UsePlaceHolder) return string.Empty;

        /* NOTE: When a type has a generic argument as in 'typeof(Predicate<>)', the T-alike
         * element is, for whatever reasons, an 'IErrorTypeSymbol'. As it happens it implements
         * 'INamedTypeSymbol' we can simply ignore.
         */

        // Intercepting special types...
        switch (source)
        {
            case IArrayTypeSymbol item: return EasyNameTypeArray(item, options);
            case IPointerTypeSymbol item: return EasyNameTypePointer(item, options);
        }

        // Processing...
        var sb = new StringBuilder();        
        var named = source as INamedTypeSymbol;
        var args = named is null ? [] : named.TypeArguments;
        var host = source.ContainingType;
        var isgen = source.IsGenericAlike();
        var xname = options.UseSpecialNames ? source.ToSpecialName() : null;

        // Intercepting wrappers...
        if (source.IsNullableWrapper())
        {
            if (options.NullableStyle == EasyNullableStyle.UseAnnotations)
            {
                var arg = args[0];
                var str = arg.EasyName(options);
                if (str.Length > 0 && str[^1] != '?') str += '?';
                return str;
            }
            if (options.NullableStyle == EasyNullableStyle.KeepWrappers)
            {
                var arg = args[0];
                var str = arg.EasyName(options);

                var xoptions = options with { GenericListOptions = null };
                source = source.OriginalDefinition;

                TryAddVariance(sb, source, options);
                TryAddNamespace(sb, source, options, xname, host, isgen);
                TryAddHost(sb, host, options, xname, isgen);
                TryAddName(sb, source, options, xname);
                str = $"{sb}<{str}>";
                return str;
            }
        }

        // Variance...
        TryAddVariance(sb, source, options);
        static void TryAddVariance(
            StringBuilder sb,
            ITypeSymbol source, EasyTypeOptions options)
        {
            if (options.UseVariance && source is ITypeParameterSymbol par)
            {
                switch (par.Variance)
                {
                    case VarianceKind.Out: sb.Append("out "); break;
                    case VarianceKind.In: sb.Append("in "); break;
                }
            }
        }

        // Namespace...
        TryAddNamespace(sb, source, options, xname, host, isgen);
        static void TryAddNamespace(
            StringBuilder sb,
            ITypeSymbol source, EasyTypeOptions options, string? xname, ITypeSymbol host, bool isgen)
        {
            if (options.NamespaceStyle != EasyNamespaceStyle.None &&
                xname == null &&
                host == null && !isgen)
            {
                var ns = source.ContainingNamespace;
                var str = ns == null ? null : EasyNamespace(ns, options);
                if (str != null && str.Length > 0) sb.Append(str).Append('.');
            }
        }

        // Host...
        TryAddHost(sb, host, options, xname, isgen);
        static void TryAddHost(
            StringBuilder sb,
            ITypeSymbol host, EasyTypeOptions options, string? xname, bool isgen)
        {
            if ((options.UseHost || options.NamespaceStyle != EasyNamespaceStyle.None) &&
                xname == null &&
                host != null && !isgen)
            {
                var str = host.EasyName(options);
                if (str.Length > 0) sb.Append(str).Append('.');
            }
        }

        // Name...
        TryAddName(sb, source, options, xname);
        static void TryAddName(
            StringBuilder sb,
            ITypeSymbol source, EasyTypeOptions options, string? xname)
        {
            if (xname != null) sb.Append(xname);
            else
            {
                var misc = default(SymbolDisplayMiscellaneousOptions);
                if (options.UseSpecialNames) misc |= SymbolDisplayMiscellaneousOptions.UseSpecialTypes;
                if (options.RemoveAttributeSuffix) misc |= SymbolDisplayMiscellaneousOptions.RemoveAttributeSuffix;

                var format = new SymbolDisplayFormat(miscellaneousOptions: misc);
                var str = source.ToDisplayString(format);
                sb.Append(str);
            }
        }

        // Generic parameters...
        while (xname == null && args.Length > 0 && options.GenericListOptions != null)
        {
            if (source.IsNullableWrapper() &&
                options.NullableStyle == EasyNullableStyle.KeepWrappers) break;

            var xoptions = options.GenericListOptions;
            sb.Append('<'); for (int i = 0; i < args.Length; i++)
            {
                var arg = args[i];
                var str = arg.EasyName(xoptions);
                if (i > 0) sb.Append(str.Length > 0 ? ", " : ",");
                sb.Append(str);
            }
            sb.Append('>');
            break;
        }

        // Nullability...
        while (options.NullableStyle != EasyNullableStyle.None &&
            sb.Length > 0 &&
            sb[^1] != '?')
        {
            if (source.IsNullableWrapper()) // Special case...
            {
                if (options.NullableStyle == EasyNullableStyle.KeepWrappers)
                { if (xname != null) { sb.Append('?'); break; } }
            }

            if (source.IsNullableAnnotated()) { sb.Append('?'); break; }
            break;
        }

        // Finishing...
        return sb.ToString();
    }

    // ----------------------------------------------------

    /// <summary>
    /// Invoked when the symbol is a namespace, may return null.
    /// </summary>
    static string? EasyNamespace(this INamespaceSymbol source, EasyTypeOptions options)
    {
        var names = new List<string>();
        var node = (ISymbol?)source;

        while (node != null) // Using the given one as the first element...
        {
            if (node is INamespaceSymbol ns &&
                ns.Name != null &&
                ns.Name.Length > 0) names.Add(ns.Name);

            node = node.ContainingSymbol;
        }

        if (names.Count == 0) return null; // Return 'null' if no one was found.
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
    /// Invoked when the symbol is an array (ie: string?[]?).
    /// </summary>
    static string EasyNameTypeArray(this IArrayTypeSymbol source, EasyTypeOptions options)
    {
        var type = source.ElementType;
        var name = EasyName(type, options);

        name = $"{name}[{new string(',', source.Rank - 1)}]";

        if (!name.EndsWith('?') &&
            options.NullableStyle == EasyNullableStyle.UseAnnotations)
        {
            if (source.NullableAnnotation == NullableAnnotation.Annotated) return name + '?';

            if (source.GetAttributes().Any(x =>
                x.AttributeClass?.Name
                is (nameof(IsNullableAttribute)
                or (nameof(NullableAttribute))))) return name + '?';
            
        }
        return name;
    }

    // ----------------------------------------------------

    /// <summary>
    /// Invoked when the symbol is a pointer (ie: int*).
    /// </summary>
    static string EasyNameTypePointer(this IPointerTypeSymbol source, EasyTypeOptions options)
    {
        var type = source.PointedAtType;
        var name = EasyName(type, options);

        if (!name.EndsWith('?') &&
            options.NullableStyle == EasyNullableStyle.UseAnnotations)
        {
            if (source.NullableAnnotation == NullableAnnotation.Annotated) return name + '?';

            if (source.GetAttributes().Any(x =>
                x.AttributeClass?.Name
                is (nameof(IsNullableAttribute)
                or (nameof(NullableAttribute))))) return name + '?';

        }

        name += '*';
        return name;
    }
}