
namespace Yotei.ORM.Code;

// ========================================================
/// <inheritdoc cref="IIdentifierChain"/>
[DebuggerDisplay("{ToDebugString(5)}")]
[InvariantList<AsNullable<string>, IIdentifierPart>]
public partial class IdentifierChain : IIdentifierChain
{
    /// <summary>
    /// Initializes a new empty instance.
    /// </summary>
    /// <param name="engine"></param>
    public IdentifierChain(IEngine engine) => Items = new(engine);
    
    /// <summary>
    /// Initializes a new instance with the elements of the given range.
    /// </summary>
    /// <param name="engine"></param>
    /// <param name="range"></param>
    public IdentifierChain(
        IEngine engine, IEnumerable<IIdentifierPart> range) : this(engine) => Items.AddRange(range);
    
    /// <summary>
    /// Copy constructor.
    /// </summary>
    /// <param name="source"></param>
    protected IdentifierChain(
        IdentifierChain source) : this(source.Engine) => Items.AddRange(source);

    /// <inheritdoc/>
    public override string ToString() => Items.ToString();

    // ----------------------------------------------------

    /// <summary>
    /// Initializes a new instance with the elements obtained from the given value.
    /// </summary>
    /// <param name="engine"></param>
    /// <param name="value"></param>
    public IdentifierChain(
        IEngine engine, string? value) : this(engine) => Items.Add(value);

    /// <summary>
    /// Initializes a new instance with the elements obtained from each value in the range.
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
        if (other is null) return false; // No " || other is not IIdentifierChain valid" as we want IIdentifier equality

        if (!Engine.Equals(other.Engine)) return false; // other instead of valid...
        return string.Compare(Value, other.Value, !Engine.CaseSensitiveNames) == 0; // other instead of valid...
    }

    /// <inheritdoc/>
    public override bool Equals(object? obj) => Equals(obj as IIdentifier); // No IIdentifierPart

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

    /// <summary>
    /// The engine this instance is associated with.
    /// </summary>
    public IEngine Engine => Items.Engine;

    /// <summary>
    /// The value carried by this identifier, or null if it represents an empty or missed one.
    /// </summary>
    public string? Value => Items.Value;

    // ----------------------------------------------------

    /// <inheritdoc/>
    public virtual IIdentifierChain Replace(int index, string? value)
    {
        var builder = GetBuilder();
        var done = builder.Replace(index, value);
        return done > 0 ? builder.ToInstance() : this;
    }

    /// <inheritdoc/>
    public virtual IIdentifierChain Add(string? value)
    {
        var builder = GetBuilder();
        var done = builder.Add(value);
        return done > 0 ? builder.ToInstance() : this;
    }

    /// <inheritdoc/>
    public virtual IIdentifierChain AddRange(IEnumerable<string?> range)
    {
        var builder = GetBuilder();
        var done = builder.AddRange(range);
        return done > 0 ? builder.ToInstance() : this;
    }

    /// <inheritdoc/>
    public virtual IIdentifierChain Insert(int index, string? value)
    {
        var builder = GetBuilder();
        var done = builder.Insert(index, value);
        return done > 0 ? builder.ToInstance() : this;
    }

    /// <inheritdoc/>
    public virtual IIdentifierChain InsertRange(int index, IEnumerable<string?> range)
    {
        var builder = GetBuilder();
        var done = builder.InsertRange(index, range);
        return done > 0 ? builder.ToInstance() : this;
    }
}