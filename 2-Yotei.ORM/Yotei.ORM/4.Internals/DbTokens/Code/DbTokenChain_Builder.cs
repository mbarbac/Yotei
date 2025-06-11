namespace Yotei.ORM.Internals;

partial class DbTokenChain
{
    // ====================================================
    /// <summary>
    /// Represents a builder for <see cref="DbTokenChain"/> intances.
    /// </summary>
    [DebuggerDisplay("{ToDebugString(5)}")]
    [Cloneable]
    public partial class Builder : CoreList<IDbToken>
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
        public Builder(IEnumerable<IDbToken> range) : this() => AddRange(range);

        /// <summary>
        /// Copy constructor.
        /// </summary>
        /// <param name="source"></param>
        protected Builder(Builder source) : this() => AddRange(source);

        /// <inheritdoc/>
        public override string ToString() => ToString(rounded: false);

        /// <inheritdoc cref="DbTokenChain.ToString(bool, string)"/>
        public string ToString(bool rounded, string separator = ", ")
        {
            separator.ThrowWhenNull();

            var head = rounded ? '(' : '[';
            var tail = rounded ? ')' : ']';

            var sb = new StringBuilder();
            sb.Append(head);

            for (int i = 0; i < Count; i++)
            {
                var item = this[i];
                var str = item.ToString();

                if (i > 0) sb.Append(separator);
                sb.Append(str);
            }

            sb.Append(tail);
            return sb.ToString();
        }

        // ------------------------------------------------

        /// <inheritdoc/>
        public override IDbToken ValidateItem(IDbToken item) => item.ThrowWhenNull();

        /// <inheritdoc/>
        public override bool ExpandItems => true;

        /// <inheritdoc/>
        public override bool IsValidDuplicate(IDbToken source, IDbToken item)
            => ReferenceEquals(source, item)
            ? true
            : throw new DuplicateException("Duplicated element.").WithData(item);

        /// <inheritdoc/>
        public override IEqualityComparer<IDbToken> Comparer => _Comparer ??= new ItemComparer();
        IEqualityComparer<IDbToken>? _Comparer = null;

        readonly struct ItemComparer() : IEqualityComparer<IDbToken>
        {
            public bool Equals(IDbToken? x, IDbToken? y)
            {
                if (x is null && y is null) return true;
                if (x is null || y is null) return false;

                return x.Equals(y);
            }
            public int GetHashCode([DisallowNull] IDbToken obj) => throw new NotImplementedException();
        }

        // ------------------------------------------------

        /// <summary>
        /// Returns a new instance based upon the contents of this builder.
        /// </summary>
        /// <returns></returns>
        public virtual DbTokenChain CreateInstance() => new(this);
    }
}