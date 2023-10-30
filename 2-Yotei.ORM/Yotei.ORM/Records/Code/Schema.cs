using THost = Yotei.ORM.Records.ISchema;
using TItem = Yotei.ORM.Records.ISchemaEntry;
using TKey = string;

namespace Yotei.ORM.Records.Code;

// ========================================================
/// <summary>
/// <inheritdoc cref="THost"/>
/// </summary>
[DebuggerDisplay("{ToDebugString()}")]
[Cloneable(Specs = "(source)-*")]
public partial class Schema : THost
{
    /// <summary>
    /// Represents the collection of contents in this instance.
    /// </summary>
    protected class InnerList : CoreList<TKey, TItem>
    {
        Schema Master { get; }
        public InnerList(Schema master) => Master = master.ThrowWhenNull();
        protected InnerList(InnerList source)
        {
            source.ThrowWhenNull();
            Master = source.Master;
            AddRange(source);
        }
        public override InnerList Clone() => new(this);

        // ------------------------------------------------

        public override TItem ValidateItem(TItem item)
        {
            item.ThrowWhenNull();

            if (!ReferenceEquals(Master.Engine, item.Engine)) throw new ArgumentException(
                "The engine of the given entry is not the same as the engine of this collection.")
                .WithData(item)
                .WithData(Master, "this");

            if (item.Identifier.Value == null) throw new ArgumentException(
                "Value of the given identifier is null.")
                .WithData(item)
                .WithData(Master, "this");

            if (item.Identifier is IIdentifierMultiPart multi && multi[^1].Value == null)
                throw new ArgumentException(
                    "Value of the last part of the given identifier is null.")
                    .WithData(item)
                    .WithData(Master, "this");

            return item;
        }
        public override TKey GetKey(TItem item) => item.Identifier.Value!;
        public override TKey ValidateKey(TKey key) => key.NotNullNotEmpty();
        public override bool CompareKeys(TKey inner, TKey other)
        {
            var xinner = new IdentifierMultiPart(Master.Engine, inner);
            var xother = new IdentifierMultiPart(Master.Engine, other);
            return xinner.Match(xother);
        }
        public override bool AcceptDuplicated(TItem item)
        {
            if (this.Any(x => ReferenceEquals(x, item))) return true;
            throw new DuplicateException("Duplicated element.").WithData(item);
        }
        public override bool ExpandNexted(TItem _) => false;
        protected override int FindElement(TItem other)
        {
            for (int i = 0; i < Count; i++)
            {
                var inner = this[i];
                if (inner.Identifier.Match(other.Identifier)) return i;
            }
            return -1;
        }
    }

    /// <summary>
    /// The actual collection of elements carried by this instance.
    /// </summary>
    protected InnerList Items { get; }

    // ----------------------------------------------------

    /// <summary>
    /// Initializes a new empty instance.
    /// </summary>
    /// <param name="engine"></param>
    public Schema(IEngine engine)
    {
        Items = new(this);
        Engine = engine.ThrowWhenNull();
    }

    /// <summary>
    /// Initializes a new instance with the given element.
    /// </summary>
    /// <param name="engine"></param>
    /// <param name="item"></param>
    public Schema(IEngine engine, TItem item) : this(engine) => Items.Add(item);

    /// <summary>
    /// Initializes a new instance with the elements from the given range.
    /// </summary>
    /// <param name="engine"></param>
    /// <param name="range"></param>
    public Schema(IEngine engine, IEnumerable<TItem> range) : this(engine) => Items.AddRange(range);

