namespace Yotei.ORM.Tools.Code;

// ========================================================
/// <summary>
/// Represents a builder for frozen instances.
/// </summary>
/// <typeparam name="TKey"></typeparam>
/// <typeparam name="TItem"></typeparam>
/// <remarks>
/// This type shall be used as a template and not for inheritance purposes.
/// </remarks>
public partial class FrozenBuilder<TKey, TItem> : CoreList<TKey, TItem>
{
    /// <summary>
    /// Initializes a new empty instance.
    /// </summary>
    /// <param name="engine"></param>
    public FrozenBuilder(IEngine engine) => Engine = engine.ThrowWhenNull();

    /// <summary>
    /// Initializes a new instance with the given element.
    /// </summary>
    /// <param name="engine"></param>
    /// <param name="item"></param>
    public FrozenBuilder(IEngine engine, TItem item) : this(engine) => Add(item);

    /// <summary>
    /// Initializes a new instance with the elements from the given range.
    /// </summary>
    /// <param name="engine"></param>
    /// <param name="range"></param>
    public FrozenBuilder(IEngine engine, IEnumerable<TItem> range)
        : this(engine)
        => AddRange(range);

    /// <summary>
    /// Copy constructor.
    /// </summary>
    /// <param name="source"></param>
    protected FrozenBuilder(FrozenBuilder<TKey, TItem> source)
        : this(source.Engine)
        => AddRange(source);

    // ----------------------------------------------------

    /// <summary>
    /// The engine this instance is associated with.
    /// </summary>
    public IEngine Engine { get; }

    // ----------------------------------------------------

    /// <inheritdoc/>
    protected override TItem ValidateItem(TItem item) => base.ValidateItem(item);

    /// <inheritdoc/>
    protected override TKey GetKey(TItem item) => base.GetKey(item);

    /// <inheritdoc/>
    protected override TKey ValidateKey(TKey key) => base.ValidateKey(key);

    /// <inheritdoc/>
    protected override bool CompareKeys(TKey source, TKey item) => base.CompareKeys(source, item);

    /// <inheritdoc/>
    protected override bool SameItem(TItem source, TItem item) => base.SameItem(source, item);

    /// <inheritdoc/>
    protected override List<int> GetDuplicates(TKey key) => base.GetDuplicates(key);

    /// <inheritdoc/>
    protected override bool AcceptDuplicate(TItem source, TItem item) => base.AcceptDuplicate(source, item);
}