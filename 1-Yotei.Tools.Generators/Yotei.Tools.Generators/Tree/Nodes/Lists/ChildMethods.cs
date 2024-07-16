namespace Yotei.Tools.Generators.Internal;

// ========================================================
/// <summary>
/// Represents a collection of child methods.
/// </summary>
internal class ChildMethods : CustomList<MethodNode>
{
    /// <summary>
    /// Initializes a new empty instance.
    /// </summary>
    public ChildMethods() : base(new CustomComparer()) { }

    /// <summary>
    /// Initializes a new empty instance that uses the given comparer.
    /// </summary>
    /// <param name="comparer"></param>
    public ChildMethods(IEqualityComparer<MethodNode> comparer) : base(comparer) { }

    // ----------------------------------------------------

    class CustomComparer : IEqualityComparer<MethodNode>
    {
        public bool Equals(MethodNode x, MethodNode y)
        {
            return SymbolComparer.Default.Equals(x.Symbol, y.Symbol);
        }
        public int GetHashCode(MethodNode obj) => throw new NotSupportedException();
    }
}