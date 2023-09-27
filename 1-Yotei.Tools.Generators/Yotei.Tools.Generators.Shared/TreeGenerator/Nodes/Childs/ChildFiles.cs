namespace Yotei.Tools.Generators;

// ========================================================
/// <summary>
/// Represents a collection of child files.
/// </summary>
internal class ChildFiles : NoDuplicatesList<FileNode>
{
    bool OnComparer(string xFileName, string yFileName)
    {
        return string.Compare(xFileName, yFileName, ignoreCase: true) == 0;
    }

    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    public ChildFiles() : base()
    {
        ThrowDuplicates = false;
        Comparer = (x, y) => OnComparer(x.FileName, y.FileName);
    }

    /// <summary>
    /// Returns the element in this collection that matches the given arguments.
    /// </summary>
    /// <param name="fileName"></param>
    /// <returns></returns>
    public FileNode? Find(string fileName)
    {
        fileName = fileName.NotNullNotEmpty(nameof(fileName));

        var index = IndexOf(x => OnComparer(fileName, x.FileName));
        return index >= 0 ? this[index] : null;
    }

    /// <summary>
    /// Returns the node in the collection that matches the given arguments, or invokes the
    /// given action to create a new one that will be added to the collection and returned.
    /// </summary>
    /// <param name="fileName"></param>
    /// <param name="create"></param>
    /// <returns></returns>
    //public FileNode Locate(string fileName, Func<FileNode> create)
    //{
    //    create = create.ThrowWhenNull(nameof(create));

    //    var node = Find(fileName);
    //    if (node == null)
    //    {
    //        node = create();
    //        Add(node);
    //    }
    //    return node;
    //}
}