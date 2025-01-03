namespace Yotei.Tools.BaseGenerator;

// ========================================================
/// <summary>
/// Represents a collection of child methods.
/// </summary>
internal class ChildMethods : CustomList<MethodNode>
{
    /// <summary>
    /// Initializes a new empty instance.
    /// </summary>
    public ChildMethods() : base(new Comparer()) { }

    /// <summary>
    /// Initializes a new empty instance that uses the given comparer.
    /// </summary>
    /// <param name="comparer"></param>
    public ChildMethods(IEqualityComparer<MethodNode> comparer) : base(comparer) { }

    // ----------------------------------------------------

    /// <summary>
    /// A suitable comparer for collections of child elements.
    /// </summary>
    internal class Comparer : IEqualityComparer<MethodNode>
    {
        /// <inheritdoc/>
        public bool Equals(MethodNode x, MethodNode y)
        {
            return string.Compare(x.FileName, y.FileName, ignoreCase: true) == 0;
        }

        /// <inheritdoc/>
        public int GetHashCode(MethodNode obj) => throw new NotImplementedException();
    }
}