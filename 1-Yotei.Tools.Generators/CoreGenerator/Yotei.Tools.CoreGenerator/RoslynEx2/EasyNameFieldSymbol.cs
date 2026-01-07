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
        this IFieldSymbol source) => EasyNameFieldSymbol.Default.EasyName(source);

    /// <summary>
    /// Obtains the C#-alike easy name of the given element using the given options.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="options"></param>
    /// <returns></returns>
    public static string EasyName(this IFieldSymbol source, EasyNameFieldSymbol options)
    {
        options.ThrowWhenNull();
        return options.EasyName(source);
    }
}

// ========================================================
/// <summary>
/// Provides 'EasyName' capabilities for 'field symbol' instances.
/// </summary>
internal record EasyNameFieldSymbol
{
    /// <summary>
    /// A shared read-only instance that represents empty options.
    /// </summary>
    public static EasyNameFieldSymbol Empty { get; } = new(Mode.Empty);

    /// <summary>
    /// A shared read-only instance that represents default options.
    /// </summary>
    public static EasyNameFieldSymbol Default { get; } = new(Mode.Default);

    /// <summary>
    /// A shared read-only instance that represents full options.
    /// </summary>
    public static EasyNameFieldSymbol Full { get; } = new(Mode.Full);

    /// <summary>
    /// Initializes a new default instance.
    /// </summary>
    public EasyNameFieldSymbol() : this(Mode.Default) { }

    // ----------------------------------------------------
    
    /// <summary>
    /// If not null, then the options to use to print the return type of the field. If null,
    /// it is ignored.
    /// </summary>
    public EasyNameTypeSymbol? ReturnTypeOptions { get; init; }

    /// <summary>
    /// If not null, then the options to use to print the host type of the field. If null, it
    /// is ignored.
    /// </summary>
    public EasyNameTypeSymbol? HostTypeOptions { get; init; }

    // ----------------------------------------------------

    enum Mode { Empty, Default, Full };
    private EasyNameFieldSymbol(Mode mode)
    {
        switch (mode)
        {
            case Mode.Empty:
                break;

            case Mode.Default:
                break;

            case Mode.Full:
                ReturnTypeOptions = EasyNameTypeSymbol.Full;
                HostTypeOptions = EasyNameTypeSymbol.Full;
                break;
        }
    }

    // ----------------------------------------------------

    /// <summary>
    /// Obtains the C#-alike easy name of the given element.
    /// </summary>
    /// <param name="source"></param>
    /// <returns></returns>
    public string EasyName(IFieldSymbol source)
    {
        source.ThrowWhenNull();

        var host = source.ContainingType;
        var sb = new StringBuilder();

#if USE_MODIFIERS
        // Modifiers...
        if (UseModifiers)
        {
            var prefix = source.RefKind switch
            {
                RefKind.Ref => "ref ",
                RefKind.RefReadOnly => "ref readonly ",
                _ => string.Empty
            };
            if (prefix is not null) sb.Append(prefix);
        }
#endif

        // Return type...
        if (ReturnTypeOptions is not null)
        {
            var options = ReturnTypeOptions.HideName
                ? ReturnTypeOptions with { HideName = false }
                : ReturnTypeOptions;

            var str = options.EasyName(source.Type);
            sb.Append(str); sb.Append(' ');
        }

        // Host type...
        if (HostTypeOptions is not null && host is not null)
        {
            var options = HostTypeOptions.HideName
                ? HostTypeOptions with { HideName = false }
                : HostTypeOptions;

            var str = options.EasyName(host);
            sb.Append(str); sb.Append('.');
        }

        // Name...
        sb.Append(source.Name);

        // Finishing...
        return sb.ToString();
    }
}