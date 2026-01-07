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
        this IPropertySymbol source) => EasyNamePropertySymbol.Default.EasyName(source);

    /// <summary>
    /// Obtains the C#-alike easy name of the given element using the given options.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="options"></param>
    /// <returns></returns>
    public static string EasyName(this IPropertySymbol source, EasyNamePropertySymbol options)
    {
        options.ThrowWhenNull();
        return options.EasyName(source);
    }
}

// ========================================================
/// <summary>
/// Provides 'EasyName' capabilities for 'type' instances.
/// </summary>
internal record EasyNamePropertySymbol
{
    /// <summary>
    /// A shared read-only instance that represents empty options.
    /// </summary>
    public static EasyNamePropertySymbol Empty { get; } = new(Mode.Empty);

    /// <summary>
    /// A shared read-only instance that represents default options.
    /// </summary>
    public static EasyNamePropertySymbol Default { get; } = new(Mode.Default);

    /// <summary>
    /// A shared read-only instance that represents full options.
    /// </summary>
    public static EasyNamePropertySymbol Full { get; } = new(Mode.Full);

    /// <summary>
    /// Initializes a new default instance.
    /// </summary>
    public EasyNamePropertySymbol() : this(Mode.Default) { }

    // ----------------------------------------------------

    /// <summary>
    /// If not null, then the options to use to print the return type of the property. If null,
    /// it is ignored.
    /// </summary>
    public EasyNameTypeSymbol? ReturnTypeOptions { get; init; }

    /// <summary>
    /// If not null, then the options to use to print the host type of the property. If null, it
    /// is ignored.
    /// </summary>
    public EasyNameTypeSymbol? HostTypeOptions { get; init; }

    /// <summary>
    /// Determines if the property tech name shall be used, instead of the default "this" one.
    /// </summary>
    public bool UseTechName { get; init; }

    /// <summary>
    /// Determines if, at least, the indexed property squared bracket shall be used even if no
    /// parameter options were given.
    /// </summary>
    public bool UseBrackets { get; init; }

    /// <summary>
    /// If not null, then the options to use with the property parameters. If null, then they are
    /// ignored.
    /// </summary>
    public EasyNameParameterSymbol? ParameterOptions { get; init; }

    // ----------------------------------------------------

    enum Mode { Empty, Default, Full };
    private EasyNamePropertySymbol(Mode mode)
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
                UseTechName = true;
                UseBrackets = true;
                ParameterOptions = EasyNameParameterSymbol.Full;
                break;
        }
    }

    // ----------------------------------------------------

    /// <summary>
    /// Obtains the C#-alike easy name of the given element.
    /// </summary>
    /// <param name="source"></param>
    /// <returns></returns>
    public string EasyName(IPropertySymbol source)
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
        var name = source.Name;
        var args = source.Parameters;
        if (args.Length > 0) name = UseTechName ? source.MetadataName : "this";
        sb.Append(name);

        // Indexer parameters...
        if (args.Length > 0 && (UseBrackets || ParameterOptions is not null))
        {
            sb.Append('['); if (ParameterOptions is not null)
            {
                for (int i = 0; i < args.Length; i++)
                {
                    var arg = args[i];
                    var str = ParameterOptions.EasyName(arg);
                    if (i > 0) sb.Append((str != null && str.Length > 0) ? ", " : ",");
                    sb.Append(str);
                }
            }
            sb.Append(']');
        }

        // Finishing...
        return sb.ToString();
    }
}