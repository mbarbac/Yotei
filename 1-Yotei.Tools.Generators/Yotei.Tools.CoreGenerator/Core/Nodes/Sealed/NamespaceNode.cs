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
    /// The actual case sensitive name of this namespace, without dots.
    /// </summary>
    public string Name
    {
        get;
        private set
        {
            value = value.NotNullNotEmpty(true);
            field = !value.Contains('.') ? value : throw new ArgumentException(
                "Namespace's name cannot contain dots.").WithData(value);
        }
    }

    /// <summary>
    /// The full name of this namespace.
    /// </summary>
    public string FullName => _FullName
        ??= (ParentNode is NamespaceNode parent ? $"{parent.Name}.{Name}" : Name);
    string? _FullName;

    /// <summary>
    /// The collection of child namespaces.
    /// </summary>
    public CustomList<NamespaceNode> ChildNamespaces = new()
    { AreEqual = (x, y) => string.Compare(x.Name, y.Name) == 0 };

    /// <summary>
    /// The collection of child types.
    /// </summary>
    public CustomList<TypeNode> ChildTypes = new()
    { AreEqual = (x, y) => SymbolEqualityComparer.Default.Equals(x.Symbol, y.Symbol) };

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