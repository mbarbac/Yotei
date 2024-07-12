/*
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
    /// <br/> This property is not used when the member is a constructor.
    /// </summary>
    public EasyTypeOptions? UseMemberType { get; init; }

    /// <summary>
    /// If not null, the options to use with the return type of the given member.
    /// </summary>
    public EasyTypeOptions? UseMemberHost { get; init; }

    /// <summary>
    /// If not null, the options to use with the type arguments of the given member, if any.
    /// <br/> This property is only used when the member is a method.
    /// </summary>
    public EasyTypeOptions? UseMemberTypeArguments { get; init; }

    /// <summary>
    /// Use the arguments of the given member, if any.
    /// <br/> This property is only used when the member is a method or an indexer.
    /// </summary>
    public bool UseMemberArguments { get; init; }

    /// <summary>
    /// If not null, the options to use with the types of the arguments of the given member, if any.
    /// <br/> This property is only used when the member is a method or an indexer.
    /// </summary>
    public EasyTypeOptions? UseMemberArgumentsTypes { get; init; }

    /// <summary>
    /// Use the names of the arguments of the given member, if any.
    /// <br/> This property is only used when the member is a method or an indexer.
    /// </summary>
    public bool UseMemberArgumentsNames { get; init; }

    // ----------------------------------------------------

    public override string ToString() => _Debug;

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    string _Debug = nameof(EasyTypeOptions);

    // ----------------------------------------------------

    /// <summary>
    /// Initializes a new empty instance.
    /// </summary>
    /// <param name="useMemberType"></param>
    /// <param name="useMemberHost"></param>
    /// <param name="useMemberTypeArguments"></param>
    /// <param name="useMemberArguments"></param>
    /// <param name="useMemberArgumentsTypes"></param>
    /// <param name="useMemberArgumentsNames"></param>
    public EasyMemberOptions(
        EasyTypeOptions? useMemberType = null,
        EasyTypeOptions? useMemberHost = null,
        EasyTypeOptions? useMemberTypeArguments = null,
        bool useMemberArguments = false,
        EasyTypeOptions? useMemberArgumentsTypes = null,
        bool useMemberArgumentsNames = false)
    {
        UseMemberType = useMemberType;
        UseMemberHost = useMemberHost;
        UseMemberTypeArguments = useMemberTypeArguments;
        UseMemberArguments = useMemberArguments;
        UseMemberArgumentsTypes = useMemberArgumentsTypes;
        UseMemberArgumentsNames = useMemberArgumentsNames;
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
                    useMemberType: null,
                    useMemberHost: null,
                    useMemberTypeArguments: EasyTypeOptions.Default,
                    useMemberArguments: true,
                    useMemberArgumentsTypes: EasyTypeOptions.Default,
                    useMemberArgumentsNames: false);

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
                    useMemberType: EasyTypeOptions.Full,
                    useMemberHost: EasyTypeOptions.Full,
                    useMemberTypeArguments: EasyTypeOptions.Full,
                    useMemberArguments: true,
                    useMemberArgumentsTypes: EasyTypeOptions.Full,
                    useMemberArgumentsNames: true);

                _Full._Debug = $"{nameof(EasyMemberOptions)}.{nameof(Full)}";
            }
            return _Full;
        }
    }
    static EasyMemberOptions _Full = null!;
}*/