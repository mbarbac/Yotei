﻿namespace Yotei.Tools;

// ========================================================
/// <summary>
/// Represents a list-alike collection of not null and not-duplicated elements.
/// </summary>
/// <typeparam name="T"></typeparam>
public class NoDuplicatesList<T> : IList<T> where T : class
{
    readonly List<T> _Elements = new();

    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    /// <param name="validator"></param>
    /// <param name="comparer"></param>
    public NoDuplicatesList(Func<T, T>? validator = null, Func<T, T, bool>? comparer = null)
    {
        if (validator != null) Validator = validator;
        if (comparer != null) Comparer = comparer;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns><inheritdoc/></returns>
    public override string ToString() => $"Count: {Count}";

    /// <summary>
    /// The comparer this instance uses to compare the value of two elements.
    /// </summary>
    public Func<T, T, bool> Comparer { get; } = EqualityComparer<T>.Default.Equals;

    /// <summary>
    /// The delegate this instance calls to return a not null element.
    /// </summary>
    public Func<T, T> Validator { get; } = (element) => element.ThrowIfNull();

    /// <summary>
    /// Determines if this collection is a read-only one, or not.
    /// </summary>
    public bool IsReadOnly => false;

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns><inheritdoc/></returns>
    public IEnumerator<T> GetEnumerator() => _Elements.GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    /// <summary>
    /// Gets the number of elements in this collection.
    /// </summary>
    public int Count => _Elements.Count;

    /// <summary>
    /// Gets or sets the element stored at the given index. If the setter finds an equivalent
    /// element at the given index, then the old value is replaced by the new given one. If an
    /// equivalent element is found in any other place, then a duplicate exception is thrown.
    /// </summary>
    /// <param name="index"></param>
    /// <returns></returns>
    public T this[int index]
    {
        get => _Elements[index];
        set
        {
            value = Validator(value);

            var x = IndexOf(value);
            if (x < 0 || x == index) { _Elements[index] = value; return; }
            else throw new DuplicateException(
                "Duplicate value found.")
                .WithData(value)
                .WithData(index);

        }
    }

    /// <summary>
    /// Returns the index at which the given element, or an equivalent one, is stored in this
    /// collection, or -1 if any can be found.
    /// </summary>
    /// <param name="element"></param>
    /// <returns></returns>
    public int IndexOf(T element)
    {
        element = Validator(element);

        for (int i = 0; i < _Elements.Count; i++)
            if (Comparer(element, _Elements[i])) return i;

        return -1;
    }

    /// <summary>
    /// Returns the index of the first element in this collection matching the given predicate,
    /// or -1 if any can be found.
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    public int IndexOf(Predicate<T> predicate)
    {
        predicate = predicate.ThrowIfNull();

        for (int i = 0; i < _Elements.Count; i++) if (predicate(_Elements[i])) return i;
        return -1;
    }

    /// <summary>
    /// Returns the index of the last element in this collection matching the given predicate,
    /// or -1 if any can be found.
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    public int LastIndexOf(Predicate<T> predicate)
    {
        predicate = predicate.ThrowIfNull();

        for (int i = _Elements.Count - 1; i >= 0; i--) if (predicate(_Elements[i])) return i;
        return -1;
    }

    /// <summary>
    /// Returns a list with the indexes of all the elements in this collecion that match the
    /// given predicate.
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    public List<int> FindIndexes(Predicate<T> predicate)
    {
        predicate = predicate.ThrowIfNull();
        var list = new List<int>();

        for (int i = 0; i < _Elements.Count; i++) if (predicate(_Elements[i])) list.Add(i);
        return list;
    }

    /// <summary>
    /// Determines if this instance contains the given element, or not.
    /// </summary>
    /// <param name="element"></param>
    /// <returns></returns>
    public bool Contains(T element) => IndexOf(element) >= 0;

    /// <summary>
    /// Returns the first ocurrence in this collection that matches the given predicate, or
    /// null if any is found.
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    public T? Find(Predicate<T> predicate)
    {
        var index = IndexOf(predicate);
        return index >= 0 ? _Elements[index] : null;
    }

    /// <summary>
    /// Returns the last ocurrence in this collection that matches the given predicate, or
    /// null if any is found.
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    public T? FindLast(Predicate<T> predicate)
    {
        var index = IndexOf(predicate);
        return index >= 0 ? _Elements[index] : null;
    }

    /// <summary>
    /// Adds the given element into this collection. If a duplicate element is found, then either
    /// returns false, or an exception is thrown, as requested.
    /// </summary>
    /// <param name="element"></param>
    /// <param name="throwIfDuplicate"></param>
    public bool Add(T element, bool throwIfDuplicate = false)
    {
        element = Validator(element);

        if (Contains(element))
        {
            if (throwIfDuplicate) throw new DuplicateException(
                "Duplicate element found.")
                .WithData(element);

            return false;
        }

        _Elements.Add(element);
        return true;
    }

    void ICollection<T>.Add(T element) => Add(element);

    /// <summary>
    /// Adds the elements from the given range into this collection. If duplicate elements are
    /// found, then an exception is thrown if requested. Returns the actual number of elements
    /// added.
    /// </summary>
    /// <param name="range"></param>
    /// <param name="throwIfDuplicate"></param>
    /// <returns></returns>
    public int AddRange(IEnumerable<T> range, bool throwIfDuplicate = false)
    {
        range = range.ThrowIfNull();

        var count = 0;
        foreach (var element in range) if (Add(element, throwIfDuplicate)) count++;
        return count;
    }

    /// <summary>
    /// Inserts the given element into this collection at the given index. If a duplicate element
    /// is found, then either returns false, or an exception is thrown, as requested.
    /// </summary>
    /// <param name="index"></param>
    /// <param name="element"></param>
    /// <param name="throwIfDuplicate"></param>
    public bool Insert(int index, T element, bool throwIfDuplicate = false)
    {
        element = Validator(element);

        if (Contains(element))
        {
            if (throwIfDuplicate) throw new DuplicateException(
                "Duplicate element found.")
                .WithData(element);

            return false;
        }

        _Elements.Insert(index, element);
        return true;
    }

    void IList<T>.Insert(int index, T element) => Insert(index, element);

    /// <summary>
    /// Inserts the elements from the given range into this collection, starting at the given
    /// index. If duplicate elements are found, then an exception is thrown if requested. Returns
    /// the actual number of elements inserted.
    /// </summary>
    /// <param name="index"></param>
    /// <param name="range"></param>
    /// <param name="throwIfDuplicate"></param>
    /// <returns></returns>
    public int InsertRange(int index, IEnumerable<T> range, bool throwIfDuplicate = false)
    {
        range = range.ThrowIfNull();

        var count = 0;
        foreach (var element in range)
        {
            if (Insert(index, element, throwIfDuplicate)) count++;
            index++;
        }
        return count;
    }

    /// <summary>
    /// Removes from this collection the given element, or the first equivalent one. Returns true
    /// if an element was removed, or false otherwise.
    /// </summary>
    /// <param name="element"></param>
    /// <returns></returns>
    /// <exception cref="System.NullReferenceException"></exception>
    public bool Remove(T element)
    {
        element = Validator(element);

        var index = IndexOf(element); if (index >= 0)
        {
            _Elements.RemoveAt(index);
            return true;
        }
        return false;
    }

    /// <summary>
    /// Removes from this collection the element at the given index.
    /// </summary>
    /// <param name="index"></param>
    public void RemoveAt(int index) => _Elements.RemoveAt(index);

    /// <summary>
    /// Removes all elements from this collection.
    /// </summary>
    public void Clear() => _Elements.Clear();

    /// <summary>
    /// Gets a list with the shallow copy of the requested range of elements.
    /// </summary>
    /// <param name="index"></param>
    /// <param name="count"></param>
    /// <returns></returns>
    public List<T> GetRange(int index, int count) => _Elements.GetRange(index, count);

    /// <summary>
    /// Removes from this collection the requested range of elements.
    /// </summary>
    /// <param name="index"></param>
    /// <param name="count"></param>
    public void RemoveRange(int index, int count) => _Elements.RemoveRange(index, count);

    /// <summary>
    /// Removes from this collection the elements from the given range, or their equivalent
    /// ones. Returns the number of elements removed.
    /// </summary>
    /// <param name="range"></param>
    /// <returns>The actual number of elements removed.</returns>
    public int RemoveRange(IEnumerable<T> range)
    {
        range = range.ThrowIfNull();

        var count = 0;
        foreach (var element in range) if (Remove(element)) count++;
        return count;
    }

    /// <summary>
    /// Reverses the order of the elements in this collection.
    /// </summary>
    public void Reverse() => _Elements.Reverse();

    /// <summary>
    /// Returns a list with a shallow copy of all the elements in this collection.
    /// </summary>
    /// <returns>A list with the elements in this collection.</returns>
    public List<T> ToList() => _Elements.ToList();

    /// <summary>
    /// Returns an array with a shallow copy of all the elements in this collection.
    /// </summary>
    /// <returns>An array with the elements in this collection.</returns>
    public T[] ToArray() => _Elements.ToArray();

    /// <summary>
    /// Copies all the contents of this collection into the given array, starting at the given
    /// index in that array.
    /// </summary>
    /// <param name="array"></param>
    /// <param name="arrayIndex"></param>
    public void CopyTo(T[] array, int arrayIndex) => _Elements.CopyTo(array, arrayIndex);
}