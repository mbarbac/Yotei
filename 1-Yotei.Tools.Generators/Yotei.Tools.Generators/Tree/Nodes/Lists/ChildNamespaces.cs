namespace Yotei.Tools.Generators.Internal;

// ========================================================
/// <summary>
/// Represents a collection of child namespaces.
/// </summary>
internal class ChildNamespaces : CustomList<NamespaceNode>
{
    /// <summary>
    /// Initializes a new empty instance.
    /// </summary>
    public ChildNamespaces() : base(new CustomComparer()) { }

    /// <summary>
    /// Initializes a new empty instance that uses the given comparer.
    /// </summary>
    /// <param name="comparer"></param>
    public ChildNamespaces(IEqualityComparer<NamespaceNode> comparer) : base(comparer) { }

    // ----------------------------------------------------

    class CustomComparer : IEqualityComparer<NamespaceNode>
    {
        public bool Equals(NamespaceNode x, NamespaceNode y)
        {
            return x.Name == y.Name;
        }
        public int GetHashCode(NamespaceNode obj) => throw new NotSupportedException();
    }
}