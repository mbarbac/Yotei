namespace Yotei.Tools;

// ========================================================
/// <summary>
/// Used to provide a char comparer that uses the given comparison mode.
/// </summary>
readonly struct ByIgnoreCase(bool ignoreCase) : IEqualityComparer<char>
{
    public bool Equals(char x, char y) => ignoreCase
        ? char.ToLower(x) == char.ToLower(y)
        : x == y;

    public int GetHashCode(char obj) => ignoreCase
        ? char.ToLower(obj).GetHashCode()
        : obj.GetHashCode();
}

// ========================================================
/// <summary>
/// Used to provide a char comparer that uses the rules of the given string comparer.
/// </summary>
readonly struct ByStringComparer(IEqualityComparer<string> comparer) : IEqualityComparer<char>
{
    // We cannot avoid converting the chars to strings for the comparer.
    public bool Equals(char x, char y)
    {
        var xs = x.ToString();
        var ys = y.ToString();
        return comparer.Equals(xs, ys);
    }
    public int GetHashCode(char obj) => comparer.GetHashCode(obj.ToString());
}

// ========================================================
/// <summary>
/// Used to provide a char comparer that uses the rules of the given string comparison.
/// </summary>
readonly struct ByStringComparison(StringComparison comparison) : IEqualityComparer<char>
{
    [SuppressMessage("", "IDE0302")]
    public bool Equals(char x, char y)
    {
        Span<char> xs = stackalloc char[] { x };
        Span<char> ys = stackalloc char[] { y };
        return xs.CompareTo(ys, comparison) == 0;
    }

    public int GetHashCode(char obj) => comparison is
        StringComparison.CurrentCultureIgnoreCase or
        StringComparison.InvariantCultureIgnoreCase or
        StringComparison.OrdinalIgnoreCase
        ? char.ToLower(obj).GetHashCode()
        : obj.GetHashCode();
}

// ========================================================
public static class CharExtensions
{
    extension(char source)
    {
        /// <summary>
        /// Determines if this value is the same as the other given one, ignoring or distinction
        /// between upper and lowercase letters, as requested.
        /// </summary>
        /// <param name="other"></param>
        /// <param name="ignoreCase"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Equals(
            char other, bool ignoreCase) => char.CharComparer(ignoreCase).Equals(source, other);

        /// <summary>
        /// Determines if this value is the same as the other given one using the rules of the
        /// given comparer.
        /// </summary>
        /// <param name="other"></param>
        /// <param name="comparer"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Equals(
            char other, IEqualityComparer<char> comparer)
            => comparer.ThrowWhenNull().Equals(source, other);

        /// <summary>
        /// Determines if this value is the same as the other given one using the rules of the
        /// given string comparer.
        /// </summary>
        /// <param name="other"></param>
        /// <param name="comparer"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Equals(
            char other, IEqualityComparer<string> comparer)
            => char.CharComparer(comparer.ThrowWhenNull()).Equals(source, other);

        /// <summary>
        /// Determines if this value is the same as the other given one using the rules of the
        /// given string comparison.
        /// </summary>
        /// <param name="other"></param>
        /// <param name="comparison"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Equals(
            char other, StringComparison comparison)
            => char.CharComparer(comparison).Equals(source, other);
    }

    // ----------------------------------------------------

    extension(char)
    {
        /// <summary>
        /// Returns a default char comparer.
        /// </summary>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IEqualityComparer<char> CharComparer() => EqualityComparer<char>.Default;

        /// <summary>
        /// Returns a char comparer that uses the given comparison mode.
        /// </summary>
        /// <param name="ignoreCase"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IEqualityComparer<char> CharComparer(bool ignoreCase)
            => new ByIgnoreCase(ignoreCase);

        /// <summary>
        /// Returns a char comparer that uses the rules of the given string comparer.
        /// </summary>
        /// <param name="comparer"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IEqualityComparer<char> CharComparer(IEqualityComparer<string> comparer)
            => new ByStringComparer(comparer);

        /// <summary>
        /// Returns a char comparer that uses the rules of the given string comparison.
        /// </summary>
        /// <param name="comparison"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IEqualityComparer<char> CharComparer(StringComparison comparison)
            => new ByStringComparison(comparison);
    }
}