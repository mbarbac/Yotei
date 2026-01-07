using System.Data;

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
        this IMethodSymbol source) => EasyNameMethodSymbol.Default.EasyName(source);

    /// <summary>
    /// Obtains the C#-alike easy name of the given element using the given options.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="options"></param>
    /// <returns></returns>
    public static string EasyName(this IMethodSymbol source, EasyNameMethodSymbol options)
    {
        options.ThrowWhenNull();
        return options.EasyName(source);
    }
}

// ========================================================
/// <summary>
/// Provides 'EasyName' capabilities for 'method symbol' instances.
/// </summary>
internal record EasyNameMethodSymbol
{
    /// <summary>
    /// A shared read-only instance that represents empty options.
    /// </summary>
    public static EasyNameMethodSymbol Empty { get; } = new(Mode.Empty);

    /// <summary>
    /// A shared read-only instance that represents default options.
    /// </summary>
    public static EasyNameMethodSymbol Default { get; } = new(Mode.Default);

    /// <summary>
    /// A shared read-only instance that represents full options.
    /// </summary>
    public static EasyNameMethodSymbol Full { get; } = new(Mode.Full);

    /// <summary>
    /// Initializes a new default instance.
    /// </summary>
    public EasyNameMethodSymbol() : this(Mode.Default) { }

    // ----------------------------------------------------

    /// <summary>
    /// Determines if return type modifiers (such as 'in', 'out' and 'ref', and 'ref readonly')
    /// shall be used.
    /// </summary>
    public bool UseModifiers { get; init; }

    /// <summary>
    /// If not null, then the options to use to print the return type of the method. If null, the
    /// return type is ignored. This property is also ignored if the method is a constructor.
    /// </summary>
    public EasyNameTypeSymbol? ReturnTypeOptions { get; init; }

    /// <summary>
    /// If not null, then the options to use to print the host type of the method. If null, the
    /// host type is ignored.
    /// </summary>
    public EasyNameTypeSymbol? HostTypeOptions { get; init; }

    /// <summary>
    /// Determines if, when the method is a constructor, then the constructor tech name shall be
    /// used, instead of the default "new" one.
    /// </summary>
    public bool UseTechName { get; init; }

    /// <summary>
    /// If not null, then the options to use to print the generic type arguments of the method, if
    /// any. If null, the generic type arguments are ignored.
    /// </summary>
    public EasyNameTypeSymbol? GenericArgumentOptions { get; init; }

    /// <summary>
    /// Determines if, at least, the method parentheses shall be used even if no parameter options
    /// were given.
    /// </summary>
    public bool UseBrackets { get; init; }

    /// <summary>
    /// If not null, then the options to use with the method parameters. If null, then they are
    /// ignored.
    /// </summary>
    public EasyNameParameterSymbol? ParameterOptions { get; init; }

    // ----------------------------------------------------

    enum Mode { Empty, Default, Full };
    private EasyNameMethodSymbol(Mode mode)
    {
        switch (mode)
        {
            case Mode.Empty:
                break;

            case Mode.Default:
                break;

            case Mode.Full:
                UseModifiers = true;
                ReturnTypeOptions = EasyNameTypeSymbol.Full;
                HostTypeOptions = EasyNameTypeSymbol.Full;
                GenericArgumentOptions = EasyNameTypeSymbol.Full;
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
    public string EasyName(IMethodSymbol source)
    {
        source.ThrowWhenNull();

        var host = source.ContainingType;
        var sb = new StringBuilder();

        /// <summary>
        /// Static constructor...
        /// </summary>
        if (source.MethodKind == MethodKind.StaticConstructor)
        {
            if (HostTypeOptions is not null || !UseTechName)
            {
                var options = HostTypeOptions ?? EasyNameTypeSymbol.Empty with { HideName = false };
                var str = options.EasyName(host);
                sb.Append(str);
            }
            if (UseTechName)
            {
                if (sb.Length > 0) sb.Append('.');
                sb.Append(source.Name);
            }
        }

        /// <summary>
        /// Standard constructor...
        /// </summary>
        else if (source.MethodKind == MethodKind.Constructor)
        {
            if (HostTypeOptions is not null)
            {
                var options = HostTypeOptions.HideName
                    ? HostTypeOptions with { HideName = false }
                    : HostTypeOptions;

                var str = options.EasyName(host);
                if (str.Length > 0) { sb.Append(str); sb.Append('.'); }
            }

            var name = UseTechName ? source.Name : "new";
            sb.Append(name);
        }

        /// <summary>
        /// Regular method...
        /// </summary>
        else
        {
            // Return type...
            if (ReturnTypeOptions is not null)
            {
                if (UseModifiers)
                {
                    var prefix = source.RefKind switch
                    {
                        RefKind.Ref => "ref ",
                        RefKind.Out => "out ",
                        RefKind.RefReadOnly => "ref readonly ",
                        _ => null
                    };
                    if (prefix != null) sb.Append(prefix);
                }

                var options = ReturnTypeOptions.HideName
                    ? ReturnTypeOptions with { HideName = false }
                    : ReturnTypeOptions;

                var str = options.EasyName(source.ReturnType);
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

            // Generic arguments...
            if (GenericArgumentOptions is not null)
            {
                var args = source.TypeArguments;
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
        }

        /// <summary>
        /// Method parameters... 
        /// </summary>
        if (UseBrackets || ParameterOptions is not null)
        {
            sb.Append('('); if (ParameterOptions is not null)
            {
                var args = source.Parameters;
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