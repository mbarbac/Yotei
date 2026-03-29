#if YOTEI_TOOLS_GENERATORS
namespace Yotei.Tools.Generators;
#else
namespace Yotei.Tools;
#endif

// ========================================================
/// <summary>
/// Provides 'EasyName' capabilities for type instances.
/// </summary>
#if YOTEI_TOOLS_GENERATORS
internal
#else
public
#endif
record EasyTypeOptions
{
    /// <summary>
    /// If enabled use the type's variance (the 'in' and 'out' keywords) in the display string,
    /// if any is specified. Otherwise, it is ignored.
    /// </summary>
    public bool UseVariance { get; init; }

    /// <summary>
    /// The style to use with the type's namespace.
    /// </summary>
    public EasyNamespaceStyle NamespaceStyle { get; init; }

    /// <summary>
    /// If enabled use the type hosts chain in the display string (with this options). Otherwise,
    /// it is ignored.
    /// </summary>
    public bool UseHost { get; init; }

    /// <summary>
    /// If enabled inconditionally return an empty string as the display string. This setting is
    /// mostly used to obtain an anonymous list of generic arguments. When enabled, it shortcuts
    /// all other settings.
    /// </summary>
    public bool HideName { get; init; }

    /// <summary>
    /// Returns either a new instance with the 'HideName' setting set to false, or the current
    /// instance if it was already false.
    /// </summary>
    /// <returns></returns>
    public EasyTypeOptions WithHideNameFalse() =>
        HideName
        ? this with { HideName = false }
        : this;

    /// <summary>
    /// Returns either a new instance with the 'HideName' setting set to true, or the current
    /// instance if it was already true.
    /// </summary>
    /// <returns></returns>
    public EasyTypeOptions WithHideNameTrue() =>
        HideName
        ? this
        : this with { HideName = true };

    /// <summary>
    /// If enabled use in the display string the predefined keywords for known special types (eg:
    /// <see langword="int"/> instead of <see langword="Int32"/>).
    /// </summary>
    public bool UseSpecialNames { get; init; }

    /// <summary>
    /// If enabled remove the 'Attribute' suffix from the type's display string. Otherwise,
    /// it is kept in the display string.
    /// </summary>
    public bool RemoveAttributeSuffix { get; init; }

    /// <summary>
    /// Determines the style to use with nullable annotations.
    /// </summary>
    public EasyNullableStyle NullableStyle { get; init; }

    /// <summary>
    /// Determines the style to use with generic arguments.
    /// </summary>
    public EasyGenericStyle GenericStyle { get; init; }

    // ----------------------------------------------------

    enum Mode { Empty, Default, Full };
    EasyTypeOptions(Mode mode)
    {
        switch (mode)
        {
            case Mode.Full:
                UseVariance = true;
                NamespaceStyle = EasyNamespaceStyle.Standard;
                UseHost = true;
                UseSpecialNames = false;
                RemoveAttributeSuffix = false;
                NullableStyle = EasyNullableStyle.KeepWrappers;
                GenericStyle = EasyGenericStyle.UseEasyNames;
                break;

            case Mode.Default:
                NamespaceStyle = EasyNamespaceStyle.None;
                UseSpecialNames = true;
                RemoveAttributeSuffix = true;
                NullableStyle = EasyNullableStyle.UseAnnotations;
                GenericStyle = EasyGenericStyle.UseEasyNames;
                break;

            case Mode.Empty:
                NamespaceStyle = EasyNamespaceStyle.None;
                NullableStyle = EasyNullableStyle.None;
                GenericStyle = EasyGenericStyle.None;
                break;
        }
    }

    /// <summary>
    /// Initializes a new empty instance.
    /// </summary>
    public EasyTypeOptions() : this(Mode.Empty) { }

    /// <summary>
    /// A shared empty instance.
    /// </summary>
    public static EasyTypeOptions Empty { get; } = new(Mode.Empty);

    /// <summary>
    /// A shared instance with default settings.
    /// </summary>
    public static EasyTypeOptions Default { get; } = new(Mode.Default);

    /// <summary>
    /// A shared instance with full settings.
    /// </summary>
    public static EasyTypeOptions Full { get; } = new(Mode.Full);
}

