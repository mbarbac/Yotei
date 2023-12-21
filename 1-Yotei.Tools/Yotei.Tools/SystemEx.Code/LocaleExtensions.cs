namespace Yotei.Tools;

// ========================================================
public static class LocaleExtensions
{
    /// <summary>
    /// Compares the two char spans and returns a value indicating their lexical relationship:
    /// Zero- if they are equal, Less than zero- if the left one is less than the right one,
    /// Greater than zero- if the right one is greater than the left one.
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <returns></returns>
    public static int Compare(
        this Locale locale,
        ReadOnlySpan<char> x, ReadOnlySpan<char> y)
    {
        locale.ThrowWhenNull();
        return locale.CultureInfo.CompareInfo.Compare(x, y, locale.CompareOptions);
    }

    /// <summary>
    /// <inheritdoc cref="Locale.HasPrefix(string, string)"/>
    /// </summary>
    /// <param name="locale"></param>
    /// <param name="source"></param>
    /// <param name="prefix"></param>
    /// <returns></returns>
    public static bool HasPrefix(
        this Locale locale,
        ReadOnlySpan<char> source, ReadOnlySpan<char> prefix)
    {
        locale.ThrowWhenNull();
        return locale.CultureInfo.CompareInfo.IsPrefix(source, prefix, locale.CompareOptions);
    }

    /// <summary>
    /// <inheritdoc cref="Locale.HasSuffix(string, string)"/>
    /// </summary>
    /// <param name="locale"></param>
    /// <param name="source"></param>
    /// <param name="suffix"></param>
    /// <returns></returns>
    public static bool HasSuffix(
        this Locale locale,
        ReadOnlySpan<char> source, ReadOnlySpan<char> suffix)
    {
        locale.ThrowWhenNull();
        return locale.CultureInfo.CompareInfo.IsSuffix(source, suffix, locale.CompareOptions);
    }

    /// <summary>
    /// <inheritdoc cref="Locale.IndexOf(string, string)"/>
    /// </summary>
    /// <returns><inheritdoc/></returns>
    public static int IndexOf(
        this Locale locale,
        ReadOnlySpan<char> source, ReadOnlySpan<char> value)
    {
        locale.ThrowWhenNull();
        return locale.CultureInfo.CompareInfo.IndexOf(source, value, locale.CompareOptions);
    }

    /// <summary>
    /// <inheritdoc cref="Locale.IndexOf(string, char)"/>
    /// </summary>
    /// <returns><inheritdoc/></returns>
    public static int IndexOf(
        this Locale locale,
        ReadOnlySpan<char> source, char value)
    {
        locale.ThrowWhenNull();
        return locale.CultureInfo.CompareInfo.IndexOf(source, value.ToString(), locale.CompareOptions);
    }

    /// <summary>
    /// <inheritdoc cref="Locale.LastIndexOf(string, string)"/>
    /// </summary>
    /// <returns><inheritdoc/></returns>
    public static int LastIndexOf(
        this Locale locale,
        ReadOnlySpan<char> source, ReadOnlySpan<char> value)
    {
        locale.ThrowWhenNull();
        return locale.CultureInfo.CompareInfo.LastIndexOf(source, value, locale.CompareOptions);
    }

    /// <summary>
    /// <inheritdoc cref="Locale.LastIndexOf(string, char)"/>
    /// </summary>
    /// <returns><inheritdoc/></returns>
    public static int LastIndexOf(
        this Locale locale,
        ReadOnlySpan<char> source, char value)
    {
        locale.ThrowWhenNull();
        return locale.CultureInfo.CompareInfo.LastIndexOf(source, value.ToString(), locale.CompareOptions);
    }
}