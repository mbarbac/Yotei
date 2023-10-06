using IHost = Yotei.ORM.Core.IIdentifierMultiPart;
using IItem = Yotei.ORM.Core.IIdentifierSinglePart;

namespace Yotei.ORM.Core.Code;

// ========================================================
/// <summary>
/// <inheritdoc cref="IHost"/>
/// </summary>
public class IdentifierMultiPart : Identifier, IHost
{
    /// <summary>
    /// Represents an internal surrogate collection of elements.
    /// </summary>
    protected class Surrogate : CoreList<IItem>
    {
        public Surrogate(IHost master) => Master = master;
        readonly IHost Master;
        protected override IItem Validate(IItem item, bool add)
        {
            ArgumentNullException.ThrowIfNull(item);

            if (add && !ReferenceEquals(Master.Engine, item.Engine))
                throw new ArgumentException(
                    "Engines of the given element and of this collection don't match.")
                    .WithData(item)
                    .WithData(this);

            return item;
        }
        protected override bool Compare(IItem x, IItem y)
        {
            return x.Value == null && y.Value == null
                ? true
                : string.Compare(x.Value, y.Value, !Master.Engine.CaseSensitiveNames) == 0;
        }
        protected override bool IgnoreDuplicate(IItem _) => false;
        protected override void ThrowWhenDuplicate(IItem _) { }
        protected override bool ExpandNested => false;
    }

    /// <summary>
    /// The internal surrogate collection of elements.
    /// </summary>
    protected Surrogate Items { get; }

    // ----------------------------------------------------

    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    /// <param name="engine"></param>
    public IdentifierMultiPart(IEngine engine) : base(engine) => Items = new(this);

    /// <summary>
    /// Initializes a new instance with the given element.
    /// </summary>
    /// <param name="engine"></param>
    /// <param name="item"></param>
    public IdentifierMultiPart(IEngine engine, IItem item) : this(engine) => Items.Add(item);

    /// <summary>
    /// Initializes a new instance with the given collection of elements.
    /// </summary>
    /// <param name="engine"></param>
    /// <param name="range"></param>
    public IdentifierMultiPart(IEngine engine, IEnumerable<IItem> range) : this(engine)
        => Items.AddRange(range);

    /// <summary>
    /// Initializes a new instance with an element with the given value, or with a chain of
    /// elements if the value was a multipart one.
    /// </summary>
    /// <param name="engine"></param>
    /// <param name="value"></param>
    public IdentifierMultiPart(IEngine engine, string? value) : this(engine)
        => AddValueInternal(value);

    /// <summary>
    /// <inheritdoc cref="IHost.Clone"/>
    /// </summary>
    /// <returns></returns>
    public virtual IdentifierMultiPart Clone() => new(Engine, this);
    IHost IHost.Clone() => Clone();

    // ----------------------------------------------------

