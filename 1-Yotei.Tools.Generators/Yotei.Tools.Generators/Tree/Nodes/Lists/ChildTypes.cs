namespace Yotei.Tools.Generators.Internal;

// ========================================================
/// <summary>
/// Represents a collection of child types.
/// </summary>
internal class ChildTypes : CustomList<TypeNode>
{
    /// <summary>
    /// Initializes a new empty instance.
    /// </summary>
    public ChildTypes() : base(new CustomComparer()) { }

    /// <summary>
    /// Initializes a new empty instance that uses the given comparer.
    /// </summary>
    /// <param name="comparer"></param>
    public ChildTypes(IEqualityComparer<TypeNode> comparer) : base(comparer) { }

    // ----------------------------------------------------

    class CustomComparer : IEqualityComparer<TypeNode>
    {
        public bool Equals(TypeNode x, TypeNode y)
        {
            return SymbolComparer.Default.Equals(x.Symbol, y.Symbol);
        }
        public int GetHashCode(TypeNode obj) => throw new NotSupportedException();
    }
}