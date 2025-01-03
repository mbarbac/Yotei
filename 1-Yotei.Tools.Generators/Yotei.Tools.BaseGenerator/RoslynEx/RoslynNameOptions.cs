namespace Yotei.Tools.BaseGenerator;

// ========================================================
/// <summary>
/// Provides options for the <see cref="EasyNameExtensions"/> methods.
/// </summary>
internal record RoslynNameOptions
{
    /// <summary>
    /// Initializes a new empty instance.
    /// </summary>
    public RoslynNameOptions() { }

    /// <summary>
    /// A shared empty instance.
    /// </summary>
    public static RoslynNameOptions Empty { get; } = new();

    /// <summary>
    /// A shared instance with default settings.
    /// </summary>
    public static RoslynNameOptions Default
    {
        get
        {
            if (_Default == null)
            {
                _Default = new RoslynNameOptions
                {
                    UseTypeName = true,
                    UseTypeNullable = true,
                    UseMemberArguments = true,
                };

                var prop = typeof(RoslynNameOptions).GetProperty(nameof(UseTypeGenericArguments));
                prop!.SetValue(_Default, _Default);

                prop = typeof(RoslynNameOptions).GetProperty(nameof(UseMemberGenericArguments));
                prop!.SetValue(_Default, _Default);

                prop = typeof(RoslynNameOptions).GetProperty(nameof(UseMemberArgumentsTypes));
                prop!.SetValue(_Default, _Default);
            }
            return _Default;
        }
    }
    static RoslynNameOptions _Default = default!;

    /// <summary>
    /// A shared instance with full settings.
    /// </summary>
    public static RoslynNameOptions Full
    {
        get
        {
            if (_Full == null)
            {
                _Full = new RoslynNameOptions
                {
                    UseTypeNamespace = true,
                    UseTypeHost = true,
                    UseTypeName = true,
                    UseTypeNullable = true,

                    UseMemberArguments = true,
                    UseMemberArgumentsNames = true,
                };

                var prop = typeof(RoslynNameOptions).GetProperty(nameof(UseTypeGenericArguments));
                prop!.SetValue(_Full, _Full);

                prop = typeof(RoslynNameOptions).GetProperty(nameof(UseMemberType));
                prop!.SetValue(_Full, _Full);

                prop = typeof(RoslynNameOptions).GetProperty(nameof(UseMemberHost));
                prop!.SetValue(_Full, _Full);

                prop = typeof(RoslynNameOptions).GetProperty(nameof(UseMemberGenericArguments));
                prop!.SetValue(_Full, _Full);

                prop = typeof(RoslynNameOptions).GetProperty(nameof(UseMemberArgumentsTypes));
                prop!.SetValue(_Full, _Full);
            }
            return _Full;
        }
    }
    static RoslynNameOptions _Full = default!;

    // ----------------------------------------------------

    /// <summary>
    /// Conversion operator.
    /// </summary>
    /// <param name="options"></param>
    public static implicit operator EasyNameOptions(
        RoslynNameOptions options) => options.ThrowWhenNull().ToEasyNameOptions();

    /// <summary>
    /// Returns a new instance based upon the values carried by this one,.
    /// </summary>
    /// <returns></returns>
    public EasyNameOptions ToEasyNameOptions() => new EasyNameOptions
    {
        UseTypeNamespace = UseTypeNamespace,
        UseTypeHost = UseTypeHost,
        UseTypeName = UseTypeName,
        UseTypeGenericArguments = UseTypeGenericArguments?.ToEasyNameOptions(),

        UseMemberType = UseMemberType?.ToEasyNameOptions(),
        UseMemberHost = UseMemberHost?.ToEasyNameOptions(),
        UseMemberGenericArguments = UseMemberGenericArguments?.ToEasyNameOptions(),
        UseMemberArguments = UseMemberArguments,
        UseMemberArgumentsTypes = UseMemberArgumentsTypes?.ToEasyNameOptions(),
        UseMemberArgumentsNames = UseMemberArgumentsNames,
    };

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
    /// Adds the nullable annotation, if any, to the type name.
    /// </summary>
    public bool UseTypeNullable { get; init; }

    /// <summary>
    /// If not null, the options to use with the generic type arguments of the given type.
    /// <br/> This property is ignored if the affected object is not a type.
    /// </summary>
    public RoslynNameOptions? UseTypeGenericArguments { get; init; }

    // ----------------------------------------------------

    /// <summary>
    /// If not null, the options to use with the return type of the given member.
    /// <br/> This property is ignored if the affected object is not a member.
    /// </summary>
    public RoslynNameOptions? UseMemberType { get; init; }

    /// <summary>
    /// If not null, the options to use with the host of the given member.
    /// <br/> This property is ignored if the affected object is not a member.
    /// </summary>
    public RoslynNameOptions? UseMemberHost { get; init; }

    /// <summary>
    /// If not null, the options to use with the generic type arguments the given member.
    /// <br/> This property is ignored if the affected object is not a member.
    /// </summary>
    public RoslynNameOptions? UseMemberGenericArguments { get; init; }

    /// <summary>
    /// True to use the member arguments, if any.
    /// <br/> This property is ignored if the affected object is not a member.
    /// </summary>
    public bool UseMemberArguments { get; init; }

    /// <summary>
    /// If not null, the options to use with the types of the arguments of the given member.
    /// <br/> This property is ignored if the affected object is not a member.
    /// </summary>
    public RoslynNameOptions? UseMemberArgumentsTypes { get; init; }

    /// <summary>
    /// True to use the names of the member arguments, if any.
    /// <br/> This property is ignored if the affected object is not a member.
    /// </summary>
    public bool UseMemberArgumentsNames { get; init; }
}
