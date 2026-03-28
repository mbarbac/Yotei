using StringSpan = System.ReadOnlySpan<char>;

#if YOTEI_TOOLS_COREGENERATOR
namespace Yotei.Tools.CoreGenerator;
#else
namespace Yotei.Tools;
#endif

// ========================================================
#if YOTEI_TOOLS_COREGENERATOR
internal
#else
public
#endif
static partial class StringBuilderExtensions
{
    extension(StringBuilder source)
    {
        int IndexOf(StringSpan value, Func<char, char, bool> predicate)
        {
            if (source.Length == 0 && value.Length == 0) return 0;
            if (source.Length == 0 || value.Length == 0) return -1;
            if (value.Length > source.Length) return -1;

            for (int i = 0; i < source.Length; i++)
            {
                var found = true;
                for (int j = 0; j < value.Length; j++)
                {
                    var s = source[i + j];
                    var v = value[j];
                    if (!predicate(s, v)) { found = false; break; }
                }
                if (found) return i;
            }
            return -1;
        }

        /// <summary>
        /// Returns the index of the first ocurrence of the given value in the given source, or
        /// -1 if it cannot be found.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public int IndexOf(StringSpan value) => IndexOf(source, value, static (x, y) => x == y);

        /// <summary>
        /// Returns the index of the first ocurrence of the given value in the given source, or
        /// -1 if it cannot be found.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="ignoreCase"></param>
        /// <returns></returns>
        public int IndexOf(
            StringSpan value, bool ignoreCase)
            => IndexOf(source, value, (x, y) => x.Equals(y, ignoreCase));

        /// <summary>
        /// Returns the index of the first ocurrence of the given value in the given source, or
        /// -1 if it cannot be found.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="comparer"></param>
        /// <returns></returns>
        public int IndexOf(
            StringSpan value, IEqualityComparer<char> comparer)
            => IndexOf(source, value, (x, y) => x.Equals(y, comparer));

        /// <summary>
        /// Returns the index of the first ocurrence of the given value in the given source, or
        /// -1 if it cannot be found.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="comparer"></param>
        /// <returns></returns>
        public int IndexOf(
            StringSpan value, IEqualityComparer<string> comparer)
            => IndexOf(source, value, (x, y) => x.Equals(y, comparer));

        /// <summary>
        /// Returns the index of the first ocurrence of the given value in the given source, or
        /// -1 if it cannot be found.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="comparison"></param>
        /// <returns></returns>
        public int IndexOf(
            StringSpan value, StringComparison comparison)
            => IndexOf(source, value, (x, y) => x.Equals(y, comparison));

        // ------------------------------------------------

        int LastIndexOf(StringSpan value, Func<char, char, bool> predicate)
        {
            if (source.Length == 0 && value.Length == 0) return 0;
            if (source.Length == 0 || value.Length == 0) return -1;
            if (value.Length > source.Length) return -1;

            for (int i = source.Length - value.Length; i >= 0; i--)
            {
                var found = true;
                for (int j = 0; j < value.Length; j++)
                {
                    var s = source[i + j];
                    var v = value[j];
                    if (!predicate(s, v)) { found = false; break; }
                }
                if (found) return i;
            }
            return -1;
        }

        /// <summary>
        /// Returns the index of the last ocurrence of the given value in the given source, or
        /// -1 if it cannot be found.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public int LastIndexOf(
            StringSpan value) => LastIndexOf(source, value, static (x, y) => x == y);

        /// <summary>
        /// Returns the index of the last ocurrence of the given value in the given source, or
        /// -1 if it cannot be found.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="ignoreCase"></param>
        /// <returns></returns>
        public int LastIndexOf(
            StringSpan value, bool ignoreCase)
            => LastIndexOf(source, value, (x, y) => x.Equals(y, ignoreCase));

        /// <summary>
        /// Returns the index of the last ocurrence of the given value in the given source, or
        /// -1 if it cannot be found.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="comparer"></param>
        /// <returns></returns>
        public int LastIndexOf(
            StringSpan value, IEqualityComparer<char> comparer)
            => LastIndexOf(source, value, (x, y) => x.Equals(y, comparer));

        /// <summary>
        /// Returns the index of the last ocurrence of the given value in the given source, or
        /// -1 if it cannot be found.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="comparer"></param>
        /// <returns></returns>
        public int LastIndexOf(
            StringSpan value, IEqualityComparer<string> comparer)
            => LastIndexOf(source, value, (x, y) => x.Equals(y, comparer));

