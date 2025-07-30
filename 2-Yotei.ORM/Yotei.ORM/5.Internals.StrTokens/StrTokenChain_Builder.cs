namespace Yotei.ORM.Internals;

partial class StrTokenChain : IStrToken
{
    // ====================================================
    /// <summary>
    /// Represents a builder for <see cref="StrTokenChain"/> instances.
    /// </summary>
    [Cloneable]
    [DebuggerDisplay("{ToDebugString(5)}")]
    public partial class Builder : CoreList<IStrToken>
    {
        /// <summary>
        /// Initializes a new empty instance.
        /// </summary>
        public Builder() { }

        /// <summary>
        /// Initializes a new empty instance with the given initial capacity.
        /// </summary>
        /// <param name="capacity"></param>
        public Builder(int capacity) : this() => Capacity = capacity;

        /// <summary>
        /// Initializes a new instance with the elements of the given range.
        /// </summary>
        /// <param name="range"></param>
        public Builder(IEnumerable<IStrToken> range) : this() => AddRange(range);

        /// <summary>
        /// Copy constructor.
        /// </summary>
        /// <param name="source"></param>
        protected Builder(Builder source) : this() => AddRange(source);

        /// <inheritdoc/>
        public override string ToString() => Count == 0
            ? string.Empty
            : string.Concat(this.Select(x => x.ToString()));

        // ------------------------------------------------

        public override IStrToken ValidateItem(IStrToken item) => item.ThrowWhenNull();
        public override bool ExpandItems => true;
        public override bool IsValidDuplicate(IStrToken source, IStrToken item) => true;
        public override IEqualityComparer<IStrToken> Comparer => _Comparer ??= new TComparer();
        TComparer? _Comparer = null;
        readonly struct TComparer() : IEqualityComparer<IStrToken>
        {
            public bool Equals(IStrToken? x, IStrToken? y) => x?.EqualsEx(y) ?? false;
            public int GetHashCode(IStrToken obj) => throw new NotImplementedException();
        }

        // ------------------------------------------------

        /// <summary>
        /// Returns a new instance using the contents captured by this collection.
        /// </summary>
        /// <returns></returns>
        public virtual StrTokenChain CreateInstance() => new(this);
    }
}