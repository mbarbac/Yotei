namespace Yotei.Tools;

// ========================================================
/// <summary>
/// Provides options for the <see cref="EasyNameExtensions"/> methods.
/// </summary>
public record EasyNameOptions
{
    /// <summary>
    /// Use the namespace of the given type.
    /// </summary>
    public bool UseTypeNamespace { get; init; }

    /// <summary>
    /// Use the declaring host type of the given one.
    /// </summary>
    public bool UseTypeHost { get; init; }

    /// <summary>
    /// Use the type short name, or leave it blank.
    /// </summary>
    public bool UseTypeName { get; init; }

    /// <summary>
    /// Use the generic type arguments, if any.
    /// </summary>
    public bool UseTypeArguments { get; init; }

    // ----------------------------------------------------

    /// <summary>
    /// Use the member type.
    /// </summary>
    public bool UseMemberType { get; init; }

    /// <summary>
    /// Use the member declaring host type.
    /// </summary>
    public bool UseMemberHost { get; init; }

    /// <summary>
    /// Use the member name, or leave it blank.
    /// </summary>
    public bool UseMemberName { get; init; }

    /// <summary>
    /// Use the generic type arguments of the member, if any.
    /// </summary>
    public bool UseMemberTypeArguments { get; init; }

    /// <summary>
    /// Use placeholders for the member arguments, if any.
    /// </summary>
    public bool UseMemberArguments { get; init; }

    /// <summary>
    /// Use the types of the member arguments, if any.
    /// </summary>
    public bool UseMemberArgumentsTypes { get; init; }

    /// <summary>
    /// Use the names of the member arguments, if any.
    /// </summary>
    public bool UseMemberArgumentsNames { get; init; }

    // ----------------------------------------------------

    /// <summary>
    /// Initializes a new instance with default <c>false</c> values for its properties, except
    /// the following ones that are set to <c>true</c>:
    /// <br/> <see cref="UseTypeName"/>
    /// <br/> <see cref="UseTypeArguments"/>
    /// <br/> <see cref="UseMemberTypeArguments"/>
    /// <br/> <see cref="UseMemberArguments"/>
    /// <br/> <see cref="UseMemberArgumentsTypes"/>
    /// </summary>
    /// <param name="useTypeNamespace"></param>
    /// <param name="useTypeHost"></param>
    /// <param name="useTypeName"></param>
    /// <param name="useTypeArguments"></param>
    /// <param name="useMemberType"></param>
    /// <param name="useMemberHost"></param>
    /// <param name="useMemberName"></param>
    /// <param name="useMemberTypeArguments"></param>
    /// <param name="useMemberArguments"></param>
    /// <param name="useMemberArgumentsTypes"></param>
    /// <param name="useMemberArgumentsNames"></param>
    public EasyNameOptions(
        bool useTypeNamespace = false,
        bool useTypeHost = false,
        bool useTypeName = true,                // True by default
        bool useTypeArguments = true,           // True by default
        bool useMemberType = false,
        bool useMemberHost = false,
        bool useMemberName = true,
        bool useMemberTypeArguments = true,     // True by default
        bool useMemberArguments = true,         // True by default
        bool useMemberArgumentsTypes = true,    // True by default
        bool useMemberArgumentsNames = false)
    {
        UseTypeNamespace = useTypeNamespace;
        UseTypeHost = useTypeHost;
        UseTypeName = useTypeName;
        UseTypeArguments = useTypeArguments;
        UseMemberType = useMemberType;
        UseMemberName = useMemberName;
        UseMemberTypeArguments = useMemberTypeArguments;
        UseMemberArguments = useMemberArguments;
        UseMemberArgumentsTypes = useMemberArgumentsTypes;
        UseMemberArgumentsNames = useMemberArgumentsNames;
    }

    /// <summary>
    /// A shared default instance.
    /// </summary>
    public static EasyNameOptions Default { get; } = new();

    /// <summary>
    /// A shared instance with all options set to <c>false</c>.
    /// </summary>
    public static EasyNameOptions False { get; } = new(
        useTypeNamespace: false,
        useTypeHost: false,
        useTypeName: false,
        useTypeArguments: false,
        useMemberType: false,
        useMemberHost: false,
        useMemberName: false,
        useMemberTypeArguments: false,
        useMemberArguments: false,
        useMemberArgumentsTypes: false,
        useMemberArgumentsNames: false);

    /// <summary>
    /// A shared instance with all options set to <c>true</c>.
    /// </summary>
    public static EasyNameOptions True { get; } = new(
        useTypeNamespace: true,
        useTypeHost: true,
        useTypeName: true,
        useTypeArguments: true,
        useMemberType: true,
        useMemberHost: true,
        useMemberName: true,
        useMemberTypeArguments: true,
        useMemberArguments: true,
        useMemberArgumentsTypes: true,
        useMemberArgumentsNames: true);
}