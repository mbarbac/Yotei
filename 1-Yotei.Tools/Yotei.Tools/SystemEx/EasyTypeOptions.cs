namespace Yotei.Tools;

// ========================================================
/// <summary>
/// Provides options for the <see cref="EasyTypeExtensions"/> methods.
/// </summary>
public record EasyTypeOptions
{
    /// <summary>
    /// Use the namespace of the type, or <c>false</c> to ignore it.
    /// <br/> This option implies <see cref="UseHost"/>
    /// </summary>
    public bool UseNamespace { get; init; }

    /// <summary>
    /// Use the declaring type, or <c>false</c> to ignore it.
    /// </summary>
    public bool UseHost { get; init; }

    /// <summary>
    /// Use the type short name, or <c>false</c> to leave it blank. The later is useful when the
    /// type is an unbounded generic one, and the desired result is not to use its slot.
    /// <br/> The default value of this property is <c>true</c>.
    /// </summary>
    public bool UseName { get; init; } = true; // By default

    /// <summary>
    /// Use the type's generic arguments, or <c>false</c> to ignore them.
    /// </summary>
    public bool UseArguments { get; init; }

    /// <summary>
    /// Use the namespaces of the type's generic arguments, or <c>false</c> to ignore them.
    /// <br/> This option implies <see cref="UseArguments"/> and <see cref="UseArgumentsHosts"/>.
    /// </summary>
    public bool UseArgumentsNamespaces { get; init; }

    /// <summary>
    /// Use the host types of the type's generic arguments, or <c>false</c> to ignore them.
    /// <br/> This option implies <see cref="UseArguments"/>
    /// </summary>
    public bool UseArgumentsHosts { get; init; }

    /// <summary>
    /// Use the type names of the type's generic arguments, or <c>false</c> to ignore them.
    /// <br/> This option implies <see cref="UseArguments"/>
    /// </summary>
    public bool UseArgumentsNames { get; init; }

    // ----------------------------------------------------

    /// <summary>
    /// Initializes a new default instance.
    /// </summary>
    public EasyTypeOptions() { }

    /// <summary>
    /// A common shared default instance.
    /// </summary>
    public static EasyTypeOptions Default { get; } = new();

    /// <summary>
    /// A common shared instance with all options set.
    /// </summary>
    public static EasyTypeOptions True { get; } = new EasyTypeOptions
    {
        UseNamespace = true,
        UseHost = true,
        UseName = true,
        UseArguments = true,
        UseArgumentsNamespaces = true,
        UseArgumentsHosts = true,
        UseArgumentsNames = true,
    };

    /// <summary>
    /// A common shared instance with all options set except namespaces.
    /// </summary>
    public static EasyTypeOptions NoNamespaces { get; } = new EasyTypeOptions
    {
        UseNamespace = false,
        UseHost = true,
        UseName = true,
        UseArguments = true,
        UseArgumentsNamespaces = false,
        UseArgumentsHosts = true,
        UseArgumentsNames = true,
    };
}