        /// <summary>
        /// Returns the index of the last ocurrence of the given value in the given source, or
        /// -1 if it cannot be found.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="comparison"></param>
        /// <returns></returns>
        public int LastIndexOf(
            StringSpan value, StringComparison comparison)
            => LastIndexOf(source, value, (x, y) => x.Equals(y, comparison));

        // ------------------------------------------------

        List<int> IndexesOf(StringSpan value, Func<char, char, bool> predicate)
        {
            if (source.Length == 0 && value.Length == 0) return [0];
            if (source.Length == 0 || value.Length == 0) return [];
            if (value.Length > source.Length) return [];

            List<int> list = [];
            for (int i = 0; i < source.Length; i++)
            {
                var found = true;
                for (int j = 0; j < value.Length; j++)
                {
                    var s = source[i + j];
                    var v = value[j];
                    if (!predicate(s, v)) { found = false; break; }
                }
                if (found) list.Add(i);
            }
            return list;
        }

        /// <summary>
        /// Returns the indexes of all ocurrences of the given value in the given source.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public List<int> IndexesOf(
            StringSpan value) => IndexesOf(source, value, static (x, y) => x == y);

        /// <summary>
        /// Returns the indexes of all ocurrences of the given value in the given source.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="ignoreCase"></param>
        /// <returns></returns>
        public List<int> IndexesOf(
            StringSpan value, bool ignoreCase)
            => IndexesOf(source, value, (x, y) => x.Equals(y, ignoreCase));

        /// <summary>
        /// Returns the indexes of all ocurrences of the given value in the given source.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="comparer"></param>
        /// <returns></returns>
        public List<int> IndexesOf(
            StringSpan value, IEqualityComparer<char> comparer)
            => IndexesOf(source, value, (x, y) => x.Equals(y, comparer));

        /// <summary>
        /// Returns the indexes of all ocurrences of the given value in the given source.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="comparer"></param>
        /// <returns></returns>
        public List<int> IndexesOf(
            StringSpan value, IEqualityComparer<string> comparer)
            => IndexesOf(source, value, (x, y) => x.Equals(y, comparer));

        /// <summary>
        /// Returns the indexes of all ocurrences of the given value in the given source.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="comparison"></param>
        /// <returns></returns>
        public List<int> IndexesOf(
            StringSpan value, StringComparison comparison)
            => IndexesOf(source, value, (x, y) => x.Equals(y, comparison));

        // ------------------------------------------------

        /// <summary>
        /// Determines if the source contains the given value, or not.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool Contains(StringSpan value) => source.IndexOf(value) >= 0;

        /// <summary>
        /// Determines if the source contains the given value, or not.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="ignoreCase"></param>
        /// <returns></returns>
        public bool Contains(
            StringSpan value, bool ignoreCase) => source.IndexOf(value, ignoreCase) >= 0;

        /// <summary>
        /// Determines if the source contains the given value, or not.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="comparer"></param>
        /// <returns></returns>
        public bool Contains(
            StringSpan value, IEqualityComparer<char> comparer)
            => source.IndexOf(value, comparer) >= 0;

        /// <summary>
        /// Determines if the source contains the given value, or not.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="comparer"></param>
        /// <returns></returns>
        public bool Contains(
            StringSpan value, IEqualityComparer<string> comparer)
            => source.IndexOf(value, comparer) >= 0;

        /// <summary>
        /// Determines if the source contains the given value, or not.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="comparison"></param>
        /// <returns></returns>
        public bool Contains(
            StringSpan value, StringComparison comparison)
            => source.IndexOf(value, comparison) >= 0;

        // ----------------------------------------------------

        bool StartsWith(StringSpan value, Func<char, char, bool> predicate)
        {
            if (source.Length == 0 && value.Length == 0) return true;
            if (source.Length == 0 || value.Length == 0) return false;
            if (value.Length > source.Length) return false;

            var index = IndexOf(source, value, predicate);
            return index == 0;
        }

        /// <summary>
        /// Determines if this source starts with the given value, or not.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool StartsWith(StringSpan value)
            => StartsWith(source, value, static (x, y) => x == y);

        /// <summary>
        /// Determines if this source starts with the given value, or not.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="ignoreCase"></param>
        /// <returns></returns>
        public bool StartsWith(
            StringSpan value, bool ignoreCase)
            => StartsWith(source, value, (x, y) => x.Equals(y, ignoreCase));

        /// <summary>
        /// Determines if this source starts with the given value, or not.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="comparer"></param>
        /// <returns></returns>
        public bool StartsWith(
            StringSpan value, IEqualityComparer<char> comparer)
            => StartsWith(source, value, (x, y) => x.Equals(y, comparer));

        /// <summary>
        /// Determines if this source starts with the given value, or not.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="comparer"></param>
        /// <returns></returns>
        public bool StartsWith(
            StringSpan value, IEqualityComparer<string> comparer)
            => StartsWith(source, value, (x, y) => x.Equals(y, comparer));

        /// <summary>
        /// Determines if this source starts with the given value, or not.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="comparison"></param>
        /// <returns></returns>
        public bool StartsWith(
            StringSpan value, StringComparison comparison)
            => StartsWith(source, value, (x, y) => x.Equals(y, comparison));

        // ----------------------------------------------------

        bool EndsWith(StringSpan value, Func<char, char, bool> predicate)
        {
            if (source.Length == 0 && value.Length == 0) return true;
            if (source.Length == 0 || value.Length == 0) return false;
            if (value.Length > source.Length) return false;

            var index = IndexOf(source, value, predicate);
            return index == source.Length - value.Length;
        }

        /// <summary>
        /// Determines if this source starts with the given value, or not.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool EndsWith(StringSpan value)
            => EndsWith(source, value, static (x, y) => x == y);

        /// <summary>
        /// Determines if this source starts with the given value, or not.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="ignoreCase"></param>
        /// <returns></returns>
        public bool EndsWith(
            StringSpan value, bool ignoreCase)
            => EndsWith(source, value, (x, y) => x.Equals(y, ignoreCase));

        /// <summary>
        /// Determines if this source starts with the given value, or not.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="comparer"></param>
        /// <returns></returns>
        public bool EndsWith(
            StringSpan value, IEqualityComparer<char> comparer)
            => EndsWith(source, value, (x, y) => x.Equals(y, comparer));

        /// <summary>
        /// Determines if this source starts with the given value, or not.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="comparer"></param>
        /// <returns></returns>
        public bool EndsWith(
            StringSpan value, IEqualityComparer<string> comparer)
            => EndsWith(source, value, (x, y) => x.Equals(y, comparer));

        /// <summary>
        /// Determines if this source starts with the given value, or not.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="comparison"></param>
        /// <returns></returns>
        public bool EndsWith(
            StringSpan value, StringComparison comparison)
            => EndsWith(source, value, (x, y) => x.Equals(y, comparison));

        // ----------------------------------------------------

        StringBuilder Remove(StringSpan value, Func<char, char, bool> predicate)
        {
            var index = IndexOf(source, value, predicate);
            if (index >= 0) source = source.Remove(index, value.Length);
            return source;
        }

        /// <summary>
        /// Removes from this instance the first ocurrence of the given value.
        /// Returns a reference to this source instance to enable fluent syntax chaining.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public StringBuilder Remove(
            StringSpan value) => Remove(source, value, static (x, y) => x == y);

        /// <summary>
        /// Removes from this instance the first ocurrence of the given value.
        /// Returns a reference to this source instance to enable fluent syntax chaining.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="ignoreCase"></param>
        /// <returns></returns>
        public StringBuilder Remove(
            StringSpan value, bool ignoreCase)
            => Remove(source, value, (x, y) => x.Equals(y, ignoreCase));

        /// <summary>
        /// Removes from this instance the first ocurrence of the given value.
        /// Returns a reference to this source instance to enable fluent syntax chaining.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="comparer"></param>
        /// <returns></returns>
        public StringBuilder Remove(
            StringSpan value, IEqualityComparer<char> comparer)
            => Remove(source, value, (x, y) => x.Equals(y, comparer));

        /// <summary>
        /// Removes from this instance the first ocurrence of the given value.
        /// Returns a reference to this source instance to enable fluent syntax chaining.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="comparer"></param>
        /// <returns></returns>
        public StringBuilder Remove(
            StringSpan value, IEqualityComparer<string> comparer)
            => Remove(source, value, (x, y) => x.Equals(y, comparer));

        /// <summary>
        /// Removes from this instance the first ocurrence of the given value.
        /// Returns a reference to this source instance to enable fluent syntax chaining.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="comparison"></param>
        /// <returns></returns>
        public StringBuilder Remove(
            StringSpan value, StringComparison comparison)
            => Remove(source, value, (x, y) => x.Equals(y, comparison));

        // ----------------------------------------------------

        StringBuilder RemoveLast(StringSpan value, Func<char, char, bool> predicate)
        {
            var index = LastIndexOf(source, value, predicate);
            if (index >= 0) source = source.Remove(index, value.Length);
            return source;
        }

        /// <summary>
        /// Removes from this instance the last ocurrence of the given value.
        /// Returns a reference to this source instance to enable fluent syntax chaining.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public StringBuilder RemoveLast(
            StringSpan value) => RemoveLast(source, value, static (x, y) => x == y);

        /// <summary>
        /// Removes from this instance the last ocurrence of the given value.
        /// Returns a reference to this source instance to enable fluent syntax chaining.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="ignoreCase"></param>
        /// <returns></returns>
        public StringBuilder RemoveLast(
            StringSpan value, bool ignoreCase)
            => RemoveLast(source, value, (x, y) => x.Equals(y, ignoreCase));

        /// <summary>
        /// Removes from this instance the last ocurrence of the given value.
        /// Returns a reference to this source instance to enable fluent syntax chaining.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="comparer"></param>
        /// <returns></returns>
        public StringBuilder RemoveLast(
            StringSpan value, IEqualityComparer<char> comparer)
            => RemoveLast(source, value, (x, y) => x.Equals(y, comparer));

        /// <summary>
        /// Removes from this instance the last ocurrence of the given value.
        /// Returns a reference to this source instance to enable fluent syntax chaining.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="comparer"></param>
        /// <returns></returns>
        public StringBuilder RemoveLast(
            StringSpan value, IEqualityComparer<string> comparer)
            => RemoveLast(source, value, (x, y) => x.Equals(y, comparer));

        /// <summary>
        /// Removes from this instance the last ocurrence of the given value.
        /// Returns a reference to this source instance to enable fluent syntax chaining.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="comparison"></param>
        /// <returns></returns>
        public StringBuilder RemoveLast(
            StringSpan value, StringComparison comparison)
            => RemoveLast(source, value, (x, y) => x.Equals(y, comparison));

        // ----------------------------------------------------

        public StringBuilder RemoveAll(StringSpan value, Func<char, char, bool> predicate)
        {
            var indexes = IndexesOf(source, value, predicate);
            indexes.Reverse();

            foreach (var index in indexes) source.Remove(index, value.Length);
            return source;
        }

        /// <summary>
        /// Remove from this instance all the ocurrences of the given value.
        /// Returns a reference to this source instance to enable fluent syntax chaining.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public StringBuilder RemoveAll(
            StringSpan value) => RemoveAll(source, value, static (x, y) => x == y);

        /// <summary>
        /// Remove from this instance all the ocurrences of the given value.
        /// Returns a reference to this source instance to enable fluent syntax chaining.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="ignoreCase"></param>
        /// <returns></returns>
        public StringBuilder RemoveAll(
            StringSpan value, bool ignoreCase)
            => RemoveAll(source, value, (x, y) => x.Equals(y, ignoreCase));

        /// <summary>
        /// Remove from this instance all the ocurrences of the given value.
        /// Returns a reference to this source instance to enable fluent syntax chaining.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="comparer"></param>
        /// <returns></returns>
        public StringBuilder RemoveAll(
            StringSpan value, IEqualityComparer<char> comparer)
            => RemoveAll(source, value, (x, y) => x.Equals(y, comparer));

        /// <summary>
        /// Remove from this instance all the ocurrences of the given value.
        /// Returns a reference to this source instance to enable fluent syntax chaining.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="comparer"></param>
        /// <returns></returns>
        public StringBuilder RemoveAll(
            StringSpan value, IEqualityComparer<string> comparer)
            => RemoveAll(source, value, (x, y) => x.Equals(y, comparer));

        /// <summary>
        /// Remove from this instance all the ocurrences of the given value.
        /// Returns a reference to this source instance to enable fluent syntax chaining.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="comparison"></param>
        /// <returns></returns>
        public StringBuilder RemoveAll(
            StringSpan value, StringComparison comparison)
            => RemoveAll(source, value, (x, y) => x.Equals(y, comparison));
    }
}