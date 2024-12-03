namespace Runner.Tester;

// ========================================================
/// <summary>
/// Represents a list-alike collection of test assembly holders.
/// </summary>
public class AssemblyHolderList : IEnumerable<AssemblyHolder>
{
    readonly List<AssemblyHolder> _Items = [];

    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    public AssemblyHolderList() { }

    /// <inheritdoc/>
    public override string ToString() => $"Count: {Count}";

    /// <inheritdoc/>
    public IEnumerator<AssemblyHolder> GetEnumerator() => _Items.GetEnumerator();
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
    public AssemblyHolder this[int index] => _Items[index];

    // ----------------------------------------------------

    /// <summary>
    /// Returns the holder whose case insensitive name is given, or null if any can be found.
    /// </summary>
    /// <param name="assemblyName"></param>
    /// <returns></returns>
    public AssemblyHolder? Find(string assemblyName)
    {
        assemblyName = assemblyName.NotNullNotEmpty();
        return _Items.Find(x => string.Compare(assemblyName, x.Name, ignoreCase: true) == 0);
    }

    /// <summary>
    /// Returns the holder whose assembly is given, or null if any can be found.
    /// </summary>
    /// <param name="assembly"></param>
    /// <returns></returns>
    public AssemblyHolder? Find(Assembly assembly)
    {
        assembly.ThrowWhenNull();

        var name = assembly.GetName().Name ?? throw new ArgumentException(
            "Assembly has not a valid name.")
            .WithData(assembly);

        return Find(name);
    }

    /// <summary>
    /// Adds the given holder to this collection, or throws a duplicate exception.
    /// </summary>
    /// <param name="holder"></param>
    public void Add(AssemblyHolder holder)
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
    public bool Remove(AssemblyHolder holder)
    {
        holder.ThrowWhenNull();
        return _Items.Remove(holder);
    }

    /// <summary>
    /// Clears this collection.
    /// </summary>
    public void Clear() => _Items.Clear();
}