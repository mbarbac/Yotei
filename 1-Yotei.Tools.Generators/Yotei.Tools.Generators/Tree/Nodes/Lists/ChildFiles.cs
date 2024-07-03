namespace Yotei.Tools.Generators.Internal;

// ========================================================
/// <summary>
/// Represents a collection of child files.
/// </summary>
internal class ChildFiles : CustomList<FileNode>
{
    /// <summary>
    /// Initializes a new empty instance.
    /// </summary>
    public ChildFiles() : base(new CustomComparer()) { }

    /// <summary>
    /// Initializes a new empty instance that uses the given comparer.
    /// </summary>
    /// <param name="comparer"></param>
    public ChildFiles(IEqualityComparer<FileNode> comparer) : base(comparer) { }

    // ----------------------------------------------------

    class CustomComparer : IEqualityComparer<FileNode>
    {
        public bool Equals(FileNode x, FileNode y)
        {
            return string.Compare(x.FileName, y.FileName, ignoreCase: true) == 0;
        }
        public int GetHashCode(FileNode obj) => throw new NotSupportedException();
    }
}