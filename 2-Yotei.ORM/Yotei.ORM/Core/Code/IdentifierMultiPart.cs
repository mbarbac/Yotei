using IHost = Yotei.ORM.IIdentifierMultiPart;
using IItem = Yotei.ORM.IIdentifierSinglePart;

namespace Yotei.ORM.Code;

// ========================================================
/// <summary>
/// <inheritdoc cref="IHost"/>
/// </summary>
[DebuggerDisplay("{ToDebugString()}")]
[Cloneable(Specs = "(source)")]
public partial class IdentifierMultiPart : Identifier, IHost
{
    /// <summary>
    /// Represents the internal collection of elements carried by this instance.
    /// </summary>
    protected class InnerList : CoreList<IItem>
    {
        IHost Master;
        public InnerList(IHost master)
        {
            Master = master.ThrowWhenNull();
            Validate = (item, add) =>
            {
                item = item.ThrowWhenNull(); if (add)
                {
                    if (!ReferenceEquals(Master.Engine, item.Engine))
                        throw new ArgumentException(
                            "The engine of the given element is not the engine of this instance.")
                            .WithData(item)
                            .WithData(Master);
                }
                return item;
            };
            Compare = (inner, other) =>
            {
                return Equivalent(inner.NonTerminatedValue, other.NonTerminatedValue);
            };
            AcceptDuplicate = (_) => true;
            ExpandNested = (_) => false;
        }
        public bool Equivalent(string? x, string? y)
        {
            return
                (x is null && y is null) ||
                string.Compare(x, y, !Master.Engine.CaseSensitiveNames) == 0;
        }
    }

    /// <summary>
    /// The internal collection of elements carried by this instance.
    /// </summary>
    protected InnerList Items { get; }

    /// <summary>
    /// Returns an array with the parts obtained from the given value.
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    IItem[] ValueToParts(string? value)
    {
        return value == null
            ? [new IdentifierSinglePart(Engine)]
            : Engine.GetDotted(value).Select(x => new IdentifierSinglePart(Engine, x)).ToArray();
    }

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
    /// Initializes a new instance with the elements from the given range.
    /// </summary>
    /// <param name="engine"></param>
    /// <param name="range"></param>
    public IdentifierMultiPart(IEngine engine, IEnumerable<IItem> range) : this(engine) => Items.AddRange(range);

    /// <summary>
    /// Initializes a new instance with the parts obtained from the given value.
    /// </summary>
    /// <param name="engine"></param>
    /// <param name="value"></param>
    public IdentifierMultiPart(IEngine engine, string? value) : this(engine)
    {
        var parts = ValueToParts(value);
        Items.AddRange(parts);
    }

    /// <summary>
    /// Initializes a new instance with the parts obtained from the given range of values.
    /// </summary>
    /// <param name="engine"></param>
    /// <param name="range"></param>
    public IdentifierMultiPart(IEngine engine, IEnumerable<string?> range) : this(engine)
    {
        ArgumentNullException.ThrowIfNull(range);

        foreach (var value in range)
        {
            var parts = ValueToParts(value);
            Items.AddRange(parts);
        }
    }

    /// <summary>
    /// Copy constructor.
    /// </summary>
    /// <param name="source"></param>
    protected IdentifierMultiPart(IdentifierMultiPart source) : this(source.Engine) => Items.AddRange(source);

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    public override string ToString() => Items.ToString();

    string ToDebugString() => $"[{string.Join(", ", Items)}]";

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    public IEnumerator<IItem> GetEnumerator() => Items.GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    // ----------------------------------------------------

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    public override IIdentifier Reduce()
    {
        if (Count == 0) return new IdentifierSinglePart(Engine);
        if (Count == 1) return Items[0];

        if (Items[0].Value == null)
        {
            var values = Items.Select(x => x.NonTerminatedValue).ToList();
            while (values.Count > 0)
            {
                if (values[0] == null) values.RemoveAt(0);
                else break;
            }
            return values.Count == 0
                ? new IdentifierSinglePart(Engine)
                : new IdentifierMultiPart(Engine, values);
        }
        return this;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public override string? Value
    {
        get // Removing empty heads...
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

            return num == 0 ? null : sb.ToString();
        }
        init // Clearing and adding parts...
        {
            var parts = ValueToParts(value);

            Items.Clear();
            Items.AddRange(parts);
        }
    }

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
    /// <param name="strict"></param>
    /// <returns></returns>
    public bool Contains(IItem item, bool strict = false) => Items.Contains(item, strict);

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="item"></param>
    /// <param name="strict"></param>
    /// <returns></returns>
    public int IndexOf(IItem item, bool strict = false) => Items.IndexOf(item, strict);

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="item"></param>
    /// <param name="strict"></param>
    /// <returns></returns>
    public int LastIndexOf(IItem item, bool strict = false) => Items.LastIndexOf(item, strict);

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="item"></param>
    /// <param name="strict"></param>
    /// <returns></returns>
    public List<int> IndexesOf(IItem item, bool strict = false) => Items.IndexesOf(item, strict);

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    public bool Contains(Predicate<IItem> predicate) => Items.Contains(predicate);

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    public int IndexOf(Predicate<IItem> predicate) => Items.IndexOf(predicate);

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="predicate"></param>
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
    public IItem[] ToArray() => Items.ToArray();

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    public List<IItem> ToList() => Items.ToList();

