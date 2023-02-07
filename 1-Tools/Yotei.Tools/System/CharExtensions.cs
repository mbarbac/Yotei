namespace Yotei.Tools;

// ========================================================
public static class CharExtensions
{
    /// <summary>
    /// Determines if the two given chars are equal, or not.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="target"></param>
    /// <param name="caseSensitive"></param>
    /// <returns></returns>
    public static bool Equals(this char source, char target, bool caseSensitive)
    {
        return caseSensitive
           ? char.ToUpper(source) == char.ToUpper(target)
           : source == target;
    }

    /// <summary>
    /// Returns a new char where its diacritics have been removed.
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public static char RemoveDiacritics(this char value)
    {
        string temp = value.ToString().RemoveDiacritics();
        return temp.Length > 0 ? temp[0] : (char)0;
    }
}