namespace Yotei.Tools;

// ========================================================
/// <summary>
/// Provides options for the <see cref="EasyTypeExtensions"/> methods.
/// </summary>
public record EasyTypeOptions
{
    /// <summary>
    /// Use the declaring type of the given one, if any. <c>false</c> by default.
    /// </summary>
    public bool UseHostType { get; init; }

    /// <summary>
    /// Use the namespace of the given type. <c>false</c> by default.
    /// <br/> <c>true</c> implies <see cref="UseHostType"/>.
    /// </summary>
    public bool UseNamespace { get; init; }

    // ----------------------------------------------------

    /// <summary>
    /// A common shared instance with all properties set to null or false.
    /// </summary>
    public static EasyTypeOptions False { get; } = new();

    /// <summary>
    /// A common shared instance with all properties set to their full values.
    /// </summary>
    public static EasyTypeOptions True { get; } = new(
        useHostType: true,
        useNamespace: true);

    /// <summary>
    /// Initializes a new instance with all its properties set to null or false.
    /// </summary>
    /// <param name="useHostType"></param>
    /// <param name="useNamespace"></param>
    public EasyTypeOptions(
        bool useHostType = false,
        bool useNamespace = false)
    {
        UseHostType = useHostType;
        UseNamespace = useNamespace;
    }

    /// <summary>
    /// Returns a new instance with the options adjusted, or the original one if no chages were
    /// needed.
    /// </summary>
    /// <returns></returns>
    public EasyTypeOptions Adjust()
    {
        var options = this;
        if (UseNamespace && !UseHostType) options = options with { UseHostType = true };

        return options;
    }
}