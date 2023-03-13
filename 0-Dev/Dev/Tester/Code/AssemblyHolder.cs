namespace Dev.Tester;

// ========================================================
/// <summary>
/// A holder for a given assembly.
/// </summary>
public class AssemblyHolder
{
    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    /// <param name="assembly"></param>
    public AssemblyHolder(Assembly assembly) => Assembly = assembly.ThrowIfNull();

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns><inheritdoc/></returns>
    public override string ToString() => Name;

    /// <summary>
    /// The assembly this instance refers to.
    /// </summary>
    public Assembly Assembly { get; }

    /// <summary>
    /// The name of this assembly.
    /// </summary>
    public string Name => Assembly.GetName().Name!;

    /// <summary>
    /// The collection of type holders in this instance.
    /// </summary>
    public TypeHolderList TypeHolders { get; } = new();

    /// <summary>
    /// Populates the collection of type holders.
    /// </summary>
    public void Populate()
    {
        var types = Assembly.DefinedTypes;
        foreach (var type in types)
        {
            if (!type.IsValid()) continue;

            var holder = new TypeHolder(type);
            TypeHolders.Add(holder);
            holder.Populate();
        }
    }

    /// <summary>
    /// Ensures that only the decorated elements are taken into consideration, if any is
    /// decorated.
    /// </summary>
    public void EnsureEnforced()
    {
        foreach (var typeHolder in TypeHolders) typeHolder.EnsureEnforced();
    }

    /// <summary>
    /// Purges not enforced elements.
    /// </summary>
    public void PurgeNotEnforced()
    {
        foreach (var typeHolder in TypeHolders) typeHolder.PurgeNotEnforced();

        var items = TypeHolders.Where(x => x.MethodHolders.Count == 0).ToList();
        foreach (var item in items) TypeHolders.Remove(item);
    }
}