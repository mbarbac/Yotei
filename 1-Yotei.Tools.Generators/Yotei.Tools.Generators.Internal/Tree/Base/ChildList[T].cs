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
    public T this[int index] => Items[index];

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
    /// Adds the given object to this collection. Throws an exception if the given element is
    /// already present in this collection.
    /// </summary>
    /// <param name="item"></param>
    public void Add(T item)
    {
        item.ThrowWhenNull();

        if (Items.Contains(item)) throw new ArgumentException(
            "The given element is already in this collection.")
            .WithData(item);

        Items.Add(item);
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
}