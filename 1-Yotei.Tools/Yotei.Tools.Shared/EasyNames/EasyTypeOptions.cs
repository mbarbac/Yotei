namespace Yotei.Tools;

// ========================================================
/// <summary>
/// Describes how to obtain a C#-alike representation of a given type-alike element.
/// </summary>
public record EasyTypeOptions
{
    /// <summary>
    /// If enabled, then the returned string will be an empty one to be used as a placeholder.
    /// This setting is moslty used when the intent is to obtain an anonymous list of generic
    /// type arguments
    /// </summary>
    public bool UsePlaceHolder { get; set; }

    /// <summary>
    /// If enabled, then use the type's variance (the 'in' and 'out' keywords), if any.
    /// </summary>
    public bool UseVariance { get; set; }

    /// <summary>
    /// The style to use to obtain the namespace, if any, of the given type.
    /// <br/> A not-empty value of this property implies <see cref="UseHost"/>.
    /// </summary>
    public EasyNamespaceStyle NamespaceStyle { get; set; }

    /// <summary>
    /// If enabled, then use the type's host, if any (unless it is a special type).
    /// </summary>
    public bool UseHost { get; set; }

    /// <summary>
    /// If enabled, then use the type's special name, if possible.
    /// <br/> If enabled, it shortcircuit the other name-related options.
    /// </summary>
    public bool UseSpecialNames { get; set; }

    /// <summary>
    /// If enabled, then remove the 'Attribute' suffix, if any.
    /// </summary>
    public bool RemoveAttributeSuffix { get; set; }

    /// <summary>
    /// The style to use when the given type is a nullable one.
    /// </summary>
    public EasyNullableStyle NullableStyle { get; set; }

    /// <summary>
    /// The options to use with the generic type arguments, if any. If null, then that list of
    /// generic type arguments is ignored.
    /// <br/> Otherwise, by default, it refers to its host instance to use the same settings.
    /// <br/> To modify how the list is obtained set the value of this property to null, or to
    /// a new instance with the desired settings. For instance, to obtain an anonymous list, the
    /// new instace shall have its own <see cref="UsePlaceHolder"/> value set to true.
    /// </summary>
    public EasyTypeOptions? GenericListOptions { get; set; }

    // ----------------------------------------------------

    public enum Mode { Empty, Default, DefaultEx, Full };
    EasyTypeOptions(Mode mode)
    {
        UsePlaceHolder = false;
        UseVariance = false;
        NamespaceStyle = EasyNamespaceStyle.None;
        UseHost = false;
        UseSpecialNames = true;
        RemoveAttributeSuffix = false;
        NullableStyle = EasyNullableStyle.None;
        GenericListOptions = null;

        switch (mode)
        {
            case Mode.Default:
                RemoveAttributeSuffix = true;
                UseSpecialNames = true;
                NullableStyle = EasyNullableStyle.UseAnnotations;
                GenericListOptions = this;
                break;

            case Mode.DefaultEx:
                NamespaceStyle = EasyNamespaceStyle.Default;
                UseHost = true;
                RemoveAttributeSuffix = true;
                UseSpecialNames = true;
                NullableStyle = EasyNullableStyle.UseAnnotations;
                GenericListOptions = this;
                break;

            case Mode.Full:
                NamespaceStyle = EasyNamespaceStyle.Default;
                UseHost = true;
                UseSpecialNames = false;
                NullableStyle = EasyNullableStyle.KeepWrappers;
                GenericListOptions = this;
                break;
        }
    }

    /// <summary>
    /// Initializes a new empty instance.
    /// </summary>
    public EasyTypeOptions() : this(Mode.Empty) { }

    /// <summary>
    /// Obtains a new empty-alike instance.
    /// </summary>
    public static EasyTypeOptions Empty => new(Mode.Empty);

    /// <summary>
    /// Obtains a new default-alike instance.
    /// </summary>
    public static EasyTypeOptions Default => new(Mode.Default);

    /// <summary>
    /// Obtains a new default-alike instance that includes the namespace and host.
    /// </summary>
    public static EasyTypeOptions DefaultEx => new(Mode.DefaultEx);

    /// <summary>
    /// Obtains a new full-alike instance.
    /// </summary>
    public static EasyTypeOptions Full => new(Mode.Full);
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
        this Type source) => source.EasyName(new EasyTypeOptions());

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
        throw null;
    }
}