namespace Yotei.Tools;

// ========================================================
public static class CharExtensions
{
    /// <summary>
    /// Determines if the two given chars are equal or not.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="target"></param>
    /// <param name="caseSensitive"></param>
    /// <returns></returns>
    public static bool Equals(this char source, char target, bool caseSensitive)
    {
        return caseSensitive
            ? source == target
            : char.ToUpper(source) == char.ToUpper(target);
    }

    /// <summary>
    /// Determines if the two given chars are equal or not.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="target"></param>
    /// <param name="comparison"></param>
    /// <returns></returns>
    public static bool Equals(this char source, char target, StringComparison comparison)
    {
        var xs = new ReadOnlySpan<char>(ref source);
        var xt = new ReadOnlySpan<char>(ref target);

        return xs.Equals(xt, comparison);
    }

    /// <summary>
    /// Determines if the two given chars are equal or not.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="target"></param>
    /// <param name="comparer"></param>
    /// <returns></returns>
    public static bool Equals(this char source, char target, IComparer<string> comparer)
    {
        return comparer.Compare(source.ToString(), target.ToString()) == 0;
    }

    // ----------------------------------------------------

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
}