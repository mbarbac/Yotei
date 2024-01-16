using TItem = Yotei.ORM.IParameter;
using TKey = string;

namespace Yotei.ORM.Code;

// ========================================================
/// <summary>
/// A builder of <see cref="IParameterList"/> instances.
/// </summary>
[Cloneable]
public sealed partial class ParameterListBuilder(IEngine engine) : CoreList<TKey, TItem>
{
    /// <summary>
    /// Copy constructor.
    /// </summary>
    /// <param name="source"></param>
    ParameterListBuilder(ParameterListBuilder source) : this(source.Engine) => AddRange(source);

    /// <summary>
    /// The engine this instance is associated with.
    /// </summary>
    public IEngine Engine { get; } = engine.ThrowWhenNull();

    // ----------------------------------------------------

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    protected override TItem ValidateItem(TItem item) => item.ThrowWhenNull();

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    protected override TKey GetKey(TItem item) => item.ThrowWhenNull().Name;

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    protected override TKey ValidateKey(TKey key) => key.NotNullNotEmpty();

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    protected override bool CompareKeys(TKey source, TKey item)
        => string.Compare(source, item, !Engine.CaseSensitiveNames) == 0;

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    protected override bool SameItem(TItem source, TItem item)
        => ReferenceEquals(source, item);

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    protected override List<int> GetDuplicates(TKey key) => base.GetDuplicates(key);

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    protected override bool AcceptDuplicate(TItem source, TItem item)
        => ReferenceEquals(source, item)
        ? true
        : throw new DuplicateException("Duplicated element.").WithData(item);
}