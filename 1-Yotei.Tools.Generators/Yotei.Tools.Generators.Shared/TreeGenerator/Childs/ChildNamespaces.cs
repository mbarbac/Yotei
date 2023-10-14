namespace Yotei.Tools.Generators.Shared;

// ========================================================
/// <summary>
/// Represents a collection of child namespaces.
/// </summary>
internal class ChildNamespaces : NoDuplicatesList<NamespaceNode>
{
    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    public ChildNamespaces() : base()
    {
        ThrowDuplicates = false;
        Equivalent = (x, y) => Comparer(x.LongName, y.LongName);
    }

    /// <summary>
    /// Invoked to compare two elements.
    /// </summary>
    bool Comparer(string xname, string yname) => xname == yname;

    /// <summary>
    /// Returns the element in this collection that matches the given criteria.
    /// </summary>
    /// <param name="longName"></param>
    /// <returns></returns>
    public NamespaceNode? Find(string longName)
    {
        longName = longName.NotNullNotEmpty(nameof(longName));

        var index = IndexOf(x => Comparer(x.LongName, longName));
        return index >= 0 ? this[index] : null;
    }
}