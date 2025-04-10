using IItem = Yotei.ORM.Tools.Templates.IElement;
using TItem = Yotei.ORM.Tools.Templates.Element;

namespace Yotei.ORM.Tools.Templates;

// ========================================================
/// <summary>
/// ...
/// </summary>
[Cloneable]
public partial class ChainT : InvariantList<IItem>, IItem
{
    /// <summary>
    /// The builder associated with this type.
    /// </summary>
    [Cloneable]
    public partial class Builder : CoreList<IItem>
    {
        public Builder(bool sensitive) => Sensitive = sensitive;
        public Builder(bool sensitive, int capacity) : this(sensitive) => Capacity = capacity;
        public Builder(bool sensitive, IEnumerable<TItem> range) : this(sensitive) => AddRange(range);
        protected Builder(Builder source) : this(source.Sensitive) => AddRange(source);

        public override IItem ValidateItem(IItem item)
        {
            if (item.ThrowWhenNull() is TItem named) named.Name.NotNullNotEmpty();
            return item;
        }
        public override bool ExpandItems => true;
        public override bool IncludeDuplicated(IItem item, IItem source)
            => ReferenceEquals(item, source)
            ? true
            : throw new DuplicateException("Duplicated element.").WithData(item);

        public override IEqualityComparer<IItem> Comparer => _Comparer ??= new ItemComparer(Sensitive);
        IEqualityComparer<IItem>? _Comparer = null;
        readonly struct ItemComparer(bool Sensitive) : IEqualityComparer<IItem>
        {
            public bool Equals(IItem?x, IItem? y)
            {
                if (x is null && y is null) return true;
                if (x is null || y is null) return false;

                return x is TItem xnamed && y is TItem ynamed
                    ? string.Compare(xnamed.Name, ynamed.Name, !Sensitive) == 0
                    : ReferenceEquals(x, y);
            }
            public int GetHashCode(IItem obj) => obj is null ? 0 : obj.GetHashCode();
        }

        public bool Sensitive { get; }
    }

    // ----------------------------------------------------

    protected override Builder Items => _Items ??= new Builder(Sensitive);
    Builder? _Items = null;

    /// <summary>
    /// Initializes a new empty instance.
    /// </summary>
    /// <param name="sensitive"></param>
    public ChainT(bool sensitive) => Sensitive = sensitive;

    /// <summary>
    /// Initializes a new instance with the elements of the given range.
    /// </summary>
    /// <param name="sensitive"></param>
    /// <param name="range"></param>
    public ChainT(bool sensitive, IEnumerable<IItem> range) : this(sensitive) => Items.AddRange(range);

    /// <summary>
    /// Copy constructor.
    /// </summary>
    /// <param name="source"></param>
    protected ChainT(ChainT source) : this(source.Sensitive) => Items.AddRange(source.Items);

    /// <summary>
    /// Determines if the key of the elements in this collection are case sensitive.
    /// </summary>
    public bool Sensitive { get; }

    // ----------------------------------------------------

    /// <inheritdoc/>
    public override Builder GetBuilder() => Items.Clone();

    /// <inheritdoc/>
    public override ChainT GetRange(int index, int count) => (ChainT)base.GetRange(index, count);

    /// <inheritdoc/>
    public override ChainT Replace(int index, IItem item) => (ChainT)base.Replace(index, item);

    /// <inheritdoc/>
    public override ChainT Add(IItem item) => (ChainT)base.Add(item);

    /// <inheritdoc/>
    public override ChainT AddRange(IEnumerable<IItem> range) => (ChainT)base.AddRange(range);

    /// <inheritdoc/>
    public override ChainT Insert(int index, IItem item) => (ChainT)base.Insert(index, item);

    /// <inheritdoc/>
    public override ChainT InsertRange(int index, IEnumerable<IItem> range) => (ChainT)base.InsertRange(index, range);

    /// <inheritdoc/>
    public override ChainT RemoveAt(int index) => (ChainT)base.RemoveAt(index);

    /// <inheritdoc/>
    public override ChainT RemoveRange(int index, int count) => (ChainT)base.RemoveRange(index, count);

    /// <inheritdoc/>
    public override ChainT Remove(IItem item) => (ChainT)base.Remove(item);

    /// <inheritdoc/>
    public override ChainT RemoveLast(IItem item) => (ChainT)base.RemoveLast(item);

    /// <inheritdoc/>
    public override ChainT RemoveAll(IItem item) => (ChainT)base.RemoveAll(item);

    /// <inheritdoc/>
    public override ChainT Remove(Predicate<IItem> predicate) => (ChainT)base.Remove(predicate);

    /// <inheritdoc/>
    public override ChainT RemoveLast(Predicate<IItem> predicate) => (ChainT)base.RemoveLast(predicate);

    /// <inheritdoc/>
    public override ChainT RemoveAll(Predicate<IItem> predicate) => (ChainT)base.RemoveAll(predicate);

    /// <inheritdoc/>
    public override ChainT Clear() => (ChainT)base.Clear();
}