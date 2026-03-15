namespace Experimental;

// ========================================================
/// <summary>
/// Provides 'EasyName' capabilities for <see cref="Type"/> instances.
/// </summary>
public record EasyTypeOptions
{
    /// <summary>
    /// Use the type's variance (the 'in' and 'out' keywords), if any is specified.
    /// </summary>
    public bool UseVariance { get; init; }

    // ----------------------------------------------------

    enum Mode { Empty, Default, Full };
    EasyTypeOptions(Mode mode)
    {
        switch (mode)
        {
            case Mode.Full:
                break;

            case Mode.Default:
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
public static partial class EasyNomenExtensions
{
    /// <summary>
    /// Obtains a c#-alike string representation of the given element, using default options.
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    public static string EasyNomen(this Type type) => type.EasyNomen(EasyTypeOptions.Default);

    /// <summary>
    /// Obtains a c#-alike string representation of the given element, using the given options.
    /// </summary>
    /// <param name="type"></param>
    /// <param name="options"></param>
    /// <returns></returns>
    public static string EasyNomen(this Type type, EasyTypeOptions options)
    {
        throw null;
    }
}