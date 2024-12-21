namespace Yotei.Tools;

// ========================================================
/// <summary>
/// Provides options for the <see cref="EasyNameExtensions"/> methods.
/// </summary>
public record EasyNameOptions
{
    /// <summary>
    /// Initializes a new empty instance.
    /// </summary>
    public EasyNameOptions() { }

    /// <summary>
    /// A shared empty instance.
    /// </summary>
    public static EasyNameOptions Empty { get; } = new();

    /// <summary>
    /// A shared instance with default settings.
    /// </summary>
    public static EasyNameOptions Default
    {
        get
        {
            if (_Default == null)
            {
                _Default = new EasyNameOptions
                {
                    UseTypeName = true,
                };

                var prop = typeof(EasyNameOptions).GetProperty(nameof(UseTypeGenericArguments));
                prop!.SetValue(_Default, _Default);
            }
            return _Default;
        }
    }
    static EasyNameOptions _Default = default!;

    /// <summary>
    /// A shared instance with full settings.
    /// </summary>
    public static EasyNameOptions Full
    {
        get
        {
            if (_Full == null)
            {
                _Full = new EasyNameOptions
                {
                    UseTypeNamespace = true,
                    UseTypeHost = true,
                    UseTypeName = true,
                };

                var prop = typeof(EasyNameOptions).GetProperty(nameof(UseTypeGenericArguments));
                prop!.SetValue(_Full, _Full);
            }
            return _Full;
        }
    }
    static EasyNameOptions _Full = default!;

    // ----------------------------------------------------

    /// <summary>
    /// Use the namespace of the given type.
    /// <br/> This property is ignored if the affected object is not a type.
    /// </summary>
    public bool UseTypeNamespace { get; init; }

    /// <summary>
    /// Use the declaring host of the given type.
    /// <br/> This property is ignored if the affected object is not a type.
    /// </summary>
    public bool UseTypeHost { get; init; }

    /// <summary>
    /// Use the name of the given type.
    /// <br/> This property is ignored if the affected object is not a type.
    /// </summary>
    public bool UseTypeName { get; init; }

    /// <summary>
    /// If not null, the options to use with the generic type arguments of the given type.
    /// <br/> This property is ignored if the affected object is not a type.
    /// </summary>
    public EasyNameOptions? UseTypeGenericArguments { get; init; }
}