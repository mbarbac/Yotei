namespace Yotei.Tools.BaseGenerator;

// ========================================================
/// <summary>
/// Represents a collection of child types.
/// </summary>
internal class ChildTypes : CustomList<TypeNode>
{
    /// <summary>
    /// Initializes a new empty instance.
    /// </summary>
    public ChildTypes() : base(new Comparer()) { }

    /// <summary>
    /// Initializes a new empty instance that uses the given comparer.
    /// </summary>
    /// <param name="comparer"></param>
    public ChildTypes(IEqualityComparer<TypeNode> comparer) : base(comparer) { }

    // ----------------------------------------------------

    /// <summary>
    /// A suitable comparer for collections of child elements.
    /// </summary>
    internal class Comparer : IEqualityComparer<TypeNode>
    {
        /// <inheritdoc/>
        public bool Equals(TypeNode x, TypeNode y)
        {
            return SymbolComparer.Default.Equals(x.Symbol, y.Symbol);
        }

        /// <inheritdoc/>
        public int GetHashCode(TypeNode obj) => throw new NotImplementedException();
    }
}