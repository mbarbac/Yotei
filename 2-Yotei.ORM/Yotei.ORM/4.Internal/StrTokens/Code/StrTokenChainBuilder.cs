namespace Yotei.ORM.Internal;

// ========================================================
/// <inheritdoc cref="IStrTokenchainBuilder"/>
[DebuggerDisplay("{ToDebugString(5)}")]
[Cloneable]
public partial class StrTokenChainBuilder : CoreList<IStrToken>, IStrTokenChainBuilder
{
    /// <summary>
    /// Initializes a new empty instance.
    /// </summary>
    public StrTokenChainBuilder() : base() { }
    
    /// <summary>
    /// Initializes a new empty instance with the given initial capacity.
    /// </summary>
    /// <param name="capacity"></param>
    public StrTokenChainBuilder(int capacity) : this() => Capacity = capacity;
    
    /// <summary>
    /// Initializes a new instance with the elements of the given range.
    /// </summary>
    /// <param name="range"></param>
    public StrTokenChainBuilder(IEnumerable<IStrToken> range) : this() => AddRange(range);
    
    /// <summary>
    /// Copy constructor.
    /// </summary>
    /// <param name="source"></param>
    protected StrTokenChainBuilder(StrTokenChainBuilder source) : this() => AddRange(source);

    /// <inheritdoc/>
    public override IStrToken ValidateItem(IStrToken item) => item.ThrowWhenNull();

    /// <inheritdoc/>
    public override bool ExpandItems => true;

    /// <inheritdoc/>
    public override bool IncludeDuplicated(IStrToken item, IStrToken source) => true;

    /// <inheritdoc/>
    public override IEqualityComparer<IStrToken> Comparer => EqualityComparer<IStrToken>.Default;

    /// <inheritdoc cref="IStrTokenChainBuilder.ToInstance"/>
    public virtual StrTokenChain ToInstance() => new(this);
    IStrTokenChain IStrTokenChainBuilder.ToInstance() => ToInstance();
}