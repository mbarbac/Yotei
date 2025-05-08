using IHost = Yotei.ORM.Records.ISchema;
using THost = Yotei.ORM.Records.Code.Schema;
using IItem = Yotei.ORM.Records.ISchemaEntry;
using TKey = Yotei.ORM.IIdentifier;
using System.Runtime.InteropServices.Marshalling;

namespace Yotei.ORM.Records.Code;

partial class Schema
{
    // ====================================================
    /// <inheritdoc cref="IBuilder"/>
    [Cloneable]
    [DebuggerDisplay("{ToDebugString(5)}")]
    public partial class Builder : CoreList<TKey, IItem>, IHost.IBuilder
    {
        /// <summary>
        /// Initializes a new empty instance.
        /// </summary>
        /// <param name="engine"></param>
        public Builder(IEngine engine) => throw null;

        /// <summary>
        /// Initializes a new instance with the given element.
        /// </summary>
        /// <param name="engine"></param>
        /// <param name="item"></param>
        public Builder(IEngine engine, IItem item) => throw null;

        /// <summary>
        /// Initializes a new instance with the elements of the given range.
        /// </summary>
        /// <param name="engine"></param>
        /// <param name="item"></param>
        public Builder(IEngine engine, IEnumerable<IItem> item) => throw null;

        /// <summary>
        /// Copy constructor.
        /// </summary>
        /// <param name="source"></param>
        protected Builder(Builder source) => throw null;

        // ------------------------------------------------

        // <inheritdoc/>
        public override IItem ValidateItem(IItem item)
        {
            item.ThrowWhenNull();

            if (!Engine.Equals(item.Engine)) throw new ArgumentException(
                "Element's engine is not the same as the one of this instance.")
                .WithData(item)
                .WithData(this);

            return item;
        }

        /// <inheritdoc/>
        public override TKey GetKey(IItem item) => item.ThrowWhenNull().Identifier;

        /// <inheritdoc/>
        public override TKey ValidateKey(TKey key)
        {
            key.ThrowWhenNull();

            if (key.Value is null) throw new ArgumentException("Identifier value cannot be null.");
            if (key is IIdentifierChain chain)
            {
                if (chain.Count == 0) throw new ArgumentException("Identifier chain cannot be empty.");
                if (chain[^1].Value is null) throw new ArgumentException(
                    "Value of last part of identifier chain cannot be null.")
                    .WithData(key);
            }

            return key;
        }

        /// <inheritdoc/>
        public override IEqualityComparer<TKey> Comparer => _Comparer ??= new KeyComparer(Engine.CaseSensitiveNames);
        IEqualityComparer<TKey>? _Comparer = null;

        readonly struct KeyComparer : IEqualityComparer<TKey>
        {
            readonly bool CaseSensitive;
            public KeyComparer(bool sensitive) => CaseSensitive = sensitive;
            public bool Equals(TKey? x, TKey? y)
            {
                if (x is null && y is null) return true;
                if (x is null || y is null) return false;

                return string.Compare(x.Value, y.Value, !CaseSensitive) == 0;
            }
            public int GetHashCode([DisallowNull] TKey obj) => throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public override bool ExpandItems => false;

        /// <inheritdoc/>
        public override bool IncludeDuplicated(IItem item, IItem source) => SameItem(item, source)
            ? true
            : throw new DuplicateException("Duplicated element.");

        /// <inheritdoc/>
        protected override bool SameItem(IItem item, IItem source) => ReferenceEquals(item, source);

        /// <inheritdoc/>
        protected virtual List<int> FindDuplicates(TKey key)
        {
            throw null;
        }

        // ------------------------------------------------

        /// <inheritdoc/>
        public IEngine Engine { get; }

        /// <inheritdoc/>
        public THost ToInstance() => throw null;
        IHost IHost.IBuilder.ToInstance() => ToInstance();

        // ------------------------------------------------

        /// <inheritdoc/>
        public List<int> Match(string? specs) => throw null;

        /// <inheritdoc/>
        public List<int> Match(string? specs, out IItem? unique) => throw null;

        // ------------------------------------------------

        /// <inheritdoc/>
        public bool Contains(string identifier) => throw null;

        /// <inheritdoc/>
        public int IndexOf(string identifier) => throw null;

        /// <inheritdoc/>
        public int LastIndexOf(string identifier) => throw null;

        /// <inheritdoc/>
        public List<int> IndexesOf(string identifier) => throw null;

        // ------------------------------------------------

        /// <inheritdoc/>
        public bool Remove(string identifier) => throw null;

        /// <inheritdoc/>
        public bool RemoveLast(string identifier) => throw null;

        /// <inheritdoc/>
        public bool RemoveAll(string identifier) => throw null;
    }
}