namespace Yotei.Tools;

// ========================================================
/// <summary>
/// Describes how to obtain a C#-alike representation of a given type-alike element.
/// </summary>
public record EasyTypeOptions
{
    /// <summary>
    /// If enabled, the easy name produced will be an empty string, short-circuiting all other
    /// options. This setting is only used when the given element is part of a generic list of
    /// arguments.
    /// </summary>
    public bool HideName { get; init; }

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
    /// If not null, then the options to use to obtain the C#-alike representation of the generic
    /// arguments of the given type, if any. If null, then that list is ignored. To obtain just a
    /// list of place holders, enable the <see cref="HideName"/> property of these options.
    /// </summary>
    public EasyTypeOptions? GenericsOptions { get; init; }

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
        switch (mode)
        {
            case Mode.Empty:
                HideName = false;
                UseVariance = false;
                NamespaceStyle = EasyNamespaceStyle.None;
                UseHost = false;
                UseSpecialNames = true;
                RemoveAttributeSuffix = true;
                NullableStyle = EasyNullableStyle.None;
                GenericsOptions = this;
                break;

            case Mode.Default:
                HideName = false;
                UseVariance = false;
                NamespaceStyle = EasyNamespaceStyle.None;
                UseHost = false;
                UseSpecialNames = true;
                RemoveAttributeSuffix = true;
                NullableStyle = EasyNullableStyle.UseAnnotations;
                GenericsOptions = this;
                break;

            case Mode.Full:
                HideName = false;
                UseVariance = false;
                NamespaceStyle = EasyNamespaceStyle.Default;
                UseHost = true;
                UseSpecialNames = true;
                RemoveAttributeSuffix = false;
                NullableStyle = EasyNullableStyle.KeepWrappers;
                GenericsOptions = this;
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
        var sb = new StringBuilder();



        throw null;
    }
}