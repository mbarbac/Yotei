namespace Yotei.Tools;

// ========================================================
/// <summary>
/// Provides options for the 'Sketch' functionality.
/// </summary>
public record SketchOptions
{
    /// <summary>
    /// A shared read-only instance that represents empty options.
    /// </summary>
    public static SketchOptions Empty { get; } = new(Mode.Empty);

    /// <summary>
    /// A shared read-only instance that represents default options.
    /// </summary>
    public static SketchOptions Default { get; } = new(Mode.Default);

    /// <summary>
    /// A shared read-only instance that represents full options.
    /// </summary>
    public static SketchOptions Full { get; } = new(Mode.Full);

    /// <summary>
    /// Initializes a new default instance.
    /// </summary>
    public SketchOptions() : this(Mode.Default) { }

    // ----------------------------------------------------

    /// <summary>
    /// If 'true', prevents the use of the 'ToString' method of the value.
    /// </summary>
    public bool PreventToString { get; init; }

    /// <summary>
    /// If not null, the format string to use when formatting the value.
    /// </summary>
    public string? FormatString { get; init; }

    /// <summary>
    /// If not null, the format provider to use when formatting the value.
    /// </summary>
    public IFormatProvider? FormatProvider { get; init; }

    /// <summary>
    /// Determines if a head with the type of the value shall be used. Note that, if not null,
    /// the type options are used to obtain the type head.
    /// </summary>
    public bool UseTypeHead { get; init; }

    /// <summary>
    /// If not null, the literal used to represent NULL values. If null, then an empty string
    /// is used.
    /// </summary>
    public string? NullString { get; init; }

    /// <summary>
    /// If not null, the options to use when the value is a type.
    /// </summary>
    public EasyNameType? TypeOptions { get; init; }

    /// <summary>
    /// If not null, the options to use when the value is a constructor.
    /// </summary>
    public EasyNameConstructorInfo? ConstructorOptions { get; init; }

    /// <summary>
    /// If not null, the options to use when the value is a method.
    /// </summary>
    public EasyNameMethodInfo? MethodOptions { get; init; }

    /// <summary>
    /// If not null, the options to use when the value is a property.
    /// </summary>
    public EasyNamePropertyInfo? PropertyOptions { get; init; }

    /// <summary>
    /// If not null, the options to use when the value is a field.
    /// </summary>
    public EasyNameFieldInfo? FieldOptions { get; init; }

    /// <summary>
    /// If not null, the options to use when the value is a parameter.
    /// </summary>
    public EasyNameParameterInfo? ParameterOptions { get; init; }

    /// <summary>
    /// Determines if the shape of the value shall be used, if no other procedure has obtained
    /// the value sketch.
    /// </summary>
    public bool UseShape { get; init; }

    /// <summary>
    /// Determines if, when using the value shape, its public members shall be included.
    /// </summary>
    public bool UsePrivateMembers { get; init; }

    /// <summary>
    /// Determines if, when using the value shape, its static members shall be included.
    /// </summary>
    public bool UseStaticMembers { get; init; }

    // ----------------------------------------------------

    enum Mode { Empty, Default, Full };
    private SketchOptions(Mode mode)
    {
        switch (mode)
        {
            case Mode.Empty:
                break;

            case Mode.Default:
                NullString = "NULL";
                TypeOptions = EasyNameType.Default;
                ConstructorOptions = EasyNameConstructorInfo.Default;
                MethodOptions = EasyNameMethodInfo.Default;
                PropertyOptions = EasyNamePropertyInfo.Default;
                FieldOptions = EasyNameFieldInfo.Default;
                ParameterOptions = EasyNameParameterInfo.Default;
                UseShape = true;
                break;

            case Mode.Full:
                UseTypeHead = true;
                NullString = "NULL";
                TypeOptions = EasyNameType.Full;
                ConstructorOptions = EasyNameConstructorInfo.Full;
                MethodOptions = EasyNameMethodInfo.Full;
                PropertyOptions = EasyNamePropertyInfo.Full;
                FieldOptions = EasyNameFieldInfo.Full;
                ParameterOptions = EasyNameParameterInfo.Full;
                UseShape = true;
                UsePrivateMembers = true;
                UseStaticMembers = true;
                break;
        }
    }
}