namespace Yotei.Tools;

// ========================================================
/// <summary>
/// Describes how to obtain a C#-alike representation of a given parameter-alike element.
/// </summary>
public sealed record EasyFieldOptions
{

    // ----------------------------------------------------

    // Internal constructor
    public enum Mode { Empty, Default, Full };
    EasyFieldOptions(Mode mode)
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
    public EasyFieldOptions() : this(Mode.Empty) { }

    /// <summary>
    /// Obtains a new instance with empty-alike values that obtains the bare minimum display
    /// string.
    /// </summary>
    public static EasyFieldOptions Empty => new(Mode.Empty);

    /// <summary>
    /// Obtains a new instance with default-alike values that obtains the most common display
    /// string, without any modifiers.
    /// </summary>
    public static EasyFieldOptions Default => new(Mode.Default);

    /// <summary>
    /// Obtains a new instance with full-alike values that obtains a full display string including
    /// its modifiers and wrappers.
    /// </summary>
    public static EasyFieldOptions Full => new(Mode.Full);
}