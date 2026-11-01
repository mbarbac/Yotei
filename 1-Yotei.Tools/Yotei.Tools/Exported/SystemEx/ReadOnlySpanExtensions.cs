#if YOTEI_TOOLS_GENERATORS
using System.Reflection.Metadata.Ecma335;

namespace Yotei.Tools.Generators;
#else
namespace Yotei.Tools;
#endif

// ========================================================
#if YOTEI_TOOLS_GENERATORS
internal
#else
public
#endif
static class ReadOnlySpanExtensions
{
    extension<T>(ReadOnlySpan<T> source)
    {
        /// <summary>
        /// Determines if this span contains a value that matches the given predicate.
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public bool Contains(Predicate<T> predicate) => source.IndexOf(predicate) >= 0;

        // ------------------------------------------------

        /// <summary>
        /// Searches for the index of the first element that matches the given predicate.
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public int IndexOf(Predicate<T> predicate)
        {
            ArgumentNullException.ThrowIfNull(predicate);

            for (int i = 0; i < source.Length; i++) if (predicate(source[i])) return i;
            return -1;
        }

        // ------------------------------------------------

        /// <summary>
        /// Searches for the index of the last element that matches the given predicate.
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public int LastIndexOf(Predicate<T> predicate)
        {
            ArgumentNullException.ThrowIfNull(predicate);

            for (int i = source.Length - 1; i >= 0; i--) if (predicate(source[i])) return i;
            return -1;
        }

        // ------------------------------------------------

        /// <summary>
        /// Searches for the indexes of the ocurrences of the given value.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public List<int> IndexesOf(T value) => source.IndexesOf(value, EqualityComparer<T>.Default);

        /// <summary>
        /// Searches for the indexes of the ocurrences of the given value, using the given comparer.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="comparer"></param>
        /// <returns></returns>
        public List<int> IndexesOf(T value, IEqualityComparer<T> comparer)
        {
            ArgumentNullException.ThrowIfNull(comparer);
            return source.IndexesOf(x => comparer.Equals(x, value));
        }

        /// <summary>
        /// Searches for the indexes of the elements that match the given predicate.
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public List<int> IndexesOf(Predicate<T> predicate)
        {
            ArgumentNullException.ThrowIfNull(predicate);

            List<int> nums = [];
            for (int i = 0; i < source.Length; i++) if (predicate(source[i])) nums.Add(i);
            return nums;
        }

        // ------------------------------------------------

        /// <summary>
        /// Searches for the index of the first ocurrence of a value that is not the given one.
        /// Returns -1 if any or empty.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public int FirstCropIndex(T value) => source.FirstCropIndex(value, EqualityComparer<T>.Default);

        /// <summary>
        /// Searches for the index of the first ocurrence of a value that is not the given one,
        /// using the given comparer. Returns -1 if any or empty.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="comparer"></param>
        /// <returns></returns>
        public int FirstCropIndex(T value, IEqualityComparer<T> comparer)
        {
            ArgumentNullException.ThrowIfNull(comparer);
            return source.FirstCropIndex(x => comparer.Equals(x, value));
        }

        /// <summary>
        /// Searches for the index of the first ocurrence of an element that does not match the
        /// given predicate. Returns -1 if any or empty.
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public int FirstCropIndex(Predicate<T> predicate)
        {
            ArgumentNullException.ThrowIfNull(predicate);

            for (int i = 0; i < source.Length; i++) if (!predicate(source[i])) return i;
            return -1;
        }

        // ------------------------------------------------

        /// <summary>
        /// Searches for the index of the last ocurrence of a value that is not the given one.
        /// Returns -1 if any or empty.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public int LastCropIndex(T value) => source.LastCropIndex(value, EqualityComparer<T>.Default);

        /// <summary>
        /// Searches for the index of the last ocurrence of a value that is not the given one,
        /// using the given comparer. Returns -1 if any or empty.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="comparer"></param>
        /// <returns></returns>
        public int LastCropIndex(T value, IEqualityComparer<T> comparer)
        {
            ArgumentNullException.ThrowIfNull(comparer);
            return source.LastCropIndex(x => comparer.Equals(x, value));
        }

