using IHost = Yotei.ORM.IIdentifierMultiPart;
using IItem = Yotei.ORM.IIdentifierSinglePart;

namespace Yotei.ORM;

// ========================================================
/// <summary>
/// <inheritdoc cref="IHost"/>
/// </summary>
public class IdentifierMultiPart : Identifier, IHost
{
    // Represents the actual contents carried by this instance.
    protected class InnerList : CoreList<IItem>
    {
        public InnerList(IdentifierMultiPart master)
        {
            Master = master.ThrowWhenNull();
            ExpandNested = false;
        }
        IdentifierMultiPart Master;

        public override IItem Validate(IItem item, bool add)
        {
            ArgumentNullException.ThrowIfNull(item); if (add)
            {
                if (!ReferenceEquals(Master.Engine, item.Engine)) throw new ArgumentException(
                    "The engine of the given element is not the same as this collection's one.")
                    .WithData(item)
                    .WithData(Master);
            }
            return item;
        }
        public override bool Equivalent(IItem inner, IItem other)
        {
            return EquivalentValue(inner.Value, other.Value);
        }
        public bool EquivalentValue(string? inner, string? other)
        {
            return inner == null && other == null
                ? true
                : string.Compare(inner, other, !Master.Engine.CaseSensitiveNames) == 0;
        }
        public override bool IgnoreDuplicate(IItem _) => false;
    }

    // The actual contents carried by this instance.
    protected InnerList Items { get; }

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
    public IdentifierMultiPart(IEngine engine, IEnumerable<IItem> range) : this(engine) => Items.AddRange(range);

    /// <summary>
    /// Initializes a new instance with the given value, which can be a multipart one.
    /// </summary>
    /// <param name="engine"></param>
    /// <param name="value"></param>
    public IdentifierMultiPart(IEngine engine, string? value) : this(engine)
    {
        var parts = ValueToParts(value);
        Items.AddRange(parts);
    }

    /// <summary>
    /// Invoked to obtain a clone of this instance.
    /// </summary>
    /// <returns></returns>
    protected virtual IdentifierMultiPart Clone() => new(Engine, this);

    // ----------------------------------------------------

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
    /// Returns an array with the parts from the given value.
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    IItem[] ValueToParts(string? value) => value == null
        ? [new IdentifierSinglePart(Engine)]
        : Engine.GetDotted(value).Select(x => new IdentifierSinglePart(Engine, x)).ToArray();

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
    /// <param name="value"></param>
    /// <returns></returns>
    public int LastIndexOf(string? value)
    {
        var parts = ValueToParts(value);

        for (int i = parts.Length - 1; i >= 0; i--)
        {
            var temp = LastIndexOf(parts[i]);
            if (temp >= 0) return temp;
        }
        return -1;
    }

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
    public bool Contains(IItem item) => Items.Contains(item);

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    public int IndexOf(IItem item) => Items.IndexOf(item);

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    public int LastIndexOf(IItem item) => Items.LastIndexOf(item);

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    public List<int> IndexesOf(IItem item) => Items.IndexesOf(item);

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
    /// <param name="index"></param>
    /// <param name="count"></param>
    /// <returns></returns>
    public virtual IHost GetRange(int index, int count)
    {
        if (count == 0) return this;
        if (index == 0 && count == Count) return this;

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
    public virtual IHost Replace(int index, IItem item)
    {
        var temp = Clone();
        var done = temp.Items.Replace(index, item);
        return done > 0 ? temp : this;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="index"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    public virtual IHost Replace(int index, string? value)
    {
        var temp = Clone();
        var done = temp.ReplaceInternal(index, value);
        return done > 0 ? temp : this;
    }
    int ReplaceInternal(int index, string? value)
    {
        var parts = ValueToParts(value);
        if (parts.Length == 1)
        {
            var temp = Items.IndexOf(parts[0]);
            if (temp == index) return 0;
        }

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
        return done > 0 ? temp : this;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public virtual IHost Add(string? value)
    {
        var parts = ValueToParts(value);
        return AddRange(parts);
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
        return done > 0 ? temp : this;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="range"></param>
    /// <returns></returns>
    public virtual IHost AddRange(IEnumerable<string?> range)
    {
        var temp = Clone();
        var done = temp.AddRangeInternal(range);
        return done > 0 ? temp : this;
    }
    int AddRangeInternal(IEnumerable<string?> range)
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
        return done > 0 ? temp : this;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="index"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    public virtual IHost Insert(int index, string? value)
    {
        var parts = ValueToParts(value);
        return InsertRange(index, parts);
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
        return done > 0 ? temp : this;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="index"></param>
    /// <param name="range"></param>
    /// <returns></returns>
    public virtual IHost InsertRange(int index, IEnumerable<string?> range)
    {
        var temp = Clone();
        var done = temp.InsertRangeInternal(index, range);
        return done > 0 ? temp : this;
    }
    int InsertRangeInternal(int index, IEnumerable<string?> range)
    {
        {
            ArgumentNullException.ThrowIfNull(range);

            var count = 0; foreach (var item in range)
            {
                var parts = ValueToParts(item);
                foreach (var part in parts)
                {
                    var num = Items.Insert(index, part);
                    count += num;
                    index += num;
                }
            }
            return count;
        }
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
        return done > 0 ? temp : this;
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
        return done > 0 ? temp : this;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public virtual IHost Remove(string? value)
    {
        var temp = Clone();
        var done = temp.RemoveInternal(value);
        return done > 0 ? temp : this;
    }
    int RemoveInternal(string? value)
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
        return done > 0 ? temp : this;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public virtual IHost RemoveLast(string? value)
    {
        var temp = Clone();
        var done = temp.RemoveLastInternal(value);
        return done > 0 ? temp : this;
    }
    int RemoveLastInternal(string? value)
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
        return done > 0 ? temp : this;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public virtual IHost RemoveAll(string? value)
    {
        var temp = Clone();
        var done = temp.RemoveAllInternal(value);
        return done > 0 ? temp : this;
    }
    int RemoveAllInternal(string? value)
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
        return done > 0 ? temp : this;
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
        return done > 0 ? temp : this;
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
        return done > 0 ? temp : this;
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
        return done > 0 ? temp : this;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    public virtual IHost Clear()
    {
        var temp = Clone();
        var done = temp.Items.Clear();
        return done > 0 ? temp : this;
    }
}