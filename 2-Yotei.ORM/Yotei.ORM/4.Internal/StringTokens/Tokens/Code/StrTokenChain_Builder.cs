using IHost = Yotei.ORM.Internal.IStrTokenChain;
using THost = Yotei.ORM.Internal.Code.StrTokenChain;
using IItem = Yotei.ORM.Internal.IStrToken;

namespace Yotei.ORM.Internal.Code;

partial class StrTokenChain
{
    // ====================================================
    /// <summary>
    /// Represents a builder for <see cref="IHost"/> intances.
    /// </summary>
    [Cloneable]
    public partial class Builder : CoreList<IItem>, IHost.IBuilder
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

        // ------------------------------------------------

        /// <inheritdoc/>
        public override IItem ValidateItem(IItem item) => item.ThrowWhenNull();

        /// <inheritdoc/>
        public override bool ExpandItems => true;

        /// <inheritdoc/>
        public override bool IsValidDuplicate(IItem source, IItem item)
            => ReferenceEquals(source, item)
            ? true
            : throw new DuplicateException("Duplicated element.").WithData(item);

        /// <inheritdoc/>
        public override IEqualityComparer<IItem> Comparer { get; } = EqualityComparer<IStrToken>.Default;

        // ------------------------------------------------

        /// <inheritdoc cref="IHost.IBuilder.ToInstance"/>
        public virtual THost ToInstance() => new(this);
        IHost IHost.IBuilder.ToInstance() => ToInstance();
    }
}