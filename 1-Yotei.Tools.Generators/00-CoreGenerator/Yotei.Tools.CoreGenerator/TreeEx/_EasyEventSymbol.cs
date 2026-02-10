namespace Yotei.Tools.CoreGenerator;

// ========================================================
internal record EasyEventSymbol
{
    // ----------------------------------------------------

    /// <summary>
    /// Returns a new instance with a set of default code generation settings.
    /// </summary>
    public static EasyEventSymbol Default => new();

    /// <summary>
    /// Returns a new instance with full settings.
    /// </summary>
    public static EasyEventSymbol Full => new();
}

// ========================================================
internal static partial class EasyNameExtensions
{
    /// <summary>
    /// Returns a display string for the given element using default options.
    /// </summary>
    /// <param name="source"></param>
    /// <returns></returns>
    public static string EasyName(
        this IEventSymbol source) => source.EasyName(EasyEventSymbol.Default);

    /// <summary>
    /// Returns a display string for the given element using the given options.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="options"></param>
    /// <returns></returns>
    public static string EasyName(this IEventSymbol source, EasyEventSymbol options)
    {
        throw null;
    }
}