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
    /// Use the member name, or <c>false</c> to leave it blank (useful when the member is an
    /// special one, and the desired result is not to use its slot).
    /// </summary>
    public bool UseName { get; init; }

    /// <summary>
    /// Not <c>null</c> to use the generic type arguments of the member, if any, with the given
    /// options, or <c>null</c> to ignore them.
    /// </summary>
    public EasyTypeOptions? UseTypeArguments { get; init; }

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
    /// Initializes a new default instance that uses the member name, its generic arguments, and
    /// the type of its regular ones, if any.
    /// </summary>
    public EasyMemberOptions()
    {
        UseReturnType = null;
        UseHostType = null;
        UseName = true;
        UseTypeArguments = EasyTypeOptions.Default;
        UseArguments = true;
        UseArgumentsTypes = EasyTypeOptions.Default;
        UseArgumentsNames = false;
    }

    /// <summary>
    /// A common shared default instance.
    /// </summary>
    public static EasyMemberOptions Default { get; } = new();

    /// <summary>
    /// A common shared instance with all its options set to <c>false</c> or <c>null</c>.
    /// </summary>
    public static EasyMemberOptions Empty { get; } = new()
    {
        UseReturnType = null,
        UseHostType = null,
        UseName = false,
        UseTypeArguments = null,
        UseArguments = false,
        UseArgumentsTypes = null,
        UseArgumentsNames = false,
    };

    /// <summary>
    /// A common shared instance with all its options set to <c>true</c> or full.
    /// </summary>
    public static EasyMemberOptions Full { get; } = new()
    {
        UseReturnType = EasyTypeOptions.Full,
        UseHostType = EasyTypeOptions.Full,
        UseName = true,
        UseTypeArguments = EasyTypeOptions.Full,
        UseArguments = true,
        UseArgumentsTypes = EasyTypeOptions.Full,
        UseArgumentsNames = true,
    };
}