using System.Net.Http.Headers;

namespace Yotei.Tools.Generators.Internal;

// ========================================================
/// <summary>
/// Represents an ordered collection of elements that prevents null or duplicated ones.
/// </summary>
/// <typeparam name="T"></typeparam>
internal class ChildList<T> : IEnumerable<T>
{
    readonly List<T> Items = [];

    /// <inheritdoc/>
    public override string ToString() => $"Count: {Count}";

    /// <inheritdoc/>
    public IEnumerator<T> GetEnumerator() => Items.GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    /// <summary>
    /// Gets the number of elements in this collection.
    /// </summary>
    public int Count => Items.Count;

    /// <summary>
    /// Gets the element stored at the given index.
    /// </summary>
    /// <param name="index"></param>
    /// <returns></returns>
    public T this[int index]
    {
        get => Items[index];
        set
        {
            value.ThrowWhenNull();

            var temp = IndexOf(value);
            if (temp == index) return;

            if (Items.Contains(value)) throw new ArgumentException(
                "The given element is already in this collection.")
                .WithData(value);

            Items[index] = value;
        }
    }

    /// <summary>
    /// Returns the index of the given element in this collection, or -1 if it is not found.
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    public int IndexOf(T item)
    {
        item.ThrowWhenNull();
        return Items.IndexOf(item);
    }

    /// <summary>
    /// Returns the first ocurrence of an element that matches the given predicate, or null if
    /// any can be found.
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    public T? Find(Predicate<T> predicate)
    {
        predicate.ThrowWhenNull();
        return Items.Find(predicate);
    }

    /// <summary>
    /// Returns the last ocurrence of an element that matches the given predicate, or null if
    /// any can be found.
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    public T? FindLast(Predicate<T> predicate)
    {
        predicate.ThrowWhenNull();
        return Items.FindLast(predicate);
    }

    /// <summary>
    /// Returns all the ocurrences of elements that match the given predicate.
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    public List<T> FindAll(Predicate<T> predicate)
    {
        predicate.ThrowWhenNull();
        return Items.FindAll(predicate);
    }

    /// <summary>
    /// Adds the given object to this collection. If requested, throws an exception if the given
    /// element is already present in this collection, otherwise the element is ignored.
    /// </summary>
    /// <param name="item"></param>
    /// <param name="raiseDuplicates"></param>
    public void Add(T item, bool raiseDuplicates = true)
    {
        item.ThrowWhenNull();

        if (Items.Contains(item))
        {
            if (raiseDuplicates) throw new ArgumentException(
                "The given element is already in this collection.")
                .WithData(item);

            return;
        }

        Items.Add(item);
    }

    /// <summary>
    /// Adds the elements from the given range to this collection. If requested, throws an
    /// exception if any is already present in this collection, otherwise the element is
    /// ignored.
    /// </summary>
    /// <param name="range"></param>
    public void AddRange(IEnumerable<T> range, bool raiseDuplicates = true)
    {
        range.ThrowWhenNull();

        foreach (var item in range) Add(item, raiseDuplicates);
    }

    /// <summary>
    /// Removes the given item from this collection.
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    public bool Remove(T item)
    {
        item.ThrowWhenNull();
        return Items.Remove(item);
    }

    /// <summary>
    /// Clears this collection.
    /// </summary>
    public void Clear() => Items.Clear();

    /// <summary>
    /// Returns the underlying storage of the elements in this collection, for performance
    /// purposes. Any manipulation of this storage is at the caller's risk.
    /// </summary>
    /// <returns></returns>
    public List<T> AsList() => Items;

    /// <summary>
    /// Returns a new list with the elements in this collection.
    /// </summary>
    /// <returns></returns>
    public List<T> ToList() => new(Items);

    /// <summary>
    /// Returns a new array with the elements in this collection.
    /// </summary>
    /// <returns></returns>
    public T[] ToArray() => Items.ToArray();
}