namespace Yotei.Tools.CoreGenerator;

// ========================================================
/// <summary>
/// Represents a namespace-alike node in source generation hierarchy.
/// </summary>
internal sealed class NamespaceNode : IChildNode
{
    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    /// <param name="parent"></param>
    /// <param name="name"></param>
    public NamespaceNode(INode parent, string name)
    {
        ParentNode = parent;
        Name = name;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    public override string ToString() => $"Namespace: {Name}";

    // ----------------------------------------------------

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public INode ParentNode
    {
        get;
        private set
        {
            field = value.ThrowWhenNull();
            if (field is not FileNode and not NamespaceNode) throw new ArgumentException(
                "Parent node is not a file nor a namespace.")
                .WithData(value);
        }
    }

    /// <summary>
    /// The actual case sensitive name of this namespace.
    /// </summary>
    public string Name { get; private set => field = value.NotNullNotEmpty(true); }

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