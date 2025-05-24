namespace Yotei.ORM.Internal;

partial class DbTokenChain
{
    // ====================================================
    /// <summary>
    /// Represents a builder for <see cref="DbTokenChain"/> instances.
    /// </summary>
    [Cloneable]
    [DebuggerDisplay("{ToDebugString(5)}")]
    public partial class Builder : CoreList<DbToken>
    {
        /// <summary>
        /// Initializes a new empty instance.
        /// </summary>
        public Builder() { }

        /// <summary>
        /// Initializes a new empty instance with the given capacity.
        /// </summary>
        /// <param name="capacity"></param>
        public Builder(int capacity) : base(capacity) { }

        /// <summary>
        /// Initializes a new instance with the given token.
        /// </summary>
        /// <param name="token"></param>
        public Builder(DbToken token) => Add(token);

        /// <summary>
        /// Initializes a new instance with the tokens of the given range.
        /// </summary>
        /// <param name="range"></param>
        public Builder(IEnumerable<DbToken> range) => AddRange(range);

        /// <summary>
        /// Copy constructor.
        /// </summary>
        /// <param name="source"></param>
        protected Builder(Builder source) => AddRange(source);

        // ------------------------------------------------

        /// <inheritdoc/>
        public override DbToken ValidateItem(DbToken item) => item.ThrowWhenNull();

        /// <inheritdoc/>
        public override IEqualityComparer<DbToken> Comparer => _Comparer ??= new MyComparer();
        MyComparer? _Comparer = null;

        readonly struct MyComparer : IEqualityComparer<DbToken>
        {
            public bool Equals(DbToken? x, DbToken? y)
            {
                if (x is null && y is null) return true;
                if (x is null || y is null) return false;

                return x.Equals(y);
            }

            public int GetHashCode([DisallowNull] DbToken obj) => throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public override bool ExpandItems { get; } = true;

        /// <inheritdoc/>
        public override bool IncludeDuplicated(DbToken item, DbToken source) => true;

        /// <summary>
        /// Returns a new host instance based upon the contents of this builder.
        /// </summary>
        /// <returns></returns>
        public DbTokenChain ToInstance() => new(this);

        /// <inheritdoc/>
        protected override bool SameItem(DbToken item, DbToken source)
        {
            return
                item is null && source is null ||
                item is not null && item.Equals(source);
        }
    }
}