using TItem = Yotei.ORM.IIdentifierPart;
using TKey = string;

namespace Yotei.ORM.Code;

// ========================================================
/// <summary>
/// Represents a builder of <see cref="IIdentifier"/> instances.
/// </summary>
[Cloneable]
public partial class IdentifierBuilder : CoreList<TKey?, TItem>
{
    /// <summary>
    /// Initializes a new empty instance.
    /// </summary>
    /// <param name="engine"></param>
    public IdentifierBuilder(IEngine engine) => Engine = engine.ThrowWhenNull();

    /// <summary>
    /// Initializes a new instance with the given element.
    /// </summary>
    /// <param name="engine"></param>
    /// <param name="item"></param>
    public IdentifierBuilder(IEngine engine, TItem item) : this(engine) => Add(item);

    /// <summary>
    /// Initializes a new instance with the elements from the given range.
    /// </summary>
    /// <param name="engine"></param>
    /// <param name="range"></param>
    public IdentifierBuilder(IEngine engine, IEnumerable<TItem> range)
        : this(engine)
        => AddRange(range);

    /// <summary>
    /// Copy constructor.
    /// </summary>
    /// <param name="source"></param>
    protected IdentifierBuilder(IdentifierBuilder source)
        : this(source.Engine)
        => AddRange(source);

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    public override string ToString() => string.Join('.', this.Select(x => x.Value));

    // ----------------------------------------------------

    protected override TItem ValidateItem(TItem item)
    {
        item.ThrowWhenNull();
        if (Engine != item.Engine) throw new ArgumentException(
            "The engine of the element is not the one of this collection.")
            .WithData(item);

        return item;
    }

    protected override TKey? GetKey(TItem item) => item.ThrowWhenNull().UnwrappedValue;

    protected override TKey? ValidateKey(
        TKey? key) => new IdentifierPart(Engine, key).UnwrappedValue;

    protected override bool CompareKeys(TKey? source, TKey? item)
        => string.Compare(source, item, !Engine.CaseSensitiveNames) == 0;

    protected override bool SameItem(TItem source, TItem item)
        => ReferenceEquals(source, item);

    protected override List<int> GetDuplicates(TKey? key) => [];

    protected override bool AcceptDuplicate(TItem source, TItem item) => true;

    /// <summary>
    /// Removes empty head elements.
    /// </summary>
    void Reduce()
    {
        while (Count > 0)
        {
            if (this[0].Value == null) base.RemoveAt(0);
            else break;
        }
    }

    /// <summary>
    /// Returns a list with the elements obtained from the given value.
    /// </summary>
    /// <param name="value"></param>
    /// <param name="reduce"></param>
    /// <returns></returns>
    public List<TItem> ToParts(string? value, bool reduce)
    {
        value = value.NullWhenEmpty();

        // We may need an empty collection...
        if (value == null) return reduce ? [] : [new IdentifierPart(Engine)];

        // Terminators not used...
        if (!Engine.UseTerminators)
        {
            var items = value.Split('.');
            var parts = new List<TItem>();

            parts.AddRange(items.Select(x => new IdentifierPart(Engine, x)));
            if (reduce) Reduce(parts);
            return parts;
        }

        // Terminators used...
        else
        {
            var dots = Engine.UnwrappedIndexes(value, '.');

            // No unwrapped dots...
            if (dots.Count == 0)
            {
                var temp = new IdentifierPart(Engine, value);
                return reduce ? (temp.Value == null ? [] : [temp]) : [temp];
            }

            // With unwrapped dots...
            else
            {
                string? str;
                int len;
                var head = 0;
                var parts = new List<TItem>();

                for (int i = 0; i < dots.Count; i++)
                {
                    str = value[head..dots[i]];
                    parts.Add(new IdentifierPart(Engine, str));
                    head = dots[i] + 1;
                }

                len = value.Length - head;
                str = len == 0 ? string.Empty : value.Substring(dots[^1] + 1, len);
                parts.Add(new IdentifierPart(Engine, str));

                if (reduce) Reduce(parts);
                return parts;
            }
        }
    }

    /// <summary>
    /// Removes empty head elements.
    /// </summary>
    static void Reduce(List<TItem> parts)
    {
        while (parts.Count > 0)
        {
            if (parts[0].Value == null) parts.RemoveAt(0);
            else break;
        }
    }
    
    // ----------------------------------------------------

    /// <summary>
    /// The identifier this instance is associated with.
    /// </summary>
    public IEngine Engine { get; }

    /// <summary>
    /// The value carried by this instance, or null if it represents an empty or missed one.
    /// If not, the values of its parts are wrapped with the appropriate engine terminators.
    /// </summary>
    public string? Value
    {
        get => Count == 0 ? null : string.Join('.', this.Select(x => x.Value));
        set
        {
            var parts = ToParts(value, reduce: true);
            Clear();
            AddRange(parts);
        }
    }

    // ----------------------------------------------------

    /// <inheritdoc cref="CoreList{TItem}.Contains(TItem)"/>
    public new bool Contains(TKey? key) => base.Contains(key);