    /// <summary>
    /// Returns an array with the parts obtained from the given value. If the given value is null,
    /// then the array contains a single empty element.
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    IItem[] ValueToParts(string? value)
    {
        return value == null
            ? [new IdentifierSinglePart(Engine)]
            : Engine.GetDotted(value).Select(x => new IdentifierSinglePart(Engine, x)).ToArray();
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public override string? Value
    {
        get // Removing empty heads...
        {
            var list = new List<string?>();
            var heads = false;

            foreach (var item in Items)
            {
                var value = item.Value; if (heads || value != null)
                {
                    list.Add(value);
                    heads = true;
                }
            }
            return list.Count == 0 ? null : string.Join('.', list);
        }
        init // Adding parts as needed...
        {
            var parts = ValueToParts(value);

            Items.Clear();
            Items.AddRange(parts);
        }
    }

    /// <summary>
    /// Reduces this instance to a simpler one, if such is possible.
    /// </summary>
    /// <returns></returns>
    public override IIdentifier Reduce() => Count switch
    {
        0 => new IdentifierSinglePart(Engine),
        1 => Items[0],
        _ => this,
    };

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns><inheritdoc/></returns>
    public IEnumerator<IItem> GetEnumerator() => Items.GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

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
    public IItem this[int index] => Items[index];

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    public bool Contains(IItem item) => Items.Contains(item);

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public bool Contains(string? value) => IndexOf(value) >= 0;

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    public int IndexOf(IItem item) => Items.IndexOf(item);

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public int IndexOf(string? value)
    {
        var parts = ValueToParts(value);

        for (int i = 0; i < parts.Length; i++)
        {
            var temp = IndexOf(parts[i]);
            if (temp >= 0) return temp;
        }
        return -1;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    public int LastIndexOf(IItem item) => Items.LastIndexOf(item);

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public int LastIndexOf(string? value)
    {
        var parts = ValueToParts(value);

        for (int i = parts.Length -1; i >= 0; i--)
        {
            var temp = LastIndexOf(parts[i]);
            if (temp >= 0) return temp;
        }
        return -1;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    public List<int> IndexesOf(IItem item) => Items.IndexesOf(item);

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public List<int> IndexesOf(string? value)
    {
        var parts = ValueToParts(value);
        var list = new List<int>();

        for (int i = 0; i < parts.Length; i++)
        {
            var temp = IndexesOf(parts[i]);
            foreach (var index in temp) if (!list.Contains(index)) list.Add(index);
        }
        return list;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    public bool Contains(Predicate<IItem> predicate) => Items.Contains(predicate);

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    public int IndexOf(Predicate<IItem> predicate) => Items.IndexOf(predicate);

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    public int LastIndexOf(Predicate<IItem> predicate) => Items.LastIndexOf(predicate);

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    public List<int> IndexesOf(Predicate<IItem> predicate) => Items.IndexesOf(predicate);

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    public List<IItem> ToList() => Items.ToList();

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    public IItem[] ToArray() => Items.ToArray();

    // ----------------------------------------------------

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="index"></param>
    /// <param name="count"></param>
    /// <returns></returns>
    public virtual IHost GetRange(int index, int count)
    {
        if (count > 0)
        {
            var range = Items.GetRange(index, count);
            var temp = Clone();
            temp.Items.Clear();
            temp.Items.AddRange(range);
            return temp;
        }
        return this;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="index"></param>
    /// <param name="item"></param>
    /// <returns></returns>
    public virtual IHost ReplaceItem(int index, IItem item)
    {
        var temp = Clone();
        var done = temp.Items.ReplaceItem(index, item);
        return done == 0 ? this : temp;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="index"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    public virtual IHost ReplaceValue(int index, string? value)
    {
        var temp = Clone();
        var done = temp.ReplaceValueInternal(index, value);
        return done == 0 ? this : temp;
    }
    protected int ReplaceValueInternal(int index, string? value)
    {
        var parts = ValueToParts(value);

        var range = Items.ToArray();
        Items.RemoveAt(index);

        var count = Items.InsertRange(index, parts);
        if (count == 0)
        {
            Items.Clear();
            Items.AddRange(range);
        }
        return count;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    public virtual IHost Add(IItem item)
    {
        var temp = Clone();
        var done = temp.Items.Add(item);
        return done == 0 ? this : temp;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public virtual IHost AddValue(string? value)
    {
        var temp = Clone();
        var done = temp.AddValueInternal(value);
        return done == 0 ? this : temp;
    }
    protected int AddValueInternal(string? value)
    {
        var parts = ValueToParts(value);

        var count = 0; foreach (var part in parts) count += Items.Add(part);
        return count;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="range"></param>
    /// <returns></returns>
    public virtual IHost AddRange(IEnumerable<IItem> range)
    {
        var temp = Clone();
        var done = temp.Items.AddRange(range);
        return done == 0 ? this : temp;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="range"></param>
    /// <returns></returns>
    public virtual IHost AddValueRange(IEnumerable<string?> range)
    {
        var temp = Clone();
        var done = temp.AddValueRangeInternal(range);
        return done == 0 ? this : temp;
    }
    protected int AddValueRangeInternal(IEnumerable<string?> range)
    {
        ArgumentNullException.ThrowIfNull(range);

        var count = 0; foreach (var item in range)
        {
            var parts = ValueToParts(item);
            foreach (var part in parts) count += Items.Add(part);
        }
        return count;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="index"></param>
    /// <param name="item"></param>
    /// <returns></returns>
    public virtual IHost Insert(int index, IItem item)
    {
        var temp = Clone();
        var done = temp.Items.Insert(index, item);
        return done == 0 ? this : temp;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="index"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    public virtual IHost InsertValue(int index, string? value)
    {
        var temp = Clone();
        var done = temp.InsertValueInternal(index, value);
        return done == 0 ? this : temp;
    }
    protected int InsertValueInternal(int index, string? value)
    {
        var parts = ValueToParts(value);

        var count = 0; foreach (var part in parts)
        {
            var temp = Items.Insert(index, part);
            count += temp;
            index += temp;
        }
        return count;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="index"></param>
    /// <param name="range"></param>
    /// <returns></returns>
    public virtual IHost InsertRange(int index, IEnumerable<IItem> range)
    {
        var temp = Clone();
        var done = temp.Items.InsertRange(index, range);
        return done == 0 ? this : temp;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="index"></param>
    /// <param name="range"></param>
    /// <returns></returns>
    public virtual IHost InsertValueRange(int index, IEnumerable<string?> range)
    {
        var temp = Clone();
        var done = temp.InsertValueRangeInternal(index, range);
        return done == 0 ? this : temp;
    }
    protected int InsertValueRangeInternal(int index, IEnumerable<string?> range)
    {
        ArgumentNullException.ThrowIfNull(range);

        var count = 0; foreach (var item in range)
        {
            var parts = ValueToParts(item);
            foreach (var part in parts)
            {
                var temp = Items.Insert(index, part);
                count += temp;
                index += temp;
            }
        }
        return count;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="index"></param>
    /// <returns></returns>
    public virtual IHost RemoveAt(int index)
    {
        var temp = Clone();
        var done = temp.Items.RemoveAt(index);
        return done == 0 ? this : temp;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    public virtual IHost Remove(IItem item)
    {
        var temp = Clone();
        var done = temp.Items.Remove(item);
        return done == 0 ? this : temp;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public virtual IHost RemoveValue(string? value)
    {
        var temp = Clone();
        var done = temp.RemoveValueInternal(value);
        return done == 0 ? this : temp;
    }
    protected int RemoveValueInternal(string? value)
    {
        var parts = ValueToParts(value);

        var count = 0; foreach (var part in parts) count += Items.Remove(part);
        return count;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    public virtual IHost RemoveLast(IItem item)
    {
        var temp = Clone();
        var done = temp.Items.RemoveLast(item);
        return done == 0 ? this : temp;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public virtual IHost RemoveLastValue(string? value)
    {
        var temp = Clone();
        var done = temp.RemoveLastValueInternal(value);
        return done == 0 ? this : temp;
    }
    protected int RemoveLastValueInternal(string? value)
    {
        var parts = ValueToParts(value);

        var count = 0; foreach (var part in parts) count += Items.RemoveLast(part);
        return count;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    public virtual IHost RemoveAll(IItem item)
    {
        var temp = Clone();
        var done = temp.Items.RemoveAll(item);
        return done == 0 ? this : temp;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public virtual IHost RemoveAllValues(string? value)
    {
        var temp = Clone();
        var done = temp.RemoveAllValuesInternal(value);
        return done == 0 ? this : temp;
    }
    protected int RemoveAllValuesInternal(string? value)
    {
        var parts = ValueToParts(value);

        var count = 0; foreach (var part in parts) count += Items.RemoveAll(part);
        return count;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="index"></param>
    /// <param name="count"></param>
    /// <returns></returns>
    public virtual IHost RemoveRange(int index, int count)
    {
        var temp = Clone();
        var done = temp.Items.RemoveRange(index, count);
        return done == 0 ? this : temp;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    public virtual IHost Remove(Predicate<IItem> predicate)
    {
        var temp = Clone();
        var done = temp.Items.Remove(predicate);
        return done == 0 ? this : temp;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    public virtual IHost RemoveLast(Predicate<IItem> predicate)
    {
        var temp = Clone();
        var done = temp.Items.RemoveLast(predicate);
        return done == 0 ? this : temp;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    public virtual IHost RemoveAll(Predicate<IItem> predicate)
    {
        var temp = Clone();
        var done = temp.Items.RemoveAll(predicate);
        return done == 0 ? this : temp;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    public virtual IHost Clear()
    {
        var temp = Clone();
        var done = temp.Items.Clear();
        return done == 0 ? this : temp;
    }
}