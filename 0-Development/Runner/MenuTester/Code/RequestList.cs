namespace Runner;

// ========================================================
/// <summary>
/// Represents a collection of explicit inclusion or exclusion request.
/// </summary>
public class RequestList : IEnumerable<Request>
{
    readonly List<Request> Items = [];

    /// <summary>
    /// Initializes a new empty instance.
    /// </summary>
    public RequestList() { }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns><inheritdoc/></returns>
    public override string ToString() => $"Count: {Count}";

    // ----------------------------------------------------

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns><inheritdoc/></returns>
    public IEnumerator<Request> GetEnumerator() => Items.GetEnumerator();
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
    public Request this[int index] => Items[index];

    /// <summary>
    /// Adds the given element to this collection.
    /// </summary>
    /// <param name="holder"></param>
    public void Add(Request holder)
    {
        holder.ThrowWhenNull();
        Items.Add(holder);
    }

    /// <summary>
    /// Removes the given element from this collection.
    /// </summary>
    /// <param name="holder"></param>
    /// <returns></returns>
    public bool Remove(Request holder)
    {
        holder.ThrowWhenNull();
        return Items.Remove(holder);
    }

    /// <summary>
    /// Clears this collection.
    /// </summary>
    public void Clear() => Items.Clear();
}