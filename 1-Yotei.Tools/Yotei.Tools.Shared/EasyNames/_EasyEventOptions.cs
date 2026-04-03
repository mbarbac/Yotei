#if YOTEI_TOOLS_GENERATORS
namespace Yotei.Tools.Generators;
#else
namespace Yotei.Tools;
#endif

// ========================================================
/// <summary>
/// Describes how to obtain the display string of an event element.
/// </summary>
#if YOTEI_TOOLS_GENERATORS
internal
#else
public
#endif
record EasyEventOptions
{
    /// <summary>
    /// If enabled, then use the member accessibility modifiers, if any. Otherwise, they are
    /// ignored.
    /// </summary>
    public bool UseAccessibility { get; init; }

    /// <summary>
    /// If enabled and accesibility is used, then also use the 'private' modifier. In all other
    /// cases, it is ignored.
    /// </summary>
    public bool UsePrivate { get; init; }

    /// <summary>
    /// If enabled, then use the element's modifiers, if possible. Otherwise, they are ignored.
    /// </summary>
    public bool UseModifiers { get; init; }

    /// <summary>
    /// If not null, then the options to use with the element's return type. Otherwise, it is
    /// ignored.
    /// </summary>
    public EasyTypeOptions? MemberTypeOptions { get; init; }

    /// <summary>
    /// If not null, then the options to use with the element's host type. Otherwise, it is
    /// ignored.
    /// </summary>
    public EasyTypeOptions? HostTypeOptions { get; init; }

    // ----------------------------------------------------

    /// <summary>
    /// Initializes a new empty instance.
    /// </summary>
    public EasyEventOptions() { }

    /// <summary>
    /// A shared instance with empty-alike settings.
    /// </summary>
    public static EasyEventOptions Empty { get; } = new();

    /// <summary>
    /// A shared instance with default-alike settings.
    /// </summary>
    public static EasyEventOptions Default { get; } = new();

    /// <summary>
    /// A shared instance with full-alike settings.
    /// </summary>
    public static EasyEventOptions Full { get; } = new()
    {
        UseAccessibility = true,
        UsePrivate = true,
        UseModifiers = true,
        MemberTypeOptions = EasyTypeOptions.Full,
        HostTypeOptions = EasyTypeOptions.Full,
    };
}

/*
// ========================================================
#if YOTEI_TOOLS_GENERATORS
internal
#else
public
#endif
static partial class EasyNameExtensions
{
    /// <summary>
    /// Obtains a c#-alike string representation of the given element, using default options.
    /// </summary>
    /// <param name="source"></param>
    /// <returns></returns>
    public static string EasyName(
        this EventInfo source) => source.EasyName(EasyEventOptions.Default);

    /// <summary>
    /// Obtains a c#-alike string representation of the given element, using the given options.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="options"></param>
    /// <returns></returns>
    public static string EasyName(
        this EventInfo source, EasyEventOptions options)
    {
        ArgumentNullException.ThrowIfNull(source);
        ArgumentNullException.ThrowIfNull(options);

        throw null;
    }
}
*/