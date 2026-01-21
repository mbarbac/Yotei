namespace Yotei.Tools;

// NOTE: Not using use an extension block because there are some problems when a generic 'T[]'
// array is the element being extended. In particular 'CS9293: Cannot use an extension parameter
// in this context'.

// ========================================================
public static class ArrayExtensions
{
    /// <summary>
    /// Returns a new array whose elements are either clones of the original ones, if recursive
    /// cloning is requested and possible, or the original ones otherwise.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="source"></param>
    /// <param name="recursive"></param>
    /// <returns></returns>
    public static T[] Clone<T>(this T[] source, bool recursive)
    {
        ArgumentNullException.ThrowIfNull(source);

        if (recursive)
        {
            if (source.Length == 0) return [];

            var len = source.Length;
            var target = new T[len];
            for (int i = 0; i < len; i++) target[i] = source[i].TryClone()!;
            return target;
        }
        else
        {
            return (T[])source.Clone();
        }
    }

    // ----------------------------------------------------

    /// <summary>
    /// Determines if the array contains the given element, using the default equality comparer
    /// of ots type.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    public static bool Contains<T>(this T[] source, T value) => source.IndexOf(value) >= 0;

    /// <summary>
    /// Determines if the array contains at least one element that matches the given predicate.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="source"></param>
    /// <param name="predicate"></param>
    /// <returns></returns>
    public static bool Contains<T>(
        this T[] source, Predicate<T> predicate) => source.IndexOf(predicate) >= 0;

    // ----------------------------------------------------

    /// <summary>
    /// Returns the cero-based index of the first ocurrence of the given value, using the default
    /// equality comparer of its type, or -1 if any.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    public static int IndexOf<T>(this T[] source, T value)
    {
        var comparer = EqualityComparer<T>.Default;
        return source.IndexOf(x => comparer.Equals(x, value));
    }

    /// <summary>
    /// Returns the cero-based index of the first ocurrence of the given value, starting at the
    /// given index, using the default equality comparer of its type, or -1 if any.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="source"></param>
    /// <param name="start"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    public static int IndexOf<T>(this T[] source, int start, T value)
    {
        var comparer = EqualityComparer<T>.Default;
        return source.IndexOf(start, x => comparer.Equals(x, value));
    }

    /// <summary>
    /// Returns the cero-based index of the first ocurrence of an element in the array that matches
    /// the given predicate, or -1 if any,
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="source"></param>
    /// <param name="predicate"></param>
    /// <returns></returns>
    public static int IndexOf<T>(
        this T[] source, Predicate<T> predicate) => source.IndexOf(0, predicate);

    /// <summary>
    /// Returns the cero-based index of the first ocurrence of an element in the array that matches
    /// the given predicate, starting from the given position, or -1 if any,
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="source"></param>
    /// <param name="start"></param>
    /// <param name="predicate"></param>
    /// <returns></returns>
    public static int IndexOf<T>(this T[] source, int start, Predicate<T> predicate)
    {
        ArgumentNullException.ThrowIfNull(source);
        ArgumentNullException.ThrowIfNull(predicate);
        ArgumentOutOfRangeException.ThrowIfLessThan(start, 0);
        ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual(start, source.Length);

        for (int i = start; i < source.Length; i++) if (predicate(source[i])) return i;
        return -1;
    }

    // ----------------------------------------------------

    /// <summary>
    /// Returns the cero-based index of the last ocurrence of the given value, using the default
    /// equality comparer of its type, or -1 if any.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    public static int LastIndexOf<T>(this T[] source, T value)
    {
        var comparer = EqualityComparer<T>.Default;
        return source.LastIndexOf(x => comparer.Equals(x, value));
    }

    /// <summary>
    /// Returns the cero-based index of the last ocurrence of the given value, starting at the
    /// given index, using the default equality comparer of its type, or -1 if any.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="source"></param>
    /// <param name="start"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    public static int LastIndexOf<T>(this T[] source, int start, T value)
    {
        var comparer = EqualityComparer<T>.Default;
        return source.LastIndexOf(start, x => comparer.Equals(x, value));
    }

    /// <summary>
    /// Returns the cero-based index of the last ocurrence of an element in the array that matches
    /// the given predicate, or -1 if any,
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="source"></param>
    /// <param name="predicate"></param>
    /// <returns></returns>
    public static int LastIndexOf<T>(
        this T[] source, Predicate<T> predicate) => source.LastIndexOf(0, predicate);

    /// <summary>
    /// Returns the cero-based index of the last ocurrence of an element in the array that matches
    /// the given predicate, starting from the given position, or -1 if any,
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="source"></param>
    /// <param name="start"></param>
    /// <param name="predicate"></param>
    /// <returns></returns>
    public static int LastIndexOf<T>(this T[] source, int start, Predicate<T> predicate)
    {
        ArgumentNullException.ThrowIfNull(source);
        ArgumentNullException.ThrowIfNull(predicate);
        ArgumentOutOfRangeException.ThrowIfLessThan(start, 0);
        ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual(start, source.Length);

        for (int i = source.Length - 1; i >= start; i--) if (predicate(source[i])) return i;
        return -1;
    }

