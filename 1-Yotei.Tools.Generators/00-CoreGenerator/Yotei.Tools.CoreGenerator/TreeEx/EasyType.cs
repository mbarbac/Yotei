namespace Yotei.Tools.CoreGenerator;

// ========================================================
internal record EasyType
{
    /// <summary>
    /// Include the 'in' and 'out'  keywords in the display string. This setting is only used
    /// with variant type parameters.
    /// </summary>
    public bool UseVariance { get; set; }

    /// <summary>
    /// If not null, then the options to include the type's namespace. If null, it is ignored.
    /// If this setting is enabled, then the <see cref="UseHosts"/> one is also implicitly enabled.
    /// </summary>
    public EasyNamespace? NamespaceOptions { get; set; }

    /// <summary>
    /// Include the type's host ones in the display string. 
    /// </summary>
    public bool UseHost { get; set; }

    /// <summary>
    /// If enabled, hides the type's name by returning an empty string. This setting is almost only
    /// used to obtain an anonymous list of generic arguments. If this setting is enabled then it
    /// shortcuts all other settings.
    /// </summary>
    public bool HideName { get; set; }

    /// <summary>
    /// If enabled, include the prefined keywords for suitable special types (eg: <c>int</c> instead
    /// of <c>Int32</c>).
    /// </summary>
    public bool UseSpecialNames { get; set; }

    /// <summary>
    /// If enabled removes the 'Attribute' suffix from the type's display string, when possible.
    /// </summary>
    public bool RemoveAttributeSuffix { get; set; }

    /// <summary>
    /// Specifies the nhe nullable style to use.
    /// </summary>
    public NullableStyle NullableStyle { get; set; }

    /// <summary>
    /// If not null, then the options to include the list of generic arguments of the type. If null,
    /// then they are ignored.
    /// </summary>
    public EasyType? GenericOptions { get; set; }
}

// ========================================================
internal static partial class EasyNameExtensions
{
    /// <summary>
    /// Gets a new format instance suitable for EasyName purposes.
    /// </summary>
    static SymbolDisplayFormat ToDisplayFormat(EasyType options)
    {
        var misc = default(SymbolDisplayMiscellaneousOptions);
        if (options.UseSpecialNames) misc |= SymbolDisplayMiscellaneousOptions.UseSpecialTypes;
        if (options.RemoveAttributeSuffix) misc |= SymbolDisplayMiscellaneousOptions.RemoveAttributeSuffix;

        return new SymbolDisplayFormat(miscellaneousOptions: misc);
    }

    /// <summary>
    /// Determines if the given type is a nullable wrapper or not.
    /// </summary>
    static bool IsNullableWrapper(ITypeSymbol source) =>
        source is INamedTypeSymbol named &&
        named.Arity == 1 && (
        source.OriginalDefinition.SpecialType == SpecialType.System_Nullable_T ||
        source.OriginalDefinition.Name == nameof(IsNullable<>));

    // ----------------------------------------------------

    /// <summary>
    /// Returns a display string for the given element using default options.
    /// </summary>
    /// <param name="source"></param>
    /// <returns></returns>
    public static string EasyName(this ITypeSymbol source) => source.EasyName(new());

