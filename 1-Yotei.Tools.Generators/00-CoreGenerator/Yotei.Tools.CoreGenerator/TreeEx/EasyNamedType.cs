namespace Yotei.Tools.CoreGenerator;

// ========================================================
internal record EasyNamedType
{
    /// <summary>
    /// If <see langword="true"/> then include the '<see langword="in"/>' and '<see langword="out"/>'
    /// keywords before the type in the display string. This setting is only used with variant type
    /// parameters.
    /// </summary>
    public bool UseVariance { get; set; }

    /// <summary>
    /// If <see langword="true"/> then the 'global' namespace will be included in the display
    /// string. This setting is only used if the '<see cref="UseNamespace"/>' is enabled.
    /// </summary>
    public bool UseGlobalNamespace { get; set; }

    /// <summary>
    /// If <see langword="true"/> then the namespace of the given type will be included in the
    /// display string. If this setting is enabled, then the '<see cref="UseHosts"/>' one is also
    /// enabled implicitly.
    /// </summary>
    public bool UseNamespace { get; set; }

    /// <summary>
    /// If <see langword="true"/> then the host types of the given one will be included in the
    /// display string.
    /// </summary>
    public bool UseHosts { get; set; }

    /// <summary>
    /// If <see langword="true"/> then the display string will be an empty one, and shortcuts all
    /// other settings. This setting is mostly used to obtain an empty list of generic arguments.
    /// </summary>
    public bool HideName { get; set; }

    /// <summary>
    /// If <see langword="true"/> then the display string uses keywords for predefined types.
    /// </summary>
    public bool UseSpecialNames { get; set; }

    /// <summary>
    /// If <see langword="true"/> then removes the 'Attribute' suffix, if possible.
    /// </summary>
    public bool RemoveAttributeSuffix { get; set; }

    /// <summary>
    /// The nullable style to use.
    /// </summary>
    public NullableStyle NullableStyle { get; set; }

    /// <summary>
    /// The options to use to include the list of generic arguments in the display string. When it
    /// is <see langword="null"/> then the generic arguments' list will be ignored.
    /// </summary>
    public EasyNamedType? GenericOptions { get; set; }
}

// ========================================================
internal static partial class EasyNameExtensions
{
    /// <summary>
    /// Obtains a format instance based upon the given options.
    /// </summary>
    static SymbolDisplayFormat ToFormat(EasyNamedType options)
    {
        var globalns = default(SymbolDisplayGlobalNamespaceStyle);
        var typestyle = default(SymbolDisplayTypeQualificationStyle);
        var genopts = default(SymbolDisplayGenericsOptions);
        var misc = default(SymbolDisplayMiscellaneousOptions);

        if (options.UseVariance) genopts |= SymbolDisplayGenericsOptions.IncludeVariance;
        if (options.UseGlobalNamespace && options.UseNamespace) globalns = SymbolDisplayGlobalNamespaceStyle.Included;
        if (options.UseSpecialNames) misc |= SymbolDisplayMiscellaneousOptions.UseSpecialTypes;
        if (options.RemoveAttributeSuffix) misc |= SymbolDisplayMiscellaneousOptions.RemoveAttributeSuffix;
        if (options.NullableStyle == NullableStyle.UseAnnotations) misc |= SymbolDisplayMiscellaneousOptions.IncludeNullableReferenceTypeModifier;
        if (options.NullableStyle == NullableStyle.UseWrappers) misc |= SymbolDisplayMiscellaneousOptions.ExpandNullable;

        return new(
            globalNamespaceStyle: globalns,
            typeQualificationStyle: typestyle,
            genericsOptions: genopts,
            miscellaneousOptions: misc);
    }

    // ----------------------------------------------------

    /// <summary>
    /// Returns a display string for the given element using default options.
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    public static string EasyName(this INamedTypeSymbol item) => item.EasyName(new());

    /// <summary>
    /// Returns a display string for the given element using the given options.
    /// </summary>
    /// <param name="item"></param>
    /// <param name="options"></param>
    /// <returns></returns>
    public static string EasyName(this INamedTypeSymbol item, EasyNamedType options)
    {
        // Shortcut hide name...
        if (options.HideName) return string.Empty;

        // Shortcut nullable wrappers...
        if (options.NullableStyle == NullableStyle.UseWrappers &&
            item.Arity > 0 && (
            item.OriginalDefinition.SpecialType == SpecialType.System_Nullable_T ||
            item.OriginalDefinition.Name == "IsNullable"))
        {
            var arg = item.TypeArguments[0];
            var str = EasyName((INamedTypeSymbol)arg, options);
            if (!str.EndsWith('?')) str += '?';
            return str;
        }

        // Standard display string...
        var format = ToFormat(options);
        var name = item.ToDisplayString(format);

        // Generic arguments...
        if (item.Arity > 0 && options.GenericOptions is not null)
        {
            var sb = new StringBuilder();
            sb.Append('<');

            for (var i = 0; i < item.TypeArguments.Length; i++)
            {
                var arg = item.TypeArguments[i];
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
            var ats = item.GetAttributes().Any(x => x.AttributeClass?.Name == "IsNullableAttribute");
            if (ats) name += '?';
        }

        // Finishing...
        return name;
    }
}