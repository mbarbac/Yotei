namespace Yotei.Tools;

// ========================================================
public static class ArrayExtensions
{
    class ItemComparer<T> : IEqualityComparer<T>
    {
        bool IEqualityComparer<T>.Equals(T? x, T? y) => x.EqualsEx(y);
        int IEqualityComparer<T>.GetHashCode(T obj) => throw new NotImplementedException();
    }

    // ----------------------------------------------------

    /// <summary>
    /// Provides a typed enumeration over the elements of the collection.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="source"></param>
    /// <returns></returns>
    public static IEnumerable<T> AsEnumerable<T>(this T[] source)
    {
        source.ThrowWhenNull();
        foreach (var value in source) yield return value;
    }

    // ----------------------------------------------------

    /// <summary>
    /// Returns either a shallow copy of the original array, or a deep one where the elements are
    /// also clones themselves.
    /// <br/> Always returns a new instance.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="source"></param>
    /// <param name="deep"></param>
    /// <returns></returns>
    [SuppressMessage("", "IDE0300")]
    [SuppressMessage("", "CA1825")]
    public static T[] Clone<T>(this T[] source, bool deep)
    {
        source.ThrowWhenNull();

        if (deep)
        {
            if (source.Length == 0) return new T[0];

            var len = source.Length;
            var target = new T[len];
            for (int i = 0; i < len; i++) target[i] = source[i].TryClone()!;
            return target;
        }
        else return (T[])source.Clone();
    }

    // ----------------------------------------------------

