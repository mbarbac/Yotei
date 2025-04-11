using IItem = Yotei.ORM.Tools.Templates.IElement;
using TItem = Yotei.ORM.Tools.Templates.Element;
using TKey = string;

namespace Yotei.ORM.Tools.Templates;

// ========================================================
/// <summary>
/// ...
/// </summary>
[Cloneable]
public partial class ChainKT : InvariantList<TKey, IItem>, IChain<TKey, IItem>, IItem
{
    /// <summary>
    /// The builder associated with this type.
    /// </summary>
    [Cloneable]
    public partial class Builder : CoreList<TKey, IItem>
    {
        public Builder(bool sensitive) => Sensitive = sensitive;
        public Builder(bool sensitive, int capacity) : this(sensitive) => Capacity = capacity;
        public Builder(bool sensitive, IEnumerable<TItem> range) : this(sensitive) => AddRange(range);
        protected Builder(Builder source) : this(source.Sensitive) => AddRange(source);

        public override IItem ValidateItem(IItem item) => item.ThrowWhenNull();
        public override string ValidateKey(string key) => key.NotNullNotEmpty();
        public override string GetKey(IItem item) => item is TItem named
            ? named.Name
            : throw new ArgumentException("Element is not a named instance.").WithData(item);
        public override bool ExpandItems => true;
        public override bool IncludeDuplicated(IItem item, IItem source)
            => ReferenceEquals(item, source)
            ? true
            : throw new DuplicateException("Duplicated element.").WithData(item);        
        public override IEqualityComparer<string> Comparer => Sensitive
            ? StringComparer.Ordinal
            : StringComparer.OrdinalIgnoreCase;
        
        public bool Sensitive { get; }
    }

    // ----------------------------------------------------

    protected override Builder Items => _Items ??= new Builder(Sensitive);
    Builder? _Items = null;

    /// <summary>
    /// Initializes a new empty instance.
    /// </summary>
    /// <param name="sensitive"></param>
    public ChainKT(bool sensitive) => Sensitive = sensitive;

    /// <summary>
    /// Initializes a new instance with the elements of the given range.
    /// </summary>
    /// <param name="sensitive"></param>
    /// <param name="range"></param>
    public ChainKT(bool sensitive, IEnumerable<IItem> range) : this(sensitive) => Items.AddRange(range);

    /// <summary>
    /// Copy constructor.
    /// </summary>
    /// <param name="source"></param>
    protected ChainKT(ChainKT source) : this(source.Sensitive) => Items.AddRange(source.Items);

    /// <summary>
    /// Determines if the key of the elements in this collection are case sensitive.
    /// </summary>
    public bool Sensitive { get; }

    // ----------------------------------------------------

    /// <inheritdoc/>
    public override Builder GetBuilder() => Items.Clone();

    /// <inheritdoc/>
    public override ChainKT GetRange(int index, int count) => (ChainKT)base.GetRange(index, count);
    IChain<TKey, IItem> IChain<TKey, IItem>.GetRange(int index, int count) => GetRange(index, count);

    /// <inheritdoc/>
    public override ChainKT Replace(int index, IItem item) => (ChainKT)base.Replace(index, item);
    IChain<TKey, IItem> IChain<TKey, IItem>.Replace(int index, IItem item) => Replace(index, item);

    /// <inheritdoc/>
    public override ChainKT Add(IItem item) => (ChainKT)base.Add(item);
    IChain<TKey, IItem> IChain<TKey, IItem>.Add(IItem item) => Add(item);

    /// <inheritdoc/>
    public override ChainKT AddRange(IEnumerable<IItem> range) => (ChainKT)base.AddRange(range);
    IChain<TKey, IItem> IChain<TKey, IItem>.AddRange(IEnumerable<IItem> range) => AddRange(range);

    /// <inheritdoc/>
    public override ChainKT Insert(int index, IItem item) => (ChainKT)base.Insert(index, item);
    IChain<TKey, IItem> IChain<TKey, IItem>.Insert(int index, IItem item) => Insert(index, item);

    /// <inheritdoc/>
    public override ChainKT InsertRange(int index, IEnumerable<IItem> range) => (ChainKT)base.InsertRange(index, range);
    IChain<TKey, IItem> IChain<TKey, IItem>.InsertRange(int index, IEnumerable<IItem> range) => InsertRange(index, range);

    /// <inheritdoc/>
    public override ChainKT RemoveAt(int index) => (ChainKT)base.RemoveAt(index);
    IChain<TKey, IItem> IChain<TKey, IItem>.RemoveAt(int index) => RemoveAt(index);

    /// <inheritdoc/>
    public override ChainKT RemoveRange(int index, int count) => (ChainKT)base.RemoveRange(index, count);
    IChain<TKey, IItem> IChain<TKey, IItem>.RemoveRange(int index, int count) => RemoveRange(index, count);

    /// <inheritdoc/>
    public override ChainKT Remove(TKey key) => (ChainKT)base.Remove(key);
    IChain<TKey, IItem> IChain<TKey, IItem>.Remove(TKey key) => Remove(key);

    /// <inheritdoc/>
    public override ChainKT RemoveLast(TKey key) => (ChainKT)base.RemoveLast(key);
    IChain<TKey, IItem> IChain<TKey, IItem>.RemoveLast(TKey key) => RemoveLast(key);

    /// <inheritdoc/>
    public override ChainKT RemoveAll(TKey key) => (ChainKT)base.RemoveAll(key);
    IChain<TKey, IItem> IChain<TKey, IItem>.RemoveAll(TKey key) => RemoveAll(key);

    /// <inheritdoc/>
    public override ChainKT Remove(Predicate<IItem> predicate) => (ChainKT)base.Remove(predicate);
    IChain<TKey, IItem> IChain<TKey, IItem>.Remove(Predicate<IItem> predicate) => Remove(predicate);

    /// <inheritdoc/>
    public override ChainKT RemoveLast(Predicate<IItem> predicate) => (ChainKT)base.RemoveLast(predicate);
    IChain<TKey, IItem> IChain<TKey, IItem>.RemoveLast(Predicate<IItem> predicate) => RemoveLast(predicate);

    /// <inheritdoc/>
    public override ChainKT RemoveAll(Predicate<IItem> predicate) => (ChainKT)base.RemoveAll(predicate);
    IChain<TKey, IItem> IChain<TKey, IItem>.RemoveAll(Predicate<IItem> predicate) => RemoveAll(predicate);

    /// <inheritdoc/>
    public override ChainKT Clear() => (ChainKT)base.Clear();
    IChain<TKey, IItem> IChain<TKey, IItem>.Clear() => Clear();
}