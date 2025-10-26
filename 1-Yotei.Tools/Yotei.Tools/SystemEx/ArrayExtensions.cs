namespace Yotei.Tools;

// ========================================================
public static class ArrayExtensions
{
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
    /// default comparer of the element's type.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    public static List<int> IndexesOf<T>(
        this T[] source, T value)
        => source.IndexesOf(value, EqualityComparer<T>.Default);

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
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="source"></param>
    /// <param name="index"></param>
    /// <param name="count"></param>
    /// <returns></returns>
    public static T[] GetRange<T>(this T[] source, int index, int count) => throw null;

    // ----------------------------------------------------

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="source"></param>
    /// <param name="oldvalue"></param>
    /// <param name="newvalue"></param>
    /// <returns></returns>
    public static T[] Replace<T>(this T[] source, T oldvalue, T newvalue) => throw null;

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="source"></param>
    /// <param name="oldvalue"></param>
    /// <param name="newvalue"></param>
    /// <param name="comparer"></param>
    /// <returns></returns>
    public static T[] Replace<T>(
        this T[] source, T oldvalue, T newvalue, IEqualityComparer<T> comparer) => throw null;

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="source"></param>
    /// <param name="index"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    public static T[] ReplaceIndex<T>(this T[] source, int index, T value) => throw null;

    // ----------------------------------------------------

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    public static T[] Add<T>(this T[] source, T value) => throw null;

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="source"></param>
    /// <param name="range"></param>
    /// <returns></returns>
    public static T[] AddRange<T>(this T[] source, IEnumerable<T> range) => throw null;

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="source"></param>
    /// <param name="index"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    public static T[] Insert<T>(this T[] source, int index, T value) => throw null;

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="source"></param>
    /// <param name="index"></param>
    /// <param name="range"></param>
    /// <returns></returns>
    public static T[] InsertRange<T>(this T[] source, int index, IEnumerable<T> range) => throw null;

    // ----------------------------------------------------

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="source"></param>
    /// <param name="index"></param>
    /// <returns></returns>
    public static T[] RemoveAt<T>(this T[] source, int index) => throw null;

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="source"></param>
    /// <param name="index"></param>
    /// <param name="count"></param>
    /// <returns></returns>
    public static T[] RemoveRange<T>(this T[] source, int index, int count) => throw null;

    // ----------------------------------------------------

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    public static T[] Remove<T>(this T[] source, T value) => throw null;

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    public static T[] RemoveLast<T>(this T[] source, T value) => throw null;

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    public static T[] RemoveAll<T>(this T[] source, T value) => throw null;

    // ----------------------------------------------------

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="source"></param>
    /// <param name="predicate"></param>
    /// <returns></returns>
    public static T[] Remove<T>(this T[] source, Predicate<T> predicate) => throw null;

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="source"></param>
    /// <param name="predicate"></param>
    /// <returns></returns>
    public static T[] RemoveLast<T>(this T[] source, Predicate<T> predicate) => throw null;

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="source"></param>
    /// <param name="predicate"></param>
    /// <returns></returns>
    /// <exception cref="System.NullReferenceException"></exception>
    public static T[] RemoveAll<T>(this T[] source, Predicate<T> predicate) => throw null;

    // ----------------------------------------------------

    /// <summary>
    /// Returns a new array whose elements have been cleared.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="source"></param>
    /// <returns></returns>
    public static T[] Clear<T>(this T[] source) => new T[source.Length];
}