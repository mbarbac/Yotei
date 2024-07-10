#pragma warning disable IDE0017
namespace Yotei.Tools;

// ========================================================
/// <summary>
/// Provides options for the <see cref="EasyNameExtensions"/> methods.
/// </summary>
public record EasyMemberOptions
{
    /// <summary>
    /// If not null, the options to use with the return type of the given member.
    /// </summary>
    public EasyTypeOptions? UseType { get; init; }

    /// <summary>
    /// If not null, the options to use with the return type of the given member.
    /// </summary>
    public EasyTypeOptions? UseHost { get; init; }

    /// <summary>
    /// If not null, the options to use with the type arguments of the given member, if any.
    /// </summary>
    public EasyTypeOptions? UseTypeArguments { get; init; }

    /// <summary>
    /// Use the arguments of the given member, if any.
    /// </summary>
    public bool UseArguments { get; init; }

    /// <summary>
    /// If not null, the options to use with the types of the arguments of the given member, if any.
    /// </summary>
    public EasyTypeOptions? UseArgumentsTypes { get; init; }

    /// <summary>
    /// Use the names of the arguments of the given member, if any.
    /// </summary>
    public bool UseArgumentsNames { get; init; }

    // ----------------------------------------------------

    public override string ToString() => _Debug;

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    string _Debug = nameof(EasyTypeOptions);

    // ----------------------------------------------------

    /// <summary>
    /// Initializes a new empty instance.
    /// </summary>
    public EasyMemberOptions(
        EasyTypeOptions? useType = null,
        EasyTypeOptions? useHost = null,
        EasyTypeOptions? useTypeArguments = null,
        bool useArguments = false,
        EasyTypeOptions? useArgumentsTypes = null,
        bool useArgumentsNames = false)
    {
        UseType = useType;
        UseHost = useHost;
        UseTypeArguments = useTypeArguments;
        UseArguments = useArguments;
        UseArgumentsTypes = useArgumentsTypes;
        UseArgumentsNames = useArgumentsNames;
    }

    /// <summary>
    /// A shared empty instance.
    /// </summary>
    public static EasyMemberOptions Empty { get; } = new();

    /// <summary>
    /// A shared default instance.
    /// </summary>
    public static EasyMemberOptions Default
    {
        get
        {
            if (_Default == null)
            {
                _Default = new(
                    useType: null,
                    useHost: null,
                    useTypeArguments: EasyTypeOptions.Default,
                    useArguments: true,
                    useArgumentsTypes: EasyTypeOptions.Default,
                    useArgumentsNames: false);

                _Default._Debug = $"{nameof(EasyMemberOptions)}.{nameof(Default)}";
            }
            return _Default;
        }
    }
    static EasyMemberOptions _Default = null!;

    /// <summary>
    /// A shared full instance.
    /// </summary>
    public static EasyMemberOptions Full
    {
        get
        {
            if (_Full == null)
            {
                _Full = new(
                    useType: EasyTypeOptions.Full,
                    useHost: EasyTypeOptions.Full,
                    useTypeArguments: EasyTypeOptions.Full,
                    useArguments: true,
                    useArgumentsTypes: EasyTypeOptions.Full,
                    useArgumentsNames: true);

                _Full._Debug = $"{nameof(EasyMemberOptions)}.{nameof(Full)}";
            }
            return _Full;
        }
    }
    static EasyMemberOptions _Full = null!;
}