#if YOTEI_TOOLS_GENERATORS
namespace Yotei.Tools.Generators;
#else
namespace Yotei.Tools;
#endif

// ========================================================
/// <summary>
/// Describes how to obtain the display string of a parameter element.
/// </summary>
#if YOTEI_TOOLS_GENERATORS
internal
#else
public
#endif
record EasyParameterOptions
{
    /// <summary>
    /// If enabled, then use the 'this' prefix before the first parameter of an extension method.
    /// </summary>
    public bool UseThis { get; init; }

    /// <summary>
    /// If enabled, then use the element's modifiers (params, scoped, ref, in, out, ...) keywords,
    /// if possible. Otherwise, they are ignored.
    /// </summary>
    public bool UseModifiers { get; init; }

    /// <summary>
    /// If not null, then the options to use with the element's type. Otherwise, it is ignored.
    /// </summary>
    public EasyTypeOptions? TypeOptions { get; init; }

    /// <summary>
    /// If enabled, then use the element's name. Otherwise, it is ignored.
    /// </summary>
    public bool UseName{ get; init; }

    // ----------------------------------------------------

    /// <summary>
    /// Initializes a new empty instance.
    /// </summary>
    public EasyParameterOptions() { }

    /// <summary>
    /// A shared instance with empty-alike settings.
    /// </summary>
    public static EasyParameterOptions Empty { get; } = new();

    /// <summary>
    /// A shared instance with default-alike settings.
    /// </summary>
    public static EasyParameterOptions Default { get; } = new()
    {
        UseThis = true,
        UseModifiers = true,
        TypeOptions = EasyTypeOptions.Default,
    };

    /// <summary>
    /// A shared instance with full-alike settings.
    /// </summary>
    public static EasyParameterOptions Full { get; } = new()
    {
        UseThis = true,
        UseModifiers = true,
        TypeOptions = EasyTypeOptions.Full,
        UseName = true,
    };
}

// ========================================================
#if YOTEI_TOOLS_GENERATORS
internal
#else
public
#endif
static partial class EasyNameExtensions
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
    public static string EasyName(
        this ParameterInfo source, EasyParameterOptions options)
    {
        ArgumentNullException.ThrowIfNull(source);
        ArgumentNullException.ThrowIfNull(options);

        var sb = new StringBuilder();

        // Parameter type...
        if (options.TypeOptions != null)
        {
            // The type itself...
            var xoptions = options.TypeOptions;
            var type = source.ParameterType;
            var str = type.EasyName(xoptions);

            // Nullability...
            while (xoptions.NullableStyle != EasyNullableStyle.None)
            {
                if (xoptions.NullableStyle == EasyNullableStyle.KeepWrappers &&
                    type.IsNullableWrapper())
                    break;

                if (str.Length > 0 && str[^1] != '?')
                {
                    if (type.IsNullableAnnotated()) { str += '?'; break; }
                    if (source.IsNullableAnnotated()) { str += '?'; break; }
                }

                break;
            }

            // Adding...
            if (str.Length > 0) sb.Append(str); // no space here!
        }

        // Parameter name...
        if (options.UseName)
        {
            if (sb.Length > 0) sb.Append(' ');
            sb.Append(source.Name);
        }

        // Modifiers...
        if (sb.Length > 0)
        {
            if (options.UseModifiers)
            {
                if (source.IsIn) sb.Insert(0, "in ");
                else if (source.IsOut) sb.Insert(0, "out ");
                else if (source.ParameterType.IsByRef)
                {
                    var ronly =
                        source.HasReadOnlyAttribute() ||
                        source.ParameterType.HasReadOnlyAttribute();

                    sb.Insert(0, ronly ? "ref readonly " : "ref ");
                }

                if (source.IsDefined(typeof(ParamArrayAttribute), false)) sb.Insert(0, "params ");
            }
            if (options.UseThis)
            {
                var method = source.Member as MethodInfo;
                if (method != null &&
                    method.IsDefined(typeof(ExtensionAttribute), false) &&
                    source.Position == 0)
                    sb.Insert(0, "params ");
            }
        }

        // Finishing...
        return sb.ToString();
    }
}