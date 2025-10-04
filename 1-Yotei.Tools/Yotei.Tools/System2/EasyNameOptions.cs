namespace Yotei.Tools;

// ========================================================
/// <summary>
/// Provides options for the <see cref="EasyNameExtensions"/> methods, which are ment to return
/// C#-alike name representations of objects.
/// </summary>
public record EasyNameOptions
{
    /// <summary>
    /// A shared default instance.
    /// </summary>
    public static EasyNameOptions Default { get; } = new();

    /// <summary>
    /// A shared instance with all settings set to false or null.
    /// </summary>
    public static EasyNameOptions Empty
    {
        get
        {
            if (_Empty is null)
            {
                _Empty = new()
                {
                    UseTypeNamespace = false,
                    UseTypeHost = false,
                    UseTypeName = false,
                    UseMemberName = false,
                    UseMemberArgumentNames = false,
                };
            }
            return _Empty;
        }
    }
    static EasyNameOptions _Empty = default!;

    /// <summary>
    /// A shared instance with full settings enabled.
    /// </summary>
    public static EasyNameOptions Full
    {
        get
        {
            if (_Full is null)
            {
                _Full = new()
                {
                    UseTypeNamespace = true,
                    UseTypeHost = true,
                    UseTypeName = true,
                    UseMemberName = true,
                    UseMemberArgumentNames = true,
                };
                var type = typeof(EasyNameOptions);

                var prop = type.GetProperty(nameof(UseTypeGenericArguments));
                prop!.SetValue(_Full, _Full);

                prop = type.GetProperty(nameof(UseMemberReturnType));
                prop!.SetValue(_Full, _Full);

                prop = type.GetProperty(nameof(UseMemberHostType));
                prop!.SetValue(_Full, _Full);

                prop = type.GetProperty(nameof(UseMemberGenericArguments));
                prop!.SetValue(_Full, _Full);

                prop = type.GetProperty(nameof(UseMemberArgumentTypes));
            }
            return _Full;
        }
    }
    static EasyNameOptions _Full = default!;

    // ----------------------------------------------------

    /// <summary>
    /// Initializes a new default instance.
    /// </summary>
    public EasyNameOptions() { }

    /// <summary>
    /// If true, indicates that the namespace of the given type shall be included.
    /// <br/> This property is ignored if the element is not a type.
    /// </summary>
    public bool UseTypeNamespace { get; init; }

    /// <summary>
    /// If true, indicates that the host of the given type shall be included.
    /// <br/> This property is ignored if the element is not a type.
    /// </summary>
    public bool UseTypeHost { get; init; }

    /// <summary>
    /// If true, indicates that the name of the given type shall be included, which is
    /// usefull to control the appearance of generic types.
    /// <br/> The default value of this property is '<c>true</c>'.
    /// <br/> This property is ignored if the element is not a type.
    /// </summary>
    public bool UseTypeName { get; init; } = true;

    /// <summary>
    /// If not null, indicates that the generic arguments of the given type shall be included,
    /// and provides the options to be used with them.
    /// <br/> This property is ignored if the element is not a type.
    /// </summary>
    public EasyNameOptions? UseTypeGenericArguments { get; init; }

    // ----------------------------------------------------

    /// <summary>
    /// If not null, indicates that the member return type shall be included, and provides the
    /// options to be used with it.
    /// <br/> This property is ignored if the element is not a suitable member.
    /// </summary>
    public EasyNameOptions? UseMemberReturnType { get; init; }

    /// <summary>
    /// If not null, indicates that the host type of the member type shall be included, and provides
    /// the options to be used with it.
    /// <br/> This property is ignored if the element is not a suitable member.
    /// </summary>
    public EasyNameOptions? UseMemberHostType { get; init; }

    /// <summary>
    /// If true, indicates that the name of the member shall be included, which is usefull to
    /// control the appearance of arguments of methods and properties.
    /// <br/> The default value of this property is '<c>true</c>'.
    /// <br/> This property is ignored if the element is not a suitable member.
    /// </summary>
    public bool UseMemberName { get; init; } = true;

    /// <summary>
    /// If not null, indicates that the generic arguments of the member shall be included, and
    /// provides the options to be used with them.
    /// <br/> This property is ignored if the element is not a suitable member.
    /// </summary>
    public EasyNameOptions? UseMemberGenericArguments { get; init; }

    /// <summary>
    /// If not null, indicates that the types of the arguments of the member shall be included,
    /// and provides the options to be used with them.
    /// <br/> This property is ignored if the element is not a suitable member.
    /// </summary>
    public EasyNameOptions? UseMemberArgumentTypes { get; init; }

    /// <summary>
    /// If true, indicates that the names of the arguments of the member shall be included.
    /// <br/> This property is ignored if the element is not a suitable member.
    /// </summary>
    public bool UseMemberArgumentNames { get; init; }
}