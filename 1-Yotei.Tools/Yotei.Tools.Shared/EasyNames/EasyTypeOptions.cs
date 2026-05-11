namespace Yotei.Tools;

// ========================================================
/// <summary>
/// Describes how to obtain a C#-alike representation of a given type-alike element.
/// </summary>
public sealed record EasyTypeOptions
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

    static long LastId = 0;
    long Id;

    // Mostly for debug purposes
    public override string ToString() => GenericListOptions == null
        ? $"#{Id}"
        : $"#{Id}({GenericListOptions.Id})";

    // Internal constructor
    public enum Mode { Empty, Default, Full };
    EasyTypeOptions(Mode mode)
    {
        Id = Interlocked.Increment(ref LastId);
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

    // ----------------------------------------------------

    // Copy constructor
    public EasyTypeOptions(EasyTypeOptions source)
    {
        Id = Interlocked.Increment(ref LastId);
        UsePlaceHolder = source.UsePlaceHolder;
        UseVariance = source.UseVariance;
        UseAccessibility = source.UseAccessibility;
        UseModifiers = source.UseModifiers;
        UseKind = source.UseKind;
        NamespaceStyle = source.NamespaceStyle;
        UseHost = source.UseHost;
        UseSpecialNames = source.UseSpecialNames;
        RemoveAttributeSuffix = source.RemoveAttributeSuffix;
        NullableStyle = source.NullableStyle;
        GenericListOptions =
            source.GenericListOptions == null ? null :
            ReferenceEquals(source, source.GenericListOptions) ? this :
            new(source.GenericListOptions);
    }

    /// <summary>
    /// Returns a new EasyTypeOptions instance whose prefix-related values have been updated with
    /// the new given values, provided they are not null ones. If not changes are required, then
    /// the original instance is returned. If recursive is enabled, then these updates are also
    /// applied to any nested GenericListOptions, unless the nested option itself is null.
    /// </summary>
    /// <param name="recursive"></param>
    /// <param name="useVariance"></param>
    /// <param name="useAccessibility"></param>
    /// <param name="useModifiers"></param>
    /// <param name="useKind"></param>
    /// <returns></returns>
    public EasyTypeOptions WithPrefixes(bool recursive,
        bool? useVariance = false,
        bool? useAccessibility = false,
        bool? useModifiers = false,
        bool? useKind = false)
    {
        var options = TryChanges(
            this,
            useVariance, useAccessibility, useModifiers, useKind);

        var item = options;
        while (recursive)
        {
            if (item.GenericListOptions == null) break;
            if (ReferenceEquals(item, item.GenericListOptions)) break;

            var temp = TryChanges(
                item.GenericListOptions,
                useVariance, useAccessibility, useModifiers, useKind);

            if (ReferenceEquals(item.GenericListOptions, temp)) break;
            item.GenericListOptions = temp;
            item = temp;
        }

        return options;

        /// <summary>
        /// Tries to apply the requested changes (if not null) to the given instance. Returns
        /// a new modified instance if any changes have been applied, or the original instance
        /// otherwise.
        /// </summary>
        static EasyTypeOptions TryChanges(EasyTypeOptions options,
            bool? useVariance = false,
            bool? useAccessibility = false,
            bool? useModifiers = false,
            bool? useKind = false)
        {
            if ((useVariance.HasValue && options.UseVariance != useVariance.Value) ||
                (useAccessibility.HasValue && options.UseAccessibility != useAccessibility.Value) ||
                (useModifiers.HasValue && options.UseModifiers != useModifiers.Value) ||
                (useKind.HasValue && options.UseKind != useKind.Value))
            {
                var xoptions = new EasyTypeOptions(options);

                if (useVariance.HasValue) xoptions.UseVariance = useVariance.Value;
                if (useAccessibility.HasValue) xoptions.UseAccessibility = useAccessibility.Value;
                if (useModifiers.HasValue) xoptions.UseModifiers = useModifiers.Value;
                if (useKind.HasValue) xoptions.UseKind = useKind.Value;
                return xoptions;
            }

            return options;
        }
    }
}
