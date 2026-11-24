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
static class IEnumerableExtensions
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
            action.ThrowWhenNull();
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
            predicate.ThrowWhenNull();
            action.ThrowWhenNull();

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
        /// Invoked to obtain a span from the given collection. The returned value can either be
        /// the underlying span, or an ad-hoc buffer rented from the default shared pool to hold
        /// the contents of the collection. In the later case, if the returned buffer is not null,
        /// it can be returned to the default shared pool for reuse.
        /// </summary>
        /// <param name="array"></param>
        /// <returns></returns>
        public ReadOnlySpan<T> ToSpan(out T[]? array, int sizeHint = 8)
        {
            array = null;

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
                if (sizeHint <= 0) sizeHint = 8;
                array = ArrayPool<T>.Shared.Rent(sizeHint);
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

    // ----------------------------------------------------

    /// <summary>
    /// If the array is not a null one, then it is returned to the default shared pool for reuse.
    /// If it is null, then this method is ignored. By default, its contents are cleared before it
    /// is returned.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="array"></param>
    /// <param name="clear"></param>
    public static void TryReturnToPool<T>(this T[]? array, bool clear = true)
    {
        if (array is not null) ArrayPool<T>.Shared.Return(array, clear);
    }
}