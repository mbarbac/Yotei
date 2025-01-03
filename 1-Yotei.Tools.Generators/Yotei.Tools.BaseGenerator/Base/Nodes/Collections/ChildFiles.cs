namespace Yotei.Tools.BaseGenerator;

// ========================================================
/// <summary>
/// Represents a collection of child files.
/// </summary>
internal class ChildFiles : CustomList<FileNode>
{
    /// <summary>
    /// Initializes a new empty instance.
    /// </summary>
    public ChildFiles() : base(new Comparer()) { }

    /// <summary>
    /// Initializes a new empty instance that uses the given comparer.
    /// </summary>
    /// <param name="comparer"></param>
    public ChildFiles(IEqualityComparer<FileNode> comparer) : base(comparer) { }

    // ----------------------------------------------------

    /// <summary>
    /// A suitable comparer for collections of child elements.
    /// </summary>
    internal class Comparer : IEqualityComparer<FileNode>
    {
        /// <inheritdoc/>
        public bool Equals(FileNode x, FileNode y)
        {
            return string.Compare(x.FileName, y.FileName, ignoreCase: true) == 0;
        }

        /// <inheritdoc/>
        public int GetHashCode(FileNode obj) => throw new NotImplementedException();
    }
}