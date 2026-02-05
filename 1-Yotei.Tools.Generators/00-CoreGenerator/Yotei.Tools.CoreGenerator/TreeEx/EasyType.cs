namespace Yotei.Tools.CoreGenerator;

// ========================================================
internal record EasyType
{
    /// <summary>
    /// If <see langword="true"/>, include the '<see langword="in"/>' and '<see langword="out"/>'
    /// keywords before the type in the display string. This setting is only used with variant
    /// type parameters.
    /// </summary>
    public bool UseVariance { get; set; }

    /// <summary>
    /// If not <see langword="null"/> the options to use to include the type's namespace in the
    /// display string. If <see langword="null"/>, then it is ignored.
    /// </summary>
    public EasyNamespace? NamespaceOptions { get; set; }

    /// <summary>
    /// If <see langword="true"/>, include the type's host ones in the display string. 
    /// </summary>
    public bool UseHosts { get; set; }

    /// <summary>
    /// If <see langword="true"/>, returns an empty string. This setting is mostly used with the
    /// generic arguments to obtain an anonymous list of generics. If this setting is enabled then
    /// it shortcuts all other settings.
    /// </summary>
    public bool HideName { get; set; }

    /// <summary>
    /// The nullable style to use.
    /// </summary>
    public NullableStyle NullableStyle { get; set; }

    /// <summary>
    /// If not <see langword="null"/> the options to use to include the list of generic arguments
    /// of the type in the display string. If <see langword="null"/>, then they are ignored.
    /// </summary>
    public EasyType? GenericOptions { get; set; }

    // ----------------------------------------------------

    /// <summary>
    /// Gets a new empty instance.
    /// </summary>
    public static EasyType Empty => new();

    /// <summary>
    /// Gets a new default instance:
    /// <br/>- Use nullable annotations.
    /// <br/>- Include generic arguments with their namespaces.
    /// </summary>
    public static EasyType Default => new()
    {
        NullableStyle = NullableStyle.UseAnnotations,
        GenericOptions = new() { NamespaceOptions = new() { UseContainingNamespace = true } }
    };

    // ----------------------------------------------------

    /// <summary>
    /// Returns a display string for the given element using this options.
    /// </summary>
    /// <param name="source"></param>
    /// <returns></returns>
    public string EasyName(INamedTypeSymbol source) => source.EasyName(this);
}

// ========================================================
internal static partial class EasyNameExtensions
{
    /// <summary>
    /// Returns a display string for the given element using default options.
    /// </summary>
    /// <param name="source"></param>
    /// <returns></returns>
    public static string EasyName(this INamedTypeSymbol source) => source.EasyName(new());

    /// <summary>
    /// Returns a display string for the given element using the given options.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="options"></param>
    /// <returns></returns>
    public static string EasyName(this INamedTypeSymbol source, EasyType options)
    {
        // Shortcut hide name...
        if (options.HideName) return string.Empty;

        // Shortcut nullable wrappers...
        if (options.NullableStyle == NullableStyle.UseWrappers &&
            source.Arity > 0 && (
            source.OriginalDefinition.SpecialType == SpecialType.System_Nullable_T ||
            source.OriginalDefinition.Name == "IsNullable"))
        {
            var arg = source.TypeArguments[0];
            var str = EasyName((INamedTypeSymbol)arg, options);
            if (!str.EndsWith('?')) str += '?';
            return str;
        }

        // Processing...
        var sb = new StringBuilder();
        var host = source.ContainingType;
        var isgen = source.TypeKind == TypeKind.TypeParameter;

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
        if (options.NamespaceOptions is not null && host is null && !isgen)
        {
            var ns = source.ContainingNamespace;
            if (ns is not null)
            {
                var str = ns.EasyName(options.NamespaceOptions);
                if (str.Length > 0) sb.Append(str).Append('.');
            }
        }

        throw null;
    }
    /*

        

        // Standard display string...
        var format = ToFormat(options);
        var name = source.ToDisplayString(format);

        // Generic arguments...
        if (source.Arity > 0 && options.GenericOptions is not null)
        {
            var sb = new StringBuilder();
            sb.Append('<');

            for (var i = 0; i < source.TypeArguments.Length; i++)
            {
                var arg = source.TypeArguments[i];
                var str = EasyName((INamedTypeSymbol)arg, options.GenericOptions);
                if (i > 0) sb.Append(str.Length > 0 ? ", " : ",");
                sb.Append(str);
            }

            sb.Append('>');
            name += sb.ToString();
        }

        // Decorated nullability...
        if (options.NullableStyle == NullableStyle.UseAnnotations && !name.EndsWith('?'))
        {
            var ats = source.GetAttributes().Any(x => x.AttributeClass?.Name == "IsNullableAttribute");
            if (ats) name += '?';
        }

        // Finishing...
        return name;
    }
     */
}