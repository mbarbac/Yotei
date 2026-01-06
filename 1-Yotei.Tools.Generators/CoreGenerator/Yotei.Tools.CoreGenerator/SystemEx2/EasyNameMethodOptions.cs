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
/// Provides 'EasyName' capabilities for 'method' instances.
/// </summary>
internal record EasyNameMethodOptions
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
    /// If not null, then the options to use to print the return type of the method. If null, the
    /// return type is ignored.
    /// </summary>
    public EasyNameTypeOptions? ReturnTypeOptions { get; init; }

    /// <summary>
    /// If not null, then the options to use to print the host type of the method. If null, the
    /// host type is ignored.
    /// </summary>
    public EasyNameTypeOptions? HostTypeOptions { get; init; }

    /// <summary>
    /// If not null, then the options to use to print the generic type arguments of the method, if
    /// any. If null, the generic type arguments are ignored.
    /// </summary>
    public EasyNameTypeOptions? GenericArgumentOptions { get; init; }

    /// <summary>
    /// Determines if, at least, the method parentheses shall be used even if no parameter options
    /// were given.
    /// </summary>
    public bool UseBrackets { get; init; }

    /// <summary>
    /// If not null, then the options to use with the method parameters. If null, then they are
    /// ignored.
    /// </summary>
    public EasyNameParameterOptions? ParameterOptions { get; init; }

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
    public string EasyName(MethodInfo source)
    {
        source.ThrowWhenNull();

        var host = source.DeclaringType;
        var sb = new StringBuilder();

        // Return type...
        if (ReturnTypeOptions is not null)
        {
            var str = ReturnTypeOptions.EasyName(source.ReturnType);
            if (str.Length > 0) { sb.Append(str); sb.Append(' '); }
        }

        // Host type...
        if (HostTypeOptions is not null && host is not null)
        {
            var str = HostTypeOptions.EasyName(host);
            if (str.Length > 0) { sb.Append(str); sb.Append('.'); }
        }

        // Name...
        sb.Append(source.Name);

        // Generic arguments...
        if (GenericArgumentOptions is not null)
        {
            var args = source.GetGenericArguments();
            if (args.Length > 0)
            {
                sb.Append('<'); for (int i = 0; i < args.Length; i++)
                {
                    var arg = args[i];
                    var str = GenericArgumentOptions.EasyName(arg);
                    if (i > 0) sb.Append(str.Length > 0 ? ", " : ",");
                    sb.Append(str);
                }
                sb.Append('>');
            }
        }

        // Method parameters...
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