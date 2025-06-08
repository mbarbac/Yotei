namespace Yotei.ORM.Internal.Code;

partial class StrTokenChain
{
    // ====================================================
    /// <inheritdoc cref="IStrTokenChain.IBuilder"/>
    [DebuggerDisplay("{ToDebugString(5)}")]
    [Cloneable]
    public partial class Builder : CoreList<IStrToken>, IStrTokenChain.IBuilder
    {
        /// <summary>
        /// Initializes a new empty instance.
        /// </summary>
        public Builder() { }

        /// <summary>
        /// Initializes a new instance with the given initial capacity.
        /// </summary>
        /// <param name="capacity"></param>
        public Builder(int capacity) : this() => Capacity = capacity;

        /// <summary>
        /// Initializes a new instance with the elements from the given range.
        /// </summary>
        /// <param name="range"></param>
        public Builder(IEnumerable<IStrToken> range) : this() => AddRange(range);

        /// <summary>
        /// Copy constructor.
        /// </summary>
        /// <param name="source"></param>
        public Builder(Builder source) : this() => AddRange(source);

        // ------------------------------------------------

        /// <inheritdoc/>
        public override IStrToken ValidateItem(IStrToken item) => item.ThrowWhenNull();

        /// <inheritdoc/>
        public override IEqualityComparer<IStrToken> Comparer => EqualityComparer<IStrToken>.Default;

        /// <inheritdoc/>
        public override bool ExpandItems => true;

        /// <inheritdoc/>
        public override bool IsValidDuplicate(IStrToken source, IStrToken item) => true;

        // ------------------------------------------------

        /// <inheritdoc cref="IStrTokenChain.IBuilder.ToInstance"/>
        public virtual StrTokenChain ToInstance() => throw null;
        IStrTokenChain IStrTokenChain.IBuilder.ToInstance() => ToInstance();
    }
}