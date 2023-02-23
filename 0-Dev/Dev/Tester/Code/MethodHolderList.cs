namespace Dev.Tester;

// ========================================================
/// <summary>
/// Represents a  collection of method holders.
/// </summary>
internal class MethodHolderList : IEnumerable<MethodHolder>
{
    readonly List<MethodHolder> _Items = new();

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
    public IEnumerator<MethodHolder> GetEnumerator() => _Items.GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    /// <summary>
    /// The number of elements in this collection.
    /// </summary>
    public int Count => _Items.Count;

    /// <summary>
    /// Returns the element stored at the given index.
    /// </summary>
    /// <param name="index"></param>
    /// <returns></returns>
    public MethodHolder this[int index] => _Items[index];

    /// <summary>
    /// Returns the index at which the given element is stored in this collection, or -1 if it is
    /// not found.
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    public int IndexOf(MethodHolder item)
    {
        item = item.ThrowIfNull();
        return _Items.IndexOf(item);
    }

    /// <summary>
    /// Returns the element in this collection that carries the given metadata, or whose metadata
    /// carries the name of the given one.
    /// </summary>
    /// <param name="method"></param>
    /// <returns></returns>
    public MethodHolder? Find(MethodInfo method)
    {
        method = method.ThrowIfNull();

        return
            _Items.Find(x => ReferenceEquals(method, x.MethodInfo)) ??
            Find(method.Name);
    }

    /// <summary>
    /// Returns the element in this collection whose metadata has the given name.
    /// </summary>
    /// <param name="method"></param>
    /// <returns></returns>
    public MethodHolder? Find(string methodName)
    {
        methodName = methodName.NotNullNotEmpty();
        return _Items.Find(x => string.Compare(methodName, x.Name) == 0);
    }

    /// <summary>
    /// Adds into this collection the given element.
    /// </summary>
    /// <param name="item"></param>
    public void Add(MethodHolder item)
    {
        item = item.ThrowIfNull();

        var temp = Find(item.MethodInfo);
        if (temp != null) throw new DuplicateException("Metadata already exists.");

        _Items.Add(item);
    }

    /// <summary>
    /// Adds into this collection a new element for the given metadata, or returns the existing
    /// one.
    /// </summary>
    /// <param name="method"></param>
    /// <returns></returns>
    public MethodHolder Add(MethodInfo method)
    {
        method = method.ThrowIfNull();

        var item = Find(method);
        if (item == null)
        {
            item = new(method);
            _Items.Add(item);
        }

        return item;
    }

    /// <summary>
    /// Removes from this collection the given element.
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    public bool Remove(MethodHolder item)
    {
        item = item.ThrowIfNull();
        return _Items.Remove(item);
    }

    /// <summary>
    /// Clears this collection.
    /// </summary>
    public void Clear() => _Items.Clear();
}