    /// <inheritdoc cref="CoreList{TItem}.IndexOf(TItem)"/>
    public new int IndexOf(TKey key) => base.IndexOf(key);

    /// <inheritdoc cref="CoreList{TItem}.LastIndexOf(TItem)"/>
    public new int LastIndexOf(TKey key) => base.LastIndexOf(key);

    // ----------------------------------------------------

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="index"></param>
    /// <param name="item"></param>
    /// <returns></returns>
    public override int Replace(int index, TItem item)
    {
        var num = base.Replace(index, item);
        Reduce();
        return num;
    }

    /// <summary>
    /// Replaces the element at the given index with the ones obtained from the given value.
    /// Returns the number of changes made.
    /// </summary>
    /// <param name="index"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    public int Replace(int index, string? value)
    {
        var parts = ToParts(value, reduce: false);

        if (parts.Count == 0) return 0;
        if (parts.Count == 1 && this[index].Value == parts[0].Value) return 0;

        RemoveAt(index);

        var num = 0; foreach (var part in parts)
        {
            if (index == 0 && part.Value == null) continue;

            var r = Insert(index, part);
            num += r;
            index += r;
        }

        Reduce();
        return num + 1;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    public override int Add(TItem item)
    {
        if (Count == 0 && item.Value == null) return 0;

        var num = base.Add(item);
        Reduce();
        return num;
    }

    /// <summary>
    /// Adds to this collection the elements obtained from the given value. Returns the number
    /// of changes made.
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public int Add(string? value)
    {
        var parts = ToParts(value, reduce: false);

        if (parts.Count == 0) return 0;
        if (parts.Count > 1) return AddRange(parts);

        return Add(parts[0]);
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="range"></param>
    /// <returns></returns>
    public override int AddRange(IEnumerable<TItem> range)
    {
        var num = base.AddRange(range);
        Reduce();
        return num;
    }

    /// <summary>
    /// Adds to this collection the elements obtained from the given range of values. Returns
    /// the number of changes made.
    /// </summary>
    /// <param name="values"></param>
    /// <returns></returns>
    public int AddRange(IEnumerable<string?> values)
    {
        values.ThrowWhenNull();

        if (values is ICollection irange && irange.Count == 0) return 0;
        if (values is ICollection<string?> trange && trange.Count == 0) return 0;

        var num = 0; foreach (var value in values)
        {
            var r = Add(value);
            num += r;
        }
        return num;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="index"></param>
    /// <param name="item"></param>
    /// <returns></returns>
    public override int Insert(int index, TItem item)
    {
        if (index == 0 && item.Value == null) return 0;

        var num = base.Insert(index, item);
        Reduce();
        return num;
    }

    /// <summary>
    /// Inserts into this collection the elements obtained from the given value, starting at
    /// the given index. Returns the number of changes made.
    /// </summary>
    /// <param name="index"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    public int Insert(int index, string? value)
    {
        var parts = ToParts(value, reduce: false);

        if (parts.Count == 0) return 0;
        if (parts.Count > 1) return InsertRange(index, parts);

        return Insert(index, parts[0]);
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="index"></param>
    /// <param name="range"></param>
    /// <returns></returns>
    public override int InsertRange(int index, IEnumerable<TItem> range)
    {
        var num = base.InsertRange(index, range);
        Reduce();
        return num;
    }

    /// <summary>
    /// Inserts into this collection the elements obtained from the given range of values,
    /// starting at the given index. Returns the number of changes made.
    /// </summary>
    /// <param name="index"></param>
    /// <param name="values"></param>
    /// <returns></returns>
    public int InsertRange(int index, IEnumerable<string?> values)
    {
        values.ThrowWhenNull();

        if (values is ICollection irange && irange.Count == 0) return 0;
        if (values is ICollection<string?> trange && trange.Count == 0) return 0;

        var num = 0; foreach (var value in values)
        {
            var r = Insert(index, value);
            num += r;
            index += r;
        }
        return num;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="index"></param>
    /// <returns></returns>
    public override int RemoveAt(int index)
    {
        var num = base.RemoveAt(index);
        Reduce();
        return num;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="index"></param>
    /// <param name="count"></param>
    /// <returns></returns>
    public override int RemoveRange(int index, int count)
    {
        var num = base.RemoveRange(index, count);
        Reduce();
        return num;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    public override int Remove(TKey? key)
    {
        var num = base.Remove(key);
        Reduce();
        return num;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    public override int RemoveLast(TKey? key)
    {
        var num = base.RemoveLast(key);
        Reduce();
        return num;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    public override int RemoveAll(TKey? key)
    {
        var num = base.RemoveAll(key);
        Reduce();
        return num;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    public override int Remove(Predicate<TItem> predicate)
    {
        var num = base.Remove(predicate);
        Reduce();
        return num;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    public override int RemoveLast(Predicate<TItem> predicate)
    {
        var num = base.RemoveLast(predicate);
        Reduce();
        return num;
    }
    
    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    public override int RemoveAll(Predicate<TItem> predicate)
    {
        var num = base.RemoveAll(predicate);
        Reduce();
        return num;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    public override int Clear()
    {
        var num = base.Clear();
        Reduce();
        return num;
    }
}