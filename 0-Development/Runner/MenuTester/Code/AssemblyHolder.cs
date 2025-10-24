namespace Runner;

// ========================================================
/// <summary>
/// Represents a test assembly.
/// </summary>
public class AssemblyHolder
{
    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    /// <param name="assembly"></param>
    public AssemblyHolder(Assembly assembly)
    {
        Assembly = assembly.ThrowWhenNull();
        _ = Name;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns><inheritdoc/></returns>
    public override string ToString() => Name;

    // ----------------------------------------------------

    /// <summary>
    /// The assembly this instance refers to.
    /// </summary>
    public Assembly Assembly { get; }

    /// <summary>
    /// The name of this assembly.
    /// </summary>
    public string Name => Assembly.GetName().Name
        ?? throw new InvalidOperationException("Assembly name is null.");

    /// <summary>
    /// The list of type holders in this instance.
    /// </summary>
    public TypeHolderList TypeHolders { get; } = [];
}