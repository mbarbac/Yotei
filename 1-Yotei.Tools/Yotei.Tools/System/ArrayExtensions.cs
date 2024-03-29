﻿namespace Yotei.Tools;

// ========================================================
public static class ArrayExtensions
{
    /// <summary>
    /// Returns a new array that is a duplicate of the given one.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="source"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T[] Duplicate<T>(this T[] source)
    {
        source.ThrowWhenNull();

        if (source.Length == 0) return [];

        var target = new T[source.Length];
        Array.Copy(source, target, source.Length);
        return target;
    }

    /// <summary>
    /// Returns an enumerator that iterates through the given array in a strong-type fashion.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="source"></param>
    /// <returns></returns>
    public static IEnumerator<T> GetTypedEnumerator<T>(this T[] source)
    {
        source.ThrowWhenNull();
        foreach (var value in source) yield return value;
    }

    // ----------------------------------------------------

    /// <summary>
    /// Returns a new array with the given number of original elements, from the given index.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="source"></param>
    /// <param name="index"></param>
    /// <param name="count"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T[] GetRange<T>(this T[] source, int index, int count)
    {
        source.ThrowWhenNull();

        if (count == 0) return [];

        var target = new T[count];
        Array.Copy(source, index, target, 0, count);
        return target;
    }

    /// <summary>
    /// Returns a new array where the original element at the given index has been replaced
    /// by the new given one.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="source"></param>
    /// <param name="index"></param>
    /// <param name="item"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T[] Replace<T>(this T[] source, int index, T item)
    {
        source.ThrowWhenNull();

        var target = new T[source.Length];
        Array.Copy(source, target, source.Length);
        target[index] = item;
        return target;
    }

    /// <summary>
    /// Returns a new array where the given element has been added to the original array.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="source"></param>
    /// <param name="item"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T[] Add<T>(this T[] source, T item)
    {
        source.ThrowWhenNull();

        if (source.Length == 0) return [item];

        var target = new T[source.Length + 1];
        Array.Copy(source, target, source.Length);
        target[source.Length] = item;
        return target;
    }

    /// <summary>
    /// Returns a new array where the elements from the given range have been added to the
    /// original array.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="source"></param>
    /// <param name="range"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T[] AddRange<T>(this T[] source, IEnumerable<T> range)
    {
        source.ThrowWhenNull();
        range.ThrowWhenNull();

        var array = range is T[] others ? others : range.ToArray();
        if (array.Length == 0) return source.Duplicate();

        var target = new T[source.Length + array.Length];
        Array.Copy(source, target, source.Length);
        Array.Copy(array, 0, target, source.Length, array.Length);
        return target;
    }

    /// <summary>
    /// Returns a new array where the given element has been inserted into the original array
    /// at the given index.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="source"></param>
    /// <param name="index"></param>
    /// <param name="item"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T[] Insert<T>(this T[] source, int index, T item)
    {
        source.ThrowWhenNull();

        var target = new T[source.Length + 1];
        Array.Copy(source, target, index);
        target[index] = item;
        Array.Copy(source, index, target, index + 1, source.Length - index);
        return target;
    }

    /// <summary>
    /// Returns a new array where the elements from the given range have been inserted into
    /// the original array, starting at the given index.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="source"></param>
    /// <param name="index"></param>
    /// <param name="range"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T[] InsertRange<T>(this T[] source, int index, IEnumerable<T> range)
    {
        source.ThrowWhenNull();
        range.ThrowWhenNull();

        if (source.Length == 0 && index > 0)
            throw new ArgumentException("Index must be cero when inserting in empty arrays").WithData(index);

        var array = range is T[] others ? others : range.ToArray();
        if (array.Length == 0) return source.Duplicate();

        var target = new T[source.Length + array.Length];
        Array.Copy(source, target, index);
        Array.Copy(array, 0, target, index, array.Length);
        Array.Copy(source, index, target, index + array.Length, source.Length - index);
        return target;
    }

    /// <summary>
    /// Returns a new array where the original element at the given index has been removed.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="source"></param>
    /// <param name="index"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T[] RemoveAt<T>(this T[] source, int index)
    {
        source.ThrowWhenNull();

        if (source.Length == 0) throw new InvalidOperationException(
            "Cannot remove elements from empty arrays.");

        var target = new T[source.Length - 1];
        Array.Copy(source, target, index);
        Array.Copy(source, index + 1, target, index, source.Length - index - 1);
        return target;
    }

