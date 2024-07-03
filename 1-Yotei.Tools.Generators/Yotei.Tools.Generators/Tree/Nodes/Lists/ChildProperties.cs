namespace Yotei.Tools.Generators.Internal;

// ========================================================
/// <summary>
/// Represents a collection of child properties.
/// </summary>
internal class ChildProperties : CustomList<PropertyNode>
{
    /// <summary>
    /// Initializes a new empty instance.
    /// </summary>
    public ChildProperties() : base(new CustomComparer()) { }

    /// <summary>
    /// Initializes a new empty instance that uses the given comparer.
    /// </summary>
    /// <param name="comparer"></param>
    public ChildProperties(IEqualityComparer<PropertyNode> comparer) : base(comparer) { }

    // ----------------------------------------------------

    class CustomComparer : IEqualityComparer<PropertyNode>
    {
        public bool Equals(PropertyNode x, PropertyNode y)
        {
            return SymbolEqualityComparer.Default.Equals(x.Symbol, y.Symbol);
        }
        public int GetHashCode(PropertyNode obj) => throw new NotSupportedException();
    }
}