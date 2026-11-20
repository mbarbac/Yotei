#if NETSTANDARD2_0
#pragma warning disable IDE0079
#pragma warning disable CS0436
#endif

namespace Yotei.Tools;

// ========================================================
/// <summary>
/// Describes how to obtain the easy C#-alike name of a given type.
/// <br/> Instances of this type are intended to be immutable ones.
/// </summary>
#if NETSTANDARD2_0
internal
#else
public
#endif
record EasyType : IEquatable<EasyType>
{
    /// <summary>
    /// If enabled, returns an empty placeholder instead of the type's easy name (for instance
    /// when producing an anonymous list of generic type arguments).
    /// </summary>
    public bool UsePlaceHolder { get; init; }

    /// <summary>
    /// If enabled, use the type's variance (the 'in' and 'out' keywords), if any.
    /// </summary>
    public bool UseVariance { get; init; }

    /// <summary>
    /// If enabled, use the type's accessibility modifiers ('public', 'internal', etc), if any.
    /// By convention, the 'private' one is ignored.
    /// </summary>
    public bool UseAccessibility { get; init; }

    /// <summary>
    /// If enabled, use type's standard modifiers ('static', 'abstract', etc), if any.
    /// </summary>
    public bool UseModifiers { get; init; }

    /// <summary>
    /// If enabled, use the type's kind ('class', 'record', etc), if any.
    /// </summary>
    public bool UseKind { get; init; }

    /// <summary>
    /// The style used to obtain the namespace of the type, if any.
    /// </summary>
    public EasyNamespace NamespaceStyle { get; init; }

    /// <summary>
    /// If enabled, use the type's host.
    /// </summary>
    public bool UseHost { get; init; }

    /// <summary>
    /// If enabled, use the type's special name, if possible.
    /// </summary>
    public bool UseSpecialNames { get; init; }

    /// <summary>
    /// If enabled, remove the 'Attribute' suffix, if any.
    /// </summary>
    public bool RemoveAttributeSuffix { get; init; }

    /// <summary>
    /// The style to use with nullability annotations and wrappers.
    /// </summary>
    public EasyNullable NullableStyle { get; init; }

    /// <summary>
    /// If not <see langword="null"/>, then the options to use to obtain the list of generic type
    /// arguments of the type. Otherwise, the list is ignored.
    /// </summary>
    public EasyType? GenericListOptions { get; init; }

