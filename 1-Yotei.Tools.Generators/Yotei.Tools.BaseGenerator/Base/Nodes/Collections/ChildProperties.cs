namespace Yotei.Tools.BaseGenerator;

// ========================================================
/// <summary>
/// Represents a collection of child properties.
/// </summary>
internal class ChildProperties : CustomList<PropertyNode>
{
    /// <summary>
    /// Initializes a new empty instance.
    /// </summary>
    public ChildProperties() : base(new Comparer()) { }

    /// <summary>
    /// Initializes a new empty instance that uses the given comparer.
    /// </summary>
    /// <param name="comparer"></param>
    public ChildProperties(IEqualityComparer<PropertyNode> comparer) : base(comparer) { }

    // ----------------------------------------------------

    /// <summary>
    /// A suitable comparer for collections of child elements.
    /// </summary>
    internal class Comparer : IEqualityComparer<PropertyNode>
    {
        /// <inheritdoc/>
        public bool Equals(PropertyNode x, PropertyNode y)
        {
            return SymbolComparer.Default.Equals(x.Symbol, y.Symbol);
        }

        /// <inheritdoc/>
        public int GetHashCode(PropertyNode obj) => throw new NotImplementedException();
    }
}