using System.Diagnostics.Tracing;

namespace Yotei.Tools;

// ========================================================
/// <summary>
/// Describes how to obtain a C#-alike representation of a given type-alike element.
/// </summary>
public record EasyTypeOptions
{
    /// <summary>
    /// If enabled, then use the type's variance (the 'in' and 'out' keywords), if any.
    /// </summary>
    public bool UseVariance { get; init; }

    /// <summary>
    /// The style to use to obtain the namespace, if any, of the given type.
    /// <br/> A not-empty value of this property implies <see cref="UseHost"/>.
    /// </summary>
    public EasyNamespaceStyle NamespaceStyle { get; init; }

    /// <summary>
    /// If enabled, then use the type's host, if any (unless it is a special type).
    /// </summary>
    public bool UseHost { get; init; }

    /// <summary>
    /// If enabled, then use the type's special name, if possible.
    /// <br/> If enabled, it shortcircuit the other name-related options.
    /// </summary>
    public bool UseSpecialNames { get; init; }

    /// <summary>
    /// If enabled, then remove the 'Attribute' suffix, if any.
    /// </summary>
    public bool RemoveAttributeSuffix { get; init; }

    /// <summary>
    /// The style to use when the given type is a nullable one.
    /// </summary>
    public EasyNullableStyle NullableStyle { get; init; }

    /// <summary>
    /// Describes how to include the generic type arguments, if any.
    /// </summary>
    public EasyGenericListStyle GenericListStyle { get; init; }

    // ----------------------------------------------------

    /// <summary>
    /// Initializes a new empty instance.
    /// </summary>
    public EasyTypeOptions() : this(Mode.Empty) { }

    /// <summary>
    /// A shared instance with empty-alike settings.
    /// </summary>
    public static EasyTypeOptions Empty { get; } = new(Mode.Empty);

    /// <summary>
    /// A shared instance with default-alike settings.
    /// </summary>
    public static EasyTypeOptions Default { get; } = new(Mode.Default);

    /// <summary>
    /// A shared instance with full-alike settings.
    /// </summary>
    public static EasyTypeOptions Full { get; } = new(Mode.Full);

    /// <summary>
    /// Options for the private constructor.
    /// </summary>
    enum Mode { Empty, Default, Full };
    EasyTypeOptions(Mode mode)
    {
        UseVariance = false;
        NamespaceStyle = EasyNamespaceStyle.None;
        UseHost = false;
        UseSpecialNames = true;
        RemoveAttributeSuffix = false;
        NullableStyle = EasyNullableStyle.None;
        GenericListStyle = EasyGenericListStyle.None;

        switch (mode)
        {
            case Mode.Default:
                RemoveAttributeSuffix = true;
                NullableStyle = EasyNullableStyle.UseAnnotations;
                GenericListStyle = EasyGenericListStyle.UseNames;
                break;

            case Mode.Full:
                NamespaceStyle = EasyNamespaceStyle.Default;
                UseHost = true;
                NullableStyle = EasyNullableStyle.KeepWrappers;
                GenericListStyle = EasyGenericListStyle.UseNames;
                break;
        }
    }
}

// ========================================================
public static partial class EasyNameExtensions
{
    /// <summary>
    /// Obtains a C#-alike representation for a given type-alike element, using default options.
    /// </summary>
    /// <param name="source"></param>
    /// <returns></returns>
    public static string EasyName(
        this Type source) => source.EasyName(EasyTypeOptions.Default);

    /// <summary>
    /// Obtains a C#-alike representation for a given type-alike element, using the given options.
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
    /// Invoked when the details of the original closed/bounded generic arguments, if any, have
    /// been captured. Otherwise, through recursion, those details are lost.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="types"></param>
    /// <param name="options"></param>
    /// <returns></returns>
    static string EasyName(this Type source, Type[] types, EasyTypeOptions options)
    {
        // Intercepting wrappers...
        if (source.IsNullableWrapper() &&
            options.NullableStyle != EasyNullableStyle.KeepWrappers)
        {
            var arg = source.GetGenericArguments()[0];
            var str = arg.EasyName(options);
            if (str.Length > 0 && str[^1] != '?') str += '?';
            return str;
        }

        // Capturing...
        var sb = new StringBuilder();
        var isgen = source.IsGenericAlike();
        var host = source.DeclaringType;
        var xname = options.UseSpecialNames ? source.ToSpecialName() : null;

        // Variance...
        if (options.UseVariance && source.IsGenericParameter)
        {
            var gpa = source.GenericParameterAttributes;
            var variance = gpa & GenericParameterAttributes.VarianceMask;

            if ((variance & GenericParameterAttributes.Covariant) != 0) sb.Append("out ");
            if ((variance & GenericParameterAttributes.Contravariant) != 0) sb.Append("in ");
        }

        // Namespace...
        if (options.NamespaceStyle != EasyNamespaceStyle.None &&
            xname == null &&
            host == null && !isgen)
        {
            var str = source.Namespace;
            if (str != null && str.Length > 0)
            {
                if (options.NamespaceStyle == EasyNamespaceStyle.UseGlobal) sb.Append("global::");
                sb.Append(str).Append('.');
            }
        }

        // Host...
        if ((options.UseHost || options.NamespaceStyle != EasyNamespaceStyle.None) &&
            xname == null &&
            host != null && !isgen)
        {
            var str = host.EasyName(types, options); // Using captured types...
            if (str.Length > 0) sb.Append(str).Append('.');
        }

        // Name...
        if (xname != null) sb.Append(xname);
        else
        {
            var name = source.Name;
            var index = name.IndexOf('`'); if (index >= 0) name = name[..index];
            if (name.Length > 0 && name[^1] == '&') name = name[..^1];

            if (options.RemoveAttributeSuffix &&
                name != ATTRIBUTE &&
                name.EndsWith(ATTRIBUTE))
                name = name.RemoveLast(ATTRIBUTE).ToString();

            sb.Append(name);
        }

        // Generic attributes...
        if (xname == null && options.GenericListStyle != EasyGenericListStyle.None)
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
        while (options.NullableStyle != EasyNullableStyle.None && sb.Length > 0 && sb[^1] != '?')
        {
            // Host source is a wrapped nullable...
            if (source.IsNullableWrapper())
            {
                if (options.NullableStyle == EasyNullableStyle.KeepWrappers)
                {
                    if (xname != null) sb.Append('?');
                    break;
                }
            }

            // Standard case...
            if (source.IsNullableAnnotated())
            {
                sb.Append('?'); break;
            }

            break;
        }

        // Finishing...
        return sb.ToString();
    }
}