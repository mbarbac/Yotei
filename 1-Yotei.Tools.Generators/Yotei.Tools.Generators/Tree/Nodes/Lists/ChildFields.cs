namespace Yotei.Tools.Generators.Internal;

// ========================================================
/// <summary>
/// Represents a collection of child fields.
/// </summary>
internal class ChildFields : CustomList<FieldNode>
{
    /// <summary>
    /// Initializes a new empty instance.
    /// </summary>
    public ChildFields() : base(new CustomComparer()) { }

    /// <summary>
    /// Initializes a new empty instance that uses the given comparer.
    /// </summary>
    /// <param name="comparer"></param>
    public ChildFields(IEqualityComparer<FieldNode> comparer) : base(comparer) { }

    // ----------------------------------------------------

    class CustomComparer : IEqualityComparer<FieldNode>
    {
        public bool Equals(FieldNode x, FieldNode y)
        {
            return SymbolComparer.Default.Equals(x.Symbol, y.Symbol);
        }
        public int GetHashCode(FieldNode obj) => throw new NotSupportedException();
    }
}