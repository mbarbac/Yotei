#pragma warning disable IDE0017

namespace Yotei.Tools.Generators.Internal;

// ========================================================
/// <summary>
/// Provides options for the <see cref="EasyNameExtensions"/> methods.
/// </summary>
internal record EasyTypeOptions
{
    /// <summary>
    /// Use the namespace of the given type.
    /// </summary>
    public bool UseTypeNamespace { get; init; }

    /// <summary>
    /// Use the declaring host of the given type.
    /// </summary>
    public bool UseTypeHost { get; init; }

    /// <summary>
    /// When the type is a type argument, whether to use its name or not.
    /// </summary>
    public bool UseTypeName { get; init; }

    /// <summary>
    /// Whether to add the nullable annotation, if any, or not.
    /// </summary>
    public bool AddNullable { get; init; }

    /// <summary>
    /// If not null, the options to use with the type arguments of the given type.
    /// </summary>
    public EasyTypeOptions? UseTypeArguments { get; init; }

    // ----------------------------------------------------

    public override string ToString() => _Debug;

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    string _Debug = nameof(EasyTypeOptions);

    // ----------------------------------------------------

    /// <summary>
    /// Initializes a new empty instance.
    /// </summary>
    /// <param name="useTypeNamespace"></param>
    /// <param name="useTypeHost"></param>
    /// <param name="useTypeName"></param>
    /// <param name="addNullable"></param>
    /// <param name="useTypeArguments"></param>
    public EasyTypeOptions(
        bool useTypeNamespace = false,
        bool useTypeHost = false,
        bool useTypeName = false,
        bool addNullable = false,
        EasyTypeOptions? useTypeArguments = null)
    {
        UseTypeNamespace = useTypeNamespace;
        UseTypeHost = useTypeHost;
        UseTypeName = useTypeName;
        AddNullable = addNullable;
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
                _Default = new(
                    useTypeNamespace: false,
                    useTypeHost: false,
                    useTypeName: true,
                    addNullable: true);

                _Default._Debug = $"{nameof(EasyTypeOptions)}.{nameof(Default)}";

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
                    useTypeNamespace: true,
                    useTypeHost: true,
                    useTypeName: true,
                    addNullable: true);

                _Full._Debug = $"{nameof(EasyTypeOptions)}.{nameof(Full)}";

                var prop = typeof(EasyTypeOptions).GetProperty(nameof(UseTypeArguments));
                prop!.SetValue(_Full, _Full);
            }
            return _Full;
        }
    }
    static EasyTypeOptions _Full = null!;
}