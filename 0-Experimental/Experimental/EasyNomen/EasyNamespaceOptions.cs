namespace Experimental;

// ========================================================
/// <summary>
/// Provides 'EasyName' capabilities for the namespace portion of other elements.
/// </summary>
public record EasyNamespaceOptions
{
    /// <summary>
    /// Use the host namespace of the given one.
    /// </summary>
    public bool UseHostNamespace { get; init; }

    /// <summary>
    /// Use the 'global' namespace.
    /// </summary>
    public bool UseGlobalNamespace { get; init; }

    // ----------------------------------------------------

    enum Mode { Empty, Default, Full };
    EasyNamespaceOptions(Mode mode)
    {
        switch (mode)
        {
            case Mode.Full:
                UseHostNamespace = true;
                UseGlobalNamespace = true;
                break;

            case Mode.Default:
                UseHostNamespace = true;
                break;
        }
    }

    /// <summary>
    /// Initializes a new empty instance.
    /// </summary>
    public EasyNamespaceOptions() : this(Mode.Empty) { }

    /// <summary>
    /// A shared empty instance.
    /// </summary>
    public static EasyNamespaceOptions Empty { get; } = new(Mode.Empty);

    /// <summary>
    /// A shared instance with default settings.
    /// </summary>
    public static EasyNamespaceOptions Default { get; } = new(Mode.Default);

    /// <summary>
    /// A shared instance with full settings.
    /// </summary>
    public static EasyNamespaceOptions Full { get; } = new(Mode.Full);
}