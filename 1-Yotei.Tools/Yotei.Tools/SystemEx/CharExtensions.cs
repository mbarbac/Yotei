namespace Yotei.Tools;

// ========================================================
public static class CharExtensions
{
    extension(char source)
    {
        /// <summary>
        /// Determines if the source character is the same as the given one.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="ignoreCase"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Equals(
            char value, bool ignoreCase)
            => char.CharComparer(ignoreCase).Equals(source, value);

        /// <summary>
        /// Determines if the source character is the same as the given one.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="comparer"></param>
        /// <returns></returns>
        public bool Equals(
            char value, IEqualityComparer<char> comparer)
            => comparer.ThrowWhenNull().Equals(source, value);

        /// <summary>
        /// Determines if the source character is the same as the given one.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="comparer"></param>
        /// <returns></returns>
        public bool Equals(
            char value, IEqualityComparer<string> comparer)
            => char.CharComparer(comparer.ThrowWhenNull()).Equals(source, value);

        /// <summary>
        /// Determines if the source character is the same as the given one.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="comparison"></param>
        /// <returns></returns>
        public bool Equals(
            char value, StringComparison comparison)
            => char.CharComparer(comparison).Equals(source, value);
    }

    // ----------------------------------------------------

    extension(char)
    {
        /// <summary>
        /// Returns a default char comparer.
        /// </summary>
        public static IEqualityComparer<char> CharComparer() => new ComparerDefault();

        /// <summary>
        /// Gets a char comparer that ignores or not the case as requested.
        /// <br/> When ignoring case, this instance works by comparing their low case versions.
        /// </summary>
        /// <param name="ignoreCase"></param>
        /// <returns></returns>
        public static IEqualityComparer<char> CharComparer(bool ignoreCase) => new ComparerByIgnoreCase(ignoreCase);

        /// <summary>
        /// Gets a char comparer that uses the given string comparer instance.
        /// </summary>
        /// <param name="comparer"></param>
        /// <returns></returns>
        public static IEqualityComparer<char> CharComparer(IEqualityComparer<string> comparer) => new ComparerByComparer(comparer);

        /// <summary>
        /// Gets a char comparer that uses the given comparison mode.
        /// </summary>
        /// <param name="comparison"></param>
        /// <returns></returns>
        public static IEqualityComparer<char> CharComparer(StringComparison comparison) => new ComparerByComparison(comparison);
    }

    // ----------------------------------------------------

    /// <summary>
    /// Compares two chars.
    /// </summary>
    readonly struct ComparerDefault() : IEqualityComparer<char>
    {
        public bool Equals(char x, char y) => x == y;
        public int GetHashCode(char obj) => obj.GetHashCode();
    }

    /// <summary>
    /// Compares two chars ignoring or not the case as requested.
    /// </summary>
    /// <param name="ignoreCase"></param>
    readonly struct ComparerByIgnoreCase(bool ignoreCase) : IEqualityComparer<char>
    {
        public bool Equals(char x, char y) => ignoreCase
            ? char.ToLower(x) == char.ToLower(y)
            : x == y;

        public int GetHashCode(char obj) => ignoreCase
            ? char.ToLower(obj).GetHashCode()
            : obj.GetHashCode();
    }

    /// <summary>
    /// Compares to chars using the given string comparer.
    /// </summary>
    /// <param name="comparer"></param>
    readonly struct ComparerByComparer(IEqualityComparer<string> comparer) : IEqualityComparer<char>
    {
        public bool Equals(char x, char y)
        {
            // We need to convert chars to strings to use the given comparer...
            var xs = x.ToString();
            var ys = y.ToString();
            return comparer.Equals(xs, ys);
        }

        public int GetHashCode(char obj) => comparer.GetHashCode(obj.ToString());
    }

    /// <summary>
    /// Compares two chars using the given comparison mode.
    /// </summary>
    /// <param name="comparison"></param>
    readonly struct ComparerByComparison(StringComparison comparison) : IEqualityComparer<char>
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
}