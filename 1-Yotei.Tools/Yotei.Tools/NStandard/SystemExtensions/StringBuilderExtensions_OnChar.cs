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

        // ------------------------------------------------

        /// <summary>
        /// Determines if the builder contains the given value.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool Contains(char value) => source.IndexOf(value) >= 0;

        /// <summary>
        /// Determines if the builder contains the given value.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="ignoreCase"></param>
        /// <returns></returns>
        public bool Contains(
            char value, bool ignoreCase) => source.IndexOf(value, ignoreCase) >= 0;

        /// <summary>
        /// Determines if the builder contains the given value.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="comparer"></param>
        /// <returns></returns>
        public bool Contains(
            char value, IEqualityComparer<char> comparer) => source.IndexOf(value, comparer) >= 0;

        /// <summary>
        /// Determines if the builder contains the given value.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="comparer"></param>
        /// <returns></returns>
        public bool Contains(
            char value, IEqualityComparer<string> comparer) => source.IndexOf(value, comparer) >= 0;

        /// <summary>
        /// Determines if the builder contains the given value.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="comparison"></param>
        /// <returns></returns>
        public bool Contains(
            char value, StringComparison comparison) => source.IndexOf(value, comparison) >= 0;

        // ------------------------------------------------

        /// <summary>
        /// Determines if the span starts with the given value.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool StartsWith(char value) => source.Length > 0 && value.Equals(source[0]);

        /// <summary>
        /// Determines if the span starts with the given value.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="ignoreCase"></param>
        /// <returns></returns>
        public bool StartsWith(char value, bool ignoreCase)
            => source.Length > 0
            && value.Equals(source[0], ignoreCase);

        /// <summary>
        /// Determines if the span starts with the given value.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="comparer"></param>
        /// <returns></returns>
        public bool StartsWith(
            char value, IEqualityComparer<char> comparer)
            => source.Length > 0
            && comparer.Equals(value, source[0]);

        /// <summary>
        /// Determines if the span starts with the given value.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="comparer"></param>
        /// <returns></returns>
        public bool StartsWith(
            char value, IEqualityComparer<string> comparer)
            => source.Length > 0
            && value.Equals(source[0], comparer);

        /// <summary>
        /// Determines if the span starts with the given value.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="comparison"></param>
        /// <returns></returns>
        public bool StartsWith(
            char value, StringComparison comparison)
            => source.Length > 0
            && value.Equals(source[0], comparison);

        // ------------------------------------------------

        /// <summary>
        /// Determines if the span ends with the given value.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool EndsWith(char value) => source.Length > 0 && value.Equals(source[^1]);

        /// <summary>
        /// Determines if the span ends with the given value.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="ignoreCase"></param>
        /// <returns></returns>
        public bool EndsWith(char value, bool ignoreCase)
            => source.Length > 0
            && value.Equals(source[^1], ignoreCase);

        /// <summary>
        /// Determines if the span ends with the given value.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="comparer"></param>
        /// <returns></returns>
        public bool EndsWith(
            char value, IEqualityComparer<char> comparer)
            => source.Length > 0
            && comparer.Equals(value, source[^1]);

        /// <summary>
        /// Determines if the span ends with the given value.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="comparer"></param>
        /// <returns></returns>
        public bool EndsWith(
            char value, IEqualityComparer<string> comparer)
            => source.Length > 0
            && value.Equals(source[^1], comparer);

        /// <summary>
        /// Determines if the span ends with the given value.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="comparison"></param>
        /// <returns></returns>
        public bool EndsWith(
            char value, StringComparison comparison)
            => source.Length > 0
            && value.Equals(source[^1], comparison);

        // ------------------------------------------------

        int IndexOfAny(IEnumerable<char> values, Func<char, char, bool> predicate)
        {
            values.ThrowWhenNull();

            for (int i = 0; i < source.Length; i++)
            {
                foreach (var value in values)
                    if (predicate(value, source[i])) return i;
            }
            return -1;
        }

        /// <summary>
        /// Returns the index of the first ocurrence of any of the given values.
        /// </summary>
        /// <param name="values"></param>
        /// <returns></returns>
        public int IndexOfAny(
            IEnumerable<char> values)
            => source.IndexOfAny(values, (x, y) => x == y);

        /// <summary>
        /// Returns the index of the first ocurrence of any of the given values.
        /// </summary>
        /// <param name="values"></param>
        /// <param name="ignoreCase"></param>
        /// <returns></returns>
        public int IndexOfAny(
            IEnumerable<char> values, bool ignoreCase)
            => source.IndexOfAny(values, (x, y) => x.Equals(y, ignoreCase));

        /// <summary>
        /// Returns the index of the first ocurrence of any of the given values.
        /// </summary>
        /// <param name="values"></param>
        /// <param name="comparer"></param>
        /// <returns></returns>
        public int IndexOfAny(
            IEnumerable<char> values, IEqualityComparer<char> comparer)
            => source.IndexOfAny(values, (x, y) => comparer.Equals(x, y));

        /// <summary>
        /// Returns the index of the first ocurrence of any of the given values.
        /// </summary>
        /// <param name="values"></param>
        /// <param name="comparer"></param>
        /// <returns></returns>
        public int IndexOfAny(
            IEnumerable<char> values, IEqualityComparer<string> comparer)
            => source.IndexOfAny(values, (x, y) => x.Equals(y, comparer));

        /// <summary>
        /// Returns the index of the first ocurrence of any of the given values.
        /// </summary>
        /// <param name="values"></param>
        /// <param name="comparison"></param>
        /// <returns></returns>
        public int IndexOfAny(
            IEnumerable<char> values, StringComparison comparison)
            => source.IndexOfAny(values, (x, y) => x.Equals(y, comparison));

        // ------------------------------------------------

        int LastIndexOfAny(IEnumerable<char> values, Func<char, char, bool> predicate)
        {
            values.ThrowWhenNull();

            for (int i = source.Length - 1; i >= 0; i--)
            {
                foreach (var value in values)
                    if (predicate(value, source[i])) return i;
            }
            return -1;
        }

        /// <summary>
        /// Returns the index of the first ocurrence of any of the given values.
        /// </summary>
        /// <param name="values"></param>
        /// <returns></returns>
        public int LastIndexOfAny(
            IEnumerable<char> values) => source.LastIndexOfAny(values, (x, y) => x == y);

        /// <summary>
        /// Returns the index of the first ocurrence of any of the given values.
        /// </summary>
        /// <param name="values"></param>
        /// <param name="ignoreCase"></param>
        /// <returns></returns>
        public int LastIndexOfAny(
            IEnumerable<char> values, bool ignoreCase)
            => source.LastIndexOfAny(values, (x, y) => x.Equals(y, ignoreCase));

        /// <summary>
        /// Returns the index of the first ocurrence of any of the given values.
        /// </summary>
        /// <param name="values"></param>
        /// <param name="comparer"></param>
        /// <returns></returns>
        public int LastIndexOfAny(
            IEnumerable<char> values, IEqualityComparer<char> comparer)
            => source.LastIndexOfAny(values, (x, y) => comparer.Equals(x, y));

        /// <summary>
        /// Returns the index of the first ocurrence of any of the given values.
        /// </summary>
        /// <param name="values"></param>
        /// <param name="comparer"></param>
        /// <returns></returns>
        public int LastIndexOfAny(
            IEnumerable<char> values, IEqualityComparer<string> comparer)
            => source.LastIndexOfAny(values, (x, y) => x.Equals(y, comparer));

        /// <summary>
        /// Returns the index of the first ocurrence of any of the given values.
        /// </summary>
        /// <param name="values"></param>
        /// <param name="comparison"></param>
        /// <returns></returns>
        public int LastIndexOfAny(
            IEnumerable<char> values, StringComparison comparison)
            => source.LastIndexOfAny(values, (x, y) => x.Equals(y, comparison));

        // ------------------------------------------------

        /// <summary>
        /// Determines if the span contains any of the given values.
        /// </summary>
        /// <param name="values"></param>
        /// <returns></returns>
        public bool ContainsAny(IEnumerable<char> values) => source.IndexOfAny(values) >= 0;

        /// <summary>
        /// Determines if the span contains any of the given values.
        /// </summary>
        /// <param name="values"></param>
        /// <param name="ignoreCase"></param>
        /// <returns></returns>
        public bool ContainsAny(
            IEnumerable<char> values, bool ignoreCase)
            => source.IndexOfAny(values, ignoreCase) >= 0;

        /// <summary>
        /// Determines if the span contains any of the given values.
        /// </summary>
        /// <param name="values"></param>
        /// <param name="comparer"></param>
        /// <returns></returns>
        public bool ContainsAny(
            IEnumerable<char> values, IEqualityComparer<char> comparer)
            => source.IndexOfAny(values, comparer) >= 0;

        /// <summary>
        /// Determines if the span contains any of the given values.
        /// </summary>
        /// <param name="values"></param>
        /// <param name="comparer"></param>
        /// <returns></returns>
        public bool ContainsAny(
            IEnumerable<char> values, IEqualityComparer<string> comparer)
            => source.IndexOfAny(values, comparer) >= 0;

        /// <summary>
        /// Determines if the span contains any of the given values.
        /// </summary>
        /// <param name="values"></param>
        /// <param name="comparison"></param>
        /// <returns></returns>
        public bool ContainsAny(
            IEnumerable<char> values, StringComparison comparison)
            => source.IndexOfAny(values, comparison) >= 0;

        // ------------------------------------------------

        StringBuilder TrimStart(char value, Func<char, char, bool> predicate)
        {
            while (source.Length > 0)
            {
                var item = source[0];
                if (predicate(value, item)) source.Remove(0, 1);
                else break;
            }
            return source;
        }

        /// <summary>
        /// Removes from the builder all leading ocurrences of the given value.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public StringBuilder TrimStart(char value) => source.TrimStart(value, (x, y) => x == y);

        /// <summary>
        /// Removes from the builder all leading ocurrences of the given value.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="ignoreCase"></param>
        /// <returns></returns>
        public StringBuilder TrimStart(
            char value, bool ignoreCase)
            => source.TrimStart(value, (x, y) => x.Equals(y, ignoreCase));

        /// <summary>
        /// Removes from the builder all leading ocurrences of the given value.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="comparer"></param>
        /// <returns></returns>
        public StringBuilder TrimStart(
            char value, IEqualityComparer<char> comparer)
            => source.TrimStart(value, (x, y) => comparer.Equals(x, y));

        /// <summary>
        /// Removes from the builder all leading ocurrences of the given value.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="comparer"></param>
        /// <returns></returns>
        public StringBuilder TrimStart(
            char value, IEqualityComparer<string> comparer)
            => source.TrimStart(value, (x, y) => x.Equals(y, comparer));

        /// <summary>
        /// Removes from the builder all leading ocurrences of the given value.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="comparison"></param>
        /// <returns></returns>
        public StringBuilder TrimStart(
            char value, StringComparison comparison)
            => source.TrimStart(value, (x, y) => x.Equals(y, comparison));

        // ------------------------------------------------

        StringBuilder TrimEnd(char value, Func<char, char, bool> predicate)
        {
            while (source.Length > 0)
            {
                var item = source[^1];
                if (predicate(value, item)) source.Length--;
                else break;
            }
            return source;
        }

        /// <summary>
        /// Removes from the builder all trailing ocurrences of the given value.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public StringBuilder TrimEnd(char value) => source.TrimEnd(value, (x, y) => x == y);

        /// <summary>
        /// Removes from the builder all trailing ocurrences of the given value.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="ignoreCase"></param>
        /// <returns></returns>
        public StringBuilder TrimEnd(
            char value, bool ignoreCase)
            => source.TrimEnd(value, (x,y) => x.Equals(y, ignoreCase));

        /// <summary>
        /// Removes from the builder all trailing ocurrences of the given value.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="comparer"></param>
        /// <returns></returns>
        public StringBuilder TrimEnd(
            char value, IEqualityComparer<char> comparer)
            => source.TrimEnd(value, (x, y) => comparer.Equals(x, y));

        /// <summary>
        /// Removes from the builder all trailing ocurrences of the given value.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="comparer"></param>
        /// <returns></returns>
        public StringBuilder TrimEnd(
            char value, IEqualityComparer<string> comparer)
            => source.TrimEnd(value, (x, y) => x.Equals(y, comparer));

        /// <summary>
        /// Removes from the builder all trailing ocurrences of the given value.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="comparison"></param>
        /// <returns></returns>
        public StringBuilder TrimEnd(
            char value, StringComparison comparison)
            => source.TrimEnd(value, (x, y) => x.Equals(y, comparison));

        // ------------------------------------------------

        /// <summary>
        /// Removes from the builder all leading and trailing ocurrences of the given value.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public StringBuilder Trim(char value)
            => source.TrimStart(value).TrimEnd(value);

        /// <summary>
        /// Removes from the builder all leading and trailing ocurrences of the given value.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="ignoreCase"></param>
        /// <returns></returns>
        public StringBuilder Trim(char value, bool ignoreCase)
            => source.TrimStart(value, ignoreCase).TrimEnd(value, ignoreCase);

        /// <summary>
        /// Removes from the builder all leading and trailing ocurrences of the given value.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="comparer"></param>
        /// <returns></returns>
        public StringBuilder Trim(
            char value, IEqualityComparer<char> comparer)
            => source.TrimStart(value, comparer).TrimEnd(value, comparer);

        /// <summary>
        /// Removes from the builder all leading and trailing ocurrences of the given value.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="comparer"></param>
        /// <returns></returns>
        public StringBuilder Trim(
            char value, IEqualityComparer<string> comparer)
            => source.TrimStart(value, comparer).TrimEnd(value, comparer);

        /// <summary>
        /// Removes from the builder all leading and trailing ocurrences of the given value.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="comparison"></param>
        /// <returns></returns>
        public StringBuilder Trim(
            char value, StringComparison comparison)
            => source.TrimStart(value, comparison).TrimEnd(value, comparison);

        // ------------------------------------------------

        /// <summary>
        /// Removes from the builder the first ocurrence of the given value.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public StringBuilder Remove(char value)
        {
            var index = source.IndexOf(value);
            if (index >= 0) source.Remove(index, 1);
            return source;
        }

        /// <summary>
        /// Removes from the builder the first ocurrence of the given value.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="ignoreCase"></param>
        /// <returns></returns>
        public StringBuilder Remove(char value, bool ignoreCase)
        {
            var index = source.IndexOf(value, ignoreCase);
            if (index >= 0) source.Remove(index, 1);
            return source;
        }

        /// <summary>
        /// Removes from the builder the first ocurrence of the given value.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="comparer"></param>
        /// <returns></returns>
        public StringBuilder Remove(char value, IEqualityComparer<char> comparer)
        {
            var index = source.IndexOf(value, comparer);
            if (index >= 0) source.Remove(index, 1);
            return source;
        }

        /// <summary>
        /// Removes from the builder the first ocurrence of the given value.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="comparer"></param>
        /// <returns></returns>
        public StringBuilder Remove(char value, IEqualityComparer<string> comparer)
        {
            var index = source.IndexOf(value, comparer);
            if (index >= 0) source.Remove(index, 1);
            return source;
        }

        /// <summary>
        /// Removes from the builder the first ocurrence of the given value.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="comparison"></param>
        /// <returns></returns>
        public StringBuilder Remove(char value, StringComparison comparison)
        {
            var index = source.IndexOf(value, comparison);
            if (index >= 0) source.Remove(index, 1);
            return source;
        }

        // ------------------------------------------------

        /// <summary>
        /// Removes from the builder the last ocurrence of the given value.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public StringBuilder RemoveLast(char value)
        {
            var index = source.LastIndexOf(value);
            if (index >= 0) source.Remove(index, 1);
            return source;
        }

        /// <summary>
        /// RemoveLasts from the builder the last ocurrence of the given value.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="ignoreCase"></param>
        /// <returns></returns>
        public StringBuilder RemoveLast(char value, bool ignoreCase)
        {
            var index = source.LastIndexOf(value, ignoreCase);
            if (index >= 0) source.Remove(index, 1);
            return source;
        }

        /// <summary>
        /// RemoveLasts from the builder the last ocurrence of the given value.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="comparer"></param>
        /// <returns></returns>
        public StringBuilder RemoveLast(char value, IEqualityComparer<char> comparer)
        {
            var index = source.LastIndexOf(value, comparer);
            if (index >= 0) source.Remove(index, 1);
            return source;
        }

        /// <summary>
        /// RemoveLasts from the builder the last ocurrence of the given value.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="comparer"></param>
        /// <returns></returns>
        public StringBuilder RemoveLast(char value, IEqualityComparer<string> comparer)
        {
            var index = source.LastIndexOf(value, comparer);
            if (index >= 0) source.Remove(index, 1);
            return source;
        }

        /// <summary>
        /// RemoveLasts from the builder the last ocurrence of the given value.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="comparison"></param>
        /// <returns></returns>
        public StringBuilder RemoveLast(char value, StringComparison comparison)
        {
            var index = source.LastIndexOf(value, comparison);
            if (index >= 0) source.Remove(index, 1);
            return source;
        }

        // ------------------------------------------------

        /// <summary>
        /// Removes from the builder all ocurrences of the given value.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public StringBuilder RemoveAll(char value)
        {
            while (true)
            {
                var index = source.IndexOf(value);
                if (index >= 0) source.Remove(index, 1);
                else break;
            }
            return source;
        }

        /// <summary>
        /// Removes from the builder all ocurrences of the given value.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="ignoreCase"></param>
        /// <returns></returns>
        public StringBuilder RemoveAll(char value, bool ignoreCase)
        {
            while (true)
            {
                var index = source.IndexOf(value, ignoreCase);
                if (index >= 0) source.Remove(index, 1);
                else break;
            }
            return source;
        }

        /// <summary>
        /// Removes from the builder all ocurrences of the given value.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="comparer"></param>
        /// <returns></returns>
        public StringBuilder RemoveAll(char value, IEqualityComparer<char> comparer)
        {
            while (true)
            {
                var index = source.IndexOf(value, comparer);
                if (index >= 0) source.Remove(index, 1);
                else break;
            }
            return source;
        }

        /// <summary>
        /// Removes from the builder all ocurrences of the given value.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="comparer"></param>
        /// <returns></returns>
        public StringBuilder RemoveAll(char value, IEqualityComparer<string> comparer)
        {
            while (true)
            {
                var index = source.IndexOf(value, comparer);
                if (index >= 0) source.Remove(index, 1);
                else break;
            }
            return source;
        }

        /// <summary>
        /// Removes from the builder all ocurrences of the given value.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="comparison"></param>
        /// <returns></returns>
        public StringBuilder RemoveAll(char value, StringComparison comparison)
        {
            while (true)
            {
                var index = source.IndexOf(value, comparison);
                if (index >= 0) source.Remove(index, 1);
                else break;
            }
            return source;
        }
    }
}