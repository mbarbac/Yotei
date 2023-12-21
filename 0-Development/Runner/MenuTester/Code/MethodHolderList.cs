namespace Runner.Tester;

// ========================================================
/// <summary>
/// Represents a list-alike collection of method holders.
/// </summary>
public class MethodHolderList : IEnumerable<MethodHolder>
{
    readonly List<MethodHolder> _Items = [];

    /// <summary>
    /// Initializes a new empty instance.
    /// </summary>
    public MethodHolderList() { }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    public override string ToString() => $"Count: {Count}";

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    public IEnumerator<MethodHolder> GetEnumerator() => _Items.GetEnumerator();
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
    public MethodHolder this[int index] => _Items[index];

    // ----------------------------------------------------

    /// <summary>
    /// Returns the holder whose method name is given, or null if any.
    /// </summary>
    /// <param name="methodName"></param>
    /// <returns></returns>
    public MethodHolder? Find(string methodName)
    {
        methodName = methodName.NotNullNotEmpty();
        return _Items.Find(x => string.Compare(methodName, x.Name) == 0);
    }

    /// <summary>
    /// Returns the holder whose metadata is given, or null if any.
    /// </summary>
    /// <param name="method"></param>
    /// <returns></returns>
    public MethodHolder? Find(MethodInfo method)
    {
        method = method.ThrowWhenNull();
        return Find(method.Name);
    }

    /// <summary>
    /// Adds the given holder to this collection.
    /// </summary>
    /// <param name="holder"></param>
    public void Add(MethodHolder holder)
    {
        holder = holder.ThrowWhenNull();

        if (Find(holder.Method) != null) throw new DuplicateException(
            "This collection already contains the given metadata.")
            .WithData(holder);

        _Items.Add(holder);
    }

    /// <summary>
    /// Removes the given holder from this collection.
    /// </summary>
    /// <param name="holder"></param>
    /// <returns></returns>
    public bool Remove(MethodHolder holder)
    {
        holder = holder.ThrowWhenNull();
        return _Items.Remove(holder);
    }

    /// <summary>
    /// Clears this collection.
    /// </summary>
    public void Clear() => _Items.Clear();
}