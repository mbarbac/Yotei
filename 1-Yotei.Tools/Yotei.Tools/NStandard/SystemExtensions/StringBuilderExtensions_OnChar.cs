using StringSpan = System.ReadOnlySpan<char>;

#if NETSTANDARD2_0
#pragma warning disable IDE0079
#pragma warning disable CS0436
#endif

namespace Yotei.Tools;

// ========================================================
#if NETSTANDARD2_0
internal
#else
public
#endif
static partial class StringBuilderExtensions
{
    extension(StringBuilder source)
    {
        int IndexOf(char value, Func<char, char, bool> predicate)
        {
            for (int i = 0; i < source.Length; i++)
                if (predicate(source[i], value)) return i;

            return -1;
        }

        /// <summary>
        /// Returns the index of the first ocurrence of the given value.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public int IndexOf(char value) => source.IndexOf(value, (x, y) => x == y);

        /// <summary>
        /// Returns the index of the first ocurrence of the given value.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="ignoreCase"></param>
        /// <returns></returns>
        public int IndexOf(
            char value, bool ignoreCase)
            => source.IndexOf(value, (x, y) => x.Equals(y, ignoreCase));

        /// <summary>
        /// Returns the index of the first ocurrence of the given value.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="comparer"></param>
        /// <returns></returns>
        public int IndexOf(
            char value, IEqualityComparer<char> comparer)
            => source.IndexOf(value, (x, y) => comparer.Equals(x, y));

        /// <summary>
        /// Returns the index of the first ocurrence of the given value.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="comparer"></param>
        /// <returns></returns>
        public int IndexOf(
            char value, IEqualityComparer<string> comparer)
            => source.IndexOf(value, (x, y) => x.Equals(y, comparer));

        /// <summary>
        /// Returns the index of the first ocurrence of the given value.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="comparison"></param>
        /// <returns></returns>
        public int IndexOf(
            char value, StringComparison comparison)
            => source.IndexOf(value, (x, y) => x.Equals(y, comparison));

        // ------------------------------------------------

        int LastIndexOf(char value, Func<char, char, bool> predicate)
        {
            for (int i = source.Length - 1; i >= 0; i--)
                if (predicate(source[i], value)) return i;

            return -1;
        }

        /// <summary>
        /// Returns the index of the last ocurrence of the given value.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public int LastIndexOf(char value) => source.LastIndexOf(value, (x, y) => x == y);

        /// <summary>
        /// Returns the index of the last ocurrence of the given value.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="ignoreCase"></param>
        /// <returns></returns>
        public int LastIndexOf(
            char value, bool ignoreCase)
            => source.LastIndexOf(value, (x, y) => x.Equals(y, ignoreCase));

        /// <summary>
        /// Returns the index of the last ocurrence of the given value.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="comparer"></param>
        /// <returns></returns>
        public int LastIndexOf(
            char value, IEqualityComparer<char> comparer)
            => source.LastIndexOf(value, (x, y) => comparer.Equals(x, y));

        /// <summary>
        /// Returns the index of the last ocurrence of the given value.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="comparer"></param>
        /// <returns></returns>
        public int LastIndexOf(
            char value, IEqualityComparer<string> comparer)
            => source.LastIndexOf(value, (x, y) => x.Equals(y, comparer));

        /// <summary>
        /// Returns the index of the last ocurrence of the given value.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="comparison"></param>
        /// <returns></returns>
        public int LastIndexOf(
            char value, StringComparison comparison)
            => source.LastIndexOf(value, (x, y) => x.Equals(y, comparison));

        // ------------------------------------------------

        List<int> IndexesOf(char value, Func<char, char, bool> predicate)
        {
            List<int> nums = [];

            for (int i = 0; i < source.Length; i++)
                if (predicate(source[i], value)) nums.Add(i);

            return nums;
        }

        /// <summary>
        /// Returns the indexes of the ocurrences of the given value.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public List<int> IndexesOf(char value) => source.IndexesOf(value, (x, y) => x == y);

        /// <summary>
        /// Returns the indexes of the ocurrences of the given value.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="ignoreCase"></param>
        /// <returns></returns>
        public List<int> IndexesOf(
            char value, bool ignoreCase)
            => source.IndexesOf(value, (x, y) => x.Equals(y, ignoreCase));

        /// <summary>
        /// Returns the indexes of the ocurrences of the given value.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="comparer"></param>
        /// <returns></returns>
        public List<int> IndexesOf(
            char value, IEqualityComparer<char> comparer)
            => source.IndexesOf(value, (x, y) => comparer.Equals(x, y));

        /// <summary>
        /// Returns the indexes of the ocurrences of the given value.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="comparer"></param>
        /// <returns></returns>
        public List<int> IndexesOf(
            char value, IEqualityComparer<string> comparer)
            => source.IndexesOf(value, (x, y) => x.Equals(y, comparer));

        /// <summary>
        /// Returns the indexes of the ocurrences of the given value.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="comparison"></param>
        /// <returns></returns>
        public List<int> IndexesOf(
            char value, StringComparison comparison)
            => source.IndexesOf(value, (x, y) => x.Equals(y, comparison));
    }
}