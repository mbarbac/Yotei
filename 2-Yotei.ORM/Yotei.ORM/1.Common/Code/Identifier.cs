using T = Yotei.ORM.IIdentifierPart;
using K = string;

namespace Yotei.ORM.Code;

// ========================================================
/// <inheritdoc cref="IIdentifier"/>
[Cloneable]
public sealed partial class Identifier : FrozenList<K?, T>, IIdentifier
{
    protected override IdentifierBuilder Items => _Items ??= new(Engine);
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
    public Identifier(IEngine engine, T item) : this(engine) => Items.Add(item);

    /// <summary>
    /// Initializes a new instance with the elements of the given range.
    /// </summary>
    /// <param name="engine"></param>
    /// <param name="range"></param>
    public Identifier(IEngine engine, IEnumerable<T> range) : this(engine) => Items.AddRange(range);

    /// <summary>
    /// Initializes a new instance with the elements obtained from the given value.
    /// </summary>
    /// <param name="engine"></param>
    /// <param name="value"></param>
    public Identifier(IEngine engine, string? value) : this(engine) => Items.Add(value);

    /// <summary>
    /// Initializes a new instance with the elements obtained from the given range of values.
    /// </summary>
    /// <param name="engine"></param>
    /// <param name="range"></param>
    public Identifier(IEngine engine, IEnumerable<string?> range) : this(engine) => Items.AddRange(range);

    /// <summary>
    /// Copy constructor.
    /// </summary>
    /// <param name="source"></param>
    Identifier(Identifier source) : this(source.Engine) => Items.AddRange(source);

    /// <inheritdoc/>
    public override string ToString() => Value ?? string.Empty;

    /// <inheritdoc/>
    public IEngine Engine { get; }

    /// <inheritdoc/>
    public string? Value => Items.Value;

    // ----------------------------------------------------

    /// <inheritdoc/>
    public IIdentifier Replace(int index, string? value)
    {
        var clone = Clone();
        var num = clone.Items.Replace(index, value);
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
    public IIdentifier AddRange(IEnumerable<string?> range)
    {
        var clone = Clone();
        var num = clone.Items.AddRange(range);
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
    public IIdentifier InsertRange(int index, IEnumerable<string?> range)
    {
        var clone = Clone();
        var num = clone.Items.InsertRange(index, range);
        return num > 0 ? clone : this;
    }

    // ----------------------------------------------------

    /// <inheritdoc/>
    public override IIdentifier GetRange(int index, int count) => (IIdentifier)base.GetRange(index, count);

    /// <inheritdoc/>
    public override IIdentifier Replace(int index, T item) => (IIdentifier)base.Replace(index, item);

    /// <inheritdoc/>
    public override IIdentifier Add(T item) => (IIdentifier)base.Add(item);

    /// <inheritdoc/>
    public override IIdentifier AddRange(IEnumerable<T> range) => (IIdentifier)base.AddRange(range);

    /// <inheritdoc/>
    public override IIdentifier Insert(int index, T item) => (IIdentifier)base.Insert(index, item);

    /// <inheritdoc/>
    public override IIdentifier InsertRange(int index, IEnumerable<T> range) => (IIdentifier)base.InsertRange(index, range);

    /// <inheritdoc/>
    public override IIdentifier RemoveAt(int index) => (IIdentifier)base.RemoveAt(index);

    /// <inheritdoc/>
    public override IIdentifier RemoveRange(int index, int count) => (IIdentifier)base.RemoveRange(index, count);

    /// <inheritdoc/>
    public override IIdentifier Remove(K? key) => (IIdentifier)base.Remove(key);

    /// <inheritdoc/>
    public override IIdentifier RemoveLast(K? key) => (IIdentifier)base.RemoveLast(key);

    /// <inheritdoc/>
    public override IIdentifier RemoveAll(K? key) => (IIdentifier)base.RemoveAll(key);

    /// <inheritdoc/>
    public override IIdentifier Remove(Predicate<T> predicate) => (IIdentifier)base.Remove(predicate);

    /// <inheritdoc/>
    public override IIdentifier RemoveLast(Predicate<T> predicate) => (IIdentifier)base.RemoveLast(predicate);

    /// <inheritdoc/>
    public override IIdentifier RemoveAll(Predicate<T> predicate) => (IIdentifier)base.RemoveAll(predicate);

    /// <inheritdoc/>
    public override IIdentifier Clear() => (IIdentifier)base.Clear();
}