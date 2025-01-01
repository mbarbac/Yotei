namespace Yotei.Tools;

// ========================================================
public static class CharExtensions
{
    /// <summary>
    /// Returns a new char where its diacritics, if any, have been removed.
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public static char RemoveDiacritics(this char value)
    {
        string temp = value.ToString().RemoveDiacritics();
        return temp.Length > 0 ? temp[0] : (char)0;
    }

    // ----------------------------------------------------

    /// <summary>
    /// Determines if the two given chars are equal or not.
    /// </summary>
    /// <param name="value"></param>
    /// <param name="other"></param>
    /// <param name="caseSensitive"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool Equals(this char value, char other, bool caseSensitive)
    {
        return caseSensitive
            ? value == other
            : char.ToUpper(value) == char.ToUpper(other);
    }

    /// <summary>
    /// Determines if the two given chars are equal or not.
    /// </summary>
    /// <param name="value"></param>
    /// <param name="other"></param>
    /// <param name="comparer"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool Equals(this char value, char other, IEqualityComparer<char> comparer)
    {
        return comparer.ThrowWhenNull().Equals(value, other);
    }

    // ----------------------------------------------------

    /// <summary>
    /// Determines if the two given chars are equal or not.
    /// </summary>
    /// <param name="value"></param>
    /// <param name="other"></param>
    /// <param name="comparison"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool Equals(this char value, char other, StringComparison comparison)
    {
        var svalue = new ReadOnlySpan<char>([value]);
        var sother = new ReadOnlySpan<char>([other]);

        return svalue.Equals(sother, comparison);
    }

    /// <summary>
    /// Determines if the two given chars are equal or not.
    /// </summary>
    /// <param name="value"></param>
    /// <param name="other"></param>
    /// <param name="comparer"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool Equals(this char value, char other, IEqualityComparer<string> comparer)
    {
        comparer.ThrowWhenNull();

        var svalue = new string([value]);
        var sother = new string([other]);

        return comparer.Equals(svalue, sother);
    }
}