    /// <summary>
    /// Returns the indexes of the ocurrences of the given value in the source array, using the
    /// default element comparer.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    public static List<int> IndexesOf<T>(
        this T[] source, T value)
        => source.IndexesOf(value, new ItemComparer<T>());

    /// <summary>
    /// Returns the indexes of the ocurrences of the given value in the source array, using the
    /// given comparer.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="comparer"></param>
    /// <returns></returns>
    public static List<int> IndexesOf<T>(this T[] source, T value, IEqualityComparer<T> comparer)
    {
        source.ThrowWhenNull();
        comparer.ThrowWhenNull();

        List<int> list = [];
        for (int i = 0; i < source.Length; i++)
        {
            var item = source[i];
            var same =
                (item is null && value is null) ||
                ((item is not null && value is not null) && item.Equals(value));

            if (same) list.Add(i);
        }
        return list;
    }

    // ----------------------------------------------------

    /// <summary>
    /// Determines if this source array contains an element that matches the given predicate.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="source"></param>
    /// <param name="predicate"></param>
    /// <returns></returns>
    public static bool Contains<T>(
        this T[] source, Predicate<T> predicate) => source.IndexOf(predicate) >= 0;

    /// <summary>
    /// Returns the index of the first ocurrence of an element that matches the given predicate,
    /// or -1 if any.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="source"></param>
    /// <param name="predicate"></param>
    /// <returns></returns>
    public static int IndexOf<T>(this T[] source, Predicate<T> predicate)
    {
        source.ThrowWhenNull();
        predicate.ThrowWhenNull();

        for (int i = 0; i < source.Length; i++) if (predicate(source[i])) return i;
        return -1;
    }

    /// <summary>
    /// Returns the index of the last ocurrence of an element that matches the given predicate,
    /// or -1 if any.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="source"></param>
    /// <param name="predicate"></param>
    /// <returns></returns>
    public static int LastIndexOf<T>(this T[] source, Predicate<T> predicate)
    {
        source.ThrowWhenNull();
        predicate.ThrowWhenNull();

        for (int i = source.Length - 1; i >= 0; i--) if (predicate(source[i])) return i;
        return -1;
    }

    /// <summary>
    /// Returns the indexes of all the ocurrences of elements that match the given predicate,
    /// or -1 if any.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="source"></param>
    /// <param name="predicate"></param>
    /// <returns></returns>
    public static List<int> IndexesOf<T>(this T[] source, Predicate<T> predicate)
    {
        source.ThrowWhenNull();
        predicate.ThrowWhenNull();

        List<int> list = [];
        for (int i = 0; i < source.Length; i++) if (predicate(source[i])) list.Add(i);
        return list;
    }

    // ----------------------------------------------------

    /// <summary>
    /// Returns a new array where all leading and trailing default values have been removed.
    /// <br/> Returns the original one if no changes were made.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="source"></param>
    /// <returns></returns>
    public static T[] Trim<T>(this T[] source) => source.TrimStart().TrimEnd();

    /// <summary>
    /// Returns a new array where all leading default values have been removed.
    /// <br/> Returns the original one if no changes were made.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="source"></param>
    /// <returns></returns>
    public static T[] TrimStart<T>(this T[] source)
    {
        source.ThrowWhenNull();

        if (source.Length == 0) return source;

        var index = source.IndexOf(x => !x.EqualsEx(default));
        if (index <= 0) return source;

        var num = source.Length - index;
        var target = new T[num];
        Array.Copy(source, index, target, 0, num);
        return target;
    }

    /// <summary>
    /// Returns a new array where all trailing default values have been removed.
    /// <br/> Returns the original one if no changes were made.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="source"></param>
    /// <returns></returns>
    public static T[] TrimEnd<T>(this T[] source)
    {
        source.ThrowWhenNull();

        if (source.Length == 0) return source;

        var index = source.LastIndexOf(x => !x.EqualsEx(default));
        if (index < 0) return source;
        if (index ==  source.Length - 1) return source;

        var num = index + 1;
        var target = new T[num];
        Array.Copy(source, 0, target, 0, num);
        return target;
    }

    // ----------------------------------------------------

    /// <summary>
    /// Returns a new array that contains a shallow copy of the given number of original elements,
    /// starting from the given index.
    /// <br/> Returns the original one if no changes were made.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="source"></param>
    /// <param name="index"></param>
    /// <param name="count"></param>
    /// <returns></returns>
    public static T[] GetRange<T>(this T[] source, int index, int count)
    {
        source.ThrowWhenNull();

        if (index < 0) throw new IndexOutOfRangeException("Index cannot be negative.").WithData(index);
        if (count < 0) throw new ArgumentException("Count cannot be negative.").WithData(count);

        if (index > source.Length) throw new IndexOutOfRangeException(nameof(index)).WithData(index);
        if (count > source.Length) throw new ArgumentOutOfRangeException(nameof(count)).WithData(count);

        if (count < 0 || (index + count) > source.Length)
            throw new ArgumentOutOfRangeException("Index plus count is out of bounds.", (Exception?)null)
            .WithData(index)
            .WithData(count);

        if (count == 0) return [];
        if (index == 0 && count == source.Length) return source;
        

        var target = new T[count];
        Array.Copy(source, index, target, 0, count);
        return target;
    }

    // ----------------------------------------------------

    /// <summary>
    /// Returns a new instance where the original old element, if present, has been replaced by
    /// the new given one,  if they were not equal, using a default comparer.
    /// <br/> Returns the original one if no changes were made.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="source"></param>
    /// <param name="oldvalue"></param>
    /// <param name="newvalue"></param>
    /// <returns></returns>
    public static T[] Replace<T>(
        this T[] source, T oldvalue, T newvalue)
        => source.Replace(oldvalue, newvalue, new ItemComparer<T>());

    /// <summary>
    /// Returns a new instance where the first ocurrence of the original old element, if any, has
    /// been replaced by the new given one,  if they were not equal, using a default comparer.
    /// <br/> Returns the original one if no changes were made.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="source"></param>
    /// <param name="oldvalue"></param>
    /// <param name="newvalue"></param>
    /// <param name="comparer"></param>
    /// <returns></returns>
    public static T[] Replace<T>(
        this T[] source, T oldvalue, T newvalue, IEqualityComparer<T> comparer)
    {
        source.ThrowWhenNull();
        comparer.ThrowWhenNull();

        if (comparer.Equals(oldvalue, newvalue)) return source;

        var index = source.IndexOf(x => comparer.Equals(x, oldvalue));
        if (index < 0) return source;

        var target = (T[])source.Clone();
        target[index] = newvalue;
        return target;
    }

    // ----------------------------------------------------

    /// <summary>
    /// Returns a new instance where the original element at the given index has been replaced by
    /// the new given one if they are not equal, using the element default comparer.
    /// <br/> Returns the original one if no changes were made.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="source"></param>
    /// <param name="index"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    public static T[] ReplaceAt<T>(
        this T[] source, int index, T value)
        => source.ReplaceAt(index, value, new ItemComparer<T>());

    /// <summary>
    /// Returns a new instance where the original element at the given index has been replaced by
    /// the new given one if they are not equal, using the given equality comparer.
    /// <br/> Returns the original one if no changes were made.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="source"></param>
    /// <param name="index"></param>
    /// <param name="value"></param>
    /// <param name="comparer"></param>
    /// <returns></returns>
    public static T[] ReplaceAt<T>(
        this T[] source, int index, T value, IEqualityComparer<T> comparer)
    {
        source.ThrowWhenNull();
        comparer.ThrowWhenNull();

        var item = source[index];
        if (comparer.Equals(item, value)) return source;

        var target = (T[])source.Clone();
        target[index] = value;
        return target;
    }

    // ----------------------------------------------------

    /// <summary>
    /// Returns a new array where the given element has been added to the original one.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    public static T[] Add<T>(this T[] source, T value)
    {
        source.ThrowWhenNull();

        if (source.Length == 0) return [value];

        var target = new T[source.Length + 1];
        Array.Copy(source, target, source.Length);
        target[source.Length] = value;
        return target;
    }

    /// <summary>
    /// Returns a new array where the elements of the given range have been added to the original
    /// array.
    /// <br/> Returns the original one if no changes were made.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="source"></param>
    /// <param name="range"></param>
    /// <returns></returns>
    public static T[] AddRange<T>(this T[] source, IEnumerable<T> range)
    {
        source.ThrowWhenNull();
        range.ThrowWhenNull();

        if (range is T[] items)
        {
            if (items.Length == 0) return source;
            if (source.Length == 0) return items;

            var target = new T[source.Length + items.Length];
            Array.Copy(source, target, source.Length);
            Array.Copy(items, 0, target, source.Length, items.Length);
            return target;
        }
        else
        {
            var temps = range.ToList(); if (temps.Count == 0) return source;
            if (source.Length == 0) return [.. temps];

            var list = source.ToList();
            list.AddRange(temps);
            return [.. list];
        }
    }

    /// <summary>
    /// Returns a new array where the given element has been inserted into the original one, at
    /// the given index.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="source"></param>
    /// <param name="index"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    public static T[] Insert<T>(this T[] source, int index, T value)
    {
        source.ThrowWhenNull();

        if (source.Length == 0) return [value];

        var target = new T[source.Length + 1];
        Array.Copy(source, target, index);
        target[index] = value;

        Array.Copy(source, index, target, index + 1, source.Length - index);
        return target;
    }

    /// <summary>
    /// Returns a new array where the elements of the given range have been inserted into the
    /// original array, starting at the given index.
    /// <br/> Returns the original one if no changes were made.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="source"></param>
    /// <param name="index"></param>
    /// <param name="range"></param>
    /// <returns></returns>
    public static T[] InsertRange<T>(this T[] source, int index, IEnumerable<T> range)
    {
        source.ThrowWhenNull();
        range.ThrowWhenNull();

        if (index < 0) throw new IndexOutOfRangeException("Index cannot be negative.").WithData(index);
        if (index > source.Length) throw new IndexOutOfRangeException(nameof(index)).WithData(index);

        if (range is T[] items)
        {
            if (items.Length == 0) return source;
            if (source.Length == 0) return items;

            var target = new T[source.Length + items.Length];
            Array.Copy(source, target, index);
            Array.Copy(items, 0, target, index, items.Length);
            Array.Copy(source, index, target, index + items.Length, source.Length - index);
            return target;
        }
        else
        {
            var temps = range.ToList(); if (temps.Count == 0) return source;
            if (source.Length == 0) return [.. temps];

            var list = source.ToList();
            list.InsertRange(index, temps);
            return [.. list];
        }
    }

    // ----------------------------------------------------

    /// <summary>
    /// Returns a new instance where the element at the given index has been removed.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="source"></param>
    /// <param name="index"></param>
    /// <returns></returns>
    public static T[] RemoveAt<T>(this T[] source, int index)
    {
        source.ThrowWhenNull();

        if (index < 0) throw new IndexOutOfRangeException("Index cannot be negative.").WithData(index);
        if (index > source.Length) throw new IndexOutOfRangeException(nameof(index)).WithData(index);

        if (source.Length == 0) return source;
        if (source.Length == 1) return [];

        var target = new T[source.Length - 1];
        Array.Copy(source, target, index);
        Array.Copy(source, index + 1, target, index, source.Length - index - 1);
        return target;
    }

    /// <summary>
    /// Returns a new instance where the given number of element, starting from the given index,
    /// have been removed.
    /// <br/> Returns the original one if no changes were made.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="source"></param>
    /// <param name="index"></param>
    /// <param name="count"></param>
    /// <returns></returns>
    public static T[] RemoveRange<T>(this T[] source, int index, int count)
    {
        source.ThrowWhenNull();

        if (index < 0) throw new IndexOutOfRangeException("Index cannot be negative.").WithData(index);
        if (count < 0) throw new ArgumentException("Count cannot be negative.").WithData(count);

        if (index > source.Length) throw new IndexOutOfRangeException(nameof(index)).WithData(index);
        if (count > source.Length) throw new ArgumentOutOfRangeException(nameof(count)).WithData(count);

        if (count < 0 || (index + count) > source.Length)
            throw new ArgumentOutOfRangeException("Index plus count is out of bounds.", (Exception?)null)
            .WithData(index)
            .WithData(count);

        if (count == 0) return source;
        if (index == 0 && count == source.Length) return [];

        var target = new T[source.Length - count];
        Array.Copy(source, target, index);
        Array.Copy(source, index + count, target, index, source.Length - index - count);
        return target;
    }

    // ----------------------------------------------------

    /// <summary>
    /// Returns a new array where the first ocurrence of the given value has been removed, if
    /// found using the default element comparer.
    /// <br/> Returns the original one if no changes were made.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    public static T[] Remove<T>(
        this T[] source, T value) => source.Remove(value, new ItemComparer<T>());

    /// <summary>
    /// Returns a new array where the first ocurrence of the given value has been removed, if
    /// found using the given comparer.
    /// <br/> Returns the original one if no changes were made.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="comparer"></param>
    /// <returns></returns>
    public static T[] Remove<T>(this T[] source, T value, IEqualityComparer<T> comparer)
    {
        source.ThrowWhenNull();
        comparer.ThrowWhenNull();

        var index = source.IndexOf(x => comparer.Equals(x, value));
        return index >= 0 ? source.RemoveAt(index) : source;
    }

    // ----------------------------------------------------

    /// <summary>
    /// Returns a new array where the last ocurrence of the given value has been removed, if
    /// found using the default element comparer.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    public static T[] RemoveLast<T>(
        this T[] source, T value) => source.RemoveLast(value, new ItemComparer<T>());

    /// <summary>
    /// Returns a new array where the last ocurrence of the given value has been removed, if
    /// found using the given comparer.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="comparer"></param>
    /// <returns></returns>
    public static T[] RemoveLast<T>(this T[] source, T value, IEqualityComparer<T> comparer)
    {
        source.ThrowWhenNull();
        comparer.ThrowWhenNull();

        var index = source.LastIndexOf(x => comparer.Equals(x, value));
        return index >= 0 ? source.RemoveAt(index) : source;
    }

    // ----------------------------------------------------

    /// <summary>
    /// Returns a new array where all the ocurrences of the given value have been removed, if
    /// found using the default element comparer.
    /// <br/> Returns the original one if no changes were made.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    public static T[] RemoveAll<T>(
        this T[] source, T value) => source.RemoveAll(value, new ItemComparer<T>());

    /// <summary>
    /// Returns a new array where all the ocurrences of the given value have been removed, if
    /// found using the given comparer.
    /// <br/> Returns the original one if no changes were made.
    /// </summary>
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="comparer"></param>
    /// <returns></returns>
    public static T[] RemoveAll<T>(this T[] source, T value, IEqualityComparer<T> comparer)
    {
        source.ThrowWhenNull();
        comparer.ThrowWhenNull();

        if (source.Length == 0) return source;

        var num = 0;
        var items = source.ToList();
        while (true)
        {
            if (items.Remove(value)) num++;
            else break;
        }
        return num > 0 ? [.. items] : source;
    }

    // ----------------------------------------------------

    /// <summary>
    /// Returns a new array where the first element that matches the given predicate has been
    /// removed, if any.
    /// <br/> Returns the original one if no changes were made.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="source"></param>
    /// <param name="predicate"></param>
    /// <returns></returns>
    public static T[] Remove<T>(this T[] source, Predicate<T> predicate)
    {
        source.ThrowWhenNull();
        predicate.ThrowWhenNull();

        var index = source.IndexOf(x => predicate(x));
        return index >= 0 ? source.RemoveAt(index) : source;
    }

    /// <summary>
    /// Returns a new array where the last element that matches the given predicate has been
    /// removed, if any.
    /// <br/> Returns the original one if no changes were made.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="source"></param>
    /// <param name="predicate"></param>
    /// <returns></returns>
    public static T[] RemoveLast<T>(this T[] source, Predicate<T> predicate)
    {
        source.ThrowWhenNull();
        predicate.ThrowWhenNull();

        var index = source.LastIndexOf(x => predicate(x));
        return index >= 0 ? source.RemoveAt(index) : source;
    }

    /// <summary>
    /// Returns a new array where all the elements that match the given predicate have been
    /// removed, if any.
    /// <br/> Returns the original one if no changes were made.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="source"></param>
    /// <param name="predicate"></param>
    /// <returns></returns>
    /// <exception cref="System.NullReferenceException"></exception>
    public static T[] RemoveAll<T>(this T[] source, Predicate<T> predicate)
    {
        source.ThrowWhenNull();
        predicate.ThrowWhenNull();

        if (source.Length == 0) return source;

        var num = 0;
        var items = source.ToList();
        while (true)
        {
            var index = items.FindIndex(x => predicate(x));
            if (index >= 0)
            {
                items.RemoveAt(index);
                num++;
            }
            else break;
        }
        return num > 0 ? [.. items] : source;
    }

    // ----------------------------------------------------

    /// <summary>
    /// Returns a new array whose elements have been cleared.
    /// <br/> Always returns a new instance.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="source"></param>
    /// <returns></returns>
    public static T[] Clear<T>(this T[] source) => new T[source.Length];

    // ----------------------------------------------------

    /// <summary>
    /// Returns a new array with the requested lenght, adding or removing leading elements as
    /// needed. The value of the added ones is obtained from the given '<paramref name="pad"/>'
    /// argument.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="source"></param>
    /// <param name="len"></param>
    /// <param name="pad"></param>
    /// <returns></returns>
    public static T[] ResizeHead<T>(this T[] source, int len, T pad = default!)
    {
        throw null;
    }

    /// <summary>
    /// Returns a new array with the requested lenght, adding or removing trailing elements as
    /// needed. The value of the added ones is obtained from the given '<paramref name="pad"/>'
    /// argument.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="source"></param>
    /// <param name="len"></param>
    /// <param name="pad"></param>
    /// <returns></returns>
    public static T[] ResizeTail<T>(this T[] source, int len, T pad = default!)
    {
        throw null;
    }
}