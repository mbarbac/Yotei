namespace Runner;

// ========================================================
/// <summary>
/// Represents a collection of test assemblies.
/// </summary>
public class AssemblyHolderList : IEnumerable<AssemblyHolder>
{
    readonly List<AssemblyHolder> Items = [];

    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    public AssemblyHolderList() { }

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
    public IEnumerator<AssemblyHolder> GetEnumerator() => Items.GetEnumerator();
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
    public AssemblyHolder this[int index] => Items[index];

    /// <summary>
    /// Returns the holder whose case insentive name is given, or null if any can be found in this
    /// collection.
    /// </summary>
    /// <param name="assemblyName"></param>
    /// <returns></returns>
    public AssemblyHolder? Find(string assemblyName)
    {
        assemblyName = assemblyName.NotNullNotEmpty(true);
        return Items.Find(x => string.Compare(assemblyName, x.Name, ignoreCase: true) == 0);
    }

    /// <summary>
    /// Returns the holder whose case insentive name matches the one of the given assembly, or
    /// null if any can be found in this collection.
    /// </summary>
    /// <param name="assembly"></param>
    /// <returns></returns>
    public AssemblyHolder? Find(Assembly assembly)
    {
        assembly.ThrowWhenNull();

        var name = assembly.GetName().Name ?? throw new ArgumentNullException(nameof(assembly));
        return Find(name);
    }

    /// <summary>
    /// Adds the given element to this collection.
    /// </summary>
    /// <param name="holder"></param>
    public void Add(AssemblyHolder holder)
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
    public bool Remove(AssemblyHolder holder)
    {
        holder.ThrowWhenNull();
        return Items.Remove(holder);
    }

    /// <summary>
    /// Clears this collection.
    /// </summary>
    public void Clear() => Items.Clear();
}