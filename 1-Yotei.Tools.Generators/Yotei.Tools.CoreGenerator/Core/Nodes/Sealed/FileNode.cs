namespace Yotei.Tools.CoreGenerator;

// ========================================================
/// <summary>
/// Represents a file-alike top-most node in the source generation hierarchy.
/// </summary>
internal sealed class FileNode : INode
{
    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    /// <param name="name"></param>
    public FileNode(string name)
    {
        Name = name;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    public override string ToString() => $"File: {Name}";

    // ----------------------------------------------------

    /// <summary>
    /// The actual case-insensitive name of this namespace.
    /// </summary>
    public string Name { get; private set => field = value.NotNullNotEmpty(true); }

    /// <summary>
    /// The collection of child namespaces.
    /// </summary>
    public CustomList<NamespaceNode> ChildNamespaces = new()
    { AreEqual = (x, y) => string.Compare(x.Name, y.Name) == 0 };

    // ----------------------------------------------------

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="context"></param>
    /// <returns></returns>
    public bool Validate(SourceProductionContext context) => throw null;

    // ----------------------------------------------------

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="context"></param>
    /// <param name="cb"></param>
    public void Emit(SourceProductionContext context, CodeBuilder cb) => throw null;
}