using System.Buffers;
using System.Data;

namespace Yotei.Tools.CoreGenerator;

// ========================================================
internal static class CharExtensions
{
    // bool Equals(value)

    /// <summary>
    /// Determines if this character is the same as the given one.
    /// <br/> This method works by comparing the lower case version of both characters.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="caseSensitive"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool Equals(this char source, char value, bool caseSensitive)
    {
        return caseSensitive
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
    /// /// OPTIMIZE: char.Equals(char, string comparison) allocates two temporary strings.
    public static bool Equals(
        this char source, char value, StringComparison comparison)
        => new CharComparerByComparison(comparison).Equals(source, value);

    readonly struct CharComparerByComparison(StringComparison Comparison) : IEqualityComparer<char>
    {
        public bool Equals(char x, char y)
        {
            var xs = x.ToString();
            var ys = y.ToString();
            return xs.Equals(ys, Comparison);
        }
        public int GetHashCode(char obj) => throw new NotImplementedException();
    }
}