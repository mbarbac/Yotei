namespace Yotei.Tools;

// ========================================================
/// <summary>
/// Describes how to obtain a C#-alike representation of a given property-alike element.
/// </summary>
public sealed record EasyPropertyOptions
{

    // ----------------------------------------------------

    // Internal constructor
    public enum Mode { Empty, Default, Full };
    EasyPropertyOptions(Mode mode)
    {
        switch (mode)
        {
            case Mode.Default:
                break;

            case Mode.Full:
                break;
        }
    }

    /// <summary>
    /// Initializes a new empty instance.
    /// </summary>
    public EasyPropertyOptions() : this(Mode.Empty) { }

    /// <summary>
    /// Obtains a new instance with empty-alike values that obtains the bare minimum display
    /// string.
    /// </summary>
    public static EasyPropertyOptions Empty => new(Mode.Empty);

    /// <summary>
    /// Obtains a new instance with default-alike values that obtains the most common display
    /// string.
    /// </summary>
    public static EasyPropertyOptions Default => new(Mode.Default);

    /// <summary>
    /// Obtains a new instance with full-alike values that obtains a full display string.
    /// </summary>
    public static EasyPropertyOptions Full => new(Mode.Full);
}