    // ----------------------------------------------------

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="other"></param>
    /// <returns></returns>
    public virtual bool Equals(EasyType? other)
    {
        if (other is null) return false;

        if (UsePlaceHolder != other.UsePlaceHolder ||
            UseVariance != other.UseVariance ||
            UseAccessibility != other.UseAccessibility ||
            UseModifiers != other.UseModifiers ||
            UseKind != other.UseKind ||
            NamespaceStyle != other.NamespaceStyle ||
            UseHost != other.UseHost ||
            UseSpecialNames != other.UseSpecialNames ||
            RemoveAttributeSuffix != other.RemoveAttributeSuffix ||
            NullableStyle != other.NullableStyle ||
            !GenericListOptions.EqualsEx(other.GenericListOptions))
            return false;

        return true;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    public override int GetHashCode()
    {
        var code = HashCode.Combine(UsePlaceHolder);
        code = HashCode.Combine(code, UseVariance);
        code = HashCode.Combine(code, UseAccessibility);
        code = HashCode.Combine(code, UseModifiers);
        code = HashCode.Combine(code, UseKind);
        code = HashCode.Combine(code, NamespaceStyle);
        code = HashCode.Combine(code, UseHost);
        code = HashCode.Combine(code, UseSpecialNames);
        code = HashCode.Combine(code, RemoveAttributeSuffix);
        code = HashCode.Combine(code, NullableStyle);
        code = HashCode.Combine(code, GenericListOptions?.GetHashCode() ?? 0);
        return code;
    }

    // ----------------------------------------------------

    /// <summary>
    /// A shared empty instance.
    /// </summary>
    public static EasyType Empty { get; } = new(BuildMode.Empty);

    /// <summary>
    /// A shared instance with default values:
    /// <br/>- Use special names.
    /// <br/>- Removes attribute suffix.
    /// <br/>- Use nullability annotations.
    /// <br/>- Use default options with generic lists.
    /// </summary>
    public static EasyType Default { get; } = new(BuildMode.Default);

    /// <summary>
    /// A shared instance with full values.
    /// </summary>
    public static EasyType Full { get; } = new(BuildMode.Full);

    /// <summary>
    /// Enumerates the build modes.
    /// </summary>
    enum BuildMode { Empty, Default, Full };
    EasyType(BuildMode mode)
    {
        UsePlaceHolder = false; // Always false except explicitly set
        UseVariance = false;
        UseAccessibility = false;
        UseModifiers = false;
        UseKind = false;
        NamespaceStyle = EasyNamespace.None;
        UseHost = false;
        UseSpecialNames = false;
        RemoveAttributeSuffix = false;
        NullableStyle = EasyNullable.None;
        GenericListOptions = null;

        switch (mode)
        {
            case BuildMode.Default:
                UseSpecialNames = true;
                RemoveAttributeSuffix = true;
                NullableStyle = EasyNullable.Annotate;
                GenericListOptions = this;
                break;

            case BuildMode.Full:
                UseVariance = true;
                UseAccessibility = true;
                UseModifiers = true;
                UseKind = true;
                NamespaceStyle = EasyNamespace.Complete;
                UseHost = true;
                NullableStyle = EasyNullable.KeepWrappers;
                GenericListOptions = this;
                break;
        }
    }

    // ----------------------------------------------------

    /// <summary>
    /// Returns a new instance where the given values which are not-null ones have been set to
    /// the returned instance, and to its generic list options (provided it is not a null one).
    /// </summary>
    /// <returns></returns>
    public EasyType With(
        bool? usePlaceHolder = null, bool? useVariance = null, bool? useAccessibility = null,
        bool? useModifiers = null, bool? useKind = null, EasyNamespace? namespaceStyle = null,
        bool? useHost = null, bool? useSpecialNames = null, bool? removeAttributeSuffix = null,
        EasyNullable? nullableStyle = null)
    {
        // Detecting changes...
        var changes = usePlaceHolder != null || useVariance != null || useAccessibility != null ||
            useModifiers != null || useKind != null || namespaceStyle != null || useHost != null ||
            useSpecialNames != null || removeAttributeSuffix != null || nullableStyle != null;

        // Setting values...
        var item = this; if (changes) item = item with
        {
            UsePlaceHolder = usePlaceHolder ?? this.UsePlaceHolder,
            UseVariance = useVariance ?? this.UseVariance,
            UseAccessibility = useAccessibility ?? this.UseAccessibility,
            UseModifiers = useModifiers ?? this.UseModifiers,
            UseKind = useKind ?? this.UseKind,
            NamespaceStyle = namespaceStyle ?? this.NamespaceStyle,
            UseHost = useHost ?? this.UseHost,
            UseSpecialNames = useSpecialNames ?? this.UseSpecialNames,
            RemoveAttributeSuffix = removeAttributeSuffix ?? this.RemoveAttributeSuffix,
            NullableStyle = nullableStyle ?? this.NullableStyle
        };

        var options = item.GenericListOptions;
        if (options is not null && changes) item = item with
        {
            GenericListOptions = options with
            {
                UsePlaceHolder = usePlaceHolder ?? options.UsePlaceHolder,
                UseVariance = useVariance ?? options.UseVariance,
                UseAccessibility = useAccessibility ?? options.UseAccessibility,
                UseModifiers = useModifiers ?? options.UseModifiers,
                UseKind = useKind ?? options.UseKind,
                NamespaceStyle = namespaceStyle ?? options.NamespaceStyle,
                UseHost = useHost ?? options.UseHost,
                UseSpecialNames = useSpecialNames ?? options.UseSpecialNames,
                RemoveAttributeSuffix = removeAttributeSuffix ?? options.RemoveAttributeSuffix,
                NullableStyle = nullableStyle ?? options.NullableStyle
            }
        };

        // Finishing...
        return item;
    }
}