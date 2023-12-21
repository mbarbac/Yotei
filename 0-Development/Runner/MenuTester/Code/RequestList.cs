namespace Runner.Tester;

// ========================================================
/// <summary>
/// Represents a list-alike collection of requests.
/// </summary>
public class RequestList : IEnumerable<Request>
{
    readonly List<Request> _Items = [];

    /// <summary>
    /// Initializes a new empty instance.
    /// </summary>
    public RequestList() { }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    public override string ToString() => $"Count: {Count}";

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    public IEnumerator<Request> GetEnumerator() => _Items.GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    /// <summary>
    /// Gets the number of elements in this collection.
    /// </summary>
    public int Count => _Items.Count;

    /// <summary>
    /// Gets the element stored at the given index.
    /// </summary>
    /// <param name="index"></param>
    /// <returns></returns>
    public Request this[int index] => _Items[index];

    // ----------------------------------------------------

    /// <summary>
    /// Adds the given item to this collection.
    /// </summary>
    /// <param name="item"></param>
    public void Add(Request item)
    {
        item = item.ThrowWhenNull();
        _Items.Add(item);
    }

    /// <summary>
    /// Removes the given item from this collection.
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    public bool Remove(Request item)
    {
        item = item.ThrowWhenNull();
        return _Items.Remove(item);
    }

    /// <summary>
    /// Clears this collection.
    /// </summary>
    public void Clear() => _Items.Clear();
}