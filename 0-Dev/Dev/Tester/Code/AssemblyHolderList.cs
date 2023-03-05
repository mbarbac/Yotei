namespace Dev.Tester;

// ========================================================
/// <summary>
/// A collection of assembly holders.
/// </summary>
internal class AssemblyHolderList : IEnumerable<AssemblyHolder>
{
    readonly List<AssemblyHolder> Items = new();

    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    public AssemblyHolderList() { }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns><inheritdoc/></returns>
    public override string ToString() => $"Count:{Count}";

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns><inheritdoc/></returns>
    public IEnumerator<AssemblyHolder> GetEnumerator() => Items.GetEnumerator();
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
    public AssemblyHolder this[int index] => Items[index];

    /// <summary>
    /// Returns the holder an assembly with the given name, or null.
    /// </summary>
    /// <param name="assemblyName"></param>
    /// <returns></returns>
    public AssemblyHolder? Find(string assemblyName)
    {
        assemblyName = assemblyName.NotNullNotEmpty();
        return Items.Find(x => string.Compare(assemblyName, x.Name, ignoreCase: true) == 0);
    }

    /// <summary>
    /// Returns the holder for the given assembly, or null.
    /// </summary>
    /// <param name="assembly"></param>
    /// <returns></returns>
    public AssemblyHolder? Find(Assembly assembly)
    {
        assembly = assembly.ThrowIfNull();

        return
            Items.Find(x => ReferenceEquals(x.Assembly, assembly)) ??
            Find(assembly.GetName().Name!);
    }

    /// <summary>
    /// Adds the given item into this collection.
    /// </summary>
    /// <param name="item"></param>
    public void Add(AssemblyHolder item)
    {
        item = item.ThrowIfNull();

        if (Find(item.Assembly) != null) throw new DuplicateException(
            $"Assembly '{item}' is already in this collection.");

        Items.Add(item);
    }

    /// <summary>
    /// Removes the given item from this collection.
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    public bool Remove(AssemblyHolder item)
    {
        item = item.ThrowIfNull();
        return Items.Remove(item);
    }

    /// <summary>
    /// Clears this collection.
    /// </summary>
    public void Clear() => Items.Clear();
}