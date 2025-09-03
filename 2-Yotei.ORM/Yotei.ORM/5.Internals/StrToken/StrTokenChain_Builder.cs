namespace Yotei.ORM.Internals;

partial class StrTokenChain
{
    // ====================================================
    /// <summary>
    /// Represents a builder for <see cref="IHost"/> instances.
    /// </summary>
    [Cloneable]
    [DebuggerDisplay("{ToDebugString(5)}")]
    public partial class Builder : CoreList<IStrToken>
    {
        /// <summary>
        /// Initializes a new empty instance.
        /// </summary>
        public Builder() : base() { }

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
        protected Builder(Builder source) : this() => AddRange(source);

        /// <inheritdoc/>
        public override string ToString() => Count == 0
            ? string.Empty
            : string.Concat(this.Select(x => x.ToString()));

        // ------------------------------------------------

        /// <inheritdoc/>
        public override IStrToken ValidateItem(IStrToken item) => item.ThrowWhenNull();

        /// <inheritdoc/>
        public override bool ExpandItems => true;

        /// <inheritdoc/>
        public override bool IsValidDuplicate(IStrToken source, IStrToken item) => true;

        /// <inheritdoc/>
        public override IEqualityComparer<IStrToken> Comparer => _Comparer ??= new();
        MyComparer? _Comparer;
        readonly struct MyComparer() : IEqualityComparer<IStrToken>
        {
            public bool Equals(IStrToken? x, IStrToken? y)
            {
                if (x is null && y is null) return true;
                if (x is null || y is null) return false;

                return x?.Equals(y) ?? false;
            }

            public int GetHashCode(IStrToken obj) => throw new NotImplementedException();
        }

        // ------------------------------------------------

        /// <summary>
        /// Returns a new instance based upon the contents of this builder.
        /// </summary>
        /// <returns></returns>
        public virtual StrTokenChain CreateInstance() => new(this);
    }
}