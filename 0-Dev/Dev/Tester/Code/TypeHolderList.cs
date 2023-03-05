namespace Dev.Tester;

// ========================================================
/// <summary>
/// A collection of type holders.
/// </summary>
internal class TypeHolderList : IEnumerable<TypeHolder>
{
    readonly List<TypeHolder> Items = new();

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
    /// <inheritdoc/>
    /// </summary>
    /// <returns><inheritdoc/></returns>
    public IEnumerator<TypeHolder> GetEnumerator() => Items.GetEnumerator();
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
    public TypeHolder this[int index] => Items[index];

    /// <summary>
    /// Returns the holder for a type with the given full name, or null.
    /// </summary>
    /// <param name="typeFullName"></param>
    /// <returns></returns>
    public TypeHolder? Find(string typeFullName)
    {
        typeFullName = typeFullName.NotNullNotEmpty();
        return Items.Find(x => string.Compare(typeFullName, x.FullName) == 0);
    }

    /// <summary>
    /// Returns the holder for the given type, or null.
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    public TypeHolder? Find(Type type)
    {
        type = type.ThrowIfNull();

        return
            Items.Find(x => ReferenceEquals(x.Type, type)) ??
            Find(type.FullName!);
    }

    /// <summary>
    /// Adds the given item into this collection.
    /// </summary>
    /// <param name="item"></param>
    public void Add(TypeHolder item)
    {
        item = item.ThrowIfNull();

        if (Find(item.Type) != null ) throw new DuplicateException(
            $"Type '{item}'  is already in this collection.");

        Items.Add(item);
    }

    /// <summary>
    /// Removes the given item from this collection.
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    public bool Remove(TypeHolder item)
    {
        item = item.ThrowIfNull();
        return Items.Remove(item);
    }

    /// <summary>
    /// Clears this collection.
    /// </summary>
    public void Clear() => Items.Clear();
}