namespace Yotei.Tools.BaseGenerator;

// ========================================================
internal static class CharExtensions
{
    /// <summary>
    /// Determines if the value of this char is the same as the value of the other given one.
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
    /// Determines if the value of this char is the same as the value of the other given one.
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

    static readonly object Locker = new();
    static readonly char[] ValueArray = new char[1];
    static readonly char[] OtherArray = new char[1];

    /// <summary>
    /// Determines if the value of this char is the same as the value of the other given one.
    /// </summary>
    /// <param name="value"></param>
    /// <param name="other"></param>
    /// <param name="comparison"></param>
    /// <returns></returns>
    public static bool Equals(this char value, char other, StringComparison comparison)
    {
        lock (Locker)
        {
            ValueArray[0] = value; var svalue = new ReadOnlySpan<char>(ValueArray);
            OtherArray[0] = other; var sother = new ReadOnlySpan<char>(OtherArray);

            return svalue.Equals(sother, comparison);
        }
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