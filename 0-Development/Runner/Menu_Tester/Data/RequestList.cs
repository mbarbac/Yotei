namespace Runner.Tester;

// ========================================================
/// <summary>
/// Represents a list-alike collection of explicit requests.
/// </summary>
public class RequestList : IEnumerable<Request>
{
    readonly List<Request> _Items = [];

    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    public RequestList() { }

    /// <inheritdoc/>
    public override string ToString() => $"Count: {Count}";

    /// <inheritdoc/>
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
    /// Adds the given request to this collection, even if it is a duplicated one.
    /// </summary>
    /// <param name="holder"></param>
    public void Add(Request holder)
    {
        holder.ThrowWhenNull();
        _Items.Add(holder);
    }

    /// <summary>
    /// Removes the given holder from this collection.
    /// </summary>
    /// <param name="holder"></param>
    /// <returns></returns>
    public bool Remove(Request holder)
    {
        holder.ThrowWhenNull();
        return _Items.Remove(holder);
    }

    /// <summary>
    /// Clears this collection.
    /// </summary>
    public void Clear() => _Items.Clear();
}