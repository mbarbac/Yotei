using TItem = Yotei.ORM.IIdentifierPart;
using TKey = string;

namespace Yotei.ORM.Code;

// ========================================================
/// <inheritdoc cref="IIdentifier"/>
[Cloneable]
public sealed partial class Identifier : IIdentifier
{
    /// <summary>
    /// The builder that maintains the items of this instance.
    /// </summary>
    IdentifierBuilder Items { get; }

    /// <summary>
    /// Invoked to create a new builder of the appropriate type.
    /// </summary>
    /// <param name="engine"></param>
    /// <returns></returns>
    static IdentifierBuilder CreateBuilder(IEngine engine) => new(engine);

    /// <inheritdoc/>
    public IdentifierBuilder ToBuilder() => Items.Clone();
    ICoreList<TKey?, TItem> IIdentifier.ToBuilder() => ToBuilder();

    // ----------------------------------------------------

    /// <summary>
    /// Initializes a new empty instance.
    /// </summary>
    /// <param name="engine"></param>
    public Identifier(IEngine engine)
    {
        Engine = engine.ThrowWhenNull();
        Items = CreateBuilder(engine);
    }

    /// <summary>
    /// Initializes a new instance with the given element.
    /// </summary>
    /// <param name="engine"></param>
    /// <param name="item"></param>
    public Identifier(IEngine engine, TItem item) : this(engine) => Items.Add(item);

    /// <summary>
    /// Initializes a new instance with the elements of the given range.
    /// </summary>
    /// <param name="engine"></param>
    /// <param name="range"></param>
    public Identifier(
        IEngine engine, IEnumerable<TItem> range) : this(engine) => Items.AddRange(range);

    /// <summary>
    /// Initializes a new instance with the elements obtained from the given value.
    /// </summary>
    /// <param name="engine"></param>
    /// <param name="value"></param>
    public Identifier(IEngine engine, string? value) : this(engine) => Items.Add(value);

    /// <summary>
    /// Copy constructor.
    /// </summary>
    /// <param name="source"></param>
    Identifier(Identifier source) : this(source.Engine) => Items.AddRange(source);

    /// <inheritdoc/>
    public IEnumerator<TItem> GetEnumerator() => Items.GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    /// <inheritdoc/>
    public override string ToString() => $"Count: {Count}";

    // ----------------------------------------------------

    /// <inheritdoc/>
    public string? Value
    {
        get
        {
            if (!_Initialized)
            {
                _Initialized = true;
                _Value = Items.Count == 0 ? null : string.Join('.', Items.Select(x => x.Value));
            }
            return _Value;
        }
        init
        {
            var parts = Items.ToParts(value, reduce: true);

            Items.Clear();
            Items.AddRange(parts);

            _Value = null;
            _Initialized = false;
        }
    }
    string? _Value;
    bool _Initialized;

    /// <inheritdoc/>
    public bool Match(string? specs)
    {
        if ((specs = specs.NullWhenEmpty()) == null) return true;

        var target = new Identifier(Engine, specs);
        var source = this;

        for (int i = 0; ; i++)
        {
            if (i >= target.Count) break;
            if (i >= source.Count)
            {
                while (i < target.Count)
                {
                    var value = target[^(i + 1)].UnwrappedValue;
                    if (value != null) return false;
                    i++;
                }
            }

            var tvalue = target[^(i + 1)].UnwrappedValue; if (tvalue == null) continue;
            var svalue = source[^(i + 1)].UnwrappedValue;
            if (!Compare(svalue, tvalue)) return false;
        }

        return true;
    }

    bool Compare(string? source, string? target)
        => string.Compare(source, target, !Engine.CaseSensitiveNames) == 0;

    // ----------------------------------------------------

    /// <inheritdoc/>
    public IEngine Engine { get; }

    /// <inheritdoc/>
    public int Count => Items.Count;

    /// <inheritdoc/>
    public TItem this[int index] => Items[index];

    /// <inheritdoc/>
    public bool Contains(TKey? key) => Items.Contains(key);

    /// <inheritdoc/>
    public int IndexOf(TKey? key) => Items.IndexOf(key);

    /// <inheritdoc/>
    public int LastIndexOf(TKey? key) => Items.LastIndexOf(key);

    /// <inheritdoc/>
    public List<int> IndexesOf(TKey? key) => Items.IndexesOf(key);

    /// <inheritdoc/>
    public bool Contains(Predicate<TItem> predicate) => Items.Contains(predicate);

    /// <inheritdoc/>
    public int IndexOf(Predicate<TItem> predicate) => Items.IndexOf(predicate);

    /// <inheritdoc/>
    public int LastIndexOf(Predicate<TItem> predicate) => Items.LastIndexOf(predicate);

    /// <inheritdoc/>
    public List<int> IndexesOf(Predicate<TItem> predicate) => Items.IndexesOf(predicate);

    /// <inheritdoc/>
    public TItem[] ToArray() => Items.ToArray();

    /// <inheritdoc/>
    public List<TItem> ToList() => Items.ToList();

    // ----------------------------------------------------

