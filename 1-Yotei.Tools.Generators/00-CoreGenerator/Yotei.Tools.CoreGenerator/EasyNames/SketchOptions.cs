namespace Yotei.Tools.CoreGenerator;

// ========================================================
/// <summary>
/// Provides options for 'Sketch' methods.
/// </summary>
internal record SketchOptions
{
    /// <summary>
    /// If not null then the options to use with reflection alike type elements.
    /// </summary>
    public EasyTypeOptions? EasyTypeOptions { get; init; }

    /// <summary>
    /// If not null then the options to use with reflection alike method elements.
    /// </summary>
    public EasyMethodOptions? EasyMethodOptions { get; init; }

    /// <summary>
    /// If not null then the options to use with reflection alike parameter elements.
    /// </summary>
    public EasyParameterOptions? EasyParameterOptions { get; init; }

    /// <summary>
    /// If not null then the options to use with reflection alike property elements.
    /// </summary>
    public EasyPropertyOptions? EasyPropertyOptions { get; init; }

    /// <summary>
    /// If not null then the options to use with reflection alike field elements.
    /// </summary>
    public EasyFieldOptions? EasyFieldOptions { get; init; }

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
    public string? NullString {  get; init; }

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

    enum Mode { Empty, Default, Full };
    SketchOptions(Mode mode)
    {
        switch (mode)
        {
            case Mode.Full:
                EasyTypeOptions = EasyTypeOptions.Full;
                EasyMethodOptions = EasyMethodOptions.Full;
                EasyParameterOptions = EasyParameterOptions.Full;
                EasyPropertyOptions = EasyPropertyOptions.Full;
                EasyFieldOptions = EasyFieldOptions.Full;
                
                HeadOptions = EasyTypeOptions.Full;
                NullString = "NULL";
                UseShape = true;
                UsePrivateMembers = true;
                UseStaticMembers = true;
                break;

            case Mode.Default:
                EasyTypeOptions = EasyTypeOptions.Default;
                EasyMethodOptions = EasyMethodOptions.Default;
                EasyParameterOptions = EasyParameterOptions.Default;
                EasyPropertyOptions = EasyPropertyOptions.Default;
                EasyFieldOptions = EasyFieldOptions.Default;

                NullString = "NULL";
                UseShape = true;
                break;
        }
    }

    /// <summary>
    /// Initializes a new empty instance.
    /// </summary>
    public SketchOptions() : this(Mode.Empty) { }

    /// <summary>
    /// A shared empty instance.
    /// </summary>
    public static SketchOptions Empty { get; } = new(Mode.Empty);

    /// <summary>
    /// A shared instance with default settings.
    /// </summary>
    public static SketchOptions Default { get; } = new(Mode.Default);

    /// <summary>
    /// A shared instance with full settings.
    /// </summary>
    public static SketchOptions Full { get; } = new(Mode.Full);
}