    // ----------------------------------------------------

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public bool Contains(string? value) => IndexOf(value) >= 0;

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public int IndexOf(string? value)
    {
        var part = new IdentifierSinglePart(Engine, value);
        return IndexOf(part);
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public int LastIndexOf(string? value)
    {
        var part = new IdentifierSinglePart(Engine, value);
        return LastIndexOf(part);
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public List<int> IndexesOf(string? value)
    {
        var part = new IdentifierSinglePart(Engine, value);
        return IndexesOf(part);
    }

    // ----------------------------------------------------

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="index"></param>
    /// <param name="count"></param>
    /// <returns></returns>
    public virtual IHost GetRange(int index, int count)
    {
        if (index == 0 && count == Count) return this;
        if (count == 0)
        {
            if (Count == 0) return this;

            var other = Clone(); other.Items.Clear();
            return other;
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
    /// <param name="strict"
    /// <returns></returns>
    public virtual IHost Replace(int index, IItem item, bool strict = false)
    {
        var temp = Clone();
        var num = temp.Items.Replace(index, item, strict);
        return num > 0 ? temp : this;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    public virtual IHost Add(IItem item)
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
    public virtual IHost AddRange(IEnumerable<IItem> range)
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
    public virtual IHost Insert(int index, IItem item)
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
    public virtual IHost InsertRange(int index, IEnumerable<IItem> range)
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
    public virtual IHost RemoveAt(int index)
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
    public virtual IHost RemoveRange(int index, int count)
    {
        var temp = Clone();
        var num = temp.Items.RemoveRange(index, count);
        return num > 0 ? temp : this;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="item"></param>
    /// <param name="strict"></param>
    /// <returns></returns>
    public virtual IHost Remove(IItem item, bool strict = false)
    {
        var temp = Clone();
        var num = temp.Items.Remove(item, strict);
        return num > 0 ? temp : this;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="item"></param>
    /// <param name="strict"></param>
    /// <returns></returns>
    public virtual IHost RemoveLast(IItem item, bool strict = false)
    {
        var temp = Clone();
        var num = temp.Items.RemoveLast(item, strict);
        return num > 0 ? temp : this;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    public virtual IHost RemoveAll(IItem item, bool strict = false)
    {
        var temp = Clone();
        var num = temp.Items.RemoveAll(item, strict);
        return num > 0 ? temp : this;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    public virtual IHost Remove(Predicate<IItem> predicate)
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
    public virtual IHost RemoveLast(Predicate<IItem> predicate)
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
    public virtual IHost RemoveAll(Predicate<IItem> predicate)
    {
        var temp = Clone();
        var num = temp.Items.RemoveAll(predicate);
        return num > 0 ? temp : this;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    public virtual IHost Clear()
    {
        var temp = Clone();
        var num = temp.Items.Clear();
        return num > 0 ? temp : this;
    }

    // ----------------------------------------------------

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="index"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    public virtual IHost Replace(int index, string? value)
    {
        var temp = Clone();
        var num = temp.ReplaceInternal(index, value);
        return num > 0 ? temp : this;
    }
    int ReplaceInternal(int index, string? value)
    {
        var range = Items.ToArray();
        Items.RemoveAt(index);

        var count = InsertInternal(index, value);
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
    /// <param name="value"></param>
    /// <returns></returns>
    public virtual IHost Add(string? value)
    {
        var temp = Clone();
        var num = temp.AddInternal(value);
        return num > 0 ? temp : this;
    }
    int AddInternal(string? value)
    {
        var parts = ValueToParts(value);
        return Items.AddRange(parts);
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="range"></param>
    /// <returns></returns>
    public virtual IHost AddRange(IEnumerable<string?> range)
    {
        var temp = Clone();
        var num = temp.AddRangeInternal(range);
        return num > 0 ? temp : this;
    }
    int AddRangeInternal(IEnumerable<string?> range)
    {
        ArgumentNullException.ThrowIfNull(range);

        var count = 0; foreach (var value in range)
        {
            count += AddInternal(value);
        }
        return count;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public virtual IHost Insert(int index, string? value)
    {
        var temp = Clone();
        var num = temp.InsertInternal(index, value);
        return num > 0 ? temp : this;
    }
    int InsertInternal(int index, string? value)
    {
        var parts = ValueToParts(value);
        return Items.InsertRange(index, parts);
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="range"></param>
    /// <returns></returns>
    public virtual IHost InsertRange(int index, IEnumerable<string?> range)
    {
        var temp = Clone();
        var num = temp.InsertRangeInternal(index, range);
        return num > 0 ? temp : this;
    }
    int InsertRangeInternal(int index, IEnumerable<string?> range)
    {
        ArgumentNullException.ThrowIfNull(range);

        var count = 0; foreach (var value in range)
        {
            var temp = InsertInternal(index, value);
            count += temp;
            index += temp;
        }
        return count;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public virtual IHost Remove(string? value)
    {
        var temp = Clone();
        var num = temp.RemoveInternal(value);
        return num > 0 ? temp : this;
    }
    int RemoveInternal(string? value)
    {
        var parts = ValueToParts(value);
        var count = 0;
        foreach (var part in parts)
        {
            count = Items.Remove(part);
        }
        return count;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public virtual IHost RemoveLast(string? value)
    {
        var temp = Clone();
        var num = temp.RemoveLastInternal(value);
        return num > 0 ? temp : this;
    }
    int RemoveLastInternal(string? value)
    {
        var parts = ValueToParts(value);
        var count = 0;
        foreach (var part in parts)
        {
            count = Items.RemoveLast(part);
        }
        return count;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public virtual IHost RemoveAll(string? value)
    {
        var temp = Clone();
        var num = temp.RemoveAllInternal(value);
        return num > 0 ? temp : this;
    }
    int RemoveAllInternal(string? value)
    {
        var parts = ValueToParts(value);
        var count = 0;
        foreach (var part in parts)
        {
            count = Items.RemoveAll(part);
        }
        return count;
    }
}