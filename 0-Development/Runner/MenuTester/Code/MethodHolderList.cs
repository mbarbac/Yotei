namespace Runner;

// ========================================================
/// <summary>
/// Represents a collection of test methods.
/// </summary>
public class MethodHolderList : IEnumerable<MethodHolder>
{
    readonly List<MethodHolder> Items = [];

    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    public MethodHolderList() { }

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
    public IEnumerator<MethodHolder> GetEnumerator() => Items.GetEnumerator();
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
    public MethodHolder this[int index] => Items[index];

    /// <summary>
    /// Returns the holder whose name is given, or null if any can be found in this collection.
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    public MethodHolder? Find(string name)
    {
        name = name.NotNullNotEmpty(true);
        return Items.Find(x => string.Compare(name, x.Name) == 0);
    }

    /// <summary>
    /// Returns the holder whose case sensitive name matches the one of the given method, or
    /// null if any can be found in this collection.
    /// </summary>
    /// <param name="method"></param>
    /// <returns></returns>
    public MethodHolder? Find(MethodInfo method)
    {
        method.ThrowWhenNull();

        var name = method.Name;
        return Find(name);
    }

    /// <summary>
    /// Adds the given element to this collection.
    /// </summary>
    /// <param name="holder"></param>
    public void Add(MethodHolder holder)
    {
        holder.ThrowWhenNull();

        if (Find(holder.Name) != null) throw new DuplicateException(
            "This collection already contains a duplicated element.")
            .WithData(holder);

        Items.Add(holder);
    }

    /// <summary>
    /// Removes the given element from this collection.
    /// </summary>
    /// <param name="holder"></param>
    /// <returns></returns>
    public bool Remove(MethodHolder holder)
    {
        holder.ThrowWhenNull();
        return Items.Remove(holder);
    }

    /// <summary>
    /// Clears this collection.
    /// </summary>
    public void Clear() => Items.Clear();
}