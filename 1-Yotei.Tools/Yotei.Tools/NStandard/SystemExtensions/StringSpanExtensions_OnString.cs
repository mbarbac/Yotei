using System.Net.Http;
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
static partial class StringSpanExtensions
{
    extension(StringSpan source)
    {
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
        /// Returns the index of the first ocurrence of the given value, or -1 if it cannot be
        /// found.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="ignoreCase"></param>
        /// <returns></returns>
        public int IndexOf(
            StringSpan value, bool ignoreCase)
            => source.IndexOf(value, (x, y) => x.Equals(y, ignoreCase));

        /// <summary>
        /// Returns the index of the first ocurrence of the given value, or -1 if it cannot be
        /// found.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="comparer"></param>
        /// <returns></returns>
        public int IndexOf(
            StringSpan value, IEqualityComparer<char> comparer)
            => source.IndexOf(value, (x, y) => comparer.Equals(x, y));

        /// <summary>
        /// Returns the index of the first ocurrence of the given value, or -1 if it cannot be
        /// found.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="comparer"></param>
        /// <returns></returns>
        public int IndexOf(
            StringSpan value, IEqualityComparer<string> comparer)
            => source.IndexOf(value, (x, y) => x.Equals(y, comparer));

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
        /// Returns the index of the first ocurrence of the given value, or -1 if it cannot be
        /// found.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="ignoreCase"></param>
        /// <returns></returns>
        public int LastIndexOf(
            StringSpan value, bool ignoreCase)
            => source.LastIndexOf(value, (x, y) => x.Equals(y, ignoreCase));

        /// <summary>
        /// Returns the index of the first ocurrence of the given value, or -1 if it cannot be
        /// found.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="comparer"></param>
        /// <returns></returns>
        public int LastIndexOf(
            StringSpan value, IEqualityComparer<char> comparer)
            => source.LastIndexOf(value, (x, y) => comparer.Equals(x, y));

        /// <summary>
        /// Returns the index of the first ocurrence of the given value, or -1 if it cannot be
        /// found.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="comparer"></param>
        /// <returns></returns>
        public int LastIndexOf(
            StringSpan value, IEqualityComparer<string> comparer)
            => source.LastIndexOf(value, (x, y) => x.Equals(y, comparer));

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
        /// Returns the indexes of all ocurrences of the given value.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public List<int> IndexesOf(StringSpan value) => source.IndexesOf(value, (x, y) => x == y);

        /// <summary>
        /// Returns the indexes of all ocurrences of the given value.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="ignoreCase"></param>
        /// <returns></returns>
        public List<int> IndexesOf(
            StringSpan value, bool ignoreCase)
            => source.IndexesOf(value, (x, y) => x.Equals(y, ignoreCase));

        /// <summary>
        /// Returns the indexes of all ocurrences of the given value.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="comparer"></param>
        /// <returns></returns>
        public List<int> IndexesOf(
            StringSpan value, IEqualityComparer<char> comparer)
            => source.IndexesOf(value, (x, y) => comparer.Equals(x, y));

        /// <summary>
        /// Returns the indexes of all ocurrences of the given value.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="comparer"></param>
        /// <returns></returns>
        public List<int> IndexesOf(
            StringSpan value, IEqualityComparer<string> comparer)
            => source.IndexesOf(value, (x, y) => x.Equals(y, comparer));

        /// <summary>
        /// Returns the indexes of all ocurrences of the given value.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="comparison"></param>
        /// <returns></returns>
        public List<int> IndexesOf(
            StringSpan value, StringComparison comparison)
            => source.IndexesOf(value, (x, y) => x.Equals(y, comparison));

        // ------------------------------------------------

        /// <summary>
        /// Determines is the span contains the given value.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool Contains(StringSpan value) => source.IndexOf(value) >= 0;

        /// <summary>
        /// Determines is the span contains the given value.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="ignoreCase"></param>
        /// <returns></returns>
        public bool Contains(
            StringSpan value, bool ignoreCase) => source.IndexOf(value, ignoreCase) >= 0;

        /// <summary>
        /// Determines is the span contains the given value.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="comparer"></param>
        /// <returns></returns>
        public bool Contains(
            StringSpan value, IEqualityComparer<char> comparer)
            => source.IndexOf(value, comparer) >= 0;

        /// <summary>
        /// Determines is the span contains the given value.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="comparer"></param>
        /// <returns></returns>
        public bool Contains(
            StringSpan value, IEqualityComparer<string> comparer)
            => source.IndexOf(value, comparer) >= 0;

        // ------------------------------------------------

        /// <summary>
        /// Determines if the span starts with the given value.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="ignoreCase"></param>
        /// <returns></returns>
        public bool StartsWith(
            StringSpan value, bool ignoreCase)
            => source.IndexOf(value, ignoreCase) == 0;

