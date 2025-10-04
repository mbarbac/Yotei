namespace Yotei.Tools;

// ========================================================
/// <summary>
/// Provides options for the <see cref="EasyNameExtensions"/> methods, which are ment to return
/// C#-alike name representations of objects.
/// </summary>
public record EasyNameOptions
{
    /// <summary>
    /// A shared instance with all settings set to false or null.
    /// </summary>
    public static EasyNameOptions Empty { get; } = new();

    /// <summary>
    /// A shared instance with useful default settings:
    /// <br/>- Type: Name[K, T]
    /// <br/>- Member: Name(Type, Type)
    /// </summary>
    public static EasyNameOptions Default
    {
        get
        {
            if (_Default == null)
            {
                _Default = new()
                {
                    UseTypeNamespace = false,
                    UseTypeHost = false,
                    UseTypeName = true,
                    UseMemberName = true,
                    UseMemberArguments = true,
                    UseMemberArgumentNames = false,
                };
                var type = typeof(EasyNameOptions);
                
                var prop = type.GetProperty(nameof(UseTypeGenericArguments))!;
                prop!.SetValue(_Default, _Default);
                
                prop = type.GetProperty(nameof(UseMemberReturnType))!;
                prop!.SetValue(_Default, null);
                
                prop = type.GetProperty(nameof(UseMemberHostType))!;
                prop!.SetValue(_Default, null);
                
                prop = type.GetProperty(nameof(UseMemberGenericArguments))!;
                prop!.SetValue(_Default, _Default);
                
                prop = type.GetProperty(nameof(UseMemberArgumentTypes))!;
                prop!.SetValue(_Default, _Default);
            }
            return _Default;
        }
    }
    static EasyNameOptions _Default = null!;

    /// <summary>
    /// A shared instance with full settings enabled.
    /// </summary>
    public static EasyNameOptions Full
    {
        get
        {
            if (_Full == null)
            {
                _Full = new()
                {
                    UseTypeNamespace = true,
                    UseTypeHost = true,
                    UseTypeName = true,
                    UseMemberName = true,
                    UseMemberArguments = true,
                    UseMemberArgumentNames = true,
                };
                var type = typeof(EasyNameOptions);

                var prop = type.GetProperty(nameof(UseTypeGenericArguments))!;
                prop!.SetValue(_Full, _Full);
                
                prop = type.GetProperty(nameof(UseMemberReturnType))!;
                prop!.SetValue(_Full, _Full);
                
                prop = type.GetProperty(nameof(UseMemberHostType))!;
                prop!.SetValue(_Full, _Full);
                
                prop = type.GetProperty(nameof(UseMemberGenericArguments))!;
                prop!.SetValue(_Full, _Full);
                
                prop = type.GetProperty(nameof(UseMemberArgumentTypes))!;
                prop!.SetValue(_Full, _Full);
            }
            return _Full;
        }
    }
    static EasyNameOptions _Full = null!;

    // ----------------------------------------------------

    /// <summary>
    /// Initializes a new empty instance.
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
    /// <br/> This property is ignored if the element is not a type.
    /// </summary>
    public bool UseTypeName { get; init; }

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
    /// <br/> This property is ignored if the element is not a suitable member.
    /// </summary>
    public bool UseMemberName { get; init; }

    /// <summary>
    /// If not null, indicates that the generic arguments of the member shall be included, and
    /// provides the options to be used with them.
    /// <br/> This property is ignored if the element is not a suitable member.
    /// </summary>
    public EasyNameOptions? UseMemberGenericArguments { get; init; }

    /// <summary>
    /// If true, indicates that even if no names and no types of the arguments are included,
    /// the brackets and placeholders of that arguments shall be included.
    /// <br/> This property is ignored if the element is not a suitable member.
    /// </summary>
    public bool UseMemberArguments { get; init; }

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