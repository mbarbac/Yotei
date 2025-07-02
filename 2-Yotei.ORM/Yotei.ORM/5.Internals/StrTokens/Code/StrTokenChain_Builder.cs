using THost = Yotei.ORM.Internals.StrTokenChain;
using IItem = Yotei.ORM.Internals.IStrToken;

namespace Yotei.ORM.Internals;

partial class StrTokenChain
{
    // ====================================================
    /// <summary>
    /// Represents a builder for <see cref="THost"/> instances.
    /// </summary>
    [DebuggerDisplay("{ToDebugString(5)}")]
    [Cloneable]
    public partial class Builder : CoreList<IItem>
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
        public Builder(IEnumerable<IItem> range) : this() => AddRange(range);

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
        public override IItem ValidateItem(IItem item) => item.ThrowWhenNull();

        /// <inheritdoc/>
        public override bool ExpandItems => true;

        /// <inheritdoc/>
        public override bool IsValidDuplicate(IItem source, IItem item) => true;

        /// <inheritdoc/>
        public override IEqualityComparer<IItem> Comparer => _Comparer ??= new ItemComparer();
        IEqualityComparer<IItem>? _Comparer = null;

        readonly struct ItemComparer() : IEqualityComparer<IItem>
        {
            public bool Equals(IItem? x, IItem? y)
            {
                if (x is null && y is null) return true;
                if (x is null || y is null) return false;

                return x.Equals(y);
            }
            public int GetHashCode([DisallowNull] IItem obj) => throw new NotImplementedException();
        }

        // ------------------------------------------------

        /// <summary>
        /// Returns a new instance using the contents captured by this collection.
        /// </summary>
        /// <returns></returns>
        public virtual THost CreateInstance() => new(this);
    }
}
