using TItem = Yotei.ORM.IIdentifierPart;
using TKey = string;

namespace Yotei.ORM.Code;

// ========================================================
/// <inheritdoc cref="IIdentifier"/>
[Cloneable]
public sealed partial class Identifier : FrozenList<TKey?, TItem>, IIdentifier
{
    protected override IdentifierBuilder Items => _Items ??= new IdentifierBuilder(Engine);
    IdentifierBuilder? _Items = null;

    /// <summary>
    /// Initializes a new empty instance.
    /// </summary>
    /// <param name="engine"></param>
    public Identifier(IEngine engine) => Engine = engine.ThrowWhenNull();

    /// <summary>
    /// Initializes a new instance with the given element.
    /// </summary>
    /// <param name="engine"></param>
    /// <param name="item"></param>
    public Identifier(IEngine engine, TItem item) : this(engine) => Items.Add(item);

    /// <summary>
    /// Initializes a new instance with the elements from the given range.
    /// </summary>
    /// <param name="engine"></param>
    /// <param name="range"></param>
    public Identifier(IEngine engine, IEnumerable<TItem> range)
        : this(engine)
        => Items.AddRange(range);

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
    public override string ToString() => Value ?? string.Empty;

    // ----------------------------------------------------

    /// <inheritdoc/>
    public IEngine Engine { get; }

    /// <inheritdoc/>
    public string? Value
    {
        get => Items.Value;
        init => Items.Value = value;
    }

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
    public override IIdentifier GetRange(int index, int count) => (IIdentifier)base.GetRange(index, count);

    /// <inheritdoc/>
    public override IIdentifier Replace(int index, TItem item) => (IIdentifier)base.Replace(index, item);

    /// <inheritdoc/>
    public IIdentifier Replace(int index, string? value)
    {
        var clone = Clone();
        var num = clone.Items.Replace(index, value);
        return num > 0 ? clone : this;
    }

    /// <inheritdoc/>
    public override IIdentifier Add(TItem item) => (IIdentifier)base.Add(item);

    /// <inheritdoc/>
    public IIdentifier Add(string? value)
    {
        var clone = Clone();
        var num = clone.Items.Add(value);
        return num > 0 ? clone : this;
    }

    /// <inheritdoc/>
    public override IIdentifier AddRange(IEnumerable<TItem> range) => (IIdentifier)base.AddRange(range);

    /// <inheritdoc/>
    public IIdentifier AddRange(IEnumerable<string?> range)
    {
        var clone = Clone();
        var num = clone.Items.AddRange(range);
        return num > 0 ? clone : this;
    }

    /// <inheritdoc/>
    public override IIdentifier Insert(int index, TItem item) => (IIdentifier)base.Insert(index, item);

    /// <inheritdoc/>
    public IIdentifier Insert(int index, string? value)
    {
        var clone = Clone();
        var num = clone.Items.Insert(index, value);
        return num > 0 ? clone : this;
    }

    /// <inheritdoc/>
    public override IIdentifier InsertRange(int index, IEnumerable<TItem> range) => (IIdentifier)base.InsertRange(index, range);

    /// <inheritdoc/>
    public IIdentifier InsertRange(int index, IEnumerable<string?> range)
    {
        var clone = Clone();
        var num = clone.Items.InsertRange(index, range);
        return num > 0 ? clone : this;
    }

    /// <inheritdoc/>
    public override IIdentifier RemoveAt(int index) => (IIdentifier)base.RemoveAt(index);

    /// <inheritdoc/>
    public override IIdentifier RemoveRange(int index, int count) => (IIdentifier)base.RemoveRange(index, count);

    /// <inheritdoc/>
    public override IIdentifier Remove(TKey? key) => (IIdentifier)base.Remove(key);

    /// <inheritdoc/>
    public override IIdentifier RemoveLast(TKey? key) => (IIdentifier)base.RemoveLast(key);

    /// <inheritdoc/>
    public override IIdentifier RemoveAll(TKey? key) => (IIdentifier)base.RemoveAll(key);

    /// <inheritdoc/>
    public override IIdentifier Remove(Predicate<TItem> predicate) => (IIdentifier)base.Remove(predicate);

    /// <inheritdoc/>
    public override IIdentifier RemoveLast(Predicate<TItem> predicate) => (IIdentifier)base.RemoveLast(predicate);

    /// <inheritdoc/>
    public override IIdentifier RemoveAll(Predicate<TItem> predicate) => (IIdentifier)base.RemoveAll(predicate);

    /// <inheritdoc/>
    public override IIdentifier Clear() => (IIdentifier)base.Clear();
}