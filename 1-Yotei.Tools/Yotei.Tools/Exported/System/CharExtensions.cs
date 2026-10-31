#if YOTEI_TOOLS_GENERATORS
namespace Yotei.Tools.Generators;
#else
namespace Yotei.Tools;
#endif

// ========================================================
/// <summary>
/// Represents the ocurrence of a duplicated element when it was not supposed to happen.
/// </summary>
#if YOTEI_TOOLS_GENERATORS
internal
#else
public
#endif
static class CharExtensions
{
    extension(char source)
    {
        /// <summary>
        /// Compares the source and target chars using the given ignore case mode.
        /// </summary>
        /// <param name="target"></param>
        /// <param name="ignoreCase"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Equals(char target, bool ignoreCase) => CharComparer(ignoreCase).Equals(source, target);

        /// <summary>
        /// Compares the source and target chars using the rules of the given string comparer.
        /// </summary>
        /// <param name="target"></param>
        /// <param name="comparer"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Equals(char target, IEqualityComparer<string> comparer) => CharComparer(comparer).Equals(source, target);

        /// <summary>
        /// Compares the source and target chars using the rules of the given string comparison.
        /// </summary>
        /// <param name="target"></param>
        /// <param name="comparison"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Equals(char target, StringComparison comparison) => CharComparer(comparison).Equals(source, target);
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
        /// Returns a char comparer based upon the given ignore case mode.
        /// </summary>
        /// <param name="ignoreCase"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IEqualityComparer<char> CharComparer(bool ignoreCase)
        {
            return ignoreCase ? ByIgnoreCaseTrue : ByIgnoreCaseFalse;
        }

        /// <summary>
        ///  Returns a char comparer that uses the rules of the given string comparer.
        /// </summary>
        /// <param name="comparer"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IEqualityComparer<char> CharComparer(IEqualityComparer<string> comparer)
        {
            if (ReferenceEquals(comparer, StringComparer.CurrentCulture)) return ByStringComparerCurrentCulture;
            else if (ReferenceEquals(comparer, StringComparer.CurrentCultureIgnoreCase)) return ByStringComparerCurrentCultureIgnoreCase;
            else if (ReferenceEquals(comparer, StringComparer.InvariantCulture)) return ByStringComparerInvariantCulture;
            else if (ReferenceEquals(comparer, StringComparer.InvariantCultureIgnoreCase)) return ByStringComparerInvariantCultureIgnoreCase;
            else if (ReferenceEquals(comparer, StringComparer.Ordinal)) return ByStringComparerOrdinal;
            else if (ReferenceEquals(comparer, StringComparer.OrdinalIgnoreCase)) return ByStringComparerOrdinalIgnoreCase;
            return new ByStringComparer(comparer);
        }

        /// <summary>
        ///  Returns a char comparer that uses the rules of the given comparison mode.
        /// </summary>
        /// <param name="comparison"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IEqualityComparer<char> CharComparer(StringComparison comparison)
        {
            return comparison switch
            {
                StringComparison.CurrentCulture => ByStringComparisonCurrentCulture,
                StringComparison.CurrentCultureIgnoreCase => ByStringComparisonCurrentCultureIgnoreCase,
                StringComparison.InvariantCulture => ByStringComparisonInvariantCulture,
                StringComparison.InvariantCultureIgnoreCase => ByStringComparisonInvariantCultureIgnoreCase,
                StringComparison.Ordinal => ByStringComparisonOrdinal,
                StringComparison.OrdinalIgnoreCase => ByStringComparisonOrdinalIgnoreCase,
                _ => new ByStringComparison(comparison)
            };
        }
    }

    // ----------------------------------------------------

    /// <summary>
    /// Compares chars using the given ignore case mode.
    /// </summary>
    readonly struct ByIgnoreCase(bool ignoreCase) : IEqualityComparer<char>
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Equals(char x, char y) => ignoreCase
            ? char.ToLower(x) == char.ToLower(y)
            : x == y;

        public int GetHashCode(char obj) => ignoreCase
            ? char.ToLower(obj).GetHashCode()
            : obj.GetHashCode();
    }
    static readonly IEqualityComparer<char> ByIgnoreCaseTrue = new ByIgnoreCase(true);
    static readonly IEqualityComparer<char> ByIgnoreCaseFalse = new ByIgnoreCase(false);

    /// <summary>
    /// Compares chars using the rules of the given string comparer.
    /// </summary>
    readonly struct ByStringComparer(IEqualityComparer<string> comparer) : IEqualityComparer<char>
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [SuppressMessage("", "IDE0302")]
        public bool Equals(char x, char y)
        {
            //var xs = x.ToString();
            //var ys = y.ToString();
            //return comparer.Equals(xs, ys);

            Span<char> xs = stackalloc char[1]; xs[0] = x;
            Span<char> ys = stackalloc char[1]; ys[0] = y;
            return comparer.Equals(new string(xs), new string(ys));
        }
        public int GetHashCode(char obj) => comparer.GetHashCode(obj.ToString());
    }
    static readonly IEqualityComparer<char> ByStringComparerCurrentCulture = new ByStringComparer(StringComparer.CurrentCulture);
    static readonly IEqualityComparer<char> ByStringComparerCurrentCultureIgnoreCase = new ByStringComparer(StringComparer.CurrentCultureIgnoreCase);
    static readonly IEqualityComparer<char> ByStringComparerInvariantCulture = new ByStringComparer(StringComparer.InvariantCulture);
    static readonly IEqualityComparer<char> ByStringComparerInvariantCultureIgnoreCase = new ByStringComparer(StringComparer.InvariantCultureIgnoreCase);
    static readonly IEqualityComparer<char> ByStringComparerOrdinal = new ByStringComparer(StringComparer.Ordinal);
    static readonly IEqualityComparer<char> ByStringComparerOrdinalIgnoreCase = new ByStringComparer(StringComparer.OrdinalIgnoreCase);

    /// <summary>
    /// Compares chars using the rules of the given string comparison mode.
    /// </summary>
    /// <param name="comparison"></param>
    readonly struct ByStringComparison(StringComparison comparison) : IEqualityComparer<char>
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [SuppressMessage("", "IDE0079")]
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
    static readonly IEqualityComparer<char> ByStringComparisonCurrentCulture = new ByStringComparison(StringComparison.CurrentCulture);
    static readonly IEqualityComparer<char> ByStringComparisonCurrentCultureIgnoreCase = new ByStringComparison(StringComparison.CurrentCultureIgnoreCase);
    static readonly IEqualityComparer<char> ByStringComparisonInvariantCulture = new ByStringComparison(StringComparison.InvariantCulture);
    static readonly IEqualityComparer<char> ByStringComparisonInvariantCultureIgnoreCase = new ByStringComparison(StringComparison.InvariantCultureIgnoreCase);
    static readonly IEqualityComparer<char> ByStringComparisonOrdinal = new ByStringComparison(StringComparison.Ordinal);
    static readonly IEqualityComparer<char> ByStringComparisonOrdinalIgnoreCase = new ByStringComparison(StringComparison.OrdinalIgnoreCase);
}