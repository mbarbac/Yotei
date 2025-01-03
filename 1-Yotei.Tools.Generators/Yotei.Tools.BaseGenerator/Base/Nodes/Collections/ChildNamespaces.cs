namespace Yotei.Tools.BaseGenerator;

// ========================================================
/// <summary>
/// Represents a collection of child namespaces.
/// </summary>
internal class ChildNamespaces : CustomList<NamespaceNode>
{
    /// <summary>
    /// Initializes a new empty instance.
    /// </summary>
    public ChildNamespaces() : base(new Comparer()) { }

    /// <summary>
    /// Initializes a new empty instance that uses the given comparer.
    /// </summary>
    /// <param name="comparer"></param>
    public ChildNamespaces(IEqualityComparer<NamespaceNode> comparer) : base(comparer) { }

    // ----------------------------------------------------

    /// <summary>
    /// A suitable comparer for collections of child elements.
    /// </summary>
    internal class Comparer : IEqualityComparer<NamespaceNode>
    {
        /// <inheritdoc/>
        public bool Equals(NamespaceNode x, NamespaceNode y)
        {
            return x.Name == y.Name;
        }

        /// <inheritdoc/>
        public int GetHashCode(NamespaceNode obj) => throw new NotImplementedException();
    }
}