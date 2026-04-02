#if YOTEI_TOOLS_GENERATORS
namespace Yotei.Tools.Generators;
#else
namespace Yotei.Tools;
#endif

// ========================================================
/// <summary>
/// Describes how to obtain the display string of a field element.
/// </summary>
#if YOTEI_TOOLS_GENERATORS
internal
#else
public
#endif
record EasyFieldOptions
{

    // ----------------------------------------------------

    /// <summary>
    /// Initializes a new empty instance.
    /// </summary>
    public EasyFieldOptions() { }

    /// <summary>
    /// A shared instance with empty-alike settings.
    /// </summary>
    public static EasyFieldOptions Empty { get; } = new();

    /// <summary>
    /// A shared instance with default-alike settings.
    /// </summary>
    public static EasyFieldOptions Default { get; } = new();

    /// <summary>
    /// A shared instance with full-alike settings.
    /// </summary>
    public static EasyFieldOptions Full { get; } = new();
}

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
        this FieldInfo source) => source.EasyName(EasyFieldOptions.Default);

    /// <summary>
    /// Obtains a c#-alike string representation of the given element, using the given options.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="options"></param>
    /// <returns></returns>
    public static string EasyName(
        this FieldInfo source, EasyFieldOptions options)
    {
        ArgumentNullException.ThrowIfNull(source);
        ArgumentNullException.ThrowIfNull(options);

        throw null;
    }
}