    // ----------------------------------------------------

    /// <summary>
    /// Searches the array for the ocurrences of the given value, using the default equality
    /// comparer of its type, and return a list with their cero-based indexes.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    public static List<int> IndexesOf<T>(this T[] source, T value)
    {
        var comparer = EqualityComparer<T>.Default;
        return source.IndexesOf(x => comparer.Equals(x, value));
    }

    /// <summary>
    /// Searches the array for the ocurrences of the given value, starting at the given index,
    /// using the default equality comparer of its type, and return a list with their cero-based
    /// indexes.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="start"></param>
    /// <returns></returns>
    public static List<int> IndexesOf<T>(this T[] source, int start, T value)
    {
        var comparer = EqualityComparer<T>.Default;
        return source.IndexesOf(start, x => comparer.Equals(x, value));
    }

    /// <summary>
    /// Searches the array from the beginning for elements of the matrix that satisfy the given
    /// predicate, and returns a list with their cero-based indexes.
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    public static List<int> IndexesOf<T>(
        this T[] source, Predicate<T> predicate) => source.IndexesOf(0, predicate);

    /// <summary>
    /// Searches the array from the given index for elements of the matrix that satisfy the given
    /// predicate, and returns a list with their cero-based indexes.
    /// </summary>
    /// <param name="start"></param>
    /// <param name="predicate"></param>
    /// <returns></returns>
    public static List<int> IndexesOf<T>(
        this T[] source, int start, Predicate<T> predicate)
    {
        ArgumentNullException.ThrowIfNull(source);
        ArgumentNullException.ThrowIfNull(predicate);
        ArgumentOutOfRangeException.ThrowIfLessThan(start, 0);
        ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual(start, source.Length);

        List<int> list = [];
        for (int i = start; i < source.Length; i++) if (predicate(source[i])) list.Add(i);
        return list;
    }

    // ----------------------------------------------------

    /// <summary>
    /// Removes from the array all leading and trailing ocurrences of elements that match the given
    /// predicate. Returns either a new modified array, or the original one if no changes were made.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="source"></param>
    /// <param name="predicate"></param>
    /// <returns></returns>
    public static T[] Trim<T>(
        this T[] source, Predicate<T> predicate) => source.TrimStart(predicate).TrimEnd(predicate);

    /// <summary>
    /// Removes from the array all leading ocurrences of elements that match the given predicate.
    /// Returns either a new modified array, or the original one if no changes were made.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="source"></param>
    /// <param name="predicate"></param>
    /// <returns></returns>
    public static T[] TrimStart<T>(this T[] source, Predicate<T> predicate)
    {
        ArgumentNullException.ThrowIfNull(source);
        ArgumentNullException.ThrowIfNull(predicate);

        var index = -1;
        for (int i = 0; i < source.Length; i++)
        {
            if (predicate(source[i])) index = i;
            else break;
        }
        if (index >= 0)
        {
            var num = source.Length - index - 1;
            var target = new T[num];
            Array.Copy(source, index + 1, target, 0, num);
            return target;
        }
        return source;
    }

    /// <summary>
    /// Removes from the array all trailing ocurrences of elements that match the given predicate.
    /// Returns either a new modified array, or the original one if no changes were made.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="source"></param>
    /// <param name="predicate"></param>
    /// <returns></returns>
    public static T[] TrimEnd<T>(this T[] source, Predicate<T> predicate)
    {
        ArgumentNullException.ThrowIfNull(source);
        ArgumentNullException.ThrowIfNull(predicate);

        var index = -1;
        for (int i = source.Length - 1; i >= 0; i--)
        {
            if (predicate(source[i])) index = i;
            else break;
        }
        if (index >= 0)
        {
            var num = index;
            var target = new T[num];
            Array.Copy(source, 0, target, 0, num);
            return target;
        }
        return source;
    }

    // ----------------------------------------------------

    /// <summary>
    /// Returns a new array with the requested length, adding or removing leading elements as
    /// needed. When adding elements, the given <paramref name="pad"/> value is used.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="source"></param>
    /// <param name="len"></param>
    /// <param name="pad"></param>
    /// <returns></returns>
    public static T[] ResizeHead<T>(this T[] source, int len, T pad = default!)
    {
        ArgumentNullException.ThrowIfNull(source);
        ArgumentOutOfRangeException.ThrowIfNegative(len);

        if (len == 0) return [];
        if (len < source.Length) return source.AsSpan(source.Length - len).ToArray();

        var target = new T[len];
        Array.Fill(target, pad);
        Array.Copy(source, 0, target, len - source.Length, source.Length);
        return target;
    }

