namespace Runner.Tester;

// ========================================================
/// <summary>
/// Represents a list-alike collection of type holders.
/// </summary>
public class TypeHolderList : IEnumerable<TypeHolder>
{
    readonly List<TypeHolder> _Items = new();

    /// <summary>
    /// Initializes a new empty instance.
    /// </summary>
    public TypeHolderList() { }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    public override string ToString() => $"Count: {Count}";

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
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
    /// Returns the holder whose full type name is given, or null if any.
    /// </summary>
    /// <param name="typeFullName"></param>
    /// <returns></returns>
    public TypeHolder? Find(string typeFullName)
    {
        typeFullName = typeFullName.NotNullNotEmpty();
        return _Items.Find(x => string.Compare(typeFullName, x.FullName) == 0);
    }

    /// <summary>
    /// Returns the holder whose metadata is given, or null if any.
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    public TypeHolder? Find(Type type)
    {
        type = type.ThrowWhenNull();

        if (type.FullName == null) throw new ArgumentException(
            "Type has not a valid full name.")
            .WithData(type);

        return Find(type.FullName);
    }

    /// <summary>
    /// Adds the given holder to this collection.
    /// </summary>
    /// <param name="holder"></param>
    public void Add(TypeHolder holder)
    {
        holder = holder.ThrowWhenNull();

        if (Find(holder.Type) != null) throw new DuplicateException(
            "This collection already contains the given metadata.")
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
        holder = holder.ThrowWhenNull();
        return _Items.Remove(holder);
    }

    /// <summary>
    /// Clears this collection.
    /// </summary>
    public void Clear() => _Items.Clear();
}