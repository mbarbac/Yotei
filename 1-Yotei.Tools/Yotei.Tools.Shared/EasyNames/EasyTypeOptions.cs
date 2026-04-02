#if YOTEI_TOOLS_GENERATORS
namespace Yotei.Tools.Generators;
#else
namespace Yotei.Tools;
#endif

// ========================================================
/// <summary>
/// Describes how to obtain the display string of a type element.
/// </summary>
[DebuggerDisplay("{ToDebugString()}")]
#if YOTEI_TOOLS_GENERATORS
internal
#else
public
#endif
record EasyTypeOptions
{
    /// <summary>
    /// If enabled, then use the type's variance (the 'in' and 'out' keywords), if any. Otherwise,
    /// it is ignored.
    /// </summary>
    public bool UseVariance { get; init; }

    /// <summary>
    /// The style to use to obtain namespace, if any, of the given type.
    /// </summary>
    public EasyNamespaceStyle NamespaceStyle { get; init; } = EasyNamespaceStyle.None;

    /// <summary>
    /// If enabled, then use the type's host, if any, to obtain the display string. Otherwise,
    /// it is ignored.
    /// </summary>
    public bool UseHost { get; init; }

    /// <summary>
    /// If enabled, then use the type's special name, if any, in the display string.
    /// </summary>
    public bool UseSpecialNames { get; init; }

    /// <summary>
    /// If enabled, then remove the 'Attribute' suffix, if any, from the display string.
    /// </summary>
    public bool RemoveAttributeSuffix { get; init; }

    /// <summary>
    /// The style to use when the given type is a nullable one.
    /// </summary>
    public EasyNullableStyle NullableStyle { get; init; } = EasyNullableStyle.None;

    /// <summary>
    /// The style to use with the generic type arguments of the given type, if any.
    /// </summary>
    public EasyGenericListStyle GenericListStyle { get; init; } = EasyGenericListStyle.None;

    // ----------------------------------------------------

    /// <summary>
    /// Initializes a new empty instance.
    /// </summary>
    public EasyTypeOptions() { }

    /// <summary>
    /// A shared instance with empty-alike settings.
    /// </summary>
    public static EasyTypeOptions Empty { get; } = new()
    {
        NamespaceStyle = EasyNamespaceStyle.None,
        UseSpecialNames = true,
        NullableStyle = EasyNullableStyle.None,
        GenericListStyle = EasyGenericListStyle.None,
    };

    /// <summary>
    /// A shared instance with default-alike settings.
    /// </summary>
    public static EasyTypeOptions Default { get; } = new()
    {
        UseVariance = true,
        NamespaceStyle = EasyNamespaceStyle.None,
        UseSpecialNames = true,
        RemoveAttributeSuffix = true,
        NullableStyle = EasyNullableStyle.UseAnnotations,
        GenericListStyle = EasyGenericListStyle.UseNames,
    };

    /// <summary>
    /// A shared instance with full-alike settings.
    /// </summary>
    public static EasyTypeOptions Full { get; } = new()
    {
        UseVariance = true,
        NamespaceStyle = EasyNamespaceStyle.Default,
        UseHost = true,
        UseSpecialNames = false,
        RemoveAttributeSuffix = false,
        NullableStyle = EasyNullableStyle.KeepWrappers,
        GenericListStyle = EasyGenericListStyle.UseNames,
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
    /// Invoked once the details of the closed (bound) generic arguments of the original type
    /// have been obtained. Otherwise, through recursion, these details are lost.
    /// </summary>
    static string EasyName(this Type source, Type[] types, EasyTypeOptions options)
    {
        var sb = new StringBuilder();
        var isgen = source.IsGenericAlike();
        var host = source.DeclaringType;
        var xname = options.UseSpecialNames ? source.ToSpecialName() : null;

        // Intercepting nullable wrappers...
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

        // Namespace...
        if (options.NamespaceStyle != EasyNamespaceStyle.None && !isgen &&
            host == null &&
            xname == null)
        {
            var str = source.Namespace;
            if (str != null && str.Length > 0)
            {
                if (options.NamespaceStyle == EasyNamespaceStyle.UseGlobal) sb.Append("global::");
                sb.Append(str).Append('.');
            }
        }

        // Host...
        if ((options.UseHost || options.NamespaceStyle != EasyNamespaceStyle.None) && !isgen &&
            host != null &&
            xname == null)
        {
            // Here is where we need to use the captured type arguments...
            var str = host.EasyName(types, options);
            if (str.Length > 0) sb.Append(str).Append('.');
        }

        // Name...
        if (xname != null) sb.Append(xname);
        else
        {
            var str = source.Name;
            var index = str.IndexOf('`'); if (index >= 0) str = str[..index];
            if (str.Length > 0 && str[^1] == '&') str = str[..^1];

            if (options.RemoveAttributeSuffix &&
                str != ATTRIBUTE &&
                str.EndsWith(ATTRIBUTE))
                str = str.RemoveLast(ATTRIBUTE).ToString();

            sb.Append(str);
        }

        // Generic arguments...
        if (options.GenericListStyle != EasyGenericListStyle.None)
        {
            var args = source.GetGenericArguments();
            var used = host == null ? 0 : host.GetGenericArguments().Length;
            var need = args.Length - used;

            if (need > 0)
            {
                sb.Append('<'); for (int i = 0; i < need; i++)
                {
                    var arg = types[used + i];
                    var str = options.GenericListStyle == EasyGenericListStyle.PlaceHolders
                        ? string.Empty
                        : arg.EasyName(options);

                    if (i > 0) sb.Append(str.Length > 0 ? ", " : ",");
                    sb.Append(str);
                }
                sb.Append('>');
            }
        }

        // Nullable annotations...
        while (options.NullableStyle != EasyNullableStyle.None)
        {
            if (options.NullableStyle == EasyNullableStyle.KeepWrappers &&
                source.IsNullableWrapper())
                break;

            if (source.IsNullableAnnotated() &&
                sb.Length > 0 &&
                sb[^1] != '?') sb.Append('?');

            break;
        }

        // Finishing...
        return sb.ToString();
    }
}