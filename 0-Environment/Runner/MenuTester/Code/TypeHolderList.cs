namespace Runner;

// ========================================================
/// <summary>
/// Represents a collection of test types.
/// </summary>
public class TypeHolderList : IEnumerable<TypeHolder>
{
    readonly List<TypeHolder> Items = [];

    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    public TypeHolderList() { }

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
    public IEnumerator<TypeHolder> GetEnumerator() => Items.GetEnumerator();
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
    public TypeHolder this[int index] => Items[index];

    /// <summary>
    /// Returns the holder whose full name is given, or null if any can be found in this
    /// collection.
    /// </summary>
    /// <param name="fullName"></param>
    /// <returns></returns>
    public TypeHolder? Find(string fullName)
    {
        fullName = fullName.NotNullNotEmpty(true);
        return Items.Find(x => string.Compare(fullName, x.Name) == 0);
    }

    /// <summary>
    /// Returns the holder whose case sensitive name matches the one of the given type, or
    /// null if any can be found in this collection.
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    public TypeHolder? Find(Type type)
    {
        type.ThrowWhenNull();

        var name = type.FullName ?? throw new ArgumentNullException(nameof(type));
        return Find(name);
    }

    /// <summary>
    /// Adds the given element to this collection.
    /// </summary>
    /// <param name="holder"></param>
    public void Add(TypeHolder holder)
    {
        holder.ThrowWhenNull();

        if (Find(holder.FullName) != null) throw new DuplicateException(
            "This collection already contains a duplicated element.")
            .WithData(holder);

        Items.Add(holder);
    }

    /// <summary>
    /// Removes the given element from this collection.
    /// </summary>
    /// <param name="holder"></param>
    /// <returns></returns>
    public bool Remove(TypeHolder holder)
    {
        holder.ThrowWhenNull();
        return Items.Remove(holder);
    }

    /// <summary>
    /// Clears this collection.
    /// </summary>
    public void Clear() => Items.Clear();
}