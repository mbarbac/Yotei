using System.Net;

namespace Yotei.Tools;

// ========================================================
/// <summary>
/// Describes how to obtain a C#-alike representation of a given type-alike element.
/// </summary>
public record EasyTypeOptions
{
    /// <summary>
    /// If enabled, then the returned easy name will always be an empty one. This setting is
    /// moslty used when the intent is to obtain an anonymous list of generic type arguments.
    /// </summary>
    public bool UsePlaceHolder { get; set; }

    /// <summary>
    /// If enabled, then use the type's variance (the 'in' and 'out' keywords), if any.
    /// </summary>
    public bool UseVariance { get; set; }

    /// <summary>
    /// If enabled, then use the accessibility modifiers, if any.
    /// </summary>
    public bool UseAccessibility { get; set; }

    /// <summary>
    /// If enabled, then use the element's modifiers (such as static, abstract, etc). Otherwise,
    /// they are ignored.
    /// </summary>
    public bool UseModifiers { get; set; }

    /// <summary>
    /// If enabled, then use the element's kind (such as class, record, struct, interface and
    /// enum).
    /// </summary>
    public bool UseKind { get; set; }

    /// <summary>
    /// The style to use to obtain the namespace, if any, of the given type.
    /// <br/> A not-empty value of this property implies <see cref="UseHost"/>.
    /// </summary>
    public EasyNamespaceStyle NamespaceStyle { get; set; }

    /// <summary>
    /// If enabled, then use the type's host (unless the type it is an special one and the
    /// <see cref="UseSpecialNames"/> setting is enabled).
    /// </summary>
    public bool UseHost { get; set; }

    /// <summary>
    /// If enabled, then use the type's special name, if possible.
    /// </summary>
    public bool UseSpecialNames { get; set; }

    /// <summary>
    /// If enabled, then remove the 'Attribute' suffix, if any.
    /// </summary>
    public bool RemoveAttributeSuffix { get; set; }

    /// <summary>
    /// The style to use when the given type is a nullable one.
    /// </summary>
    public EasyNullableStyle NullableStyle { get; set; }

    /// <summary>
    /// If not null, then the use these options to obtain the list of generic type arguments.
    /// If null, then that list is ignored.
    /// <para>
    /// When not null then the value of this property typically refers to its own host instance,
    /// and so recursively. If you modify the <see cref="GenericListOptions"/> property of this
    /// property, it is expected you keep an appropriate recursion.
    /// <br/>- For instance, to obtain an anonymous list, the new value either must have its own
    /// <see cref="UsePlaceHolder"/> value set to true, or the new value itself points to its
    /// own host instance.
    /// </para>
    /// </summary>
    public EasyTypeOptions? GenericListOptions { get; set; }

    // ----------------------------------------------------

    public enum Mode { Empty, Default, Full };
    EasyTypeOptions(Mode mode)
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
            case Mode.Default:
                UseSpecialNames = true;
                RemoveAttributeSuffix = true;
                NullableStyle = EasyNullableStyle.UseAnnotations;
                GenericListOptions = this;
                break;

            case Mode.Full:
                UseVariance = true;
                UseAccessibility = true;
                UseModifiers = true;
                UseKind = true;
                NamespaceStyle = EasyNamespaceStyle.Default;
                UseHost = true;
                NullableStyle = EasyNullableStyle.KeepWrappers;
                GenericListOptions = this;
                break;
        }
    }

    /// <summary>
    /// Initializes a new empty instance.
    /// </summary>
    public EasyTypeOptions() : this(Mode.Empty) { }

    /// <summary>
    /// Obtains a new instance with empty-alike values that obtains the bare minimum display
    /// string.
    /// </summary>
    public static EasyTypeOptions Empty => new(Mode.Empty);

    /// <summary>
    /// Obtains a new instance with default-alike values that obtains the most common display
    /// string, without any modifiers.
    /// </summary>
    public static EasyTypeOptions Default => new(Mode.Default);

    /// <summary>
    /// Obtains a new instance with full-alike values that obtains a full display string including
    /// its kind and modifiers.
    /// </summary>
    public static EasyTypeOptions Full => new(Mode.Full);
}

// ========================================================
public static partial class EasyNameExtensions
{
    /// <summary>
    /// Returns a new EasyTypeOptions instance with updated prefix-related options, provided the
    /// given values are not null ones. This method does not modify the original EasyTypeOptions
    /// instance unless no changes are required. When recursive is true, the specified options are
    /// also applied recursively to any nested GenericListOptions.
    /// </summary>
    /// <param name="options">The EasyTypeOptions instance to modify.</param>
    /// <param name="recursive">true to apply the specified prefix options recursively to any nested generic list options; otherwise, false.</param>
    /// <param name="useVariance">Specifies whether to enable or disable variance prefixes. If null, the current setting is retained.</param>
    /// <param name="useAccessibility">Specifies whether to enable or disable accessibility prefixes. If null, the current setting is retained.</param>
    /// <param name="useModifiers">Specifies whether to enable or disable modifier prefixes. If null, the current setting is retained.</param>
    /// <param name="useKind">Specifies whether to enable or disable kind prefixes. If null, the current setting is retained.</param>
    /// <returns>A new EasyTypeOptions instance with the specified prefix options applied. If no changes are made, returns the
    /// original instance.</returns>
    /// <exception cref="UnExpectedException">Thrown if an unexpected error occurs while creating a new EasyTypeOptions instance.</exception>
    public static EasyTypeOptions WithPrefixes(this EasyTypeOptions options, bool recursive,
        bool? useVariance = false,
        bool? useAccessibility = false,
        bool? useModifiers = false,
        bool? useKind = false)
    {
        if ((!useVariance.HasValue || (options.UseVariance == useVariance.Value)) &&
            (!useAccessibility.HasValue || (options.UseAccessibility == useAccessibility.Value)) &&
            (!useModifiers.HasValue || (options.UseModifiers == useModifiers.Value)) &&
            (!useKind.HasValue || (options.UseKind == useKind.Value)))
            return options;

        var xoptions = options with { };
        if (ReferenceEquals(options, xoptions)) throw new UnExpectedException();

        if (useVariance.HasValue) xoptions.UseVariance = useVariance.Value;
        if (useAccessibility.HasValue) xoptions.UseAccessibility= useAccessibility.Value;
        if (useModifiers.HasValue) xoptions.UseModifiers= useModifiers.Value;
        if (useKind.HasValue) xoptions.UseKind= useKind.Value;

        if (recursive &&
            options.GenericListOptions != null &&
            !ReferenceEquals(options, options.GenericListOptions))
            options.GenericListOptions = options.GenericListOptions.WithPrefixes(
                recursive,
                useVariance,
                useAccessibility,
                useModifiers,
                useKind);

        return xoptions;
    }
}