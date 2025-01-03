namespace Yotei.Tools.BaseGenerator;

// ========================================================
/// <summary>
/// Represents a collection of child fields.
/// </summary>
internal class ChildFields : CustomList<FieldNode>
{
    /// <summary>
    /// Initializes a new empty instance.
    /// </summary>
    public ChildFields() : base(new Comparer()) { }

    /// <summary>
    /// Initializes a new empty instance that uses the given comparer.
    /// </summary>
    /// <param name="comparer"></param>
    public ChildFields(IEqualityComparer<FieldNode> comparer) : base(comparer) { }

    // ----------------------------------------------------

    /// <summary>
    /// A suitable comparer for collections of child elements.
    /// </summary>
    internal class Comparer : IEqualityComparer<FieldNode>
    {
        /// <inheritdoc/>
        public bool Equals(FieldNode x, FieldNode y)
        {
            return SymbolComparer.Default.Equals(x.Symbol, y.Symbol);
        }

        /// <inheritdoc/>
        public int GetHashCode(FieldNode obj) => throw new NotImplementedException();
    }
}