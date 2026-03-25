using StringSpan = System.ReadOnlySpan<char>;

#if YOTEI_TOOLS_COREGENERATOR
namespace Yotei.Tools.CoreGenerator;
#else
namespace Yotei.Tools;
#endif

// ========================================================
#if YOTEI_TOOLS_COREGENERATOR
internal
#else
public
#endif
static partial class StringSpanExtensions
{
    static bool StartsWith(StringSpan source, char value, Func<char, char, bool> predicate)
    {
        if (source.Length == 0) return false;
        return predicate(source[0], value);
    }

#if YOTEI_TOOLS_COREGENERATOR

    /// <summary>
    /// Determines if this source starts with the given value, or not.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    public static bool StartsWith(
        this StringSpan source, char value)
        => StartsWith(source, value, static (x, y) => x == y);

    /// <summary>
    /// Determines if this source starts with the given value, or not.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="comparer"></param>
    /// <returns></returns>
    public static bool StartsWith(
        this StringSpan source, char value, IEqualityComparer<char> comparer)
        => StartsWith(source, value, (x, y) => x.Equals(y, comparer));

#endif

    /// <summary>
    /// Determines if this source starts with the given value, or not.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="ignoreCase"></param>
    /// <returns></returns>
    public static bool StartsWith(
        this StringSpan source, char value, bool ignoreCase)
        => StartsWith(source, value, (x, y) => x.Equals(y, ignoreCase));

    /// <summary>
    /// Determines if this source starts with the given value, or not.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="comparer"></param>
    /// <returns></returns>
    public static bool StartsWith(
        this StringSpan source, char value, IEqualityComparer<string> comparer)
        => StartsWith(source, value, (x, y) => x.Equals(y, comparer));

    /// <summary>
    /// Determines if this source starts with the given value, or not.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="comparison"></param>
    /// <returns></returns>
    public static bool StartsWith(
        this StringSpan source, char value, StringComparison comparison)
        => StartsWith(source, value, (x, y) => x.Equals(y, comparison));

    // ----------------------------------------------------

    static bool EndsWith(StringSpan source, char value, Func<char, char, bool> predicate)
    {
        if (source.Length == 0) return false;
        return predicate(source[^1], value);
    }

#if YOTEI_TOOLS_COREGENERATOR

    /// <summary>
    /// Determines if this source starts with the given value, or not.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    public static bool EndsWith(
        this StringSpan source, char value)
        => EndsWith(source, value, static (x, y) => x == y);

    /// <summary>
    /// Determines if this source starts with the given value, or not.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="comparer"></param>
    /// <returns></returns>
    public static bool EndsWith(
        this StringSpan source, char value, IEqualityComparer<char> comparer)
        => EndsWith(source, value, (x, y) => x.Equals(y, comparer));

#endif

    /// <summary>
    /// Determines if this source starts with the given value, or not.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="ignoreCase"></param>
    /// <returns></returns>
    public static bool EndsWith(
        this StringSpan source, char value, bool ignoreCase)
        => EndsWith(source, value, (x, y) => x.Equals(y, ignoreCase));

    /// <summary>
    /// Determines if this source starts with the given value, or not.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="comparer"></param>
    /// <returns></returns>
    public static bool EndsWith(
        this StringSpan source, char value, IEqualityComparer<string> comparer)
        => EndsWith(source, value, (x, y) => x.Equals(y, comparer));

    /// <summary>
    /// Determines if this source starts with the given value, or not.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="comparison"></param>
    /// <returns></returns>
    public static bool EndsWith(
        this StringSpan source, char value, StringComparison comparison)
        => EndsWith(source, value, (x, y) => x.Equals(y, comparison));
}