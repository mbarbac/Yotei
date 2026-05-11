namespace Yotei.Tools;

// ========================================================
/// <summary>
/// Describes how to obtain a C#-alike representation of a given parameter-alike element.
/// </summary>
public sealed record EasyParameterOptions
{
    /// <summary>
    /// If enabled, and if the given parameter is the first one of an extension method, then use
    /// the 'this' prefix.
    /// </summary>
    public bool UseThis { get; set; }

    /// <summary>
    /// If enabled, then use the parameter modifiers (such as: params, scoped, ref, in, out...)
    /// if any.
    /// </summary>
    public bool UseModifiers { get; set; }

    /// <summary>
    /// If not null, then the options to use to obtain the easy name of the parameter's type.
    /// If null, then the type is ignored.
    /// </summary>
    public EasyTypeOptions? TypeOptions { get; set; }

    /// <summary>
    /// If enabled, then use the parameter's name. Otherwise, it is ignored.
    /// </summary>
    public bool UseName { get; set; }

    // ----------------------------------------------------

    // Internal constructor
    public enum Mode { Empty, Default, Full };
    EasyParameterOptions(Mode mode)
    {
        UseThis = false;
        UseModifiers = false;
        TypeOptions = null;
        UseName = false;

        switch (mode)
        {
            case Mode.Empty:
                UseName = true;
                break;

            case Mode.Default:
                UseThis = true;
                UseModifiers = true;
                TypeOptions = EasyTypeOptions.Default.WithRecursive(
                    useVariance: false,
                    useAccessibility: false,
                    useModifiers: false,
                    useKind: false);
                break;

            case Mode.Full:
                UseThis = true;
                UseModifiers = true;
                TypeOptions = EasyTypeOptions.Full.WithRecursive(
                    useVariance: false,
                    useAccessibility: false,
                    useModifiers: false,
                    useKind: false);
                UseName = true;
                break;
        }
    }

    /// <summary>
    /// Initializes a new empty instance.
    /// </summary>
    public EasyParameterOptions() : this(Mode.Empty) { }

    /// <summary>
    /// Obtains a new instance with empty-alike values that obtains the bare minimum display
    /// string.
    /// </summary>
    public static EasyParameterOptions Empty => new(Mode.Empty);

    /// <summary>
    /// Obtains a new instance with default-alike values that obtains the most common display
    /// string, without any modifiers.
    /// </summary>
    public static EasyParameterOptions Default => new(Mode.Default);

    /// <summary>
    /// Obtains a new instance with full-alike values that obtains a full display string including
    /// its modifiers and wrappers.
    /// </summary>
    public static EasyParameterOptions Full => new(Mode.Full);
}