// ========================================================
#if YOTEI_TOOLS_GENERATORS
internal
#else
public
# endif
static partial class EasyNameExtensions
{
    /// <summary>
    /// Obtains a c#-alike string representation of the given element, using default options.
    /// </summary>
    /// <param name="source"></param>
    /// <returns></returns>
    public static string EasyName(this Type source) => source.EasyName(EasyTypeOptions.Default);

    /// <summary>
    /// Obtains a c#-alike string representation of the given element, using the given options.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="options"></param>
    /// <returns></returns>
    public static string EasyName(this Type source, EasyTypeOptions options)
    {
        ArgumentNullException.ThrowIfNull(source);
        ArgumentNullException.ThrowIfNull(options);

        var types = source.GetGenericArguments();
        return source.EasyName(types, options);
    }

    // ----------------------------------------------------

    /// <summary>
    /// Invoked once the details of the closed generic arguments of the original type have been
    /// obtained. Otherwise, these details will be lost after recursive calls.
    /// </summary>
    static string EasyName(this Type source, Type[] types, EasyTypeOptions options)
    {
        // Inconditional shortcut...
        if (options.HideName) return string.Empty;

        // Processing...
        var sb = new StringBuilder();
        var isgen = source.IsGenericAlike();
        var host = source.DeclaringType;

        // Shortcut nullable wrappers...
        if (options.NullableStyle == EasyNullableStyle.UseAnnotations &&
            source.IsNullableWrapper())
        {
            var arg = source.GetGenericArguments()[0];
            var str = arg.EasyName(options);
            if (str.Length > 0 && !str.EndsWith('?')) str += '?';
            return str;
        }

        // Variance...
        if (options.UseVariance && source.IsGenericParameter)
        {
            var gpa = source.GenericParameterAttributes;
            var variance = gpa & GenericParameterAttributes.VarianceMask;

            if ((variance & GenericParameterAttributes.Covariant) != 0) sb.Append("out ");
            if ((variance & GenericParameterAttributes.Contravariant) != 0) sb.Append("in ");
        }

        // Special names...
        var xname = options.UseSpecialNames ? source.ToSpecialName() : null;

        // Namespace...
        if (options.NamespaceStyle != EasyNamespaceStyle.None &&
            host == null && !isgen &&
            xname == null)
        {
            var str = source.Namespace;
            if (str != null && str.Length > 0)
            {
                if (options.NamespaceStyle == EasyNamespaceStyle.UseGlobal) sb.Append("global:");
                sb.Append(str).Append('.');
            }
        }

        // Host...
        if ((options.UseHost || options.NamespaceStyle != EasyNamespaceStyle.None) &&
            host != null && !isgen &&
            xname == null)
        {
            var xoptions = options.WithHideNameFalse();
            var str = host.EasyName(types, xoptions);
            if (str.Length > 0) sb.Append(str).Append('.');
        }

        // Name...
        string? name = xname; if (name == null)
        {
            name = source.Name;
            var index = name.IndexOf('`'); if (index >= 0) name = name[..index];
            if (name.Length > 0 && name[^1] == '&') name = name[..^1];

            if (options.RemoveAttributeSuffix &&
                name != "Attribute" &&
                name.EndsWith("Attribute"))
                name = name.RemoveLast("Attribute").ToString();
        }
        sb.Append(name);

        // Generic arguments...
        if (options.GenericStyle != EasyGenericStyle.None)
        {
            var args = source.GetGenericArguments();
            var used = host == null ? 0 : host.GetGenericArguments().Length;
            var need = args.Length - used;

            if (need > 0)
            {
                var xoptions = options.GenericStyle == EasyGenericStyle.PlaceHolders
                    ? options.WithHideNameTrue()
                    : options.WithHideNameFalse();

                sb.Append('<'); for (int i = 0; i < need; i++)
                {
                    var arg = types[used + i];
                    var str = arg.EasyName(xoptions);
                    if (i > 0) sb.Append(str.Length > 0 ? ", " : ",");
                    sb.Append(str);
                }
                sb.Append('>');
            }
        }

        // Nullable annotations...
        if (options.NullableStyle != EasyNullableStyle.None)
        {
            if (options.NullableStyle == EasyNullableStyle.KeepWrappers &&
                source.IsNullableWrapper())
                goto ENDNULLABLE;

            if (source.HasNullableEnabledAttribute())
            {
                if (sb.Length > 0 && sb[^1] != '?') sb.Append('?');
                goto ENDNULLABLE;
            }
        }
        ENDNULLABLE:;

        // Finishing...
        return sb.ToString();
    }
}