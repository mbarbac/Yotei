
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
    /// Determines if the value of this char is the same as the value of the other given one.
    /// </summary>
    /// <param name="value"></param>
    /// <param name="other"></param>
    /// <param name="comparison"></param>
    /// <returns></returns>
    public static bool Equals(this char value, char other, StringComparison comparison)
    {
        var values = new ReadOnlySpan<char>([value]);
        var others = new ReadOnlySpan<char>([other]);

        return values.Equals(others, comparison);
    }

    /// <summary>
    /// Determines if the value of this char is the same as the value of the other given one.
    /// <br/> This method allocates two temporary string to hold and compare the characters, so
    /// it shall be used as a last resort.
    /// </summary>
    /// <param name="value"></param>
    /// <param name="other"></param>
    /// <param name="comparer"></param>
    /// <returns></returns>
    public static bool Equals(this char value, char other, IEqualityComparer<string> comparer)
    {
        comparer.ThrowWhenNull();

        var svalue = new string([value]);
        var sother = new string([other]);

        return comparer.Equals(svalue, sother);
    }
}