        /// <summary>
        /// Searches for the index of the last ocurrence of an element that does not match the
        /// given predicate. Returns -1 if any or empty.
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public int LastCropIndex(Predicate<T> predicate)
        {
            ArgumentNullException.ThrowIfNull(predicate);

            for (int i = source.Length - 1; i >= 0; i--) if (!predicate(source[i])) return i;
            return -1;
        }

        // ------------------------------------------------

        /// <summary>
        /// Removes all leading ocurrences of the default value.
        /// </summary>
        /// <returns></returns>
        public ReadOnlySpan<T> TrimStart() => source.TrimStart(default!, EqualityComparer<T>.Default);

        /// <summary>
        /// Removes all leading ocurrences of the given value, using the given predicate.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="comparer"></param>
        /// <returns></returns>
        public ReadOnlySpan<T> TrimStart(T value, IEqualityComparer<T> comparer)
        {
            ArgumentNullException.ThrowIfNull(comparer);
            return source.TrimStart(x => comparer.Equals(x, value));
        }

        /// <summary>
        /// Removes all leading ocurrences of values that match the given predicate.
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public ReadOnlySpan<T> TrimStart(Predicate<T> predicate)
        {
            ArgumentNullException.ThrowIfNull(predicate);

            var span = source; while (true)
            {
                if (span.Length == 0) break;
                if (predicate(span[0])) span = span[1..];
                else break;
            }
            return span;
        }

        // ------------------------------------------------

        /// <summary>
        /// Removes all trailing ocurrences of the default value.
        /// </summary>
        /// <returns></returns>
        public ReadOnlySpan<T> TrimEnd() => source.TrimEnd(default!, EqualityComparer<T>.Default);

        /// <summary>
        /// Removes all trailing ocurrences of the given value, using the given predicate.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="comparer"></param>
        /// <returns></returns>
        public ReadOnlySpan<T> TrimEnd(T value, IEqualityComparer<T> comparer)
        {
            ArgumentNullException.ThrowIfNull(comparer);
            return source.TrimEnd(x => comparer.Equals(x, value));
        }

        /// <summary>
        /// Removes all trailing ocurrences of values that match the given predicate.
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public ReadOnlySpan<T> TrimEnd(Predicate<T> predicate)
        {
            ArgumentNullException.ThrowIfNull(predicate);

            var span = source; while (true)
            {
                if (span.Length == 0) break;
                if (predicate(span[^1])) span = span[..^1];
                else break;
            }
            return span;
        }

        // ------------------------------------------------

        /// <summary>
        /// Removes all leading and trailing ocurrences of the default value.
        /// </summary>
        /// <returns></returns>
        public ReadOnlySpan<T> Trim() => source.Trim(default!, EqualityComparer<T>.Default);

        /// <summary>
        /// Removes all leading and trailing ocurrences of the given value, using the given comparer.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="comparer"></param>
        /// <returns></returns>
        public ReadOnlySpan<T> Trim(T value, IEqualityComparer<T> comparer)
        {
            ArgumentNullException.ThrowIfNull(comparer);
            return source.Trim(x => comparer.Equals(x, value));
        }

        /// <summary>
        /// Removes all leading and trailing ocurrences of values that match the given predicate.
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public ReadOnlySpan<T> Trim(
            Predicate<T> predicate) => source.TrimStart(predicate).TrimEnd(predicate);

        // ------------------------------------------------

        /// <summary>
        /// Determines if the span starts with a value that matches the given predicate.
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public bool StartsWith(Predicate<T> predicate)
            => source.Length > 0
            && source.IndexOf(predicate) == 0;

        // ------------------------------------------------

        /// <summary>
        /// Determines if the span ends with a value that matches the given predicate.
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public bool EndsWith(Predicate<T> predicate)
            => source.Length > 0
            && source.IndexOf(predicate) == source.Length - 1;
    }
}