    /// <summary>
    /// Returns a new array where the given number of original elements have been removed,
    /// from the given index.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="source"></param>
    /// <param name="index"></param>
    /// <param name="count"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T[] RemoveRange<T>(this T[] source, int index, int count)
    {
        source.ThrowWhenNull();

        if (count == 0) return source.Duplicate();

        var target = new T[source.Length - count];
        Array.Copy(source, target, index);
        Array.Copy(source, index + count, target, index, source.Length - index - count);
        return target;
    }

    /// <summary>
    /// Returns a new array where the first ocurrence of the given element has been removed
    /// from the original array.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="source"></param>
    /// <param name="item"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T[] Remove<T>(this T[] source, T item)
    {
        source.ThrowWhenNull();

        var index = Array.IndexOf(source, item);
        return index >= 0 ? source.RemoveAt(index) : source.Duplicate();
    }

    /// <summary>
    /// Returns a new array where the last ocurrence of the given element has been removed
    /// from the original array.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="source"></param>
    /// <param name="item"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T[] RemoveLast<T>(this T[] source, T item)
    {
        source.ThrowWhenNull();

        var index = Array.LastIndexOf(source, item);
        return index >= 0 ? source.RemoveAt(index) : source.Duplicate();
    }

    /// <summary>
    /// Returns a new array where all the ocurrences of the given element have been removed
    /// from the original array.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="source"></param>
    /// <param name="item"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T[] RemoveAll<T>(this T[] source, T item)
    {
        source.ThrowWhenNull();

        while (true)
        {
            var index = Array.IndexOf(source, item);

            if (index >= 0) source = source.RemoveAt(index);
            else break;
        }
        return source;
    }

    /// <summary>
    /// Returns a new array where the first element that matches the given predicate has been
    /// removed from the original array.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="source"></param>
    /// <param name="predicate"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T[] Remove<T>(this T[] source, Predicate<T> predicate)
    {
        source.ThrowWhenNull();
        predicate.ThrowWhenNull();

        var index = Array.FindIndex(source, predicate);
        return index >= 0 ? source.RemoveAt(index) : source.Duplicate();
    }

    /// <summary>
    /// Returns a new array where the last element that matches the given predicate has been
    /// removed from the original array.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="source"></param>
    /// <param name="predicate"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T[] RemoveLast<T>(this T[] source, Predicate<T> predicate)
    {
        source.ThrowWhenNull();
        predicate.ThrowWhenNull();

        var index = Array.FindLastIndex(source, predicate);
        return index >= 0 ? source.RemoveAt(index) : source.Duplicate();
    }

    /// <summary>
    /// Returns a new array where all the elements that match the given predicate have been
    /// removed from the original array.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="source"></param>
    /// <param name="predicate"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T[] RemoveAll<T>(this T[] source, Predicate<T> predicate)
    {
        source.ThrowWhenNull();
        predicate.ThrowWhenNull();

        while (true)
        {
            var index = Array.FindIndex(source, predicate);

            if (index >= 0) source = source.RemoveAt(index);
            else break;
        }
        return source;
    }

    /// <summary>
    /// Returns a new array where all the original elements have been removed.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="source"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T[] Clear<T>(this T[] source)
    {
        return source.Length == 0 ? source : [];
    }

    // ----------------------------------------------------

    /// <summary>
    /// Returns a new array with the requested length, adding or removing elements at the head
    /// of the original one as needed. The value of the added elements, if any, is obtained
    /// from the given 'pad' value.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="source"></param>
    /// <param name="len"></param>
    /// <param name="pad"></param>
    /// <returns></returns>
    public static T[] ResizeHead<T>(this T[] source, int len, T pad = default!)
    {
        source.ThrowWhenNull();

        if (len < 0) throw new ArgumentException(
            $"Requested length '{len}' cannot be less than cero.");

        if (len == 0) return [];

        if (len < source.Length) return source.AsSpan(source.Length - len).ToArray();

        var target = new T[len];
        for (int i = 0; i < len; i++) target[i] = pad;

        Array.Copy(source, 0, target, len - source.Length, source.Length);
        return target;
    }

    /// <summary>
    /// Returns a new array with the requested length, adding or removing elements at the tail
    /// of the original one as needed. The value of the added elements, if any, is obtained
    /// from the given 'pad' value.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="source"></param>
    /// <param name="len"></param>
    /// <param name="pad"></param>
    /// <returns></returns>
    public static T[] ResizeTail<T>(this T[] source, int len, T pad = default!)
    {
        source = source.ThrowWhenNull();

        if (len < 0) throw new ArgumentException(
            $"Requested length '{len}' cannot be less than cero.");

        if (len == 0) return [];

        if (len < source.Length) return source.AsSpan(0, len).ToArray();

        var target = new T[len];
        for (int i = 0; i < len; i++) target[i] = pad;

        Array.Copy(source, 0, target, 0, source.Length);
        return target;
    }
}