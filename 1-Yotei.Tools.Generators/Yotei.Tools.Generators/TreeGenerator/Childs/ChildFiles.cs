namespace Yotei.Tools.Generators.Shared;

// ========================================================
/// <summary>
/// Represents a collection of child files.
/// </summary>
internal class ChildFiles : CoreList<FileNode>
{
    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    public ChildFiles() : base()
    {
        AcceptDuplicate = (item) => false;
        Compare = (x, y) => Comparer(x.FileName, y.FileName);
    }

    /// <summary>
    /// Invoked to compare two elements.
    /// </summary>
    bool Comparer(string xname, string yname)
    {
        return string.Compare(xname, yname, ignoreCase: true) == 0;
    }

    /// <summary>
    /// Returns the element in this collection that matches the given criteria.
    /// </summary>
    /// <param name="fileName"></param>
    /// <returns></returns>
    public FileNode? Find(string fileName)
    {
        fileName = fileName.NotNullNotEmpty(nameof(fileName));

        var index = IndexOf(x => Comparer(x.FileName, fileName));
        return index >= 0 ? this[index] : null;
    }
}