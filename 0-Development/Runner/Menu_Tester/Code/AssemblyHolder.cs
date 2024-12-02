namespace Runner.Tester;

// ========================================================
/// <summary>
/// Represents a holder for a test assembly.
/// </summary>
public class AssemblyHolder
{
    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    /// <param name="assembly"></param>
    public AssemblyHolder(Assembly assembly) => Assembly = assembly.ThrowWhenNull();

    /// <inheritdoc/>
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
    /// The list of type holders in this instance.
    /// </summary>
    public TypeHolderList TypeHolders { get; } = [];

    // ----------------------------------------------------

    /// <summary>
    /// Invoked to populate this instance with its test classes and method.
    /// </summary>
    public void Populate()
    {
        foreach (var type in Assembly.DefinedTypes)
        {
            if (!TypeHolder.IsValidTestClass(type)) continue;

            if (TypeHolders.Find(type) != null)
            {
                var holder = new TypeHolder(type);
                holder.Populate();

                if (holder.MethodHolders.Count > 0) TypeHolders.Add(holder);
            }
        }
    }
}