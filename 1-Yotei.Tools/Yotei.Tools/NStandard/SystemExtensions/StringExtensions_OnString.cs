#if NETSTANDARD2_0
#pragma warning disable IDE0079
#pragma warning disable CS0436
#endif

using System.Buffers;
using StringSpan = System.ReadOnlySpan<char>;
namespace Yotei.Tools;

// ========================================================
#if NETSTANDARD2_0
internal
#else
public
#endif
static partial class StringExtensions
{
    extension(string source)
    {
        /// <summary>
        /// Removes the first ocurrence of the given value.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public string Remove(StringSpan value)
        {
            if (source.IsEmpty) return source;
            if (value.IsEmpty) return source;
            if (value.Length > source.Length) return source;

            var index = source.IndexOf(value);
            return index < 0
                ? source
                : string.Concat(source[..index], source[(index + value.Length)..]);
        }

        /// <summary>
        /// Removes the first ocurrence of the given value.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="ignoreCase"></param>
        /// <returns></returns>
        public string Remove(StringSpan value, bool ignoreCase)
        {
            if (source.IsEmpty) return source;
            if (value.IsEmpty) return source;
            if (value.Length > source.Length) return source;

            var index = source.IndexOf(value, ignoreCase);
            return index < 0
                ? source
                : string.Concat(source[..index], source[(index + value.Length)..]);
        }

        /// <summary>
        /// Removes the first ocurrence of the given value.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="comparer"></param>
        /// <returns></returns>
        public string Remove(StringSpan value, IEqualityComparer<char> comparer)
        {
            if (source.IsEmpty) return source;
            if (value.IsEmpty) return source;
            if (value.Length > source.Length) return source;

            var index = source.IndexOf(value, comparer);
            return index < 0
                ? source
                : string.Concat(source[..index], source[(index + value.Length)..]);
        }

        /// <summary>
        /// Removes the first ocurrence of the given value.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="comparer"></param>
        /// <returns></returns>
        public string Remove(StringSpan value, IEqualityComparer<string> comparer)
        {
            if (source.IsEmpty) return source;
            if (value.IsEmpty) return source;
            if (value.Length > source.Length) return source;

            var index = source.IndexOf(value, comparer);
            return index < 0
                ? source
                : string.Concat(source[..index], source[(index + value.Length)..]);
        }

        /// <summary>
        /// Removes the first ocurrence of the given value.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="comparison"></param>
        /// <returns></returns>
        public string Remove(StringSpan value, StringComparison comparison)
        {
            if (source.IsEmpty) return source;
            if (value.IsEmpty) return source;
            if (value.Length > source.Length) return source;

            var index = source.IndexOf(value, comparison);
            return index < 0
                ? source
                : string.Concat(source[..index], source[(index + value.Length)..]);
        }

        // ------------------------------------------------

        /// <summary>
        /// Removes the last ocurrence of the given value.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public string RemoveLast(StringSpan value)
        {
            if (source.IsEmpty) return source;
            if (value.IsEmpty) return source;
            if (value.Length > source.Length) return source;

            var index = source.LastIndexOf(value);
            return index < 0
                ? source
                : string.Concat(source[..index], source[(index + value.Length)..]);
        }

        /// <summary>
        /// Removes the last ocurrence of the given value.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="ignoreCase"></param>
        /// <returns></returns>
        public string RemoveLast(StringSpan value, bool ignoreCase)
        {
            if (source.IsEmpty) return source;
            if (value.IsEmpty) return source;
            if (value.Length > source.Length) return source;

            var index = source.LastIndexOf(value, ignoreCase);
            return index < 0
                ? source
                : string.Concat(source[..index], source[(index + value.Length)..]);
        }

        /// <summary>
        /// Removes the last ocurrence of the given value.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="comparer"></param>
        /// <returns></returns>
        public string RemoveLast(StringSpan value, IEqualityComparer<char> comparer)
        {
            if (source.IsEmpty) return source;
            if (value.IsEmpty) return source;
            if (value.Length > source.Length) return source;

            var index = source.LastIndexOf(value, comparer);
            return index < 0
                ? source
                : string.Concat(source[..index], source[(index + value.Length)..]);
        }

        /// <summary>
        /// Removes the last ocurrence of the given value.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="comparer"></param>
        /// <returns></returns>
        public string RemoveLast(StringSpan value, IEqualityComparer<string> comparer)
        {
            if (source.IsEmpty) return source;
            if (value.IsEmpty) return source;
            if (value.Length > source.Length) return source;

            var index = source.LastIndexOf(value, comparer);
            return index < 0
                ? source
                : string.Concat(source[..index], source[(index + value.Length)..]);
        }

        /// <summary>
        /// Removes the last ocurrence of the given value.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="comparison"></param>
        /// <returns></returns>
        public string RemoveLast(StringSpan value, StringComparison comparison)
        {
            if (source.IsEmpty) return source;
            if (value.IsEmpty) return source;
            if (value.Length > source.Length) return source;

            var index = source.LastIndexOf(value, comparison);
            return index < 0
                ? source
                : string.Concat(source[..index], source[(index + value.Length)..]);
        }

        // ------------------------------------------------

        public string RemoveAll(StringSpan value, bool ignoreCase)
        {
            if (source.IsEmpty) return source;
            if (value.IsEmpty) return source;
            if (value.Length > source.Length) return source;

            var len = source.Length + 1;
            var span = len <= 256 ? stackalloc char[len] : ArrayPool<char>.Shared.Rent(len);
            source.AsSpan().CopyTo(span);

            int write = 0;
            int read = 0; while (read < span.Length)
            {
                int match = span[read..].IndexOf(value, ignoreCase);

                if (match < 0) // Remaining...
                {
                    int remaining = span.Length - read;
                    if (write != read)
                        span.Slice(read, remaining).CopyTo(span[write..]);

                    write += remaining;
                    break;
                }

                if (match > 0) // Copy chars before the match...
                {
                    if (write != read)
                        span.Slice(read, match).CopyTo(span[write..]);

                    write += match;
                }

                read += match + value.Length; // Adjust read position past matched value...
            }

            var temp = span[..write];
            return temp.ToString();
        }
    }
}