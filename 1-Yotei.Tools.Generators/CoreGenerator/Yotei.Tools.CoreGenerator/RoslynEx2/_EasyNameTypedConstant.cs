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
        this TypedConstant source) => EasyNameTypedConstant.Default.EasyName(source);

    /// <summary>
    /// Obtains the C#-alike easy name of the given element using the given options.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="options"></param>
    /// <returns></returns>
    public static string EasyName(this TypedConstant source, EasyNameTypedConstant options)
    {
        options.ThrowWhenNull();
        return options.EasyName(source);
    }
}

// ========================================================
/// <summary>
/// Provides 'EasyName' capabilities for 'typed constant' instances.
/// </summary>
internal record EasyNameTypedConstant
{
    /// <summary>
    /// A shared read-only instance that represents empty options.
    /// </summary>
    public static EasyNameTypedConstant Empty { get; } = new(Mode.Empty);

    /// <summary>
    /// A shared read-only instance that represents default options.
    /// </summary>
    public static EasyNameTypedConstant Default { get; } = new(Mode.Default);

    /// <summary>
    /// A shared read-only instance that represents full options.
    /// </summary>
    public static EasyNameTypedConstant Full { get; } = new(Mode.Full);

    /// <summary>
    /// Initializes a new default instance.
    /// </summary>
    public EasyNameTypedConstant() : this(Mode.Default) { }

    // ----------------------------------------------------

    // ----------------------------------------------------

    enum Mode { Empty, Default, Full };
    private EasyNameTypedConstant(Mode mode)
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
    public string EasyName(TypedConstant source)
    {
        throw null;
    }
}