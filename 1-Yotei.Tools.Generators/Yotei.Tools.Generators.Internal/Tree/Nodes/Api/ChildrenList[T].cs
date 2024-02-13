namespace Yotei.Tools.Generators.Internal;

// ========================================================
/// <summary>
/// Represents a list of children node elements associated with a given master one.
/// </summary>
/// <typeparam name="T"></typeparam>
internal class ChildrenList<T> : CustomList<T> where T : INode
{
    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    /// <param name="master"></param>
    public ChildrenList(INode master)
    {
        Master = master.ThrowWhenNull();
        Validate = (item) =>
        {
            item.ThrowWhenNull();
            if (!ReferenceEquals(Master, item.ParentNode)) throw new ArgumentException(
                "Parent of the given node is not this instance.")
                .WithData(item)
                .WithData(this);

            return item;
        };
        Compare = (x, y) => ReferenceEquals(x, y);
        CanInclude = (item, x) => !ReferenceEquals(item, x);
    }

    /// <summary>
    /// The master node this collection belongs to.
    /// </summary>
    public INode Master { get; }
}