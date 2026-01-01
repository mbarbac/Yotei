namespace Yotei.Tools;

// ========================================================
public static partial class EasyNameExtensions
{
    /// <summary>
    /// Obtains the C#-alike easy name of the given element using default options.
    /// </summary>
    /// <param name="source"></param>
    /// <returns></returns>
    public static string EasyName(
        this MethodInfo source) => EasyNameMethodOptions.Default.EasyName(source);

    /// <summary>
    /// Obtains the C#-alike easy name of the given element using the given options.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="options"></param>
    /// <returns></returns>
    public static string EasyName(this MethodInfo source, EasyNameMethodOptions options)
    {
        options.ThrowWhenNull();
        return options.EasyName(source);
    }
}

// ========================================================
/// <summary>
/// Provides 'EasyName' capabilities for <see cref="MethodInfo"/> instances.
/// <br/> Empty options render an empty string.
/// <br/> Default options just render the name of the element.
/// </summary>
public record EasyNameMethodOptions
{
    /// <summary>
    /// A shared read-only instance that represents empty options.
    /// </summary>
    public static EasyNameMethodOptions Empty { get; } = new(Mode.Empty);

    /// <summary>
    /// A shared read-only instance that represents default options.
    /// </summary>
    public static EasyNameMethodOptions Default { get; } = new(Mode.Default);

    /// <summary>
    /// A shared read-only instance that represents full options.
    /// </summary>
    public static EasyNameMethodOptions Full { get; } = new(Mode.Full);

    /// <summary>
    /// Initializes a new default instance.
    /// </summary>
    public EasyNameMethodOptions() : this(Mode.Default) { }

    // ----------------------------------------------------

    /// <summary>
    /// If not null, then the options to use to print the return type of the method.
    /// If null, then it is ignored.
    /// <br/> The value of this property is 'null' by default.
    /// </summary>
    public EasyNameTypeOptions? ReturnTypeOptions { get; init; }

    /// <summary>
    /// If not null, then the options to use to print the host type of the method.
    /// If null, then it is ignored.
    /// <br/> The value of this property is 'null' by default.
    /// </summary>
    public EasyNameTypeOptions? HostTypeOptions { get; init; }

    /// <summary>
    /// If not null, then the options to use to print the generic type arguments of the method.
    /// If null, then it is ignored.
    /// <br/> The value of this property is 'null' by default.
    /// </summary>
    public EasyNameTypeOptions? GenericArgumentOptions { get; init; }

    /// <summary>
    /// Determines if, at least, the method parentheses shall be used.
    /// <br/> The value of this property is 'false' by default.
    /// </summary>
    public bool UseParentheses { get; init; }

    /// <summary>
    /// If not null, the options to use to print the method arguments, if any. If null, then the
    /// arguments are ignored.
    /// <br/> The value of this property is 'null' by default.
    /// </summary>
    public EasyNameParameterOptions? ArgumentOptions { get; init; }

    // ----------------------------------------------------

    enum Mode { Empty, Default, Full };
    private EasyNameMethodOptions(Mode mode)
    {
        switch (mode)
        {
            case Mode.Empty:
                break;

            case Mode.Default:
                break;

            case Mode.Full:
                ReturnTypeOptions = EasyNameTypeOptions.Full;
                HostTypeOptions = EasyNameTypeOptions.Full;
                GenericArgumentOptions = EasyNameTypeOptions.Full;
                UseParentheses = true;
                ArgumentOptions = EasyNameParameterOptions.Full;
                break;
        }
    }

    // ----------------------------------------------------

    /// <summary>
    /// Obtains the C#-alike easy name of the given element.
    /// </summary>
    /// <param name="source"></param>
    /// <returns></returns>
    public string EasyName(MethodInfo source) => throw null;
}