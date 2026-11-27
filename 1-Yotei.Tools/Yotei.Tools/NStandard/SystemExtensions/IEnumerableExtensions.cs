#if NETSTANDARD2_0
namespace Yotei.Tools.NStd20;
#else
namespace Yotei.Tools;
#endif

using System.Buffers;

// ========================================================
public static class IEnumerableExtensions
{
    extension<T>(IEnumerable<T> source)
    {
        /// <summary>
        /// Determines if the collection is an empty one.
        /// <para>
        /// In the general case, this property may drive side effects as it tries to enumerate the
        /// collection to obtain its first element. Special cases, such as strings, are intercepted
        /// fto improve performance.
        /// </para>
        /// </summary>
        public bool IsEmpty
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                if (source is string str) return str.Length == 0;
                return !source.Any();
            }
        }

        // ------------------------------------------------

        /// <summary>
        /// Executes the given action with each element of the enumeration.
        /// </summary>
        /// <param name="action"></param>
        public void ForEach(Action<T> action)
        {
            ArgumentNullException.ThrowIfNull(action);
            foreach (var item in source) action(item);
        }

        /// <summary>
        /// Executes the given action with the elements of the enumeration that match the given
        /// predicate.
        /// </summary>
        /// <param name="predicate"></param>
        /// <param name="action"></param>
        public void ForEach(Predicate<T> predicate, Action<T> action)
        {
            ArgumentNullException.ThrowIfNull(predicate);
            ArgumentNullException.ThrowIfNull(action);

            foreach (var item in source) if (predicate(item)) action(item);
        }

        // ------------------------------------------------

        /// <summary>
        /// Returns an array based in this enumeration, if the project was compile under a DEBUG
        /// environment, or the original enumeration otherwise.
        /// </summary>
        /// <returns></returns>
#if DEBUG
        public T[] ToDebugArray() => [.. source];
#else
        IEnumerable<T> ToDebugArray() => source;
#endif

        // ------------------------------------------------

        /// <summary>
        /// Invoked to obtain a span from the given collection. The returned value can either
        /// refer to the original underlying collection, or to an ad-hoc array obtained from the
        /// default shared pool. In the later case, that array is returned in the out argument
        /// so that it can be returned to that pool after its usage. Otherwise it is set to null.
        /// <br/> Caution: this method shall be used only when the original collection (if this
        /// is what is returned) is not modified.
        /// <para>
        /// Example:
        /// <br/><c></c>ArrayPool{T}.Shared.Return(array, clear: true);</para>
        /// </summary>
        /// <param name="array"></param>
        /// <param name="sizehint"></param>
        /// <returns></returns>
        public ReadOnlySpan<T> ToSpan(out T[]? array, int sizehint = 8)
        {
            array = null; // Nothing to return to the pool unless later allocated...

            // Easiest cases...
            if (source is T[] items) return items;
            if (source is ArraySegment<T> segment)
                return new ReadOnlySpan<T>(segment.Array, segment.Offset, segment.Count);

            // List<T> case, but only available in modern versions...
            if (source is List<T> list)
            {
#if NET9_0_OR_GREATER
                return CollectionsMarshal.AsSpan(list);
#endif
            }

            // Case when we can obtain the collection's size upfront...
            if (source is ICollection<T> collection)
            {
                var count = collection.Count;
                if (count == 0) return [];

                array = ArrayPool<T>.Shared.Rent(count);
                collection.CopyTo(array, 0);
                return new ReadOnlySpan<T>(array, 0, count);
            }

            // Allocating an ad-hoc array of an arbitrary size...
            else
            {
                if (sizehint <= 0) sizehint = 8;
                array = ArrayPool<T>.Shared.Rent(sizehint);
                int index = 0;

                foreach (T item in source)
                {
                    if (index == array.Length)
                    {
                        var exp = ArrayPool<T>.Shared.Rent(array.Length * 2);
                        Array.Copy(array, 0, exp, 0, index);
                        ArrayPool<T>.Shared.Return(array, true);
                        array = exp;
                    }
                    array[index++] = item;
                }
                return new ReadOnlySpan<T>(array, 0, index);
            }
        }
    }
}