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
record EasyTypeOptions
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
    public EasyNamespaceStyle NamespaceStyle { get; init; }

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
    public EasyNullableStyle NullableStyle { get; init; }

    /// <summary>
    /// If not <see langword="null"/>, then the options to use to obtain the list of generic type
    /// arguments of the type. Otherwise, the list is ignored.
    /// </summary>
    public EasyTypeOptions? GenericListOptions { get; init; }

    // ----------------------------------------------------

    /// <summary>
    /// A shared empty instance.
    /// </summary>
    public static EasyTypeOptions Empty { get; } = new(BuildMode.Empty);

    /// <summary>
    /// A shared instance with default values:
    /// <br/>- Use special names.
    /// <br/>- Removes attribute suffix.
    /// <br/>- Use nullability annotations.
    /// <br/>- Use default options with generic lists.
    /// </summary>
    public static EasyTypeOptions Default { get; } = new(BuildMode.Empty);

    /// <summary>
    /// A shared instance with full values.
    /// </summary>
    public static EasyTypeOptions Full { get; } = new(BuildMode.Empty);

    /// <summary>
    /// Enumerates the build modes.
    /// </summary>
    enum BuildMode { Empty, Default, Full};
    EasyTypeOptions(BuildMode mode)
    {
        UsePlaceHolder = false;
        UseVariance = false;
        UseAccessibility = false;
        UseModifiers = false;
        UseKind = false;
        NamespaceStyle = EasyNamespaceStyle.None;
        UseHost = false;
        UseSpecialNames = false;
        RemoveAttributeSuffix = false;
        NullableStyle = EasyNullableStyle.None;
        GenericListOptions = null;

        switch (mode)
        {
            case BuildMode.Default:
                UseSpecialNames = true;
                RemoveAttributeSuffix = true;
                NullableStyle = EasyNullableStyle.Annotate;
                GenericListOptions = this;
                break;

            case BuildMode.Full:
                UseVariance = true;
                UseAccessibility = true;
                UseModifiers = true;
                UseKind = true;
                NamespaceStyle = EasyNamespaceStyle.Global;
                UseHost = true;
                NullableStyle = EasyNullableStyle.KeepWrappers;
                GenericListOptions = this;
                break;
        }
}
}