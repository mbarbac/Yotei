namespace Yotei.Tools.CoreGenerator;

// ========================================================
internal static partial class RoslynNameExtensions
{
    /// <summary>
    /// Obtains the C#-alike easy name of the given element using default options.
    /// </summary>
    /// <param name="source"></param>
    /// <returns></returns>
    public static string EasyName(
        this AttributeData source) => EasyNameAttributeData.Default.EasyName(source);

    /// <summary>
    /// Obtains the C#-alike easy name of the given element using the given options.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="options"></param>
    /// <returns></returns>
    public static string EasyName(this AttributeData source, EasyNameAttributeData options)
    {
        options.ThrowWhenNull();
        return options.EasyName(source);
    }
}

// ========================================================
/// <summary>
/// Provides 'EasyName' capabilities for 'attribute data' instances.
/// </summary>
internal record EasyNameAttributeData
{
    /// <summary>
    /// A shared read-only instance that represents empty options.
    /// </summary>
    public static EasyNameAttributeData Empty { get; } = new(Mode.Empty);

    /// <summary>
    /// A shared read-only instance that represents default options.
    /// </summary>
    public static EasyNameAttributeData Default { get; } = new(Mode.Default);

    /// <summary>
    /// A shared read-only instance that represents full options.
    /// </summary>
    public static EasyNameAttributeData Full { get; } = new(Mode.Full);

    /// <summary>
    /// Initializes a new default instance.
    /// </summary>
    public EasyNameAttributeData() : this(Mode.Default) { }

    // ----------------------------------------------------

    // ----------------------------------------------------

    enum Mode { Empty, Default, Full };
    private EasyNameAttributeData(Mode mode)
    {
        switch (mode)
        {
            case Mode.Empty:
                break;

            case Mode.Default:
                break;

            case Mode.Full:
                break;
        }
    }

    // ----------------------------------------------------

    /// <summary>
    /// Obtains the C#-alike easy name of the given element.
    /// </summary>
    /// <param name="source"></param>
    /// <returns></returns>
    public string EasyName(AttributeData source)
    {
        throw null;
    }
}