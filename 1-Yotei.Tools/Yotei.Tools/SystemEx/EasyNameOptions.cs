namespace Yotei.Tools;

// ========================================================
/// <summary>
/// Provides options for the <see cref="EasyNameExtensions"/> methods.
/// </summary>
public record EasyTypeOptions
{
    /// <summary>
    /// Use the namespace of the given type, or <c>false</c> to ignore it.
    /// <br/> This option implies <see cref="UseTypeHost"/>
    /// </summary>
    public bool UseNamespace { get; init; }

    /// <summary>
    /// Use the declaring type of the given one, or <c>false</c> to ignore it.
    /// </summary>
    public bool UseTypeHost { get; init; }

    /// <summary>
    /// Use the type short name, or <c>false</c> to leave it blank.
    /// <br/> The default value of this property is <c>true</c>.
    /// </summary>
    public bool UseTypeName { get; init; } = true; // As an exception

    /// <summary>
    /// Use the type's generic arguments, or <c>false</c> to ignore them.
    /// </summary>
    public bool UseArguments { get; init; }

    /// <summary>
    /// Use the namespaces of the type's generic arguments, or <c>false</c> to ignore them.
    /// <br/> This option implies <see cref="UseArguments"/>
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
    public bool UseArgumentNames { get; init; }

    // ----------------------------------------------------

    /// <summary>
    /// A common shared default instance.
    /// </summary>
    public static EasyTypeOptions Default { get; } = new();

    /// <summary>
    /// A common shared instance with all options set.
    /// </summary>
    public static EasyTypeOptions True { get; } = new EasyTypeOptions() with
    {
        UseNamespace = true,
        UseTypeHost = true,
        UseTypeName = true,
        UseArguments = true,
        UseArgumentsNamespaces = true,
        UseArgumentsHosts = true,
        UseArgumentNames = true,
    };

    /// <summary>
    /// Initializes a new default instance.
    /// </summary>
    public EasyTypeOptions() { }
}