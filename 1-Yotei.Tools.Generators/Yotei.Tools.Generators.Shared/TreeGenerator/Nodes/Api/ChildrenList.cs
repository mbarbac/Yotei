namespace Yotei.Tools.Generators.Shared;

// ========================================================
/// <summary>
/// Represents a list of child elements.
/// </summary>
/// <typeparam name="T"></typeparam>
internal class ChildrenList<T> : CustomList<T> where T : INode
{
    /// <summary>
    /// The node this list belongs to.
    /// </summary>
    public INode Master { get; }

    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    /// <param name="master"></param>
    public ChildrenList(INode master)
    {
        Master = master.ThrowWhenNull();
        OnValidate = (item, add) =>
        {
            item.ThrowWhenNull();
            if (add && !ReferenceEquals(Master, item.ParentNode)) throw new ArgumentException(
                "Parent node of the given one is not this instance.")
                .WithData(item)
                .WithData(Master);
            return item;
        };
        OnCompare = (item, other) => ReferenceEquals(item, other);
        OnSameElement = (item, other) => ReferenceEquals(item, other);
        OnAcceptDuplicate = (item, other) => false;
    }
}