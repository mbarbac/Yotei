#if NETSTANDARD2_0
#pragma warning disable IDE0079, CS0436
#endif

namespace Yotei.Tools;

// ========================================================
#if NETSTANDARD2_0
internal
#else
public
#endif
static class ArrayExtensions
{
    extension<T>(T[] source)
    {
        /// <summary>
        /// Determines if this array is an empty one.
        /// </summary>
        public bool IsEmpty => source.Length == 0;

        // ------------------------------------------------

        /// <summary>
        /// Returns a typed enumerator that iterates through the elements in the array.
        /// </summary>
        /// <returns></returns>
        public IEnumerator<T> GetTypedEnumerator() => ((IEnumerable<T>)source).GetEnumerator();

        // ------------------------------------------------

        /// <summary>
        /// Returns a clone of this instance where its elements are either the original ones,
        /// or their own clones, as requested.
        /// </summary>
        /// <param name="deep"></param>
        /// <returns></returns>
        public T[] Clone(bool deep)
        {
            // Deep clone requested...
            if (deep)
            {
                var len = source.Length;
                var target = new T[len];

                for (int i = 0; i < len; i++) target[i] = source[i].TryClone()!;
                return target;
            }

            // Standard case...
            return (T[])source.Clone();
        }

        // ------------------------------------------------

        /// <summary>
        /// Returns a new array where the leading ocurrences of the given element have been
        /// removed.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public T[] TrimStart(T value)
        {
            var span = source.AsSpan();
            span = span.TrimStart(value);
            return span.Length == source.Length ? (T[])source.Clone() : span.ToArray();
        }

        /// <summary>
        /// Returns a new array where the leading ocurrences of the given element, using the
        /// given elements' comparer, have been removed.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="comparer"></param>
        /// <returns></returns>
        public T[] TrimStart(T value, IEqualityComparer<T> comparer)
        {
            var span = source.AsSpan();
            span = span.TrimStart(value, comparer);
            return span.Length == source.Length ? (T[])source.Clone() : span.ToArray();
        }

        /// <summary>
        /// Returns a new array where all leading elements that match the given predicate have
        /// been removed.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public T[] TrimStart(Predicate<T> predicate)
        {
            var span = source.AsSpan();
            span = span.TrimStart(predicate);
            return span.Length == source.Length ? (T[])source.Clone() : span.ToArray();
        }

        // ------------------------------------------------

        /// <summary>
        /// Returns a new array where the trailing ocurrences of the given element have been
        /// removed.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public T[] TrimEnd(T value)
        {
            var span = source.AsSpan();
            span = span.TrimEnd(value);
            return span.Length == source.Length ? (T[])source.Clone() : span.ToArray();
        }

        /// <summary>
        /// Returns a new array where the trailing ocurrences of the given element, using the
        /// given elements' comparer, have been removed.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="comparer"></param>
        /// <returns></returns>
        public T[] TrimEnd(T value, IEqualityComparer<T> comparer)
        {
            var span = source.AsSpan();
            span = span.TrimEnd(value, comparer);
            return span.Length == source.Length ? (T[])source.Clone() : span.ToArray();
        }

        /// <summary>
        /// Returns a new array where all trailing elements that match the given predicate have
        /// been removed.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public T[] TrimEnd(Predicate<T> predicate)
        {
            var span = source.AsSpan();
            span = span.TrimEnd(predicate);
            return span.Length == source.Length ? (T[])source.Clone() : span.ToArray();
        }

        // ------------------------------------------------

        /// <summary>
        /// Returns a new array where the leading and trailing ocurrences of the given element
        /// have been removed.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public T[] Trim(T value)
        {
            var span = source.AsSpan();
            span = span.Trim(value);
            return span.Length == source.Length ? (T[])source.Clone() : span.ToArray();
        }

        /// <summary>
        /// Returns a new array where the leading and trailing ocurrences of the given element,
        /// using the given elements' comparer, have been removed.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="comparer"></param>
        /// <returns></returns>
        public T[] Trim(T value, IEqualityComparer<T> comparer)
        {
            var span = source.AsSpan();
            span = span.Trim(value, comparer);
            return span.Length == source.Length ? (T[])source.Clone() : span.ToArray();
        }