        /// <summary>
        /// Determines if the span starts with the given value.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="comparer"></param>
        /// <returns></returns>
        public bool StartsWith(
            StringSpan value, IEqualityComparer<char> comparer)
            => source.IndexOf(value, comparer) == 0;

        /// <summary>
        /// Determines if the span starts with the given value.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="comparer"></param>
        /// <returns></returns>
        public bool StartsWith(
            StringSpan value, IEqualityComparer<string> comparer)
            => source.IndexOf(value, comparer) == 0;

        // ------------------------------------------------

        /// <summary>
        /// Determines if the span ends with the given value.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="ignoreCase"></param>
        /// <returns></returns>
        public bool EndsWith(
            StringSpan value, bool ignoreCase)
            => source.Length > 0
            && value.Length > 0
            && source.IndexOf(value, ignoreCase) == (source.Length - value.Length);

        /// <summary>
        /// Determines if the span ends with the given value.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="comparer"></param>
        /// <returns></returns>
        public bool EndsWith(
            StringSpan value, IEqualityComparer<char> comparer)
            => source.Length > 0
            && value.Length > 0
            && source.IndexOf(value, comparer) == (source.Length - value.Length);

        /// <summary>
        /// Determines if the span ends with the given value.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="comparer"></param>
        /// <returns></returns>
        public bool EndsWith(
            StringSpan value, IEqualityComparer<string> comparer)
            => source.Length > 0
            && value.Length > 0
            && source.IndexOf(value, comparer) == (source.Length - value.Length);

        // ------------------------------------------------

        /// <summary>
        /// Removes from the span all leading ocurrences of the given values.
        /// </summary>
        /// <param name="values"></param>
        /// <param name="ignoreCase"></param>
        /// <returns></returns>
        public StringSpan TrimStart(IEnumerable<char> values, bool ignoreCase)
            => source.TrimStart(values, char.CharComparer(ignoreCase));

        /// <summary>
        /// Removes from the span all leading ocurrences of the given values.
        /// </summary>
        /// <param name="values></param>
        /// <param name="comparer"></param>
        /// <returns></returns>
        public StringSpan TrimStart(IEnumerable<char> values, IEqualityComparer<string> comparer)
            => source.TrimStart(values, char.CharComparer(comparer));

        /// <summary>
        /// Removes from the span all leading ocurrences of the given value.s
        /// </summary>
        /// <param name="values"></param>
        /// <param name="comparison"></param>
        /// <returns></returns>
        public StringSpan TrimStart(IEnumerable<char> values, StringComparison comparison)
            => source.TrimStart(values, char.CharComparer(comparison));

        // ------------------------------------------------

        /// <summary>
        /// Removes from the span all trailing ocurrences of the given values.
        /// </summary>
        /// <param name="values"></param>
        /// <param name="ignoreCase"></param>
        /// <returns></returns>
        public StringSpan TrimEnd(IEnumerable<char> values, bool ignoreCase)
            => source.TrimEnd(values, char.CharComparer(ignoreCase));

        /// <summary>
        /// Removes from the span all trailing ocurrences of the given values.
        /// </summary>
        /// <param name="values></param>
        /// <param name="comparer"></param>
        /// <returns></returns>
        public StringSpan TrimEnd(IEnumerable<char> values, IEqualityComparer<string> comparer)
            => source.TrimEnd(values, char.CharComparer(comparer));

        /// <summary>
        /// Removes from the span all trailing ocurrences of the given value.s
        /// </summary>
        /// <param name="values"></param>
        /// <param name="comparison"></param>
        /// <returns></returns>
        public StringSpan TrimEnd(IEnumerable<char> values, StringComparison comparison)
            => source.TrimEnd(values, char.CharComparer(comparison));

        // ------------------------------------------------

        /// <summary>
        /// Removes from the span all leading and trailing ocurrences of the given values.
        /// </summary>
        /// <param name="values"></param>
        /// <param name="ignoreCase"></param>
        /// <returns></returns>
        public StringSpan Trim(IEnumerable<char> values, bool ignoreCase)
            => source.Trim(values, char.CharComparer(ignoreCase));

        /// <summary>
        /// Removes from the span all leading and trailing ocurrences of the given values.
        /// </summary>
        /// <param name="values></param>
        /// <param name="comparer"></param>
        /// <returns></returns>
        public StringSpan Trim(IEnumerable<char> values, IEqualityComparer<string> comparer)
            => source.Trim(values, char.CharComparer(comparer));

        /// <summary>
        /// Removes from the span all leading and trailing ocurrences of the given value.s
        /// </summary>
        /// <param name="values"></param>
        /// <param name="comparison"></param>
        /// <returns></returns>
        public StringSpan Trim(IEnumerable<char> values, StringComparison comparison)
            => source.Trim(values, char.CharComparer(comparison));
    }
}