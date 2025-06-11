namespace Yotei.ORM.Internals;

partial class DbTokenChain
{
    // ====================================================
    /// <summary>
    /// Represents a builder for <see cref="DbTokenChain"/> intances.
    /// </summary>
    [DebuggerDisplay("{ToDebugString(5)}")]
    [Cloneable]
    public partial class Builder : CoreList<DbToken>
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
        public Builder(IEnumerable<DbToken> range) : this() => AddRange(range);

        /// <summary>
        /// Copy constructor.
        /// </summary>
        /// <param name="source"></param>
        protected Builder(Builder source) : this() => AddRange(source);

        // ------------------------------------------------

        /// <inheritdoc/>
        public override DbToken ValidateItem(DbToken item) => item.ThrowWhenNull();

        /// <inheritdoc/>
        public override bool ExpandItems => true;

        /// <inheritdoc/>
        public override bool IsValidDuplicate(DbToken source, DbToken item) => true;

        /// <inheritdoc/>
        public override IEqualityComparer<DbToken> Comparer => _Comparer ??= new ItemComparer();
        IEqualityComparer<DbToken>? _Comparer = null;

        readonly struct ItemComparer() : IEqualityComparer<DbToken>
        {
            public bool Equals(DbToken? x, DbToken? y)
            {
                if (x is null && y is null) return true;
                if (x is null || y is null) return false;

                return x.Equals(y);
            }
            public int GetHashCode([DisallowNull] DbToken obj) => throw new NotImplementedException();
        }

        // ------------------------------------------------

        /// <inheritdoc cref="IHost.IBuilder.CreateInstance"/>
        public virtual DbTokenChain CreateInstance() => new(this);
    }
}