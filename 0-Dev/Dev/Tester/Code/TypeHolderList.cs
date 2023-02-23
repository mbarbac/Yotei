namespace Dev.Tester;

// ========================================================
/// <summary>
/// Represents a  collection of type holders.
/// </summary>
internal class TypeHolderList : IEnumerable<TypeHolder>
{
    readonly List<TypeHolder> _Items = new();

    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    public TypeHolderList() { }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns><inheritdoc/></returns>
    public override string ToString() => $"Count:{Count}";

    /// <summary>
    /// Returns an enumerator that can iterate through the elements in this collection.
    /// </summary>
    /// <returns></returns>
    public IEnumerator<TypeHolder> GetEnumerator() => _Items.GetEnumerator();
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
    public TypeHolder this[int index] => _Items[index];

    /// <summary>
    /// Returns the index at which the given element is stored in this collection, or -1 if it is
    /// not found.
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    public int IndexOf(TypeHolder item)
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
    public TypeHolder? Find(Type type)
    {
        type = type.ThrowIfNull();

        return
            _Items.Find(x => ReferenceEquals(type, x.Type)) ??
            Find(type.Name);
    }

    /// <summary>
    /// Returns the element in this collection whose metadata has the given name.
    /// </summary>
    /// <param name="method"></param>
    /// <returns></returns>
    public TypeHolder? Find(string typeName)
    {
        typeName = typeName.NotNullNotEmpty();
        return _Items.Find(x => string.Compare(typeName, x.Name) == 0);
    }

    /// <summary>
    /// Adds into this collection the given element.
    /// </summary>
    /// <param name="item"></param>
    public void Add(TypeHolder item)
    {
        item = item.ThrowIfNull();

        var temp = Find(item.Type);
        if (temp != null) throw new DuplicateException("Metadata already exists.");

        _Items.Add(item);
    }

    /// <summary>
    /// Adds into this collection a new element for the given metadata, or returns the existing
    /// one.
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    public TypeHolder Add(Type type)
    {
        type = type.ThrowIfNull();

        var item = Find(type);
        if (item == null)
        {
            item = new(type);
            _Items.Add(item);
        }

        return item;
    }

    /// <summary>
    /// Removes from this collection the given element.
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    public bool Remove(TypeHolder item)
    {
        item = item.ThrowIfNull();
        return _Items.Remove(item);
    }

    /// <summary>
    /// Clears this collection.
    /// </summary>
    public void Clear() => _Items.Clear();
}