        /// <summary>
        /// Returns a new array where all leading and trailing elements that match the given
        /// predicate have been removed.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public T[] Trim(Predicate<T> predicate)
        {
            var span = source.AsSpan();
            span = span.Trim(predicate);
            return span.Length == source.Length ? (T[])source.Clone() : span.ToArray();
        }

        // ------------------------------------------------

        /// <summary>
        /// Returns a new array with the requested length where that has been adjusted by either
        /// removing leading elements, or adding the needed ones using the given pad value.
        /// </summary>
        /// <param name="len"></param>
        /// <param name="pad"></param>
        /// <returns></returns>
        public T[] ResizeHead(int len, T pad = default!)
        {
            if (len < 0) throw new ArgumentOutOfRangeException(
                nameof(len),
                "Lenght cannot be negative.").WithData(len);

            if (len == 0) return [];
            if (len < source.Length) return source.AsSpan(source.Length - len).ToArray();

            var target = new T[len];
            target.AsSpan().Fill(pad);
            Array.Copy(source, 0, target, len - source.Length, source.Length);
            return target;
        }

        /// <summary>
        /// Returns a new array with the requested length where that has been adjusted by either
        /// removing trailing elements, or adding the needed ones using the given pad value.
        /// </summary>
        /// <param name="len"></param>
        /// <param name="pad"></param>
        /// <returns></returns>
        public T[] ResizeTail(int len, T pad = default!)
        {
            if (len < 0) throw new ArgumentOutOfRangeException(
                nameof(len),
                "Lenght cannot be negative.").WithData(len);

            if (len == 0) return [];
            if (len < source.Length) return source.AsSpan(0, len).ToArray();

            var target = new T[len];
            target.AsSpan().Fill(pad);
            Array.Copy(source, 0, target, 0, source.Length);
            return target;
        }

        // ------------------------------------------------

        /// <summary>
        /// Returns a new array where the given value has been added to it.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public T[] Add(T value)
        {
            var target = new T[source.Length + 1];
            Array.Copy(source, target, source.Length);
            target[source.Length] = value;
            return target;
        }

        /// <summary>
        /// Returns a new array where the values of the given range have been added to it.
        /// </summary>
        /// <param name="range"></param>
        /// <returns></returns>
        public T[] AddRange(IEnumerable<T> range)
        {
            range.ThrowWhenNull();

            IList<T> values = range switch
            {
                T[] temp => temp,
                IList<T> temp => temp,
                _ => [.. range]
            };
            if (values.Count == 0) return (T[])source.Clone();

            var target = new T[source.Length + values.Count];
            source.AsSpan().CopyTo(target);
            for (int i = 0; i < values.Count; i++) target[source.Length + i] = values[i];
            return target;
        }

        /// <summary>
        /// Returns a new array where the given value has been inserted into it, at the given
        /// index.
        /// </summary>
        /// <param name="index"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        [SuppressMessage("", "CA1512")]
        public T[] Insert(int index, T value)
        {
            if (index < 0) throw new ArgumentOutOfRangeException(
                nameof(index),
                "Index cannot be negative.").WithData(index);

            if (index > source.Length) throw new ArgumentOutOfRangeException(
                nameof(index),
                "Index bigger than length.").WithData(index);

            var target = new T[source.Length + 1];
            Array.Copy(source, target, index);
            target[index] = value;
            Array.Copy(source, index, target, index + 1, source.Length - index);
            return target;
        }

        /// <summary>
        /// Returns a new array where the values of the given range have been inserted into it,
        /// starting at the given index.
        /// </summary>
        /// <param name="index"></param>
        /// <param name="range"></param>
        /// <returns></returns>
        public T[] InsertRange(int index, IEnumerable<T> range)
        {
            range.ThrowWhenNull();

            if (index < 0) throw new ArgumentOutOfRangeException(
                nameof(index),
                "Index cannot be negative.").WithData(index);

            if (index > source.Length) throw new ArgumentOutOfRangeException(
                nameof(index),
                "Index bigger than length.").WithData(index);

            IList<T> values = range switch
            {
                T[] temp => temp,
                IList<T> temp => temp,
                _ => [.. range]
            };
            if (values.Count == 0) return (T[])source.Clone();

            var target = new T[source.Length + values.Count];
            Array.Copy(source, target, index);
            for (int i = 0; i < values.Count; i++) target[index + i] = values[i];
            var dest = index + values.Count;
            var len = source.Length - index;
            Array.Copy(source, index, target, dest, len);
            return target;
        }

