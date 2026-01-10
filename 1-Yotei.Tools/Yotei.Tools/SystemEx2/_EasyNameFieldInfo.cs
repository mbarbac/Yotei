namespace Yotei.Tools;

// ========================================================
public static partial class EasyNameExtensions
{
    /// <summary>
    /// Obtains a C#-alike name for the given element, using default options.
    /// </summary>
    /// <param name="source"></param>
    /// <returns></returns>
    public static string EasyName(
        this FieldInfo source) => source.EasyName(EasyNameFieldInfo.Default);

    /// <summary>
    /// Obtains a C#-alike name for the given element, using the given options.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="options"></param>
    /// <returns></returns>
    public static string EasyName(this FieldInfo source, EasyNameFieldInfo options)
    {
        options.ThrowWhenNull();
        return options.EasyName(source);
    }
}

// ========================================================
/// <summary>
/// Provides 'EasyName' capabilities to <see cref="Type"/> instances.
/// </summary>
public record EasyNameFieldInfo
{

    // ----------------------------------------------------

    /// <summary>
    /// A shared instance with default options.
    /// </summary>
    public static EasyNameFieldInfo Default { get; } = new EasyNameFieldInfo(Mode.Default);

    /// <summary>
    /// A shared instance with full options.
    /// </summary>
    public static EasyNameFieldInfo Full { get; } = new EasyNameFieldInfo(Mode.Full);

    /// <summary>
    /// Initializes a new empty instance.
    /// </summary>
    public EasyNameFieldInfo() : this(Mode.Empty) { }

    /// <summary>
    /// Determines the mode to use when initializing this instance.
    /// </summary>
    enum Mode { Empty, Default, Full }
    EasyNameFieldInfo(Mode mode)
    {
        switch (mode)
        {
            case Mode.Empty:
                break;

            default:
            case Mode.Default:
                break;

            case Mode.Full:
                break;
        }
    }

    // ----------------------------------------------------

    /// <summary>
    /// Obtains a C#-alike name for the given element.
    /// </summary>
    /// <param name="source"></param>
    /// <returns></returns>
    public string EasyName(FieldInfo source)
    {
        throw null;
    }
}