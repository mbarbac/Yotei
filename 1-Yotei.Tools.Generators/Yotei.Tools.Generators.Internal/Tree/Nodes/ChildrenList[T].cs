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
            if (!ReferenceEquals(Master, item.ThrowWhenNull().ParentNode))
                throw new ArgumentException(
                    "Parent node of the given one is not this instance.")
                    .WithData(item);
            return item;
        };
        Comparer = (x, y) => ReferenceEquals(x, y);
        CanInclude = (@this, item) => @this.IndexOf(item) < 0;
    }

    /// <summary>
    /// The master node this collection belongs to.
    /// </summary>
    public INode Master { get; }
}