    /// <summary>
    /// Returns a new array with the requested length, adding or removing trailing elements as
    /// needed. When adding elements, the given <paramref name="pad"/> value is used.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="source"></param>
    /// <param name="len"></param>
    /// <param name="pad"></param>
    /// <returns></returns>
    public static T[] ResizeTail<T>(this T[] source, int len, T pad = default!)
    {
        ArgumentNullException.ThrowIfNull(source);
        ArgumentOutOfRangeException.ThrowIfNegative(len);

        if (len == 0) return [];
        if (len < source.Length) return source.AsSpan(0, len).ToArray();

        var target = new T[len];
        Array.Fill(target, pad);
        Array.Copy(source, 0, target, 0, source.Length);
        return target;
    }

    // ----------------------------------------------------

    /// <summary>
    /// Returns a new array with the given element added to the original ones.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    public static T[] Add<T>(this T[] source, T value)
    {
        ArgumentNullException.ThrowIfNull(source);

        var target = new T[source.Length + 1];
        Array.Copy(source, target, source.Length);
        target[source.Length] = value;
        return target;
    }

    /// <summary>
    /// Returns a new array with the elements from the given range added to the original ones.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="source"></param>
    /// <param name="range"></param>
    /// <returns></returns>
    public static T[] AddRange<T>(this T[] source, IEnumerable<T> range)
    {
        ArgumentNullException.ThrowIfNull(source);
        ArgumentNullException.ThrowIfNull(range);

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
    /// Returns a new array with the given element inserted into the original ones at the given
    /// index.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="source"></param>
    /// <param name="index"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    public static T[] Insert<T>(this T[] source, int index, T value)
    {
        ArgumentNullException.ThrowIfNull(source);
        ArgumentOutOfRangeException.ThrowIfLessThan(index, 0);
        ArgumentOutOfRangeException.ThrowIfGreaterThan(index, source.Length);

        var target = new T[source.Length + 1];
        Array.Copy(source, target, index);
        target[index] = value;
        Array.Copy(source, index, target, index+1, source.Length - index);
        return target;
    }

    /// <summary>
    /// Returns a new array with the elements from the given range inserted into the original ones
    /// starting at the given index.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="source"></param>
    /// <param name="index"></param>
    /// <param name="range"></param>
    /// <returns></returns>
    public static T[] InsertRange<T>(this T[] source, int index, IEnumerable<T> range)
    {
        ArgumentNullException.ThrowIfNull(source);
        ArgumentNullException.ThrowIfNull(range);
        ArgumentOutOfRangeException.ThrowIfLessThan(index, 0);
        ArgumentOutOfRangeException.ThrowIfGreaterThan(index, source.Length);

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
        var len = values.Count - index;
        Array.Copy(source, index, target, dest, len);
        return target;
    }

    /// <summary>
    /// Returns a new array where the original element at the given index has been removed.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="source"></param>
    /// <param name="index"></param>
    /// <returns></returns>
    public static T[] RemoveAt<T>(this T[] source, int index) => throw null;

    /// <summary>
    /// Returns a new array where the requested number of original elements, starting at the given
    /// index, have been removed.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="source"></param>
    /// <param name="index"></param>
    /// <param name="count"></param>
    /// <returns></returns>
    public static T[] RemoveRange<T>(this T[] source, int index, int count) => throw null;

    /// <summary>
    /// Returns a new array where the first ocurrence of the given element has been removed.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    public static T[] Remove<T>(this T[] source, T value) => throw null;

    /// <summary>
    /// Returns a new array where the last ocurrence of the given element has been removed.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    public static T[] RemoveLast<T>(this T[] source, T value) => throw null;

    /// <summary>
    /// Returns a new array where all the ocurrences of the given element have been removed.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    public static T[] RemoveAll<T>(this T[] source, T value) => throw null;

    /// <summary>
    /// Returns a new array where the first element that matches the given predicate has been
    /// removed.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="source"></param>
    /// <param name="predicate"></param>
    /// <returns></returns>
    public static T[] Remove<T>(this T[] source, Predicate<T> predicate) => throw null;

    /// <summary>
    /// Returns a new array where the last element that matches the given predicate has been
    /// removed.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="source"></param>
    /// <param name="predicate"></param>
    /// <returns></returns>
    /// <exception cref="System.NullReferenceException"></exception>
    public static T[] RemoveLast<T>(this T[] source, Predicate<T> predicate) => throw null;

    /// <summary>
    /// Returns a new array where all the elements that match the given predicate have been
    /// removed.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="source"></param>
    /// <param name="predicate"></param>
    /// <returns></returns>
    public static T[] RemoveAll<T>(this T[] source, Predicate<T> predicate) => throw null;
}