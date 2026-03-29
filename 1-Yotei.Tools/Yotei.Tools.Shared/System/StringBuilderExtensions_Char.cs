using StringSpan = System.ReadOnlySpan<char>;

#if YOTEI_TOOLS_GENERATORS
namespace Yotei.Tools.CoreGenerator;
#else
namespace Yotei.Tools;
#endif

// ========================================================
#if YOTEI_TOOLS_GENERATORS
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
            if (source.Length == 0) return -1;

            for (int i = 0; i < source.Length; i++)
                if (predicate(source[i], value)) return i;

            return -1;
        }

        /// <summary>
        /// Returns the index of the first ocurrence of the given value in the given source, or
        /// -1 if it cannot be found.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public int IndexOf(char value) => IndexOf(source, value, static (x, y) => x == y);

        /// <summary>
        /// Returns the index of the first ocurrence of the given value in the given source, or
        /// -1 if it cannot be found.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="ignoreCase"></param>
        /// <returns></returns>
        public int IndexOf(
            char value, bool ignoreCase)
            => IndexOf(source, value, (x, y) => x.Equals(y, ignoreCase));

        /// <summary>
        /// Returns the index of the first ocurrence of the given value in the given source, or
        /// -1 if it cannot be found.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="comparer"></param>
        /// <returns></returns>
        public int IndexOf(
            char value, IEqualityComparer<char> comparer)
            => IndexOf(source, value, (x, y) => x.Equals(y, comparer));

        /// <summary>
        /// Returns the index of the first ocurrence of the given value in the given source, or
        /// -1 if it cannot be found.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="comparer"></param>
        /// <returns></returns>
        public int IndexOf(
            char value, IEqualityComparer<string> comparer)
            => IndexOf(source, value, (x, y) => x.Equals(y, comparer));

        /// <summary>
        /// Returns the index of the first ocurrence of the given value in the given source, or
        /// -1 if it cannot be found.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="comparison"></param>
        /// <returns></returns>
        public int IndexOf(
            char value, StringComparison comparison)
            => IndexOf(source, value, (x, y) => x.Equals(y, comparison));

        // ------------------------------------------------

        int LastIndexOf(char value, Func<char, char, bool> predicate)
        {
            if (source.Length == 0) return -1;

            for (int i = source.Length - 1; i >= 0; i--)
                if (predicate(source[i], value)) return i;

            return -1;
        }

        /// <summary>
        /// Returns the index of the last ocurrence of the given value in the given source, or
        /// -1 if it cannot be found.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public int LastIndexOf(
            char value) => LastIndexOf(source, value, static (x, y) => x == y);

        /// <summary>
        /// Returns the index of the last ocurrence of the given value in the given source, or
        /// -1 if it cannot be found.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="ignoreCase"></param>
        /// <returns></returns>
        public int LastIndexOf(
            char value, bool ignoreCase)
            => LastIndexOf(source, value, (x, y) => x.Equals(y, ignoreCase));

        /// <summary>
        /// Returns the index of the last ocurrence of the given value in the given source, or
        /// -1 if it cannot be found.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="comparer"></param>
        /// <returns></returns>
        public int LastIndexOf(
            char value, IEqualityComparer<char> comparer)
            => LastIndexOf(source, value, (x, y) => x.Equals(y, comparer));

        /// <summary>
        /// Returns the index of the last ocurrence of the given value in the given source, or
        /// -1 if it cannot be found.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="comparer"></param>
        /// <returns></returns>
        public int LastIndexOf(
            char value, IEqualityComparer<string> comparer)
            => LastIndexOf(source, value, (x, y) => x.Equals(y, comparer));

        /// <summary>
        /// Returns the index of the last ocurrence of the given value in the given source, or
        /// -1 if it cannot be found.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="comparison"></param>
        /// <returns></returns>
        public int LastIndexOf(
            char value, StringComparison comparison)
            => LastIndexOf(source, value, (x, y) => x.Equals(y, comparison));

        // ------------------------------------------------

        List<int> IndexesOf(char value, Func<char, char, bool> predicate)
        {
            if (source.Length == 0) return [];

            List<int> list = [];
            for (int i = 0; i < source.Length; i++) if (predicate(source[i], value)) list.Add(i);
            return list;
        }

        /// <summary>
        /// Returns the indexes of all ocurrences of the given value in the given source.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public List<int> IndexesOf(
            char value) => IndexesOf(source, value, static (x, y) => x == y);

        /// <summary>
        /// Returns the indexes of all ocurrences of the given value in the given source.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="ignoreCase"></param>
        /// <returns></returns>
        public List<int> IndexesOf(
            char value, bool ignoreCase)
            => IndexesOf(source, value, (x, y) => x.Equals(y, ignoreCase));

        /// <summary>
        /// Returns the indexes of all ocurrences of the given value in the given source.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="comparer"></param>
        /// <returns></returns>
        public List<int> IndexesOf(
            char value, IEqualityComparer<char> comparer)
            => IndexesOf(source, value, (x, y) => x.Equals(y, comparer));

        /// <summary>
        /// Returns the indexes of all ocurrences of the given value in the given source.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="comparer"></param>
        /// <returns></returns>
        public List<int> IndexesOf(
            char value, IEqualityComparer<string> comparer)
            => IndexesOf(source, value, (x, y) => x.Equals(y, comparer));

        /// <summary>
        /// Returns the indexes of all ocurrences of the given value in the given source.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="comparison"></param>
        /// <returns></returns>
        public List<int> IndexesOf(
            char value, StringComparison comparison)
            => IndexesOf(source, value, (x, y) => x.Equals(y, comparison));

        // ------------------------------------------------

        /// <summary>
        /// Determines if the source contains the given value, or not.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool Contains(char value) => source.IndexOf(value) >= 0;

        /// <summary>
        /// Determines if the source contains the given value, or not.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="ignoreCase"></param>
        /// <returns></returns>
        public bool Contains(
            char value, bool ignoreCase) => source.IndexOf(value, ignoreCase) >= 0;

        /// <summary>
        /// Determines if the source contains the given value, or not.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="comparer"></param>
        /// <returns></returns>
        public bool Contains(
            char value, IEqualityComparer<char> comparer)
            => source.IndexOf(value, comparer) >= 0;

        /// <summary>
        /// Determines if the source contains the given value, or not.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="comparer"></param>
        /// <returns></returns>
        public bool Contains(
            char value, IEqualityComparer<string> comparer)
            => source.IndexOf(value, comparer) >= 0;

        /// <summary>
        /// Determines if the source contains the given value, or not.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="comparison"></param>
        /// <returns></returns>
        public bool Contains(
            char value, StringComparison comparison)
            => source.IndexOf(value, comparison) >= 0;

        // ----------------------------------------------------

        bool StartsWith(char value, Func<char, char, bool> predicate)
        {
            if (source.Length == 0) return false;
            return predicate(source[0], value);
        }

        /// <summary>
        /// Determines if this source starts with the given value, or not.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool StartsWith(char value)
            => StartsWith(source, value, static (x, y) => x == y);

        /// <summary>
        /// Determines if this source starts with the given value, or not.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="ignoreCase"></param>
        /// <returns></returns>
        public bool StartsWith(
            char value, bool ignoreCase)
            => StartsWith(source, value, (x, y) => x.Equals(y, ignoreCase));

        /// <summary>
        /// Determines if this source starts with the given value, or not.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="comparer"></param>
        /// <returns></returns>
        public bool StartsWith(
            char value, IEqualityComparer<char> comparer)
            => StartsWith(source, value, (x, y) => x.Equals(y, comparer));

        /// <summary>
        /// Determines if this source starts with the given value, or not.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="comparer"></param>
        /// <returns></returns>
        public bool StartsWith(
            char value, IEqualityComparer<string> comparer)
            => StartsWith(source, value, (x, y) => x.Equals(y, comparer));

        /// <summary>
        /// Determines if this source starts with the given value, or not.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="comparison"></param>
        /// <returns></returns>
        public bool StartsWith(
            char value, StringComparison comparison)
            => StartsWith(source, value, (x, y) => x.Equals(y, comparison));

        // ----------------------------------------------------

        bool EndsWith(char value, Func<char, char, bool> predicate)
        {
            if (source.Length == 0) return false;
            return predicate(source[^1], value);
        }

        /// <summary>
        /// Determines if this source starts with the given value, or not.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool EndsWith(char value)
            => EndsWith(source, value, static (x, y) => x == y);

        /// <summary>
        /// Determines if this source starts with the given value, or not.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="ignoreCase"></param>
        /// <returns></returns>
        public bool EndsWith(
            char value, bool ignoreCase)
            => EndsWith(source, value, (x, y) => x.Equals(y, ignoreCase));

        /// <summary>
        /// Determines if this source starts with the given value, or not.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="comparer"></param>
        /// <returns></returns>
        public bool EndsWith(
            char value, IEqualityComparer<char> comparer)
            => EndsWith(source, value, (x, y) => x.Equals(y, comparer));

        /// <summary>
        /// Determines if this source starts with the given value, or not.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="comparer"></param>
        /// <returns></returns>
        public bool EndsWith(
            char value, IEqualityComparer<string> comparer)
            => EndsWith(source, value, (x, y) => x.Equals(y, comparer));

        /// <summary>
        /// Determines if this source starts with the given value, or not.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="comparison"></param>
        /// <returns></returns>
        public bool EndsWith(
            char value, StringComparison comparison)
            => EndsWith(source, value, (x, y) => x.Equals(y, comparison));

        // ----------------------------------------------------

        StringBuilder Remove(char value, Func<char, char, bool> predicate)
        {
            var index = IndexOf(source, value, predicate);
            if (index >= 0) source = source.Remove(index, 1);
            return source;
        }

        /// <summary>
        /// Removes from this instance the first ocurrence of the given value.
        /// Returns a reference to this source instance to enable fluent syntax chaining.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public StringBuilder Remove(
            char value) => Remove(source, value, static (x, y) => x == y);

        /// <summary>
        /// Removes from this instance the first ocurrence of the given value.
        /// Returns a reference to this source instance to enable fluent syntax chaining.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="ignoreCase"></param>
        /// <returns></returns>
        public StringBuilder Remove(
            char value, bool ignoreCase)
            => Remove(source, value, (x, y) => x.Equals(y, ignoreCase));

        /// <summary>
        /// Removes from this instance the first ocurrence of the given value.
        /// Returns a reference to this source instance to enable fluent syntax chaining.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="comparer"></param>
        /// <returns></returns>
        public StringBuilder Remove(
            char value, IEqualityComparer<char> comparer)
            => Remove(source, value, (x, y) => x.Equals(y, comparer));

        /// <summary>
        /// Removes from this instance the first ocurrence of the given value.
        /// Returns a reference to this source instance to enable fluent syntax chaining.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="comparer"></param>
        /// <returns></returns>
        public StringBuilder Remove(
            char value, IEqualityComparer<string> comparer)
            => Remove(source, value, (x, y) => x.Equals(y, comparer));

        /// <summary>
        /// Removes from this instance the first ocurrence of the given value.
        /// Returns a reference to this source instance to enable fluent syntax chaining.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="comparison"></param>
        /// <returns></returns>
        public StringBuilder Remove(
            char value, StringComparison comparison)
            => Remove(source, value, (x, y) => x.Equals(y, comparison));

        // ----------------------------------------------------

        StringBuilder RemoveLast(char value, Func<char, char, bool> predicate)
        {
            var index = LastIndexOf(source, value, predicate);
            if (index >= 0) source = source.Remove(index, 1);
            return source;
        }

        /// <summary>
        /// Removes from this instance the last ocurrence of the given value.
        /// Returns a reference to this source instance to enable fluent syntax chaining.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public StringBuilder RemoveLast(
            char value) => RemoveLast(source, value, static (x, y) => x == y);

        /// <summary>
        /// Removes from this instance the last ocurrence of the given value.
        /// Returns a reference to this source instance to enable fluent syntax chaining.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="ignoreCase"></param>
        /// <returns></returns>
        public StringBuilder RemoveLast(
            char value, bool ignoreCase)
            => RemoveLast(source, value, (x, y) => x.Equals(y, ignoreCase));

        /// <summary>
        /// Removes from this instance the last ocurrence of the given value.
        /// Returns a reference to this source instance to enable fluent syntax chaining.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="comparer"></param>
        /// <returns></returns>
        public StringBuilder RemoveLast(
            char value, IEqualityComparer<char> comparer)
            => RemoveLast(source, value, (x, y) => x.Equals(y, comparer));

        /// <summary>
        /// Removes from this instance the last ocurrence of the given value.
        /// Returns a reference to this source instance to enable fluent syntax chaining.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="comparer"></param>
        /// <returns></returns>
        public StringBuilder RemoveLast(
            char value, IEqualityComparer<string> comparer)
            => RemoveLast(source, value, (x, y) => x.Equals(y, comparer));

        /// <summary>
        /// Removes from this instance the last ocurrence of the given value.
        /// Returns a reference to this source instance to enable fluent syntax chaining.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="comparison"></param>
        /// <returns></returns>
        public StringBuilder RemoveLast(
            char value, StringComparison comparison)
            => RemoveLast(source, value, (x, y) => x.Equals(y, comparison));

        // ----------------------------------------------------

        public StringBuilder RemoveAll(char value, Func<char, char, bool> predicate)
        {
            var indexes = IndexesOf(source, value, predicate);
            indexes.Reverse();

            foreach (var index in indexes) source.Remove(index, 1);
            return source;
        }

        /// <summary>
        /// Remove from this instance all the ocurrences of the given value.
        /// Returns a reference to this source instance to enable fluent syntax chaining.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public StringBuilder RemoveAll(
            char value) => RemoveAll(source, value, static (x, y) => x == y);

        /// <summary>
        /// Remove from this instance all the ocurrences of the given value.
        /// Returns a reference to this source instance to enable fluent syntax chaining.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="ignoreCase"></param>
        /// <returns></returns>
        public StringBuilder RemoveAll(
            char value, bool ignoreCase)
            => RemoveAll(source, value, (x, y) => x.Equals(y, ignoreCase));

        /// <summary>
        /// Remove from this instance all the ocurrences of the given value.
        /// Returns a reference to this source instance to enable fluent syntax chaining.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="comparer"></param>
        /// <returns></returns>
        public StringBuilder RemoveAll(
            char value, IEqualityComparer<char> comparer)
            => RemoveAll(source, value, (x, y) => x.Equals(y, comparer));

        /// <summary>
        /// Remove from this instance all the ocurrences of the given value.
        /// Returns a reference to this source instance to enable fluent syntax chaining.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="comparer"></param>
        /// <returns></returns>
        public StringBuilder RemoveAll(
            char value, IEqualityComparer<string> comparer)
            => RemoveAll(source, value, (x, y) => x.Equals(y, comparer));

        /// <summary>
        /// Remove from this instance all the ocurrences of the given value.
        /// Returns a reference to this source instance to enable fluent syntax chaining.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="comparison"></param>
        /// <returns></returns>
        public StringBuilder RemoveAll(
            char value, StringComparison comparison)
            => RemoveAll(source, value, (x, y) => x.Equals(y, comparison));
    }
}