    /// <summary>
    /// Returns a display string for the given element using the given options.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="options"></param>
    /// <returns></returns>
    [SuppressMessage("", "IDE0019")]
    public static string EasyName(this ITypeSymbol source, EasyType options)
    {
        ArgumentNullException.ThrowIfNull(source);
        ArgumentNullException.ThrowIfNull(options);

        // Shortcuts...
        if (options.HideName) return string.Empty;
        switch (source)
        {
            case IErrorTypeSymbol: return "<error>";
            case IArrayTypeSymbol item: return EasyNameTypeArray(item, options);
            case IPointerTypeSymbol item: return EasyNameTypePointer(item, options);
            case IFunctionPointerTypeSymbol item: return EasyNameTypeFunctionPointer(item, options);
        }

        // Processing...
        var sb = new StringBuilder();
        var named = source as INamedTypeSymbol;
        var isgen = source.TypeKind == TypeKind.TypeParameter;
        var args = named is null ? [] : named.TypeArguments;
        var host = source.ContainingType;

        // Shortcut nullable wrappers...
        if (options.NullableStyle == NullableStyle.UseAnnotations && IsNullableWrapper(source))
        {
            var arg = args[0];
            var str = EasyName(arg, options);
            if (!str.EndsWith('?')) str += '?';
            return str;
        }

        // Variance mask...
        if (options.UseVariance && source is ITypeParameterSymbol par)
        {
            switch (par.Variance)
            {
                case VarianceKind.Out: sb.Append("out "); break;
                case VarianceKind.In: sb.Append("in "); break;
            }
        }

        // Namespace...
        if (options.NamespaceOptions != null && host == null && !isgen)
        {
            var ns = source.ContainingNamespace;
            if (ns is not null)
            {
                var str = ns.EasyName(options.NamespaceOptions);
                if (str.Length > 0) sb.Append(str).Append('.');
            }
        }

        // Host types...
        if ((options.UseHost || options.NamespaceOptions != null) && host != null && !isgen)
        {
            var xoptions = options with { HideName = false };
            var str = EasyName(host, xoptions);
            if (str.Length > 0) sb.Append(str).Append('.');
        }

        // Name...
        var format = ToDisplayFormat(options);
        var name = source.ToDisplayString(format);
        sb.Append(name);

        // Generic arguments...
        if (options.GenericOptions != null && args.Length > 0)
        {
            sb.Append('<'); for (int i = 0; i < args.Length; i++)
            {
                var xoptions = options.GenericOptions with { GenericOptions = options };
                var arg = args[i];
                var str = EasyName(arg, xoptions);
                if (i > 0) sb.Append(str.Length > 0 ? ", " : ",");
                sb.Append(str);
            }
            sb.Append('>');
        }

        // Nullability...
        while (!sb.EndsWith('?') && options.NullableStyle != NullableStyle.None)
        {
            if (source.NullableAnnotation == NullableAnnotation.Annotated)
            { sb.Append('?'); break; }

            if (source.GetAttributes().Any(x => x.AttributeClass?.Name == nameof(IsNullableAttribute)))
            { sb.Append('?'); break; }

            break;
        }

        // Finishing...
        return sb.ToString();
    }

    // ----------------------------------------------------

    /// <summary>
    /// Invoked when the type represents an array (ie: string?[]?).
    /// </summary>
    static string EasyNameTypeArray(IArrayTypeSymbol source, EasyType options)
    {
        var type = source.ElementType;
        var name = EasyName(type, options);
        name = $"{name}[{new string(',', source.Rank - 1)}]";

        while (!name.EndsWith('?') && options.NullableStyle != NullableStyle.None)
        {
            if (source.NullableAnnotation == NullableAnnotation.Annotated)
            { name += '?'; break; }

            if (source.GetAttributes().Any(x => x.AttributeClass?.Name == nameof(IsNullableAttribute)))
            { name += '?'; break; }
        }
        return name;
    }

    // ----------------------------------------------------

    /// <summary>
    /// Invoked when the type represents a pointer (ie: int*).
    /// </summary>
    static string EasyNameTypePointer(IPointerTypeSymbol source, EasyType options)
    {
        var type = source.PointedAtType;
        var name = EasyName(type, options);

        while (!name.EndsWith('?') && options.NullableStyle != NullableStyle.None)
        {
            if (source.NullableAnnotation == NullableAnnotation.Annotated)
            { name += '?'; break; }

            if (source.GetAttributes().Any(x => x.AttributeClass?.Name == nameof(IsNullableAttribute)))
            { name += '?'; break; }
        }
        name += '*';
        return name;
    }

    // ----------------------------------------------------

    /// <summary>
    /// Invoked when the type represents a function pointer (ie: delegate*<int, void>).
    /// </summary>
    static string EasyNameTypeFunctionPointer(IFunctionPointerTypeSymbol source, EasyType options)
    {
        // LOW: implement EasyName for 'IFunctionPointerTypeSymbol' instances.
        throw new NotImplementedException("IFunctionPointerTypeSymbol types are not supported.");
    }
}