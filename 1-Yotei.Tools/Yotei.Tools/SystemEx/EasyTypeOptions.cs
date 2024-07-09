namespace Yotei.Tools;

// ========================================================
/// <summary>
/// Provides options for the <see cref="EasyNameExtensions"/> methods.
/// </summary>
public record EasyTypeOptions
{
    /// <summary>
    /// Use the namespace of the given type.
    /// </summary>
    public bool UseNamespace { get; init; }

    /// <summary>
    /// Use the declaring host of the given type.
    /// </summary>
    public bool UseHost { get; init; }

    /// <summary>
    /// When the type is a type argument, whether to use its name or not.
    /// </summary>
    public bool UseName { get; init; }

    /// <summary>
    /// If not null, the options to use with the type arguments of the given type.
    /// </summary>
    public EasyTypeOptions? UseTypeArguments { get; init; }

    // ----------------------------------------------------

    /// <summary>
    /// Initializes a new empty instance.
    /// </summary>
    /// <param name="useNamespace"></param>
    /// <param name="useHost"></param>
    /// <param name="useName"></param>
    /// <param name="useTypeArguments"></param>
    public EasyTypeOptions(
        bool useNamespace = false,
        bool useHost = false,
        bool useName = false,
        EasyTypeOptions? useTypeArguments = null)
    {
        UseNamespace = useNamespace;
        UseHost = useHost;
        UseName = useName;
        UseTypeArguments = useTypeArguments;
    }

    /// <summary>
    /// A shared empty instance.
    /// </summary>
    public static EasyTypeOptions Empty { get; } = new();

    /// <summary>
    /// A shared default instance.
    /// </summary>
    public static EasyTypeOptions Default
    {
        get
        {
            if (_Default == null)
            {
                _Default = new();

                var prop = typeof(EasyTypeOptions).GetProperty(nameof(UseTypeArguments));
                prop!.SetValue(_Default, _Default);
            }
            return _Default;
        }
    }
    static EasyTypeOptions _Default = null!;

    /// <summary>
    /// A shared full instance.
    /// </summary>
    public static EasyTypeOptions Full
    {
        get
        {
            if (_Full == null)
            {
                _Full = new(
                    useNamespace: true,
                    useHost: true,
                    useName: true);

                var prop = typeof(EasyTypeOptions).GetProperty(nameof(UseTypeArguments));
                prop!.SetValue(_Full, _Full);
            }
            return _Full;
        }
    }
    static EasyTypeOptions _Full = null!;
}