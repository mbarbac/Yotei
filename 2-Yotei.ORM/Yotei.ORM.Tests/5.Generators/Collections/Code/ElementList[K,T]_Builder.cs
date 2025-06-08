using IHost = Yotei.ORM.Tests.Generators.IElementList_KT;
using THost = Yotei.ORM.Tests.Generators.ElementList_KT;
using IItem = Yotei.ORM.Tests.Generators.IElement;
using TKey = string;

namespace Yotei.ORM.Tests.Generators;

partial class ElementList_KT
{
    // ====================================================
    /// <summary>
    /// Represents a builder for <see cref="IHost"/> intances.
    /// </summary>
    [Cloneable]
    public partial class Builder : CoreList<TKey, IItem>, IHost.IBuilder
    {
        /// <summary>
        /// Initializes a new empty instance with a <see cref="StringComparison.OrdinalIgnoreCase"/>
        /// comparison.
        /// </summary>
        public Builder() : this(StringComparison.OrdinalIgnoreCase) { }

        /// <summary>
        /// Initializes a new empty instance with the given initial capacity, and a default
        /// <see cref="StringComparison.OrdinalIgnoreCase"/> comparison.
        /// </summary>
        /// <param name="capacity"></param>
        public Builder(int capacity) : this() => Capacity = capacity;

        /// <summary>
        /// Initializes a new instance with the elements of the given range, and a default
        /// <see cref="StringComparison.OrdinalIgnoreCase"/> comparison.
        /// </summary>
        /// <param name="range"></param>
        public Builder(IEnumerable<IItem> range) : this() => AddRange(range);

        // ------------------------------------------------

        /// <summary>
        /// Initializes a new empty instance.
        /// </summary>
        /// <param name="comparison"></param>
        public Builder(StringComparison comparison) => Comparison = comparison;

        /// <summary>
        /// Initializes a new empty instance with the given initial capacity.
        /// </summary>
        /// <param name="comparison"></param>
        /// <param name="capacity"></param>
        public Builder(
            StringComparison comparison, int capacity) : this(comparison) => Capacity = capacity;

        /// <summary>
        /// Initializes a new instance with the elements of the given range.
        /// </summary>
        /// <param name="comparison"></param>
        /// <param name="range"></param>
        public Builder(
            StringComparison comparison, IEnumerable<IItem> range) : this(comparison) => AddRange(range);
        
        /// <summary>
        /// Copy constructor.
        /// </summary>
        /// <param name="source"></param>
        protected Builder(Builder source) : this(source.Comparison) => AddRange(source);

        // ------------------------------------------------

        /// <inheritdoc/>
        public override IItem ValidateItem(IItem item) => item.ThrowWhenNull();

        /// <inheritdoc/>
        public override TKey GetKey(IItem item) => item.ThrowWhenNull().Name;

        /// <inheritdoc/>
        public override TKey ValidateKey(TKey key) => key.NotNullNotEmpty();

        /// <inheritdoc/>
        public override bool ExpandItems => true;

        /// <inheritdoc/>
        public override bool IsValidDuplicate(IItem source, IItem item)
            => ReferenceEquals(source, item)
            ? true
            : throw new DuplicateException("Duplicated element.").WithData(item);

        /// <inheritdoc/>
        public override IEqualityComparer<TKey> Comparer => _Comparer ??= new ItemComparer(Comparison);
        IEqualityComparer<TKey>? _Comparer = null;

        readonly struct ItemComparer(StringComparison Comparison) : IEqualityComparer<TKey>
        {
            public bool Equals(TKey? x, TKey? y)
            {
                if (x is null && y is null) return true;
                if (x is null || y is null) return false;

                return string.Compare(x, y, Comparison) == 0;
            }
            public int GetHashCode([DisallowNull] TKey obj) => throw new NotImplementedException();
        }

        // ------------------------------------------------

        /// <inheritdoc cref="IHost.IBuilder.ToInstance"/>
        public virtual THost ToInstance() => new(Comparison, this);
        IHost IHost.IBuilder.ToInstance() => ToInstance();

        /// <inheritdoc/>
        public StringComparison Comparison { get; }
    }
}