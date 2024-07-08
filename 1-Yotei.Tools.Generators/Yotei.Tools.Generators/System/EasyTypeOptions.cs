namespace Yotei.Tools.Generators.Internal;

// ========================================================
/// <summary>
/// Provides options for the <see cref="EasyTypeExtensions"/> methods.
/// </summary>
internal record EasyTypeOptions
{
    /// <summary>
    /// Use the namespace of the type, or <c>false</c> to ignore it.
    /// <br/> This option implies <see cref="UseHost"/>
    /// </summary>
    public bool UseNamespace { get; init; }

    /// <summary>
    /// Use the declaring host type, or <c>false</c> to ignore it.
    /// </summary>
    public bool UseHost { get; init; }

    /// <summary>
    /// Use the type short name, or <c>false</c> to leave it blank (useful when the type is an
    /// unbounded generic one, and the desired result is not to use its slot).
    /// </summary>
    public bool UseName { get; init; }

    /// <summary>
    /// Use the type's generic arguments, or <c>false</c> to ignore them.
    /// </summary>
    public bool UseTypeArguments { get; init; }

    /// <summary>
    /// Use the namespaces of the type's generic arguments, or <c>false</c> to ignore them.
    /// <br/> This option implies <see cref="UseTypeArguments"/> and
    /// <see cref="UseTypeArgumentsHosts"/>.
    /// </summary>
    public bool UseTypeArgumentsNamespaces { get; init; }

    /// <summary>
    /// Use the host types of the type's generic arguments, or <c>false</c> to ignore them.
    /// <br/> This option implies <see cref="UseTypeArguments"/>
    /// </summary>
    public bool UseTypeArgumentsHosts { get; init; }

    /// <summary>
    /// Use the type names of the type's generic arguments, or <c>false</c> to ignore them.
    /// <br/> This option implies <see cref="UseTypeArguments"/>
    /// </summary>
    public bool UseTypeArgumentsNames { get; init; }

    // ----------------------------------------------------

    /// <summary>
    /// Initializes a new default instance, that uses the name of the type and the names of its
    /// type arguments, if any.
    /// </summary>
    public EasyTypeOptions()
    {
        UseNamespace = false;
        UseHost = false;
        UseName = true;
        UseTypeArguments = false;
        UseTypeArgumentsNamespaces = false;
        UseTypeArgumentsHosts = false;
        UseTypeArgumentsNames = true;
    }

    /// <summary>
    /// A common shared default instance.
    /// </summary>
    public static EasyTypeOptions Default { get; } = new();

    /// <summary>
    /// A common shared instance with all its options set to <c>false</c>.
    /// </summary>
    public static EasyTypeOptions Empty { get; } = new()
    {
        UseNamespace = false,
        UseHost = false,
        UseName = false,
        UseTypeArguments = false,
        UseTypeArgumentsNamespaces = false,
        UseTypeArgumentsHosts = false,
        UseTypeArgumentsNames = false,
    };

    /// <summary>
    /// A common shared instance with all its options set to <c>true</c>.
    /// </summary>
    public static EasyTypeOptions Full { get; } = new()
    {
        UseNamespace = true,
        UseHost = true,
        UseName = true,
        UseTypeArguments = true,
        UseTypeArgumentsNamespaces = true,
        UseTypeArgumentsHosts = true,
        UseTypeArgumentsNames = true,
    };
}