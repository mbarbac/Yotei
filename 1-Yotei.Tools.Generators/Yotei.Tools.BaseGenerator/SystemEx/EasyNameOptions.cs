namespace Yotei.Tools.BaseGenerator;

// ========================================================
/// <summary>
/// Provides options for the <see cref="EasyNameExtensions"/> methods.
/// </summary>
internal record EasyNameOptions
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
                    UseMemberArguments = true,
                };

                var prop = typeof(EasyNameOptions).GetProperty(nameof(UseTypeGenericArguments));
                prop!.SetValue(_Default, _Default);

                prop = typeof(EasyNameOptions).GetProperty(nameof(UseMemberGenericArguments));
                prop!.SetValue(_Default, _Default);

                prop = typeof(EasyNameOptions).GetProperty(nameof(UseMemberArgumentsTypes));
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

                    UseMemberArguments = true,
                    UseMemberArgumentsNames = true,
                };

                var prop = typeof(EasyNameOptions).GetProperty(nameof(UseTypeGenericArguments));
                prop!.SetValue(_Full, _Full);

                prop = typeof(EasyNameOptions).GetProperty(nameof(UseMemberType));
                prop!.SetValue(_Full, _Full);

                prop = typeof(EasyNameOptions).GetProperty(nameof(UseMemberHost));
                prop!.SetValue(_Full, _Full);

                prop = typeof(EasyNameOptions).GetProperty(nameof(UseMemberGenericArguments));
                prop!.SetValue(_Full, _Full);

                prop = typeof(EasyNameOptions).GetProperty(nameof(UseMemberArgumentsTypes));
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

    // ----------------------------------------------------

    /// <summary>
    /// If not null, the options to use with the return type of the given member.
    /// <br/> This property is ignored if the affected object is not a member.
    /// </summary>
    public EasyNameOptions? UseMemberType { get; init; }

    /// <summary>
    /// If not null, the options to use with the host of the given member.
    /// <br/> This property is ignored if the affected object is not a member.
    /// </summary>
    public EasyNameOptions? UseMemberHost { get; init; }

    /// <summary>
    /// If not null, the options to use with the generic type arguments the given member.
    /// <br/> This property is ignored if the affected object is not a member.
    /// </summary>
    public EasyNameOptions? UseMemberGenericArguments { get; init; }

    /// <summary>
    /// True to use the member arguments, if any.
    /// <br/> This property is ignored if the affected object is not a member.
    /// </summary>
    public bool UseMemberArguments { get; init; }

    /// <summary>
    /// If not null, the options to use with the types of the arguments of the given member.
    /// <br/> This property is ignored if the affected object is not a member.
    /// </summary>
    public EasyNameOptions? UseMemberArgumentsTypes { get; init; }

    /// <summary>
    /// True to use the names of the member arguments, if any.
    /// <br/> This property is ignored if the affected object is not a member.
    /// </summary>
    public bool UseMemberArgumentsNames { get; init; }
}