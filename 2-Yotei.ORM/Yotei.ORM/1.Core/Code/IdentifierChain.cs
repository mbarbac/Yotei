using IBuilder = Yotei.ORM.IIdentifierChainBuilder;
using Builder = Yotei.ORM.Code.IdentifierChainBuilder;
using IItem = Yotei.ORM.IIdentifierPart;
using TKey = string;

namespace Yotei.ORM.Code;

// ========================================================
/// <inheritdoc cref="IIdentifierChain"/>
[DebuggerDisplay("{ToDebugString(5)}")]
[InvariantList<AsNullable<TKey>, IItem>]
public partial class IdentifierChain : IIdentifierChain
{
    /// <summary>
    /// Initializes a new empty instance.
    /// </summary>
    /// <param name="engine"></param>
    public IdentifierChain(IEngine engine) => Items = new(engine);

    /// <summary>
    /// Initializes a new empty instance with the given capacity.
    /// </summary>
    /// <param name="engine"></param>
    /// <param name="capacity"></param>
    public IdentifierChain(IEngine engine, int capacity) => Items = new(engine, capacity);

    /// <summary>
    /// Initializes a new instance with the elements of the given range.
    /// </summary>
    /// <param name="engine"></param>
    /// <param name="range"></param>
    public IdentifierChain(
        IEngine engine, IEnumerable<IItem> range) : this(engine) => Items.AddRange(range);

    /// <summary>
    /// Copy constructor.
    /// </summary>
    /// <param name="source"></param>
    protected IdentifierChain(
        IdentifierChain source) : this(source.Engine) => Items.AddRange(source.Items);

    /// <inheritdoc/>
    public override string ToString() => Items.ToString();

    // ----------------------------------------------------

    /// <summary>
    /// Returns a new instance with the elements obtained from the given value.
    /// </summary>
    /// <param name="engine"></param>
    /// <param name="value"></param>
    public IdentifierChain(IEngine engine, string? value) : this(engine) => Items.Add(value);

    /// <summary>
    /// Returns a new instance with the elements obtained from the given range of values.
    /// </summary>
    /// <param name="engine"></param>
    /// <param name="range"></param>
    public IdentifierChain(
        IEngine engine, IEnumerable<string?> range) : this(engine) => Items.AddRange(range);

    // ----------------------------------------------------

    /// <inheritdoc/>
    public bool Equals(IIdentifier? other)
    {
        if (ReferenceEquals(this, other)) return true;
        if (other is null) return false;
        // Not using " || other is not IIdentifierChain valid)" because we wnat Value equality

        if (!Engine.Equals(other.Engine)) return false; // Using "other" instead of "valid"...
        return string.Compare(Value, other.Value, !Engine.CaseSensitiveNames) == 0; // Using "other" instead of "valid"...
    }

    /// <inheritdoc/>
    public override bool Equals(object? obj) => Equals(obj as IIdentifier); // Not IIdentifierChain

    // Equality operator.
    public static bool operator ==(IdentifierChain? x, IIdentifier? y)
    {
        if (x is null && y is null) return true;
        if (x is null || y is null) return false;
        return x.Equals(y);
    }

    // Inequality operator.
    public static bool operator !=(IdentifierChain? x, IIdentifier? y) => !(x == y);

    /// <inheritdoc/>
    public override int GetHashCode()
    {
        var code = 0;
        for (int i = 0; i < Items.Count; i++) code = HashCode.Combine(code, Items[i]);
        return code;
    }

    // ----------------------------------------------------

    protected override Builder Items { get; }

    /// <inheritdoc/>
    public override Builder GetBuilder() => Items.Clone();
    IBuilder IIdentifierChain.GetBuilder() => GetBuilder();

    /// <inheritdoc/>
    public IEngine Engine => Items.Engine;

    /// <inheritdoc/>
    public string? Value => Items.Value;

    // ----------------------------------------------------

    /// <inheritdoc cref="IIdentifierChain.Replace(int, TKey?)"/>
    public virtual IdentifierChain Replace(int index, string? value)
    {
        var builder = GetBuilder();
        var done = builder.Replace(index, value);
        return done > 0 ? builder.ToInstance() : this;
    }
    IIdentifierChain IIdentifierChain.Replace(int index, string? value) => Replace(index, value);

    /// <inheritdoc cref="IIdentifierChain.Add(TKey?)"/>
    public virtual IdentifierChain Add(string? value)
    {
        var builder = GetBuilder();
        var done = builder.Add(value);
        return done > 0 ? builder.ToInstance() : this;
    }
    IIdentifierChain IIdentifierChain.Add(string? value) => Add(value);

    /// <inheritdoc cref="IIdentifierChain.AddRange(IEnumerable{TKey?})"/>
    public virtual IdentifierChain AddRange(IEnumerable<string?> range)
    {
        var builder = GetBuilder();
        var done = builder.AddRange(range);
        return done > 0 ? builder.ToInstance() : this;
    }
    IIdentifierChain IIdentifierChain.AddRange(IEnumerable<string?> range) => AddRange(range);

    /// <inheritdoc cref="IIdentifierChain.Insert(int, TKey?)"/>
    public virtual IdentifierChain Insert(int index, string? value)
    {
        var builder = GetBuilder();
        var done = builder.Insert(index, value);
        return done > 0 ? builder.ToInstance() : this;
    }
    IIdentifierChain IIdentifierChain.Insert(int index, string? value) => Insert(index, value);

    /// <inheritdoc cref="IIdentifierChain.InsertRange(int, IEnumerable{TKey?})"/>
    public virtual IdentifierChain InsertRange(int index, IEnumerable<string?> range)
    {
        var builder = GetBuilder();
        var done = builder.InsertRange(index, range);
        return done > 0 ? builder.ToInstance() : this;
    }
    IIdentifierChain IIdentifierChain.InsertRange(int index, IEnumerable<string?> range) => InsertRange(index, range);
}