    /// <inheritdoc/>
    public IIdentifier GetRange(int index, int count)
    {
        if (index == 0 && count == Count) return this;
        if (count == 0 && index >= 0) return Clear();

        var range = Items.ToList().GetRange(index, count);
        var clone = Clone();
        clone.Items.Clear();
        clone.Items.AddRange(range);
        return clone;
    }

    /// <inheritdoc/>
    public IIdentifier Replace(int index, TItem item)
    {
        var clone = Clone();
        var num = clone.Items.Replace(index, item);
        return num > 0 ? clone : this;
    }

    /// <inheritdoc/>
    public IIdentifier Replace(int index, string? value)
    {
        var clone = Clone();
        var num = clone.Items.Replace(index, value);
        return num > 0 ? clone : this;
    }

    /// <inheritdoc/>
    public IIdentifier Add(TItem item)
    {
        var clone = Clone();
        var num = clone.Items.Add(item);
        return num > 0 ? clone : this;
    }

    /// <inheritdoc/>
    public IIdentifier Add(string? value)
    {
        var clone = Clone();
        var num = clone.Items.Add(value);
        return num > 0 ? clone : this;
    }

    /// <inheritdoc/>
    public IIdentifier AddRange(IEnumerable<TItem> range)
    {
        if (range is ICollection irange && irange.Count == 0) return this;
        if (range is ICollection<TItem> trange && trange.Count == 0) return this;

        var clone = Clone();
        var num = clone.Items.AddRange(range);
        return num > 0 ? clone : this;
    }

    /// <inheritdoc/>
    public IIdentifier AddRange(IEnumerable<string?> range)
    {
        if (range is ICollection irange && irange.Count == 0) return this;
        if (range is ICollection<TItem> trange && trange.Count == 0) return this;

        var clone = Clone();
        var num = clone.Items.AddRange(range);
        return num > 0 ? clone : this;
    }

    /// <inheritdoc/>
    public IIdentifier Insert(int index, TItem item)
    {
        var clone = Clone();
        var num = clone.Items.Insert(index, item);
        return num > 0 ? clone : this;
    }

    /// <inheritdoc/>
    public IIdentifier Insert(int index, string? value)
    {
        var clone = Clone();
        var num = clone.Items.Insert(index, value);
        return num > 0 ? clone : this;
    }

    /// <inheritdoc/>
    public IIdentifier InsertRange(int index, IEnumerable<TItem> range)
    {
        if (range is ICollection irange && irange.Count == 0) return this;
        if (range is ICollection<TItem> trange && trange.Count == 0) return this;

        var clone = Clone();
        var num = clone.Items.InsertRange(index, range);
        return num > 0 ? clone : this;
    }

    /// <inheritdoc/>
    public IIdentifier InsertRange(int index, IEnumerable<string?> range)
    {
        if (range is ICollection irange && irange.Count == 0) return this;
        if (range is ICollection<TItem> trange && trange.Count == 0) return this;

        var clone = Clone();
        var num = clone.Items.InsertRange(index, range);
        return num > 0 ? clone : this;
    }


    /// <inheritdoc/>
    public IIdentifier RemoveAt(int index)
    {
        var clone = Clone();
        var num = clone.Items.RemoveAt(index);
        return num > 0 ? clone : this;
    }

    /// <inheritdoc/>
    public IIdentifier RemoveRange(int index, int count)
    {
        if (count == 0 && index >= 0) return this;

        var clone = Clone();
        var num = clone.Items.RemoveRange(index, count);
        return num > 0 ? clone : this;
    }

    /// <inheritdoc/>
    public IIdentifier Remove(TKey? key)
    {
        if (Count == 0) return this;

        var clone = Clone();
        var num = clone.Items.Remove(key);
        return num > 0 ? clone : this;
    }

    /// <inheritdoc/>
    public IIdentifier RemoveLast(TKey? key)
    {
        if (Count == 0) return this;

        var clone = Clone();
        var num = clone.Items.RemoveLast(key);
        return num > 0 ? clone : this;
    }

    /// <inheritdoc/>
    public IIdentifier RemoveAll(TKey? key)
    {
        if (Count == 0) return this;

        var clone = Clone();
        var num = clone.Items.RemoveAll(key);
        return num > 0 ? clone : this;
    }

    /// <inheritdoc/>
    public IIdentifier Remove(Predicate<TItem> predicate)
    {
        if (Count == 0) return this;

        var clone = Clone();
        var num = clone.Items.Remove(predicate);
        return num > 0 ? clone : this;
    }

    /// <inheritdoc/>
    public IIdentifier RemoveLast(Predicate<TItem> predicate)
    {
        if (Count == 0) return this;

        var clone = Clone();
        var num = clone.Items.RemoveLast(predicate);
        return num > 0 ? clone : this;
    }

    /// <inheritdoc/>
    public IIdentifier RemoveAll(Predicate<TItem> predicate)
    {
        if (Count == 0) return this;

        var clone = Clone();
        var num = clone.Items.RemoveAll(predicate);
        return num > 0 ? clone : this;
    }

    /// <inheritdoc/>
    public IIdentifier Clear()
    {
        if (Count == 0) return this;

        var clone = Clone();
        var num = clone.Items.Clear();
        return num > 0 ? clone : this;
    }
}