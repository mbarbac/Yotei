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
        bool Equals(StringSpan value, Func<char, char, bool> predicate)
        {
            if (source.Length != value.Length) return false;

            for (int i = 0; i < source.Length; i++)
            {
                var xsource = source[i];
                var xtarget = value[i];
                if (!predicate(xsource, xtarget)) return false;
            }
            return true;
        }

        /// <summary>
        /// Determines if the contents of this instance are the same as the other given one.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="ignoreCase"></param>
        /// <returns></returns>
        public bool Equals(
            StringSpan value, bool ignoreCase)
            => source.Equals(value, (x, y) => x.Equals(y, ignoreCase));

        /// <summary>
        /// Determines if the contents of this instance are the same as the other given one.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="comparer"></param>
        /// <returns></returns>
        public bool Equals(
            StringSpan value, IEqualityComparer<char> comparer)
            => source.Equals(value, (x, y) => comparer.Equals(x, y));

        /// <summary>
        /// Determines if the contents of this instance are the same as the other given one.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="comparer"></param>
        /// <returns></returns>
        public bool Equals(
            StringSpan value, IEqualityComparer<string> comparer)
            => source.Equals(value, (x, y) => x.Equals(y, comparer));

        /// <summary>
        /// Determines if the contents of this instance are the same as the other given one.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="comparison"></param>
        /// <returns></returns>
        public bool Equals(
            StringSpan value, StringComparison comparison)
            => source.Equals(value, (x, y) => x.Equals(y, comparison));

        // ------------------------------------------------

        int IndexOf(StringSpan value, Func<char, char, bool> predicate)
        {
            if (source.Length == 0 || value.Length == 0) return -1;
            if (value.Length > source.Length) return -1;

            var max = source.Length - value.Length;
            for (int i = 0; i <= max; i++)
            {
                var match = true; for (int j = 0; j < value.Length; j++)
                {
                    var s = source[i + j];
                    var v = value[j];
                    if (!predicate(s, v)) { match = false; break; }
                    ;
                }
                if (match) return i;
            }
            return -1;
        }

        /// <summary>
        /// Returns the index of the first ocurrence of the given value.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public int IndexOf(StringSpan value) => source.IndexOf(value, (x, y) => x == y);

        /// <summary>
        /// Returns the index of the first ocurrence of the given value.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="ignoreCase"></param>
        /// <returns></returns>
        public int IndexOf(
            StringSpan value, bool ignoreCase)
            => source.IndexOf(value, (x, y) => x.Equals(y, ignoreCase));

        /// <summary>
        /// Returns the index of the first ocurrence of the given value.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="comparer"></param>
        /// <returns></returns>
        public int IndexOf(
            StringSpan value, IEqualityComparer<char> comparer)
            => source.IndexOf(value, (x, y) => comparer.Equals(x, y));

        /// <summary>
        /// Returns the index of the first ocurrence of the given value.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="comparer"></param>
        /// <returns></returns>
        public int IndexOf(
            StringSpan value, IEqualityComparer<string> comparer)
            => source.IndexOf(value, (x, y) => x.Equals(y, comparer));

        /// <summary>
        /// Returns the index of the first ocurrence of the given value.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="comparison"></param>
        /// <returns></returns>
        public int IndexOf(
            StringSpan value, StringComparison comparison)
            => source.IndexOf(value, (x, y) => x.Equals(y, comparison));

        // ------------------------------------------------

        int LastIndexOf(StringSpan value, Func<char, char, bool> predicate)
        {
            if (source.Length == 0 || value.Length == 0) return -1;
            if (value.Length > source.Length) return -1;

            var max = source.Length - value.Length;
            for (int i = max; i >= 0; i--)
            {
                var match = true; for (int j = 0; j < value.Length; j++)
                {
                    var s = source[i + j];
                    var v = value[j];
                    if (!predicate(s, v)) { match = false; break; }
                    ;
                }
                if (match) return i;
            }
            return -1;
        }

        /// <summary>
        /// Returns the index of the last ocurrence of the given value.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public int LastIndexOf(StringSpan value) => source.LastIndexOf(value, (x, y) => x == y);

        /// <summary>
        /// Returns the index of the last ocurrence of the given value.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="ignoreCase"></param>
        /// <returns></returns>
        public int LastIndexOf(
            StringSpan value, bool ignoreCase)
            => source.LastIndexOf(value, (x, y) => x.Equals(y, ignoreCase));

        /// <summary>
        /// Returns the index of the last ocurrence of the given value.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="comparer"></param>
        /// <returns></returns>
        public int LastIndexOf(
            StringSpan value, IEqualityComparer<char> comparer)
            => source.LastIndexOf(value, (x, y) => comparer.Equals(x, y));

        /// <summary>
        /// Returns the index of the last ocurrence of the given value.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="comparer"></param>
        /// <returns></returns>
        public int LastIndexOf(
            StringSpan value, IEqualityComparer<string> comparer)
            => source.LastIndexOf(value, (x, y) => x.Equals(y, comparer));

        /// <summary>
        /// Returns the index of the last ocurrence of the given value.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="comparison"></param>
        /// <returns></returns>
        public int LastIndexOf(
            StringSpan value, StringComparison comparison)
            => source.LastIndexOf(value, (x, y) => x.Equals(y, comparison));

        // ------------------------------------------------

        List<int> IndexesOf(StringSpan value, Func<char, char, bool> predicate)
        {
            if (source.Length == 0 || value.Length == 0) return [];
            if (value.Length > source.Length) return [];

            List<int> nums = [];
            var max = source.Length - value.Length;
            for (int i = 0; i <= max; i++)
            {
                var match = true; for (int j = 0; j < value.Length; j++)
                {
                    var s = source[i + j];
                    var v = value[j];
                    if (!predicate(s, v)) { match = false; break; }
                    ;
                }
                if (match) nums.Add(i);
            }
            return nums;
        }

        /// <summary>
        /// Returns the indexes of the ocurrences of the given value.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public List<int> IndexesOf(StringSpan value) => source.IndexesOf(value, (x, y) => x == y);

        /// <summary>
        /// Returns the indexes of the ocurrences of the given value.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="ignoreCase"></param>
        /// <returns></returns>
        public List<int> IndexesOf(
            StringSpan value, bool ignoreCase)
            => source.IndexesOf(value, (x, y) => x.Equals(y, ignoreCase));

        /// <summary>
        /// Returns the indexes of the ocurrences of the given value.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="comparer"></param>
        /// <returns></returns>
        public List<int> IndexesOf(
            StringSpan value, IEqualityComparer<char> comparer)
            => source.IndexesOf(value, (x, y) => comparer.Equals(x, y));

        /// <summary>
        /// Returns the indexes of the ocurrences of the given value.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="comparer"></param>
        /// <returns></returns>
        public List<int> IndexesOf(
            StringSpan value, IEqualityComparer<string> comparer)
            => source.IndexesOf(value, (x, y) => x.Equals(y, comparer));

        /// <summary>
        /// Returns the indexes of the ocurrences of the given value.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="comparison"></param>
        /// <returns></returns>
        public List<int> IndexesOf(
            StringSpan value, StringComparison comparison)
            => source.IndexesOf(value, (x, y) => x.Equals(y, comparison));
    }
}