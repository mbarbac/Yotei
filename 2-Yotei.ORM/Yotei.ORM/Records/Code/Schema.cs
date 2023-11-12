using System.Runtime.InteropServices.Marshalling;
using THost = Yotei.ORM.Records.ISchema;
using TItem = Yotei.ORM.Records.ISchemaEntry;
using TKey = Yotei.ORM.IIdentifier;

namespace Yotei.ORM.Records.Code;

// ========================================================
/// <summary>
/// <inheritdoc cref="THost"/>
/// </summary>
[DebuggerDisplay("{ToDebugString()}")]
[Cloneable]
public partial class Schema : THost
{
    /// <summary>
    /// Initializes a new empty instance.
    /// </summary>
    /// <param name="engine"></param>
    public Schema(IEngine engine)
    {
        Items = CreateInnerList();
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
    public Schema(
        IEngine engine, IEnumerable<TItem> range) : this(engine) => Items.AddRange(range);

    /// <summary>
    /// Copy constructor.
    /// </summary>
    /// <param name="source"></param>
    protected Schema(Schema source)
    {
        source.ThrowWhenNull();

        Items = CreateInnerList();
        Engine = source.Engine;
        Items.AddRange(source.Items);
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
    public override string ToString() => string.Join('.', Items.Select(x => ToString(x)));

    string ToString(ISchemaEntry entry)
    {
        var str = entry.Identifier.Value ?? string.Empty;
        if (entry.IsPrimaryKey) str += "(PK)";
        return str;
    }

    protected string ToDebugString()
    {
        var items = Items.Count < DEBUGCOUNT ? Items : Items.Take(DEBUGCOUNT);
        return $"({Count}):[{string.Join(", ", items)}]";
    }
    static int DEBUGCOUNT = 8;

    // ----------------------------------------------------

    /// <summary>
    /// Represents the internal collection of elements in this instance.
    /// </summary>
    class InnerList : CoreList<TKey, TItem>
    {
        readonly THost Master;
        public InnerList(THost master) => Master = master.ThrowWhenNull();
        public new string ToDebugString() => base.ToDebugString();

        public override TItem ValidateItem(TItem item) => item.ThrowWhenNull();
        public override TKey GetKey(TItem item) => item.Identifier;
        public override TKey ValidateKey(TKey key)
        {
            key.ThrowWhenNull();

            if (!Master.Engine.Equals(key.Engine)) throw new ArgumentException(
                "Engine of identifier is not the same as the engine of this schema.")
                .WithData(key, "identifier")
                .WithData(Master, "schema");

            if (key.Count == 0) throw new ArgumentException(
                "Identifier cannot be empty.")
                .WithData(key, "identifier");

            if (key.Value == null) throw new ArgumentException(
                "Identifier cannot carry a null value.")
                .WithData(key, "identifier");

            if (key[^1].Value == null) throw new ArgumentException(
                "Last part of the given identifier cannot carry a null value.")
                .WithData(key, "identifier");

            return key;
        }
        public override bool CompareKeys(TKey source, TKey target)
        {
            return source.Match(target);
        }
        public override bool AcceptDuplicate(TItem item)
        {
            if (this.Any(x => ReferenceEquals(x, item))) return true;
            throw new DuplicateException("Duplicated element.").WithData(item);
        }

        protected override int FindDuplicate(TItem item)
        {
            var index = base.FindDuplicate(item);
            if (index < 0)
            {
                if (this.Any(x => x.Identifier.Match(item.Identifier))) index = 1000;
                if (this.Any(x => x.Identifier.Match(item.Identifier))) index = 1000;
            }
            return index;
        }
    }

    /// <summary>
    /// Obtains an inner list to be used by this instance.
    /// </summary>
    /// <returns></returns>
    InnerList CreateInnerList() => new(this);

    /// <summary>
    /// The internal collection of elements in this instance.
    /// </summary>
    InnerList Items { get; }

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
    /// <param name="identifier"></param>
    /// <returns></returns>
    public bool Contains(string identifier)
    {
        var item = new Identifier(Engine, identifier);
        return Items.Contains(item);
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="identifier"></param>
    /// <returns></returns>
    public int IndexOf(string identifier)
    {
        var item = new Identifier(Engine, identifier);
        return Items.IndexOf(item);
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="identifier"></param>
    /// <returns></returns>
    public int LastIndexOf(string identifier)
    {
        var item = new Identifier(Engine, identifier);
        return Items.LastIndexOf(item);
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="identifier"></param>
    /// <returns></returns>
    public List<int> IndexesOf(string identifier)
    {
        var item = new Identifier(Engine, identifier);
        return Items.IndexesOf(item);
    }

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
    /// <param name="identifier"></param>
    /// <param name="unique"></param>
    /// <returns></returns>
    public List<TItem> Match(string identifier, out TItem? unique)
    {
        var item = new Identifier(Engine, identifier);

        identifier.ThrowWhenNull();

        var nums = Items.IndexesOf(x => x.Identifier.Match(item));
        var items = nums.Select(x => Items[x]).ToList();

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
        if (count == 0)
        {
            if (Count == 0) return this;
            return Clear();
        }

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
    /// <param name="identifier"></param>
    /// <returns></returns>
    public virtual THost Remove(string identifier)
    {
        var temp = Clone();

        var item = new Identifier(Engine, identifier);
        var num = temp.Items.Remove(item);
        return num > 0 ? temp : this;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="identifier"></param>
    /// <returns></returns>
    public virtual THost RemoveLast(string identifier)
    {
        var temp = Clone();

        var item = new Identifier(Engine, identifier);
        var num = temp.Items.RemoveLast(item);
        return num > 0 ? temp : this;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="identifier"></param>
    /// <returns></returns>
    public virtual THost RemoveAll(string identifier)
    {
        var temp = Clone();

        var item = new Identifier(Engine, identifier);
        var num = temp.Items.RemoveAll(item);
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