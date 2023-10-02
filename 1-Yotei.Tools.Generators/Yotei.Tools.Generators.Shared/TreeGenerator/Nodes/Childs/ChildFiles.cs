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
}