        // ------------------------------------------------

        /// <summary>
        /// Returns a new array where the element at the given index has been removed.
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public T[] RemoveAt(int index)
        {
            if (index < 0) throw new ArgumentOutOfRangeException(
                nameof(index),
                "Index cannot be negative.").WithData(index);

            if (index > source.Length) throw new ArgumentOutOfRangeException(
                nameof(index),
                "Index bigger than length.").WithData(index);

            if (source.Length == 1) return [];

            var target = new T[source.Length - 1];
            Array.Copy(source, target, index);
            Array.Copy(source, index + 1, target, index, source.Length - index - 1);
            return target;
        }

        /// <summary>
        /// Returns a new array where the given number of elements, starting at the given index,
        /// have been removed.
        /// </summary>
        /// <param name="index"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public T[] RemoveRange(int index, int count)
        {
            if (index < 0) throw new ArgumentOutOfRangeException(
                nameof(index),
                "Index cannot be negative.").WithData(index);

            if (count < 0) throw new ArgumentOutOfRangeException(
                nameof(count),
                "Count cannot be negative.").WithData(count);

            // Special case 'RemoveRange(0,0)' when source is empty...
            if (index == 0 && count == 0) return (T[])source.Clone();

            if (index > source.Length) throw new ArgumentOutOfRangeException(
               nameof(index),
               "Index bigger than length.").WithData(index);

            if ((index + count) > source.Length) throw new ArgumentOutOfRangeException(
               nameof(index),
               "Invalid arguments than length.").WithData(index).WithData(count);

            if (count == 0) return (T[])source.Clone();
            if (index == 0 && count == source.Length) return [];

            var target = new T[source.Length - count];
            Array.Copy(source, target, index);
            Array.Copy(source, index + count, target, index, source.Length - index - count);
            return target;
        }

        /// <summary>
        /// Returns a new array where the first ocurrence of the given element has been removed.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public T[] Remove(T value) => throw null;

        /// <summary>
        /// Returns a new array where the first ocurrence of the given element, using the given
        /// comparer, has been removed.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="comparer"></param>
        /// <returns></returns>
        public T[] Remove(T value, IEqualityComparer<T> comparer) => throw null;

        /// <summary>
        /// Returns a new array where the last ocurrence of the given element has been removed.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public T[] RemoveLast(T value) => throw null;

        /// <summary>
        /// Returns a new array where the last ocurrence of the given element, using the given
        /// comparer, has been removed.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="comparer"></param>
        /// <returns></returns>
        public T[] RemoveLast(T value, IEqualityComparer<T> comparer) => throw null;

        /// <summary>
        /// Returns a new array where all ocurrences of the given element have been removed.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public T[] RemoveAll(T value) => throw null;

        /// <summary>
        /// Returns a new array where all ocurrences of the given element, using the given
        /// comparer, have been removed.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="comparer"></param>
        /// <returns></returns>
        public T[] RemoveAll(T value, IEqualityComparer<T> comparer) => throw null;

        /// <summary>
        /// Returns a new array where the first element that matches the given predicate has been
        /// removed.
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public T[] Remove(Predicate<T> predicate) => throw null;

        /// <summary>
        /// Returns a new array where the last element that matches the given predicate has been
        /// removed.
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public T[] RemoveLast(Predicate<T> predicate) => throw null;

        /// <summary>
        /// Returns a new array where all elements that match the given predicate have been
        /// removed.
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public T[] RemoveAll(Predicate<T> predicate) => throw null;

        /// <summary>
        /// Returns a new instance where all its elements have been cleared by setting them to
        /// the given one.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public T[] Clear(T value = default!)
        {
            var len = source.Length;
            if (len == 0) return [];

            var target = new T[len]; for (int i = 0; i < len; i++) target[i] = value;
            return target;
        }
    }
}