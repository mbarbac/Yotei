namespace Runner.Tester;

// ========================================================
/// <summary>
/// Represents a list-alike collection of test class holders.
/// </summary>
public class TypeHolderList : IEnumerable<TypeHolder>
{
    readonly List<TypeHolder> _Items = [];

    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    public TypeHolderList() { }

    /// <inheritdoc/>
    public override string ToString() => $"Count: {Count}";

    /// <inheritdoc/>
    public IEnumerator<TypeHolder> GetEnumerator() => _Items.GetEnumerator();
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
    public TypeHolder this[int index] => _Items[index];

    // ----------------------------------------------------

    /// <summary>
    /// Returns the holder whose type full name is given, or null if any can be found.
    /// </summary>
    /// <param name="fullName"></param>
    /// <returns></returns>
    public TypeHolder? Find(string fullName)
    {
        fullName = fullName.NotNullNotEmpty();
        return _Items.Find(x => string.Compare(fullName, x.FullName) == 0);
    }

    /// <summary>
    /// Returns the holder whose type is given, or null if any can be found.
    /// </summary>
    /// <param name="assembly"></param>
    /// <returns></returns>
    public TypeHolder? Find(Type type)
    {
        type.ThrowWhenNull();

        var name = type.FullName ?? throw new ArgumentException(
            "Cannot find the full name of the given type.")
            .WithData(type);

        return Find(name);
    }

    /// <summary>
    /// Adds the given holder to this collection, or throws a duplicate exception.
    /// </summary>
    /// <param name="holder"></param>
    public void Add(TypeHolder holder)
    {
        holder.ThrowWhenNull();

        if (Find(holder.FullName) != null) throw new DuplicateException(
            "This collection already contains a duplicated element.")
            .WithData(holder);

        _Items.Add(holder);
    }

    /// <summary>
    /// Removes the given holder from this collection.
    /// </summary>
    /// <param name="holder"></param>
    /// <returns></returns>
    public bool Remove(TypeHolder holder)
    {
        holder.ThrowWhenNull();
        return _Items.Remove(holder);
    }

    /// <summary>
    /// Clears this collection.
    /// </summary>
    public void Clear() => _Items.Clear();
}