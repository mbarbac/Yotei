﻿namespace Yotei.ORM.Tools;

// ========================================================
/// <summary>
/// Represents a list-alike collection of elements.
/// </summary>
/// <typeparam name="T"></typeparam>
[Cloneable]
public partial interface ICoreList<T>
    : IList<T>, IList, IReadOnlyList<T>
    , ICollection<T>, ICollection, IReadOnlyCollection<T>
{
    /// <summary>
    /// Gets the number of elements in this collection.
    /// </summary>
    new int Count { get; }

    /// <summary>
    /// Gets or sets the element at the given index.
    /// </summary>
    /// <param name="index"></param>
    /// <returns></returns>
    new T this[int index] { get; set; }

    /// <summary>
    /// Determines if this collection contains the given element.
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    new bool Contains(T item);

    /// <summary>
    /// Gets the index of the first ocurrence of the given element in this collection, or -1 if
    /// any is found.
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    new int IndexOf(T item);

    /// <summary>
    /// Gets the index of the last ocurrence of the given element in this collection, or -1 if
    /// any is found.
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    int LastIndexOf(T item);

    /// <summary>
    /// Gets the indexes of the ocurrences of the given element in this collection.
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    List<int> IndexesOf(T item);

    /// <summary>
    /// Determines if this collection contains an element that satisfies the given predicate.
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    bool Contains(Predicate<T> predicate);

    /// <summary>
    /// Gets the index of the first element in this collection that matches the given predicate,
    /// or -1 if any is found.
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    int IndexOf(Predicate<T> predicate);

    /// <summary>
    /// Gets the index of the last element in this collection that matches the given predicate,
    /// or -1 if any is found.
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    int LastIndexOf(Predicate<T> predicate);

    /// <summary>
    /// Gets the indexes of all the elements in this collection that matches the given predicate.
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    List<int> IndexesOf(Predicate<T> predicate);

    /// <summary>
    /// Gets an array with the elements in this collection.
    /// </summary>
    /// <returns></returns>
    T[] ToArray();

    /// <summary>
    /// Gets a list with the elements in this collection.
    /// </summary>
    /// <returns></returns>
    List<T> ToList();

    /// <summary>
    /// Returns a list with a shallow copy of the given number of elements, starting from the
    /// given index.
    /// </summary>
    /// <param name="index"></param>
    /// <param name="count"></param>
    /// <returns></returns>
    List<T> ToList(int index, int count);

    /// <summary>
    /// Gets or sets the total number of elements the internal data structures can hold without
    /// resizing.
    /// </summary>
    int Capacity { get; set; }

    /// <summary>
    /// Trims the internal structures of this collection.
    /// </summary>
    void Trim();

    // ----------------------------------------------------

    /// <summary>
    /// Replaces the element at the given index with the given one.
    /// </summary>
    /// <param name="index"></param>
    /// <param name="item"></param>
    /// <returns>The number of changes made, it being either the number of actual inserts
    /// performed or, if none, the number of actual removals.</returns>
    int Replace(int index, T item);

    /// <summary>
    /// Adds to this collection the given element.
    /// </summary>
    /// <param name="item"></param>
    /// <returns>The number of changes made.</returns>
    new int Add(T item);

    /// <summary>
    /// Adds to this collection the elements from the given range.
    /// </summary>
    /// <param name="range"></param>
    /// <returns>The number of changes made.</returns>
    int AddRange(IEnumerable<T> range);

    /// <summary>
    /// Inserts into this collection the given element at the given index.
    /// </summary>
    /// <param name="index"></param>
    /// <param name="item"></param>
    /// <returns>The number of changes made.</returns>
    new int Insert(int index, T item);

    /// <summary>
    /// Inserts into this collection the elements from the given range, starting at the given
    /// index.
    /// </summary>
    /// <param name="index"></param>
    /// <param name="range"></param>
    /// <returns>The number of changes made.</returns>
    int InsertRange(int index, IEnumerable<T> range);

    /// <summary>
    /// Removes from this collection the element at the given index.
    /// </summary>
    /// <param name="index"></param>
    /// <returns>The number of changes made.</returns>
    new int RemoveAt(int index);

    /// <summary>
    /// Removes from this collection the given number of elements, starting at the given index.
    /// </summary>
    /// <param name="index"></param>
    /// <param name="count"></param>
    /// <returns>The number of changes made.</returns>
    int RemoveRange(int index, int count);

    /// <summary>
    /// Removes from this collection the first ocurrence of the given element.
    /// </summary>
    /// <param name="item"></param>
    /// <returns>The number of changes made.</returns>
    new int Remove(T item);

    /// <summary>
    /// Removes from this collection the last ocurrence of the given element.
    /// </summary>
    /// <param name="item"></param>
    /// <returns>The number of changes made.</returns>
    int RemoveLast(T item);

    /// <summary>
    /// Removes from this collection all the ocurrences of the given element.
    /// </summary>
    /// <param name="item"></param>
    /// <returns>The number of changes made.</returns>
    int RemoveAll(T item);

    /// <summary>
    /// Removes from this collection the first element that matches the given predicate.
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns>The number of changes made.</returns>
    int Remove(Predicate<T> predicate);

    /// <summary>
    /// Removes from this collection the last element that matches the given predicate.
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns>The number of changes made.</returns>
    int RemoveLast(Predicate<T> predicate);

    /// <summary>
    /// Removes from this collection all the elements that match the given predicate.
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns>The number of changes made.</returns>
    int RemoveAll(Predicate<T> predicate);

    /// <summary>
    /// Clears this collection.
    /// </summary>
    /// <returns>The number of changes made.</returns>
    new int Clear();
}