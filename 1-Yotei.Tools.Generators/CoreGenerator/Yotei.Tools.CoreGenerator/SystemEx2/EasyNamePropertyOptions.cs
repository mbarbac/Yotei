namespace Yotei.Tools.CoreGenerator;

// ========================================================
internal static partial class EasyNameExtensions
{
    /// <summary>
    /// Obtains the C#-alike easy name of the given element using default options.
    /// </summary>
    /// <param name="source"></param>
    /// <returns></returns>
    public static string EasyName(
        this PropertyInfo source) => EasyNamePropertyOptions.Default.EasyName(source);

    /// <summary>
    /// Obtains the C#-alike easy name of the given element using the given options.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="options"></param>
    /// <returns></returns>
    public static string EasyName(this PropertyInfo source, EasyNamePropertyOptions options)
    {
        options.ThrowWhenNull();
        return options.EasyName(source);
    }
}

// ========================================================
/// <summary>
/// Provides 'EasyName' capabilities for 'property' instances.
/// </summary>
internal record EasyNamePropertyOptions
{
    /// <summary>
    /// A shared read-only instance that represents empty options.
    /// </summary>
    public static EasyNamePropertyOptions Empty { get; } = new(Mode.Empty);

    /// <summary>
    /// A shared read-only instance that represents default options.
    /// </summary>
    public static EasyNamePropertyOptions Default { get; } = new(Mode.Default);

    /// <summary>
    /// A shared read-only instance that represents full options.
    /// </summary>
    public static EasyNamePropertyOptions Full { get; } = new(Mode.Full);

    /// <summary>
    /// Initializes a new default instance.
    /// </summary>
    public EasyNamePropertyOptions() : this(Mode.Default) { }

    // ----------------------------------------------------

    /// <summary>
    /// If not null, then the options to use to print the return type of the property. If null,
    /// it is ignored.
    /// </summary>
    public EasyNameTypeOptions? ReturnTypeOptions { get; init; }

    /// <summary>
    /// If not null, then the options to use to print the host type of the property. If null, it
    /// is ignored.
    /// </summary>
    public EasyNameTypeOptions? HostTypeOptions { get; init; }

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
    public EasyNameParameterOptions? ParameterOptions { get; init; }

    // ----------------------------------------------------

    enum Mode { Empty, Default, Full };
    private EasyNamePropertyOptions(Mode mode)
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
                UseTechName = true;
                UseBrackets = true;
                ParameterOptions = EasyNameParameterOptions.Full;
                break;
        }
    }

    // ----------------------------------------------------

    /// <summary>
    /// Obtains the C#-alike easy name of the given element.
    /// </summary>
    /// <param name="source"></param>
    /// <returns></returns>
    public string EasyName(PropertyInfo source)
    {
        source.ThrowWhenNull();

        var host = source.DeclaringType;
        var sb = new StringBuilder();

        // Return type...
        if (ReturnTypeOptions is not null)
        {
            var str = ReturnTypeOptions.EasyName(source.PropertyType);
            if (str.Length > 0) { sb.Append(str); sb.Append(' '); }
        }

        // Host type...
        if (HostTypeOptions is not null && host is not null)
        {
            var str = HostTypeOptions.EasyName(host);
            if (str.Length > 0) { sb.Append(str); sb.Append('.'); }
        }

        // Name...
        var name = source.Name;
        var args = source.GetIndexParameters();
        if (args.Length > 0 && !UseTechName) name = "this";
        sb.Append(name);

        // Method parameters...
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