namespace Yotei.Tools.Generators;

// ========================================================
/// <summary>
/// Represents a collection of child namespaces.
/// </summary>
internal class ChildNamespaces : NoDuplicatesList<NamespaceNode>
{
    bool OnComparer(string xLongName, string yLongName)
    {
        return xLongName == yLongName;
    }

    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    public ChildNamespaces() : base()
    {
        ThrowDuplicates = false;
        Comparer = (x, y) => OnComparer(x.LongName, y.LongName);
    }

    /// <summary>
    /// Returns the element in this collection that matches the given arguments.
    /// </summary>
    /// <param name="longName"></param>
    /// <returns></returns>
    public NamespaceNode? Find(string longName)
    {
        longName = longName.NotNullNotEmpty(nameof(longName));

        var index = IndexOf(x => OnComparer(longName, x.LongName));
        return index >= 0 ? this[index] : null;
    }
}