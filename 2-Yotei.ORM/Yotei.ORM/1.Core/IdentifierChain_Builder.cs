using THost = Yotei.ORM.IdentifierChain;
using TItem = Yotei.ORM.IdentifierUnit;
using TKey = string;

namespace Yotei.ORM;

partial record IdentifierChain
{
    // ====================================================
    /// <summary>
    /// Represents a builder for <see cref="THost"/> instances.
    /// </summary>
    [DebuggerDisplay("{ToDebugString(5)}")]
    public class Builder : CoreList<TKey?, TItem>
    {
        /// <summary>
        /// Initializes a new empty instance.
        /// </summary>
        /// <param name="engine"></param>
        public Builder(Engine engine) => Engine = engine.ThrowWhenNull();
        
        /// <summary>
        /// Initializes a new empty instance with the given initial capacity.
        /// </summary>
        /// <param name="engine"></param>
        /// <param name="capacity"></param>
        public Builder(Engine engine, int capacity) : this(engine) => Capacity = capacity;
        
        /// <summary>
        /// Initializes a new instance with the element of the given range.
        /// </summary>
        /// <param name="engine"></param>
        /// <param name="range"></param>
        public Builder(Engine engine, IEnumerable<TItem> range) : this(engine) => AddRange(range);

        /// <summary>
        /// Copy constructor.
        /// </summary>
        /// <param name="source"></param>
        protected Builder(Builder source) : this(source.Engine) => AddRange(source);
        
        /// <inheritdoc/>
        public override Builder Clone() => new(this);

        // public virtual Chain ToInstance() => new(Sensitive, this);

        // ------------------------------------------------

        /// <inheritdoc/>
        protected override TItem ValidateItem(TItem item)
        {
            item.ThrowWhenNull();

            if (!Engine.Equals(item.Engine)) throw new ArgumentException(
                "Element's engine is not the same as the one of this instance.")
                .WithData(item)
                .WithData(this);

            return item;
        }

        /// <inheritdoc/>
        protected override TKey? GetKey(TItem item) => item.ThrowWhenNull().Value;

        /// <inheritdoc/>
        protected override TKey? ValidateKey(TKey? key) => key is null
            ? null
            : new TItem(Engine, key).Value;

        /// <inheritdoc/>
        protected override bool ExpandElements => true;

        /// <inheritdoc/>
        protected override bool IsValidDuplicate(TItem source, TItem item) => true;

        /// <inheritdoc/>
        protected override IEqualityComparer<TKey?> Comparer => _Comparer ??= new(Engine.CaseSensitiveNames);
        MyComparer? _Comparer;
        readonly struct MyComparer(bool Sensitive) : IEqualityComparer<TKey?>
        {
            public bool Equals(TKey? x, TKey? y) => string.Compare(x, y, !Sensitive) == 0;
            public int GetHashCode(TKey? obj) => throw new NotImplementedException();
        }

        /// <inheritdoc/>
        protected override List<int> FindDuplicates(TKey? key) => [];

        // ------------------------------------------------

        /// <summary>
        /// The engine this instance is associated with.
        /// </summary>
        public Engine Engine { get; }
    }
}