namespace Dev.Tester;

// ========================================================
/// <summary>
/// Represents a  collection of assembly holders.
/// </summary>
internal class AssemblyHolderList : IEnumerable<AssemblyHolder>
{
    readonly List<AssemblyHolder> _Items = new();

    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    public AssemblyHolderList() { }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public override string ToString() => $"Count:{Count}";

    /// <summary>
    /// Returns an enumerator that can iterate through the elements in this collection.
    /// </summary>
    /// <returns></returns>
    public IEnumerator<AssemblyHolder> GetEnumerator() => _Items.GetEnumerator();
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
    public AssemblyHolder this[int index] => _Items[index];

    /// <summary>
    /// Returns the index at which the given element is stored in this collection, or -1 if it is
    /// not found.
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    public int IndexOf(AssemblyHolder item)
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
    public AssemblyHolder? Find(Assembly assembly)
    {
        assembly = assembly.ThrowIfNull();

        return
            _Items.Find(x => ReferenceEquals(assembly, x.Assembly)) ??
            Find(assembly.GetName().Name!);
    }

    /// <summary>
    /// Returns the element in this collection whose metadata has the given name.
    /// </summary>
    /// <param name="method"></param>
    /// <returns></returns>
    public AssemblyHolder? Find(string assemblyName)
    {
        assemblyName = assemblyName.NotNullNotEmpty();
        return _Items.Find(x => string.Compare(assemblyName, x.Name, ignoreCase: true) == 0);
    }

    /// <summary>
    /// Adds into this collection the given element.
    /// </summary>
    /// <param name="item"></param>
    public void Add(AssemblyHolder item)
    {
        item = item.ThrowIfNull();

        var temp = Find(item.Assembly);
        if (temp != null) throw new DuplicateException("Metadata already exists.");

        _Items.Add(item);
    }

    /// <summary>
    /// Adds into this collection a new element for the given metadata, or returns the existing
    /// one.
    /// </summary>
    /// <param name="assembly"></param>
    /// <returns></returns>
    public AssemblyHolder Add(Assembly assembly)
    {
        assembly = assembly.ThrowIfNull();

        var item = Find(assembly);
        if (item == null)
        {
            item = new(assembly);
            _Items.Add(item);
        }

        return item;
    }

    /// <summary>
    /// Removes from this collection the given element.
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    public bool Remove(AssemblyHolder item)
    {
        item = item.ThrowIfNull();
        return _Items.Remove(item);
    }

    /// <summary>
    /// Clears this collection.
    /// </summary>
    public void Clear() => _Items.Clear();
}