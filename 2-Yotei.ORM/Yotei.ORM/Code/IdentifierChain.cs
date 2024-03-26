using T = Yotei.ORM.IIdentifierPart;
using K = string;

namespace Yotei.ORM.Code;

// ========================================================
/// <inheritdoc cref="IIdentifierChain"/>
[Cloneable]
public sealed partial class IdentifierChain : FrozenList<K?, T>, IIdentifierChain
{
    protected override IdentifierChainBuilder Items { get; }

    /// <summary>
    /// Initializes a new empty instance.
    /// </summary>
    /// <param name="engine"></param>
    public IdentifierChain(IEngine engine) => Items = new(engine);

    /// <summary>
    /// Initializes a new instance with the given element.
    /// </summary>
    /// <param name="engine"></param>
    /// <param name="item"></param>
    public IdentifierChain(IEngine engine, T item) : this(engine) => Items.Add(item);

    /// <summary>
    /// Initializes a new instance with the elements from the given range.
    /// </summary>
    /// <param name="engine"></param>
    /// <param name="range"></param>
    public IdentifierChain(IEngine engine, IEnumerable<T> range) : this(engine) => Items.AddRange(range);

    /// <summary>
    /// Copy constructor.
    /// </summary>
    /// <param name="source"></param>
    IdentifierChain(IdentifierChain source) : this(source.Engine) => Items.AddRange(source);

    /// <summary>
    /// Initializes a new instance with the elements obtained from the given value.
    /// </summary>
    /// <param name="engine"></param>
    /// <param name="item"></param>
    public IdentifierChain(IEngine engine, string? value) : this(engine) => Items.Add(value);

    /// <summary>
    /// Initializes a new instance with the elements obtained from the given values.
    /// </summary>
    /// <param name="engine"></param>
    /// <param name="values"></param>
    public IdentifierChain(IEngine engine, IEnumerable<string?> values) : this(engine) => Items.AddRange(values);

    /// <inheritdoc/>
    public override string ToString() => Items.ToString();

    // ----------------------------------------------------

    /// <inheritdoc/>
    public bool Equals(IIdentifier? other)
    {
        if (other is null) return false;
        if (!Engine.Equals(other.Engine)) return false;

        return string.Compare(Value, other.Value, !Engine.CaseSensitiveNames) == 0;
    }
    public override bool Equals(object? obj) => Equals(obj as IIdentifier);
    public static bool operator ==(IdentifierChain x, IIdentifier y) => x is not null && x.Equals(y);
    public static bool operator !=(IdentifierChain x, IIdentifier y) => !(x == y);
    public override int GetHashCode() => HashCode.Combine(Value);

    // ----------------------------------------------------

    /// <inheritdoc/>
    public IEngine Engine => Items.Engine;

    /// <inheritdoc/>
    public string? Value => Items.Value;

    // ----------------------------------------------------

    /// <inheritdoc/>
    public override IIdentifierChain GetRange(int index, int count) => (IIdentifierChain)base.GetRange(index, count);

    /// <inheritdoc/>
    public override IIdentifierChain Replace(int index, T item) => (IIdentifierChain)base.Replace(index, item);

    /// <inheritdoc/>
    public override IIdentifierChain Add(T item) => (IIdentifierChain)base.Add(item);

    /// <inheritdoc/>
    public override IIdentifierChain AddRange(IEnumerable<T> range) => (IIdentifierChain)base.AddRange(range);

    /// <inheritdoc/>
    public override IIdentifierChain Insert(int index, T item) => (IIdentifierChain)base.Insert(index, item);

    /// <inheritdoc/>
    public override IIdentifierChain InsertRange(int index, IEnumerable<T> range) => (IIdentifierChain)base.InsertRange(index, range);

    /// <inheritdoc/>
    public override IIdentifierChain RemoveAt(int index) => (IIdentifierChain)base.RemoveAt(index);

    /// <inheritdoc/>
    public override IIdentifierChain RemoveRange(int index, int count) => (IIdentifierChain)base.RemoveRange(index, count);

    /// <inheritdoc/>
    public override IIdentifierChain Remove(K? key) => (IIdentifierChain)base.Remove(key);

    /// <inheritdoc/>
    public override IIdentifierChain RemoveLast(K? key) => (IIdentifierChain)base.RemoveLast(key);

    /// <inheritdoc/>
    public override IIdentifierChain RemoveAll(K? key) => (IIdentifierChain)base.RemoveAll(key);

    /// <inheritdoc/>
    public override IIdentifierChain Remove(Predicate<T> predicate) => (IIdentifierChain)base.Remove(predicate);

    /// <inheritdoc/>
    public override IIdentifierChain RemoveLast(Predicate<T> predicate) => (IIdentifierChain)base.RemoveLast(predicate);

    /// <inheritdoc/>
    public override IIdentifierChain RemoveAll(Predicate<T> predicate) => (IIdentifierChain)base.RemoveAll(predicate);

    /// <inheritdoc/>
    public override IIdentifierChain Clear() => (IIdentifierChain)base.Clear();

    // ----------------------------------------------------

    /// <inheritdoc/>
    public IIdentifierChain Replace(int index, string? value)
    {
        var clone = Clone();
        var num = clone.Items.Replace(index, value);
        return num > 0 ? clone : this;
    }

    /// <inheritdoc/>
    public IIdentifierChain Add(string? value)
    {
        var clone = Clone();
        var num = clone.Items.Add(value);
        return num > 0 ? clone : this;
    }

    /// <inheritdoc/>
    public IIdentifierChain AddRange(IEnumerable<string?> range)
    {
        var clone = Clone();
        var num = clone.Items.AddRange(range);
        return num > 0 ? clone : this;
    }

    /// <inheritdoc/>
    public IIdentifierChain Insert(int index, string? value)
    {
        var clone = Clone();
        var num = clone.Items.Insert(index, value);
        return num > 0 ? clone : this;
    }

    /// <inheritdoc/>
    public IIdentifierChain InsertRange(int index, IEnumerable<string?> range)
    {
        var clone = Clone();
        var num = clone.Items.InsertRange(index, range);
        return num > 0 ? clone : this;
    }
}