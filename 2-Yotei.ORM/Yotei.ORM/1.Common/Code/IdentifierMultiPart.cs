using T = Yotei.ORM.IIdentifierSinglePart;
using K = string;

namespace Yotei.ORM.Code;

// ========================================================
/// <inheritdoc cref="IIdentifierMultiPart"/>
[Cloneable]
public sealed partial class IdentifierMultiPart : FrozenList<K?, T>, IIdentifierMultiPart
{
    protected override IdentifierMultiPartBuilder Items => _Items ??= new(Engine);
    IdentifierMultiPartBuilder? _Items = null;

    // ----------------------------------------------------

    /// <summary>
    /// Initializes a new empty instance.
    /// </summary>
    /// <param name="engine"></param>
    public IdentifierMultiPart(IEngine engine) => Engine = engine.ThrowWhenNull();

    /// <summary>
    /// Initializes a new instance with the given element.
    /// </summary>
    /// <param name="engine"></param>
    /// <param name="item"></param>
    public IdentifierMultiPart(IEngine engine, T item) : this(engine) => Items.Add(item);

    /// <summary>
    /// Initializes a new instance with the elements of the given range.
    /// </summary>
    /// <param name="engine"></param>
    /// <param name="range"></param>
    public IdentifierMultiPart(IEngine engine, IEnumerable<T> range) : this(engine) => Items.AddRange(range);

    /// <summary>
    /// Initializes a new instance with the elements obtained from the given value.
    /// </summary>
    /// <param name="engine"></param>
    /// <param name="value"></param>
    public IdentifierMultiPart(IEngine engine, string? value) : this(engine) => Items.Add(value);

    /// <summary>
    /// Initializes a new instance with the elements obtained from the given range of values.
    /// </summary>
    /// <param name="engine"></param>
    /// <param name="range"></param>
    public IdentifierMultiPart(IEngine engine, IEnumerable<string?> range) : this(engine) => Items.AddRange(range);

    /// <summary>
    /// Copy constructor.
    /// </summary>
    /// <param name="source"></param>
    IdentifierMultiPart(IdentifierMultiPart source) : this(source.Engine) => Items.AddRange(source);

    /// <inheritdoc/>
    public override string ToString() => Value ?? string.Empty;

    /// <inheritdoc/>
    public IEngine Engine { get; }

    /// <inheritdoc/>
    public string? Value => _Value ??= (Count == 0 ? null : string.Join('.', this.Select(x => x.Value)));
    string? _Value = null;

    // ----------------------------------------------------

    /// <inheritdoc/>
    public IIdentifierMultiPart Replace(int index, string? value)
    {
        var clone = Clone();
        var num = clone.Items.Replace(index, value);
        return num > 0 ? clone : this;
    }

    /// <inheritdoc/>
    public IIdentifierMultiPart Add(string? value)
    {
        var clone = Clone();
        var num = clone.Items.Add(value);
        return num > 0 ? clone : this;
    }

    /// <inheritdoc/>
    public IIdentifierMultiPart AddRange(IEnumerable<string?> range)
    {
        var clone = Clone();
        var num = clone.Items.AddRange(range);
        return num > 0 ? clone : this;
    }

    /// <inheritdoc/>
    public IIdentifierMultiPart Insert(int index, string? value)
    {
        var clone = Clone();
        var num = clone.Items.Insert(index, value);
        return num > 0 ? clone : this;
    }

    /// <inheritdoc/>
    public IIdentifierMultiPart InsertRange(int index, IEnumerable<string?> range)
    {
        var clone = Clone();
        var num = clone.Items.InsertRange(index, range);
        return num > 0 ? clone : this;
    }

    // ----------------------------------------------------

    /// <inheritdoc/>
    public override IIdentifierMultiPart GetRange(int index, int count)
    {
        var r = (IdentifierMultiPart)base.GetRange(index, count);
        r.Items.Reduce();
        return r;
    }

    /// <inheritdoc/>
    public override IIdentifierMultiPart Replace(int index, T item) => (IIdentifierMultiPart)base.Replace(index, item);

    /// <inheritdoc/>
    public override IIdentifierMultiPart Add(T item) => (IIdentifierMultiPart)base.Add(item);

    /// <inheritdoc/>
    public override IIdentifierMultiPart AddRange(IEnumerable<T> range) => (IIdentifierMultiPart)base.AddRange(range);

    /// <inheritdoc/>
    public override IIdentifierMultiPart Insert(int index, T item) => (IIdentifierMultiPart)base.Insert(index, item);

    /// <inheritdoc/>
    public override IIdentifierMultiPart InsertRange(int index, IEnumerable<T> range) => (IIdentifierMultiPart)base.InsertRange(index, range);

    /// <inheritdoc/>
    public override IIdentifierMultiPart RemoveAt(int index) => (IIdentifierMultiPart)base.RemoveAt(index);

    /// <inheritdoc/>
    public override IIdentifierMultiPart RemoveRange(int index, int count) => (IIdentifierMultiPart)base.RemoveRange(index, count);

    /// <inheritdoc/>
    public override IIdentifierMultiPart Remove(K? key) => (IIdentifierMultiPart)base.Remove(key);

    /// <inheritdoc/>
    public override IIdentifierMultiPart RemoveLast(K? key) => (IIdentifierMultiPart)base.RemoveLast(key);

    /// <inheritdoc/>
    public override IIdentifierMultiPart RemoveAll(K? key) => (IIdentifierMultiPart)base.RemoveAll(key);

    /// <inheritdoc/>
    public override IIdentifierMultiPart Remove(Predicate<T> predicate) => (IIdentifierMultiPart)base.Remove(predicate);

    /// <inheritdoc/>
    public override IIdentifierMultiPart RemoveLast(Predicate<T> predicate) => (IIdentifierMultiPart)base.RemoveLast(predicate);

    /// <inheritdoc/>
    public override IIdentifierMultiPart RemoveAll(Predicate<T> predicate) => (IIdentifierMultiPart)base.RemoveAll(predicate);

    /// <inheritdoc/>
    public override IIdentifierMultiPart Clear() => (IIdentifierMultiPart)base.Clear();
}