namespace Yotei.Tools;

// ========================================================
/// <summary>
/// Provides options for the <see cref="EasyMemberExtensions"/> methods.
/// </summary>
public record EasyMemberOptions
{
    /// <summary>
    /// Not <c>null</c> to use the member type, or return type, with the given options, or
    /// <c>null</c> to ignore it.
    /// </summary>
    public EasyTypeOptions? UseReturnType { get; init; }

    /// <summary>
    /// Not <c>null</c> to use the declaring host type of the member, with the given options, or
    /// <c>null</c> to ignore it.
    /// </summary>
    public EasyTypeOptions? UseHostType { get; init; }

    /// <summary>
    /// Use the member name, or <c>false</c> to leave it blank. The later is useful when the
    /// member is an special one, and the desired result is not to use its slot.
    /// </summary>
    public bool UseName { get; init; } = true; // By default

    /// <summary>
    /// Not <c>null</c> to use the generic type arguments of the member, if any, with the given
    /// options, or <c>null</c> to ignore them.
    /// </summary>
    public EasyTypeOptions? UseGenericArguments { get; init; }

    /// <summary>
    /// Use the indexed property or method arguments, or or <c>false</c> to ignore them.
    /// </summary>
    public bool UseArguments { get; init; }

    /// <summary>
    /// Not <c>null</c> to use the types of the member arguments, with the given options, or
    /// <c>null</c> to ignore them.
    /// <br/> This option implies <see cref="UseArguments"/>
    /// </summary>
    public EasyTypeOptions? UseArgumentsTypes { get; init; }

    /// <summary>
    /// Use the names of the member arguments, or <c>false</c> to ignore them.
    /// <br/> This option implies <see cref="UseArguments"/>
    /// </summary>
    public bool UseArgumentsNames { get; init; }

    // ----------------------------------------------------

    /// <summary>
    /// Initializes a new default instance.
    /// </summary>
    public EasyMemberOptions() { }

    /// <summary>
    /// A common shared default instance.
    /// </summary>
    public static EasyMemberOptions Default { get; } = new();

    /// <summary>
    /// A common shared instance with all options set.
    /// </summary>
    public static EasyMemberOptions True { get; } = new EasyMemberOptions() with
    {
        UseReturnType = EasyTypeOptions.True,
        UseHostType = EasyTypeOptions.True,
        UseName = true,
        UseGenericArguments = EasyTypeOptions.True,
        UseArguments = true,
        UseArgumentsTypes = EasyTypeOptions.True,
        UseArgumentsNames = true,
    };

    /// <summary>
    /// A common shared instance with all options set except namespaces.
    /// </summary>
    public static EasyMemberOptions TrueNoNamespaces { get; } = new EasyMemberOptions() with
    {
        UseReturnType = EasyTypeOptions.TrueNoNamespaces,
        UseHostType = EasyTypeOptions.TrueNoNamespaces,
        UseName = true,
        UseGenericArguments = EasyTypeOptions.TrueNoNamespaces,
        UseArguments = true,
        UseArgumentsTypes = EasyTypeOptions.TrueNoNamespaces,
        UseArgumentsNames = true,
    };
}