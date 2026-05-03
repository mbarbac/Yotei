namespace Yotei.Tools;

// ========================================================
/// <summary>
/// Describes how to obtain an alternate string representation of a given element.
/// </summary>
public record SketchOptions
{
    /// <summary>
    /// If not null, then the options to use with reflection type-alike elements.
    /// </summary>
    public EasyTypeOptions? TypeOptions { get; set; }

    /// <summary>
    /// If not null, then the options to use with reflection method-alike elements.
    /// </summary>
    public EasyMethodOptions? MethodOptions { get; set; }

    /// <summary>
    /// If not null, then the options to use with reflection parameter-alike elements.
    /// </summary>
    public EasyParameterOptions? ParameterOptions { get; set; }

    /// <summary>
    /// If not null, then the options to use with reflection property-alike elements.
    /// </summary>
    public EasyPropertyOptions? PropertyOptions { get; set; }

    /// <summary>
    /// If not null, then the options to use with reflection field-alike elements.
    /// </summary>
    public EasyFieldOptions? FieldOptions { get; set; }

    // ----------------------------------------------------

    /// <summary>
    /// If not null, then the options to use to obtain the element's type as a prefix.
    /// </summary>
    public EasyTypeOptions? HeadOptions { get; set; }

    /// <summary>
    /// If enabled, then ignore any 'ToString' methods in the element's type or in its base list.
    /// </summary>
    public bool PreventToString { get; set; }

    /// <summary>
    /// If not null, then the format string to use with a suitable 'ToString' method, if any.
    /// </summary>
    public string? FormatString { get; set; }

    /// <summary>
    /// If not null, then the format provider to use with a suitable 'ToString' method, if any.
    /// </summary>
    public IFormatProvider? FormatProvider { get; set; }

    /// <summary>
    /// If not null, then the string used to represent NULL values. If null, then an empty string
    /// is used instead.
    /// </summary>
    public string? NullString { get; set; }

    /// <summary>
    /// If enabled, the use the element's shape if the all other ways to obtain the alternate
    /// string representation have not succeeded. Unless otherwise requested, only the public
    /// members are used.
    /// </summary>
    public bool UseShape { get; set; }

    /// <summary>
    /// If enabled and shape is used, then also include the private members.
    /// </summary>
    public bool UsePrivateMembers { get; set; }

    /// <summary>
    /// If enabled and shape is used, then also include the static members.
    /// </summary>
    public bool UseStaticMembers { get; set; }

    // ----------------------------------------------------

    public enum Mode { Empty, Default, DefaultEx, Full };
    SketchOptions(Mode mode)
    {
        TypeOptions = null;
        MethodOptions = null;
        ParameterOptions = null;
        PropertyOptions = null;
        FieldOptions = null;

        HeadOptions = null;
        PreventToString = false;
        FormatString = null;
        FormatProvider = null;
        NullString = null;
        UseShape = false;
        UsePrivateMembers = false;
        UseStaticMembers = false;

        switch (mode)
        {
            case Mode.Default:
                TypeOptions = EasyTypeOptions.Default;
                MethodOptions = EasyMethodOptions.Default;
                ParameterOptions = EasyParameterOptions.Default;
                PropertyOptions = EasyPropertyOptions.Default;
                FieldOptions = EasyFieldOptions.Default;
                NullString = "NULL";
                UseShape = true;
                break;

            case Mode.DefaultEx:
                TypeOptions = EasyTypeOptions.DefaultEx;
                MethodOptions = EasyMethodOptions.DefaultEx;
                ParameterOptions = EasyParameterOptions.DefaultEx;
                PropertyOptions = EasyPropertyOptions.DefaultEx;
                FieldOptions = EasyFieldOptions.DefaultEx;
                NullString = "NULL";
                UseShape = true;
                break;

            case Mode.Full:
                TypeOptions = EasyTypeOptions.Full;
                MethodOptions = EasyMethodOptions.Full;
                ParameterOptions = EasyParameterOptions.Full;
                PropertyOptions = EasyPropertyOptions.Full;
                FieldOptions = EasyFieldOptions.Full;

                HeadOptions = EasyTypeOptions.Full;
                NullString = "NULL";
                UseShape = true;
                UsePrivateMembers = true;
                UseStaticMembers = true;
                break;
        }
    }

    /// <summary>
    /// Initializes a new empty instance.
    /// </summary>
    public SketchOptions() : this(Mode.Empty) { }

    /// <summary>
    /// Obtains a new empty-alike instance.
    /// </summary>
    public static SketchOptions Empty => new(Mode.Empty);

    /// <summary>
    /// Obtains a new default-alike instance.
    /// </summary>
    public static SketchOptions Default => new(Mode.Default);

    /// <summary>
    /// Obtains a new default-alike instance whose type options are default extended ones.
    /// </summary>
    public static SketchOptions DefaultEx => new(Mode.DefaultEx);

    /// <summary>
    /// Obtains a new full-alike instance.
    /// </summary>
    public static SketchOptions Full => new(Mode.Full);
}