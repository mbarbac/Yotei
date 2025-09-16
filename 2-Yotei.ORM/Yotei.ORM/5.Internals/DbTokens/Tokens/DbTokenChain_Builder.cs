namespace Yotei.ORM.Internals;

partial class DbTokenChain
{
    // ====================================================
    /// <summary>
    /// Represents a builder of <see cref="THost"/> instances.
    /// </summary>
    [DebuggerDisplay("{ToDebugString(5)}")]
    public class Builder : CoreList<IDbToken>
    {
        /// <summary>
        /// Initializes a new empty instance.
        /// </summary>
        public Builder() : base() { }

        /// <summary>
        /// Initializes a new empty instance with the given initial capacity.
        /// </summary>
        /// <param name="capacity"></param>
        public Builder(int capacity) : this() => Capacity = capacity;

        /// <summary>
        /// Initializes a new instance with the elements from the given range.
        /// </summary>
        /// <param name="range"></param>
        public Builder(IEnumerable<IDbToken> range) : this() => AddRange(range);

        /// <summary>
        /// Copy constructor.
        /// </summary>
        /// <param name="source"></param>
        protected Builder(Builder source) : this()
            => AddRange(source.Select(x => x.Clone())); // Manages mutable elements...

        /// <inheritdoc/>
        public override Builder Clone() => new(this);

        /// <inheritdoc/>
        public override string ToString() => ToString(rounded: false);

        /// <summary>
        /// Returns a string representation of this instance using the rounded or square wrapping
        /// brackets requested, and with the given separator among elements.
        /// </summary>
        /// <param name="rounded"></param>
        /// <param name="separator"></param>
        /// <returns></returns>
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
        public override bool IsValidDuplicate(IDbToken source, IDbToken item) => true;

        /// <inheritdoc/>
        public override IEqualityComparer<IDbToken> Comparer => _Comparer ??= new();
        MyComparer? _Comparer;
        readonly struct MyComparer() : IEqualityComparer<IDbToken>
        {
            public bool Equals(IDbToken? x, IDbToken? y)
            {
                if (x is null && y is null) return true;
                if (x is null || y is null) return false;
                return x.Equals(y);
            }

            public int GetHashCode(IDbToken obj) => throw new NotImplementedException();
        }

        /// <inheritdoc/>
        protected override bool SameItem(IDbToken source, IDbToken item) => source.EqualsEx(item);

        /// <inheritdoc/>
        /// As 'IsValidDuplicates' always returns 'true', we don't need to compute this.
        protected override List<int> FindDuplicates(IDbToken item) => [];

        // ------------------------------------------------

        /// <summary>
        /// Returns a new instance based upon the contents of this builder.
        /// </summary>
        /// <returns></returns>
        public virtual DbTokenChain CreateInstance() => new(this);
    }
}