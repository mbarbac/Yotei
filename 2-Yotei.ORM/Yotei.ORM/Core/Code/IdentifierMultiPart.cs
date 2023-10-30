using THost = Yotei.ORM.IIdentifierMultiPart;
using TItem = Yotei.ORM.IIdentifierSinglePart;
using TKey = string;

namespace Yotei.ORM.Code;

// ========================================================
/// <summary>
/// <inheritdoc cref="THost"/>
/// </summary>
[Cloneable(Specs = "(source)-*")]
public partial class IdentifierMultiPart : Identifier, THost
{
    /// <summary>
    /// Represents the collection of contents in this instance.
    /// </summary>
    protected class InnerList : CoreList<TKey?, TItem>
    {
        IdentifierMultiPart Master { get; }
        public InnerList(IdentifierMultiPart master) => Master = master.ThrowWhenNull();
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
                "Engine of the given element is not the engine of this instance.")
                .WithData(item)
                .WithData(Master);

            return item;
        }
        public override TKey? GetKey(TItem item) => item.NonTerminatedValue;
        public override TKey? ValidateKey(TKey? key) => key.NullWhenEmpty();
        public override bool CompareKeys(TKey? inner, TKey? other)
        {
            return inner is null && other is null
                ? true
                : string.Compare(inner, other, !Master.Engine.CaseSensitiveNames) == 0;
        }
        public override bool AcceptDuplicated(TItem item) => true;
        public override bool ExpandNexted(TItem item) => false;
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
    public IdentifierMultiPart(IEngine engine) : base(engine) => Items = new(this);

    /// <summary>
    /// Initializes a new instance with the given element.
    /// </summary>
    /// <param name="engine"></param>
    /// <param name="item"></param>
    public IdentifierMultiPart(IEngine engine, TItem item) : this(engine) => Items.Add(item);

    /// <summary>
    /// Initializes a new instance with the elements from the given range.
    /// </summary>
    /// <param name="engine"></param>
    /// <param name="range"></param>
    public IdentifierMultiPart(IEngine engine, IEnumerable<TItem> range) : this(engine) => Items.AddRange(range);

    /// <summary>
    /// Copy constructor.
    /// </summary>
    /// <param name="source"></param>
    protected IdentifierMultiPart(IdentifierMultiPart source) : this(source.Engine) => Items.AddRange(source);

    /// <summary>
    /// Initializes a new instance with the elements obtained from the given value.
    /// </summary>
    /// <param name="engine"></param>
    /// <param name="value"></param>
    public IdentifierMultiPart(IEngine engine, string? value) : this(engine) => AddInternal(value);

    /// <summary>
    /// Initializes a new instance with the elements obtained from the given range.
    /// </summary>
    /// <param name="engine"></param>
    /// <param name="range"></param>
    public IdentifierMultiPart(IEngine engine, IEnumerable<string?> range) : this(engine) => AddRangeInternal(range);

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
    public override string ToString() => string.Join('.', Items);

    // ----------------------------------------------------

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public override string? Value
    {
        // Removing empty head elements...
        get
        {
            if (_Value == null)
            {
                var sb = new StringBuilder();
                var num = 0;

                foreach (var item in Items)
                {
                    var value = item.Value; if (num > 0 || value != null)
                    {
                        if (num > 0) sb.Append('.');
                        sb.Append(value);
                        num++;
                    }
                }

                _Value = num == 0 ? null : sb.ToString();
            }
            return _Value;
        }

        // Clearing and adding the obtained parts, if any...
        init
        {
            _Value = value.NullWhenEmpty();            

            if (_Value == null) Items.Clear();
            else
            {
                var parts = ValueToParts(_Value);

                Items.Clear();
                Items.AddRange(parts);
            }
        }
    }
    string? _Value = null;

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    public override IIdentifier Reduce()
    {
        if (Count == 0) return new IdentifierSinglePart(Engine);
        if (Count == 1) return Items[0];

        if (Items[0].Value == null) // We remove empty head elements...
        {
            var values = Items.ToList(); while (values.Count > 0)
            {
                if (values[0].Value == null) values.RemoveAt(0);
                else break;
            }
            return values.Count == 0
                ? new IdentifierSinglePart(Engine)
                : new IdentifierMultiPart(Engine, values);
        }

        return this;
    }

    /// <summary>
    /// Returns an array with the single-part identifiers obtained from the given value. If it
    /// is null or empty, then the arrays contains a single empty element.
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    TItem[] ValueToParts(string? value) => value == null
        ? [new IdentifierSinglePart(Engine)]
        : Engine.GetDotted(value).Select(x => new IdentifierSinglePart(Engine, x)).ToArray();

    // ----------------------------------------------------

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
    public bool Contains(TKey? key) => Items.Contains(key);

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    public int IndexOf(TKey? key) => Items.IndexOf(key);

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    public int LastIndexOf(TKey? key) => Items.LastIndexOf(key);

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    public List<int> IndexesOf(TKey? key) => Items.IndexesOf(key);

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

    // ----------------------------------------------------

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="index"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    public virtual THost Replace(int index, string? value)
    {
        var temp = Clone();
        var num = temp.ReplaceInternal(index, value);
        return num > 0 ? temp : this;
    }
    int ReplaceInternal(int index, string? value)
    {
        Items.RemoveAt(index);
        return InsertInternal(index, value);
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public virtual THost Add(string? value)
    {
        var temp = Clone();
        var num = temp.AddInternal(value);
        return num > 0 ? temp : this;
    }
    int AddInternal(string? value)
    {
        var parts = ValueToParts(value);
        var count = 0;

        foreach (var part in parts) count += Items.Add(part);
        return count;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="range"></param>
    /// <returns></returns>
    public virtual THost AddRange(IEnumerable<string?> range)
    {
        var temp = Clone();
        var num = temp.AddRangeInternal(range);
        return num > 0 ? temp : this;
    }
    int AddRangeInternal(IEnumerable<string?> range)
    {
        range.ThrowWhenNull();

        var count = 0; foreach (var item in range)
        {
            var num = AddInternal(item);
            count += num;
        }
        return count;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="index"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    public virtual THost Insert(int index, string? value)
    {
        var temp = Clone();
        var num = temp.InsertInternal(index, value);
        return num > 0 ? temp : this;
    }
    int InsertInternal(int index, string? value)
    {
        var parts = ValueToParts(value);
        var count = 0;

        foreach (var part in parts)
        {
            var num = Items.Insert(index, part);
            count += num;
            index += num;
        }
        return count;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="index"></param>
    /// <param name="range"></param>
    /// <returns></returns>
    public virtual THost InsertRange(int index, IEnumerable<string?> range)
    {
        var temp = Clone();
        var num = temp.InsertRangeInternal(index, range);
        return num > 0 ? temp : this;
    }
    int InsertRangeInternal(int index, IEnumerable<string?> range)
    {
        range.ThrowWhenNull();

        var count = 0; foreach (var item in range)
        {
            var num = InsertInternal(index, item);
            count += num;
            index += num;
        }
        return count;
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
    public virtual THost Remove(TKey? key)
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
    public virtual THost RemoveLast(TKey? key)
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
    public virtual THost RemoveAll(TKey? key)
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