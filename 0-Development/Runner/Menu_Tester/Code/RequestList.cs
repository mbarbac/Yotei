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

    /*
    /// <summary>
    /// Returns the holder whose name is given, or null if any can be found.
    /// </summary>
    /// <param name="methodName"></param>
    /// <returns></returns>
    public Request? Find(string methodName)
    {
        methodName = methodName.NotNullNotEmpty();
        return _Items.Find(x => string.Compare(methodName, x.Name) == 0);
    }

    /// <summary>
    /// Returns the holder whose method is given, or null if any can be found.
    /// </summary>
    /// <param name="assembly"></param>
    /// <returns></returns>
    public Request? Find(MethodInfo method)
    {
        method.ThrowWhenNull();
        return Find(method.Name);
    }

    /// <summary>
    /// Adds the given holder to this collection, or throws a duplicate exception.
    /// </summary>
    /// <param name="holder"></param>
    public void Add(Request holder)
    {
        holder.ThrowWhenNull();

        if (Find(holder.Name) != null) throw new DuplicateException(
            "This collection already contains a duplicated element.")
            .WithData(holder);

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
    */

    /// <summary>
    /// Clears this collection.
    /// </summary>
    public void Clear() => _Items.Clear();
}