namespace Yotei.Tools;

// ========================================================
/// <summary>
/// Describes how to obtain an alternate display string of a given element.
/// </summary>
public record SketchOptions
{
    /// <summary>
    /// If not null then the options to use with reflection type-alike elements.
    /// </summary>
    public EasyTypeOptions? TypeOptions { get; init; }

    /// <summary>
    /// If not null then the options to use with reflection method-alike elements.
    /// </summary>
    public EasyMethodOptions? MethodOptions { get; init; }

    /// <summary>
    /// If not null then the options to use with reflection parameter-alike elements.
    /// </summary>
    public EasyParameterOptions? ParameterOptions { get; init; }

    /// <summary>
    /// If not null then the options to use with reflection property-alike elements.
    /// </summary>
    public EasyPropertyOptions? PropertyOptions { get; init; }

    /// <summary>
    /// If not null then the options to use with reflection field-alike elements.
    /// </summary>
    public EasyFieldOptions? FieldOptions { get; init; }

    // ----------------------------------------------------

    /// <summary>
    /// If not null, then the options to use to obtain the element's type as a prefix.
    /// </summary>
    public EasyTypeOptions? HeadOptions { get; init; }

    /// <summary>
    /// If enabled then the suitable 'ToString' methods are ignored. This setting is mostly used
    /// when the caller wants to enforce obtaining the element' shape.
    /// </summary>
    public bool PreventToString { get; init; }

    /// <summary>
    /// If not null, then the format to use with a suitable 'ToString' method, if such exist.
    /// </summary>
    public string? FormatString { get; init; }

    /// <summary>
    /// If not null, then the format provider to use with a suitable 'ToString' method, if such
    /// exist.
    /// </summary>
    public IFormatProvider? FormatProvider { get; init; }

    /// <summary>
    /// If not null then the string used to represent null values. If null, then an empty string
    /// is used instead.
    /// </summary>
    public string? NullString { get; init; }

    /// <summary>
    /// If enabled, then obtain the shape of the element using, by default, its public components.
    /// This setting is only used when all other possible routes have not succeeded, or when using
    /// ToString has been prevented.
    /// </summary>
    public bool UseShape { get; init; }

    /// <summary>
    /// If enabled and shape is being obtained, include the private members of the element.
    /// Enabling this setting implies enabling <see cref="UseShape"/>.
    /// </summary>
    public bool UsePrivateMembers { get; init; }

    /// <summary>
    /// If enabled and shape is being obtained, include the static members of the element.
    /// Enabling this setting implies enabling <see cref="UseShape"/>.
    /// </summary>
    public bool UseStaticMembers { get; init; }

    // ----------------------------------------------------

    /// <summary>
    /// Initializes a new empty instance.
    /// </summary>
    public SketchOptions() { }

    /// <summary>
    /// A shared instance with empty-alike settings.
    /// </summary>
    public static SketchOptions Empty { get; } = new();

    /// <summary>
    /// A shared instance with default-alike settings.
    /// </summary>
    public static SketchOptions Default { get; } = new()
    {
        TypeOptions = EasyTypeOptions.Default,
        MethodOptions = EasyMethodOptions.Default,
        ParameterOptions = EasyParameterOptions.Default,
        PropertyOptions = EasyPropertyOptions.Default,
        FieldOptions = EasyFieldOptions.Default,

        NullString = "NULL",
        UseShape = true,
    };

    /// <summary>
    /// A shared instance with full-alike settings.
    /// </summary>
    public static SketchOptions Full { get; } = new()
    {
        TypeOptions = EasyTypeOptions.Full,
        MethodOptions = EasyMethodOptions.Full,
        ParameterOptions = EasyParameterOptions.Full,
        PropertyOptions = EasyPropertyOptions.Full,
        FieldOptions = EasyFieldOptions.Full,

        HeadOptions = EasyTypeOptions.Full,
        NullString = "NULL",
        UseShape = true,
        UsePrivateMembers = true,
        UseStaticMembers = true,
    };
}