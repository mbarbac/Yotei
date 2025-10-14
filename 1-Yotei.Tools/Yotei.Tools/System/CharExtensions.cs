namespace Yotei.Tools;

// ========================================================
public static class CharExtensions
{
    /// <summary>
    /// Determines if this character is the same as the given one.
    /// <br/> This method works by comparing the lower case version of both characters.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="caseSensitive"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool Equals(this char source, char value, bool ignoreCase)
    {
        return ignoreCase
            ? char.ToLower(source) == char.ToLower(value)
            : source == value;
    }

    /// <summary>
    /// Determines if this character is the same as the given one.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="comparer"></param>
    /// <returns></returns>
    public static bool Equals(this char source, char value, IEqualityComparer<char> comparer)
    {
        return comparer.ThrowWhenNull().Equals(source, value);
    }

    /// <summary>
    /// Determines if this character is the same as the given one.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="comparer"></param>
    /// <returns></returns>
    /// OPTIMIZE: char.Equals(char, string comparer) allocates two temporary strings.
    /// Problem is that StringSpan has not a CompareTo(target, StringComparer)
    public static bool Equals(
        this char source, char value, IEqualityComparer<string> comparer)
        => new CharComparerByStringComparer(comparer.ThrowWhenNull()).Equals(source, value);

    readonly struct CharComparerByStringComparer(IEqualityComparer<string> Comparer) : IEqualityComparer<char>
    {
        public bool Equals(char x, char y)
        {
            var xs = x.ToString();
            var ys = y.ToString();
            return Comparer.Equals(xs, ys);
        }
        public int GetHashCode(char obj) => throw new NotImplementedException();
    }

    /// <summary>
    /// Determines if this character is the same as the given one.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="comparison"></param>
    /// <returns></returns>
    public static bool Equals(
        this char source, char value, StringComparison comparison)
        => new CharComparerByComparison(comparison).Equals(source, value);

    readonly struct CharComparerByComparison(StringComparison Comparison) : IEqualityComparer<char>
    {
        public bool Equals(char x, char y)
        {
            Span<char> xs = stackalloc char[1]; xs[0] = x;
            Span<char> ys = stackalloc char[1]; ys[0] = y;
            return xs.CompareTo(ys, Comparison) == 0;
        }
        public int GetHashCode(char obj) => throw new NotImplementedException();
    }
}