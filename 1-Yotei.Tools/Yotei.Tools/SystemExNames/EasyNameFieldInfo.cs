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
        this FieldInfo source) => EasyNameFieldInfo.Default.EasyName(source);

    /// <summary>
    /// Obtains the C#-alike easy name of the given element using the given options.
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
/// Provides 'EasyName' capabilities for 'field' instances.
/// </summary>
public record EasyNameFieldInfo
{
    /// <summary>
    /// A shared read-only instance that represents empty options.
    /// </summary>
    public static EasyNameFieldInfo Empty { get; } = new(Mode.Empty);

    /// <summary>
    /// A shared read-only instance that represents default options.
    /// </summary>
    public static EasyNameFieldInfo Default { get; } = new(Mode.Default);

    /// <summary>
    /// A shared read-only instance that represents full options.
    /// </summary>
    public static EasyNameFieldInfo Full { get; } = new(Mode.Full);

    /// <summary>
    /// Initializes a new default instance.
    /// </summary>
    public EasyNameFieldInfo() : this(Mode.Default) { }

    // ----------------------------------------------------

    /// <summary>
    /// If not null, then the options to use to print the return type of the field. If null,
    /// it is ignored.
    /// </summary>
    public EasyNameType? ReturnTypeOptions { get; init; }

    /// <summary>
    /// If not null, then the options to use to print the host type of the field. If null, it
    /// is ignored.
    /// </summary>
    public EasyNameType? HostTypeOptions { get; init; }

    // ----------------------------------------------------

    enum Mode { Empty, Default, Full };
    private EasyNameFieldInfo(Mode mode)
    {
        switch (mode)
        {
            case Mode.Empty:
                break;

            case Mode.Default:
                break;

            case Mode.Full:
                ReturnTypeOptions = EasyNameType.Full;
                HostTypeOptions = EasyNameType.Full;
                break;
        }
    }

    // ----------------------------------------------------

    /// <summary>
    /// Obtains the C#-alike easy name of the given element.
    /// </summary>
    /// <param name="source"></param>
    /// <returns></returns>
    public string EasyName(FieldInfo source)
    {
        source.ThrowWhenNull();

        var host = source.DeclaringType;
        var sb = new StringBuilder();

        // Return type...
        if (ReturnTypeOptions is not null)
        {
            var options = ReturnTypeOptions.HideName
                ? ReturnTypeOptions with { HideName = false }
                : ReturnTypeOptions;

            var str = options.EasyName(source.FieldType);
            if (str.Length > 0) { sb.Append(str); sb.Append(' '); }
        }

        // Host type...
        if (HostTypeOptions is not null && host is not null)
        {
            var options = HostTypeOptions.HideName
                ? HostTypeOptions with { HideName = false }
                : HostTypeOptions;

            var str = options.EasyName(host);
            if (str.Length > 0) { sb.Append(str); sb.Append('.'); }
        }

        // Name...
        sb.Append(source.Name);

        // Finishing...
        return sb.ToString();
    }
}