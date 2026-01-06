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
        this IParameterSymbol source) => EasyNameParameterSymbol.Default.EasyName(source);

    /// <summary>
    /// Obtains the C#-alike easy name of the given element using the given options.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="options"></param>
    /// <returns></returns>
    public static string EasyName(this IParameterSymbol source, EasyNameParameterSymbol options)
    {
        options.ThrowWhenNull();
        return options.EasyName(source);
    }
}

// ========================================================
/// <summary>
/// Provides 'EasyName' capabilities for 'type' instances.
/// </summary>
internal record EasyNameParameterSymbol
{
    /// <summary>
    /// A shared read-only instance that represents empty options.
    /// </summary>
    public static EasyNameParameterSymbol Empty { get; } = new(Mode.Empty);

    /// <summary>
    /// A shared read-only instance that represents default options.
    /// </summary>
    public static EasyNameParameterSymbol Default { get; } = new(Mode.Default);

    /// <summary>
    /// A shared read-only instance that represents full options.
    /// </summary>
    public static EasyNameParameterSymbol Full { get; } = new(Mode.Full);

    /// <summary>
    /// Initializes a new default instance.
    /// </summary>
    public EasyNameParameterSymbol() : this(Mode.Default) { }

    // ----------------------------------------------------

    // ----------------------------------------------------

    enum Mode { Empty, Default, Full };
    private EasyNameParameterSymbol(Mode mode)
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
    public string EasyName(IParameterSymbol source)
    {
        throw null;
    }
}