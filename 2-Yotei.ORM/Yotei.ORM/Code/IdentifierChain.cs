namespace Yotei.ORM.Code;

// ========================================================
/// <inheritdoc cref="IIdentifierChain"/>
[FrozenList<string, IIdentifierPart>]
public partial class IdentifierChain : IIdentifierChain
{
    /// <inheritdoc/>
    public virtual IdentifierChainBuilder ToBuilder() => Items.Clone();
    IIdentifierChainBuilder IIdentifierChain.ToBuilder() => ToBuilder();

    protected override IdentifierChainBuilder Items { get; }

    // ----------------------------------------------------

    /// <summary>
    /// Initializes a new empty instance.
    /// </summary>
    /// <param name="engine"></param>
    public IdentifierChain(IEngine engine) : base() => Items = new(engine);

    /// <summary>
    /// Initializes a new instance with the given element.
    /// </summary>
    /// <param name="engine"></param>
    /// <param name="item"></param>
    public IdentifierChain(IEngine engine, IIdentifierPart item) : this(engine) => Items.Add(item);

    /// <summary>
    /// Initializes a new instance with the elements from the given range.
    /// </summary>
    /// <param name="engine"></param>
    /// <param name="range"></param>
    public IdentifierChain(
        IEngine engine, IEnumerable<IIdentifierPart> range) : this(engine) => Items.AddRange(range);

    /// <summary>
    /// Initializes a new instance with the elements obtained from the given value.
    /// </summary>
    /// <param name="engine"></param>
    /// <param name="value"></param>
    public IdentifierChain(IEngine engine, string? value) : this(engine) => Items.Add(value);

    /// <summary>
    /// Initializes a new instance with the elements obtained from the given range of values.
    /// </summary>
    /// <param name="engine"></param>
    /// <param name="range"></param>
    public IdentifierChain(
        IEngine engine, IEnumerable<string?> range) : this(engine) => Items.AddRange(range);

    /// <summary>
    /// Copy constructor.
    /// </summary>
    /// <param name="source"></param>
    IdentifierChain(IdentifierChain source) : this(source.Engine) => Items.AddRange(source.Items);

    /// <inheritdoc/>
    public override string ToString() => Items.ToString();

    // ----------------------------------------------------

    /// <inheritdoc/>
    public IEngine Engine => Items.Engine;

    /// <inheritdoc/>
    public string? Value => Items.Value;

    // ----------------------------------------------------

    /// <inheritdoc/>
    public new bool Contains(string? value) => base.Contains(value!);

    /// <inheritdoc/>
    public new int IndexOf(string? value) => base.IndexOf(value!);

    /// <inheritdoc/>
    public new int LastIndexOf(string? value) => base.LastIndexOf(value!);

    /// <inheritdoc/>
    public new List<int> IndexesOf(string? value) => base.IndexesOf(value!);

    // ----------------------------------------------------

    /// <inheritdoc/>
    public IdentifierChain Replace(int index, string? value)
    {
        var builder = Items.Clone();
        var done = builder.Replace(index, value);
        return done > 0 ? new(Engine, builder) : this;
    }
    IIdentifierChain IIdentifierChain.Replace(int index, string? value) => Replace(index, value);

    /// <inheritdoc/>
    public IdentifierChain Add(string? value)
    {
        var builder = Items.Clone();
        var done = builder.Add(value);
        return done > 0 ? new(Engine, builder) : this;
    }
    IIdentifierChain IIdentifierChain.Add(string? value) => Add(value);

    /// <inheritdoc/>
    public IdentifierChain AddRange(IEnumerable<string?> range)
    {
        var builder = Items.Clone();
        var done = builder.AddRange(range);
        return done > 0 ? new(Engine, builder) : this;
    }
    IIdentifierChain IIdentifierChain.AddRange(IEnumerable<string?> range) => AddRange(range);

    /// <inheritdoc/>
    public IdentifierChain Insert(int index, string? value)
    {
        var builder = Items.Clone();
        var done = builder.Insert(index, value);
        return done > 0 ? new(Engine, builder) : this;
    }
    IIdentifierChain IIdentifierChain.Insert(int index, string? value) => Insert(index, value);

    /// <inheritdoc/>
    public IdentifierChain InsertRange(int index, IEnumerable<string?> range)
    {
        var builder = Items.Clone();
        var done = builder.InsertRange(index, range);
        return done > 0 ? new(Engine, builder) : this;
    }
    IIdentifierChain IIdentifierChain.InsertRange(int index, IEnumerable<string?> range) => InsertRange(index, range);
}