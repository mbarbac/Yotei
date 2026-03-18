namespace Yotei.Tools;

// ========================================================
/// <summary>
/// Provides 'EasyName' capabilities for argument instances.
/// </summary>
public record EasyParameterOptions
{
    /// <summary>
    /// If enabled use the 'this' keyword before the first parameter of extension methods.
    /// </summary>
    public bool UseThis { get; init; }

    /// <summary>
    /// If enabled use the 'params', 'scoped', 'ref', 'in', 'out', 'ByRef', and 'ByVal' keywords
    /// before the parameter, if possible.
    /// </summary>
    public bool UseModifiers { get; init; }

    /// <summary>
    /// If not null, then the options to use for the parameter type. Otherwise, it is ignored.
    /// </summary>
    public EasyTypeOptions? TypeOptions { get; init; }

    /// <summary>
    /// If enabled use the paramerter name. Otherwise, it is ignored.
    /// </summary>
    public bool UseName { get; init; }

    // ----------------------------------------------------

    enum Mode { Empty, Default, Full };
    EasyParameterOptions(Mode mode)
    {
        switch (mode)
        {
            case Mode.Full:
                UseThis = true;
                UseModifiers = true;
                TypeOptions = EasyTypeOptions.Full;
                UseName = true;
                break;

            case Mode.Default:
                UseThis = true;
                UseModifiers = true;
                TypeOptions = EasyTypeOptions.Default;
                break;
        }
    }

    /// <summary>
    /// Initializes a new empty instance.
    /// </summary>
    public EasyParameterOptions() : this(Mode.Empty) { }

    /// <summary>
    /// A shared empty instance.
    /// </summary>
    public static EasyParameterOptions Empty { get; } = new(Mode.Empty);

    /// <summary>
    /// A shared instance with default settings.
    /// </summary>
    public static EasyParameterOptions Default { get; } = new(Mode.Default);

    /// <summary>
    /// A shared instance with full settings.
    /// </summary>
    public static EasyParameterOptions Full { get; } = new(Mode.Full);
}

// ========================================================
public static partial class EasyNameExtensions
{
    /// <summary>
    /// Obtains a c#-alike string representation of the given element, using default options.
    /// </summary>
    /// <param name="source"></param>
    /// <returns></returns>
    public static string EasyName(
        this ParameterInfo source) => source.EasyName(EasyParameterOptions.Default);

    /// <summary>
    /// Obtains a c#-alike string representation of the given element, using the given options.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="options"></param>
    /// <returns></returns>
    public static string EasyName(this ParameterInfo source, EasyParameterOptions options)
    {
        ArgumentNullException.ThrowIfNull(source);
        ArgumentNullException.ThrowIfNull(options);

        var sb = new StringBuilder();

        // Parameter type...
        if (options.TypeOptions != null)
        {
            var xoptions = options.TypeOptions.NoHideName();
            var str = source.ParameterType.EasyName(xoptions);
            sb.Append(str);

            if (sb.Length > 0 && sb[^1] != '?' &&
                options.TypeOptions != null &&
                options.TypeOptions.NullableStyle != EasyNullableStyle.None)
            {
                if (options.TypeOptions.NullableStyle == EasyNullableStyle.KeepWrappers &&
                    source.ParameterType.IsNullableWrapper())
                    goto ENDNULLABLE;

                if (source.HasNullableEnabledAttribute() ||
                    source.ParameterType.HasNullableEnabledAttribute())
                {
                    if (sb.Length > 0 && sb[^1] != '?') sb.Append('?');
                    goto ENDNULLABLE;
                }
            }
            ENDNULLABLE:;
        }

        // Parameter name...
        if (options.UseName)
        {
            if (sb.Length > 0) sb.Append(' ');
            sb.Append(source.Name);
        }

        // Modifiers and alike...
        if (sb.Length > 0)
        {
            if (options.UseThis && IsThisParameter(source)) sb.Insert(0, "this ");
            if (options.UseModifiers)
            {
                if (source.IsIn) sb.Insert(0, "in ");
                else if (source.IsOut) sb.Insert(0, "out ");
                else if (source.ParameterType.IsByRef)
                {
                    var attr = "System.Runtime.CompilerServices.IsReadOnlyAttribute";
                    var ronly = source.GetCustomAttributes().Any(x => x.GetType().FullName == attr);
                    var prefix = ronly ? "ref readonly " : "ref ";
                    sb.Insert(0, prefix);
                }
                if (source.IsDefined(typeof(ParamArrayAttribute), false)) sb.Insert(0, "params ");
            }
        }

        // Finishing...
        return sb.ToString();

        // Determines if the parameter is the 'this' one of an extension method...
        static bool IsThisParameter(ParameterInfo source)
        {
            var method = source.Member as MethodInfo;
            return
                method != null &&
                method.IsDefined(typeof(ExtensionAttribute), false) &&
                source.Position == 0;
        }
    }
}