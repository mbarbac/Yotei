    namespace Yotei.Tools.Generators.Internal;

// ========================================================
/// <summary>
/// Provides options for the <see cref="EasyNameExtensions"/> methods.
/// </summary>
internal record EasyNameOptions
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
    /// Add the nullable annotation to the type name, if any.
    /// </summary>
    public bool UseTypeNullable { get; init; }

    /// <summary>
    /// If not null, the options to use with the type arguments of the given type.
    /// </summary>
    public EasyNameOptions? UseTypeArguments { get; init; }

    // -----------------------------------------------------

    /// <summary>
    /// If not null, the options to use with the return type of the given member.
    /// </summary>
    public EasyNameOptions? UseMemberType { get; init; }

    /// <summary>
    /// If not null, the options to use with the return type of the given member.
    /// </summary>
    public EasyNameOptions? UseMemberHost { get; init; }

    /// <summary>
    /// If not null, the options to use with the type arguments of the given member, if any.
    /// </summary>
    public EasyNameOptions? UseMemberTypeArguments { get; init; }

    /// <summary>
    /// Use the arguments of the given member, if any.
    /// </summary>
    public bool UseMemberArguments { get; init; }

    /// <summary>
    /// If not null, the options to use with the types of the arguments of the member, if any.
    /// </summary>
    public EasyNameOptions? UseMemberArgumentsTypes { get; init; }

    /// <summary>
    /// Use the names of the arguments of the given member, if any.
    /// </summary>
    public bool UseMemberArgumentsNames { get; init; }

    // -----------------------------------------------------

    /// <summary>
    /// Initializes a new empty instance.
    /// </summary>
    public EasyNameOptions()
    {
        UseTypeNamespace = false;
        UseTypeHost = false;
        UseTypeName = false;
        UseTypeNullable = false;
        UseTypeArguments = null;

        UseMemberType = null;
        UseMemberHost = null;
        UseMemberTypeArguments = null;
        UseMemberArguments = false;
        UseMemberArgumentsTypes = null;
        UseMemberArgumentsNames = false;
    }

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
                    UseTypeNamespace = false,
                    UseTypeHost = false,
                    UseTypeName = true,
                    UseTypeNullable = true,

                    UseMemberType = null,
                    UseMemberHost = null,
                    UseMemberArguments = true,
                    UseMemberArgumentsNames = false,
                };

                var prop = typeof(EasyNameOptions).GetProperty(nameof(UseTypeArguments));
                prop!.SetValue(_Default, _Default);

                prop = typeof(EasyNameOptions).GetProperty(nameof(UseMemberTypeArguments));
                prop!.SetValue(_Default, _Default);

                prop = typeof(EasyNameOptions).GetProperty(nameof(UseMemberArgumentsTypes));
                prop!.SetValue(_Default, _Default);
            }
            return _Default;
        }
    }
    static EasyNameOptions _Default = null!;

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
                    UseTypeNullable = true,

                    UseMemberArguments = true,
                    UseMemberArgumentsNames = true,
                };

                var prop = typeof(EasyNameOptions).GetProperty(nameof(UseTypeArguments));
                prop!.SetValue(_Full, _Full);

                prop = typeof(EasyNameOptions).GetProperty(nameof(UseMemberType));
                prop!.SetValue(_Full, _Full);

                prop = typeof(EasyNameOptions).GetProperty(nameof(UseMemberHost));
                prop!.SetValue(_Full, _Full);

                prop = typeof(EasyNameOptions).GetProperty(nameof(UseMemberTypeArguments));
                prop!.SetValue(_Full, _Full);

                prop = typeof(EasyNameOptions).GetProperty(nameof(UseMemberArgumentsTypes));
                prop!.SetValue(_Full, _Full);
            }
            return _Full;
        }
    }
    static EasyNameOptions _Full = null!;
}