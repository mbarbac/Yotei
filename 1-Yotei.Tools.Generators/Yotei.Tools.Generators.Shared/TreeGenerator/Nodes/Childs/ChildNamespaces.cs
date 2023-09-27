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

    /// <summary>
    /// Returns the node in the collection that matches the given arguments, or invokes the
    /// given action to create a new one that will be added to the collection and returned.
    /// </summary>
    /// <param name="longName"></param>
    /// <param name="create"></param>
    /// <returns></returns>
    //public NamespaceNode Locate(string longName, Func<NamespaceNode> create)
    //{
    //    create = create.ThrowWhenNull(nameof(create));

    //    var node = Find(longName);
    //    if (node == null)
    //    {
    //        node = create();
    //        Add(node);
    //    }
    //    return node;
    //}
}