    /// <summary>
    /// Copy constructor.
    /// </summary>
    /// <param name="source"></param>
    protected Schema(Schema source)
    {
        source.ThrowWhenNull();

        Items = new(this);
        Engine = source.Engine;
        Items.AddRange(source);
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    public IEnumerator<TItem> GetEnumerator() => Items.GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    public override string ToString() => $"Count: {Count}";

    // ----------------------------------------------------

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public IEngine Engine { get; }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public int Count => Items.Count;

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public void Trim() => Items.Trim();

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="index"></param>
    /// <returns></returns>
    public TItem this[int index] => Items[index];

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    public bool Contains(TKey key) => Items.Contains(key!);

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    public int IndexOf(TKey key) => Items.IndexOf(key!);

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    public int LastIndexOf(TKey key) => Items.LastIndexOf(key!);

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    public List<int> IndexesOf(TKey key) => Items.IndexesOf(key);

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    public bool Contains(Predicate<TItem> predicate) => Items.Contains(predicate);

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    public int IndexOf(Predicate<TItem> predicate) => Items.IndexOf(predicate);

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    public int LastIndexOf(Predicate<TItem> predicate) => Items.LastIndexOf(predicate);

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    public List<int> IndexesOf(Predicate<TItem> predicate) => Items.IndexesOf(predicate);

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    public TItem[] ToArray() => Items.ToArray();

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    public List<TItem> ToList() => Items.ToList();

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    public List<TItem> Match(TKey key, out TItem? unique)
    {
        var temps = IndexesOf(key);
        var items = temps.Select(x => this[x]).ToList();

        unique = items.Count == 1 ? items[0] : null;
        return items;
    }

    // ----------------------------------------------------

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="index"></param>
    /// <param name="count"></param>
    /// <returns></returns>
    public virtual THost GetRange(int index, int count)
    {
        if (count == Count && index == 0) return this;
        if (count == 0) return Clear();

        var range = Items.GetRange(index, count);
        var temp = Clone();
        temp.Items.Clear();
        temp.Items.AddRange(range);
        return temp;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="index"></param>
    /// <param name="item"></param>
    /// <returns></returns>
    public virtual THost Replace(int index, TItem item)
    {
        var temp = Clone();
        var num = temp.Items.Replace(index, item);
        return num > 0 ? temp : this;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    public virtual THost Add(TItem item)
    {
        var temp = Clone();
        var num = temp.Items.Add(item);
        return num > 0 ? temp : this;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="range"></param>
    /// <returns></returns>
    public virtual THost AddRange(IEnumerable<TItem> range)
    {
        var temp = Clone();
        var num = temp.Items.AddRange(range);
        return num > 0 ? temp : this;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="index"></param>
    /// <param name="item"></param>
    /// <returns></returns>
    public virtual THost Insert(int index, TItem item)
    {
        var temp = Clone();
        var num = temp.Items.Insert(index, item);
        return num > 0 ? temp : this;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="index"></param>
    /// <param name="range"></param>
    /// <returns></returns>
    public virtual THost InsertRange(int index, IEnumerable<TItem> range)
    {
        var temp = Clone();
        var num = temp.Items.InsertRange(index, range);
        return num > 0 ? temp : this;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="index"></param>
    /// <returns></returns>
    public virtual THost RemoveAt(int index)
    {
        var temp = Clone();
        var num = temp.Items.RemoveAt(index);
        return num > 0 ? temp : this;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="index"></param>
    /// <param name="count"></param>
    /// <returns></returns>
    public virtual THost RemoveRange(int index, int count)
    {
        var temp = Clone();
        var num = temp.Items.RemoveRange(index, count);
        return num > 0 ? temp : this;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    public virtual THost Remove(TKey key)
    {
        var temp = Clone();
        var num = temp.Items.Remove(key);
        return num > 0 ? temp : this;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    public virtual THost RemoveLast(TKey key)
    {
        var temp = Clone();
        var num = temp.Items.RemoveLast(key);
        return num > 0 ? temp : this;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    public virtual THost RemoveAll(TKey key)
    {
        var temp = Clone();
        var num = temp.Items.RemoveAll(key);
        return num > 0 ? temp : this;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    public virtual THost Remove(Predicate<TItem> predicate)
    {
        var temp = Clone();
        var num = temp.Items.Remove(predicate);
        return num > 0 ? temp : this;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    public virtual THost RemoveLast(Predicate<TItem> predicate)
    {
        var temp = Clone();
        var num = temp.Items.RemoveLast(predicate);
        return num > 0 ? temp : this;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    public virtual THost RemoveAll(Predicate<TItem> predicate)
    {
        var temp = Clone();
        var num = temp.Items.RemoveAll(predicate);
        return num > 0 ? temp : this;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    public virtual THost Clear()
    {
        var temp = Clone();
        var num = temp.Items.Clear();
        return num > 0 ? temp : this;
    }
}