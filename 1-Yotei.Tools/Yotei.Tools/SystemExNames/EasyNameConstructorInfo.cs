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
        this ConstructorInfo source) => EasyNameConstructorInfo.Default.EasyName(source);

    /// <summary>
    /// Obtains the C#-alike easy name of the given element using the given options.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="options"></param>
    /// <returns></returns>
    public static string EasyName(this ConstructorInfo source, EasyNameConstructorInfo options)
    {
        options.ThrowWhenNull();
        return options.EasyName(source);
    }
}

// ========================================================
/// <summary>
/// Provides 'EasyName' capabilities for 'constructor' instances.
/// </summary>
public record EasyNameConstructorInfo
{
    /// <summary>
    /// A shared read-only instance that represents empty options.
    /// </summary>
    public static EasyNameConstructorInfo Empty { get; } = new(Mode.Empty);

    /// <summary>
    /// A shared read-only instance that represents default options.
    /// </summary>
    public static EasyNameConstructorInfo Default { get; } = new(Mode.Default);

    /// <summary>
    /// A shared read-only instance that represents full options.
    /// </summary>
    public static EasyNameConstructorInfo Full { get; } = new(Mode.Full);

    /// <summary>
    /// Initializes a new default instance.
    /// </summary>
    public EasyNameConstructorInfo() : this(Mode.Default) { }

    // ----------------------------------------------------

    /// <summary>
    /// If not null, then the options to use to print the host type of the constructor. If null,
    /// it is unless the constructor is a static one.
    /// </summary>
    public EasyNameType? HostTypeOptions { get; init; }

    /// <summary>
    /// Determines if the constructor tech name shall be used, instead of the default "new" one.
    /// </summary>
    public bool UseTechName { get; init; }

    /// <summary>
    /// Determines if, at least, the constructor parentheses shall be used even if no parameter
    /// options were given.
    /// </summary>
    public bool UseBrackets { get; init; }

    /// <summary>
    /// If not null, then the options to use with the constructor parameters. If null, then they
    /// are ignored.
    /// </summary>
    public EasyNameParameterInfo? ParameterOptions { get; init; }

    // ----------------------------------------------------

    enum Mode { Empty, Default, Full };
    private EasyNameConstructorInfo(Mode mode)
    {
        switch (mode)
        {
            case Mode.Empty:
                break;

            case Mode.Default:
                break;

            case Mode.Full:
                HostTypeOptions = EasyNameType.Full;
                UseTechName = true;
                UseBrackets = true;
                ParameterOptions = EasyNameParameterInfo.Full;
                break;
        }
    }

    // ----------------------------------------------------

    /// <summary>
    /// Obtains the C#-alike easy name of the given element.
    /// </summary>
    /// <param name="source"></param>
    /// <returns></returns>
    public string EasyName(ConstructorInfo source)
    {
        source.ThrowWhenNull();

        var host = source.DeclaringType!;
        var sb = new StringBuilder();

        // Static constructor...
        if (source.IsStatic)
        {
            if (HostTypeOptions is not null || !UseTechName)
            {
                var options = HostTypeOptions ?? EasyNameType.Empty with { HideName = false };
                var str = options.EasyName(host);
                sb.Append(str);
            }
            if (UseTechName)
            {
                if (sb.Length > 0) sb.Append('.');
                sb.Append(source.Name);
            }
        }

        // Standard constructor...
        else
        {
            if (HostTypeOptions is not null)
            {
                var str = HostTypeOptions.EasyName(host);
                if (str.Length > 0) { sb.Append(str); sb.Append('.'); }
            }
            var name = UseTechName ? source.Name : "new";
            sb.Append(name);
        }

        // Constructor parameters...
        if (UseBrackets || ParameterOptions is not null)
        {
            sb.Append('('); if (ParameterOptions is not null)
            {
                var args = source.GetParameters();
                if (args.Length > 0)
                {
                    for (int i = 0; i < args.Length; i++)
                    {
                        var arg = args[i];
                        var str = ParameterOptions.EasyName(arg);
                        if (i > 0) sb.Append((str != null && str.Length > 0) ? ", " : ",");
                        sb.Append(str);
                    }
                }
            }
            sb.Append(')');
        }

        // Finishing...
        return sb.ToString();
    }
}