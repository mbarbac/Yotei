namespace Dev.Tester;

// ========================================================
/// <summary>
/// A collection of method holders.
/// </summary>
public class MethodHolderList : IEnumerable<MethodHolder>
{
    readonly List<MethodHolder> Items = new();

    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    public MethodHolderList() { }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns><inheritdoc/></returns>
    public override string ToString() => $"Count:{Count}";

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns><inheritdoc/></returns>
    public IEnumerator<MethodHolder> GetEnumerator() => Items.GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    /// <summary>
    /// The number of elements in this collection.
    /// </summary>
    public int Count => Items.Count;

    /// <summary>
    /// Returns the element stored at the given index.
    /// </summary>
    /// <param name="index"></param>
    /// <returns></returns>
    public MethodHolder this[int index] => Items[index];

    /// <summary>
    /// Returns the holder for a method with the given name, or null.
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    public MethodHolder? Find(string name)
    {
        name = name.NotNullNotEmpty();
        return Items.Find(x => string.Compare(name, x.Name) == 0);
    }

    /// <summary>
    /// Returns the holder for the given method, or null.
    /// </summary>
    /// <param name="method"></param>
    /// <returns></returns>
    public MethodHolder? Find(MethodInfo method)
    {
        method = method.ThrowIfNull();

        return
            Items.Find(x => ReferenceEquals(x.Method, method)) ??
            Find(method.Name);
    }

    /// <summary>
    /// Adds the given item into this collection.
    /// </summary>
    /// <param name="item"></param>
    public void Add(MethodHolder item)
    {
        item = item.ThrowIfNull();

        if (Find(item.Method) != null) throw new DuplicateException(
            $"Method '{item}'  is already in this collection.");

        Items.Add(item);
    }

    /// <summary>
    /// Removes the given item from this collection.
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    public bool Remove(MethodHolder item)
    {
        item = item.ThrowIfNull();
        return Items.Remove(item);
    }

    /// <summary>
    /// Clears this collection.
    /// </summary>
    public void Clear() => Items.Clear();
}