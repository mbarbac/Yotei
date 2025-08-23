using THost = Yotei.ORM.Records.Code.Schema;
using IHost = Yotei.ORM.Records.ISchema;
using IItem = Yotei.ORM.Records.ISchemaEntry;
using TKey = Yotei.ORM.IIdentifier;

namespace Yotei.ORM.Records.Code;

partial class Schema
{
    // ====================================================
    /// <inheritdoc cref="IHost.IBuilder"/>
    [Cloneable]
    [DebuggerDisplay("{ToDebugString(5)}")]
    public partial class Builder : CoreList<TKey, IItem>, IHost.IBuilder
    {
        /// <summary>
        /// Initializes a new empty instance.
        /// </summary>
        /// <param name="engine"></param>
        public Builder(IEngine engine) => Engine = engine.ThrowWhenNull();

        /// <summary>
        /// Initializes a new empty instance with the given initial capacity.
        /// </summary>
        /// <param name="engine"></param>
        /// <param name="capacity"></param>
        public Builder(IEngine engine, int capacity) : this(engine) => Capacity = capacity;

        /// <summary>
        /// Initializes a new instance with the elements of the given range.
        /// </summary>
        /// <param name="engine"></param>
        /// <param name="range"></param>
        public Builder(IEngine engine, IEnumerable<IItem> range) : this(engine) => AddRange(range);

        /// <summary>
        /// Copy constructor.
        /// </summary>
        /// <param name="source"></param>
        protected Builder(Builder source) : this(source.Engine) => AddRange(source);

        /// <inheritdoc/>
        public override string ToString() => base.ToString();

        // ------------------------------------------------

        /// <inheritdoc/>
        public override IItem ValidateItem(IItem item)
        {
            item.ThrowWhenNull();

            if (!Engine.Equals(item.Engine)) throw new ArgumentException(
                "Element's engine is not the same as the one of this instance.")
                .WithData(item)
                .WithData(this);

            ValidateKey(item.Identifier); // Needed to prevent adding invalid ones...
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
        public override bool ExpandItems => false;

        /// <inheritdoc/>
        public override bool IsValidDuplicate(IItem source, IItem item)
            => ReferenceEquals(source, item)
            ? true
            : throw new DuplicateException("Duplicated element.").WithData(item);

        /// <inheritdoc/>
        public override IEqualityComparer<TKey> Comparer => _Comparer ??= new TComparer(this);
        TComparer? _Comparer = null;
        readonly struct TComparer(Builder Master) : IEqualityComparer<TKey>
        {
            public bool Equals(TKey? x, TKey? y)
            {
                if (x is null && y is null) return true;
                if (x is null || y is null) return false;

                return string.Compare(x.Value, y.Value, !Master.Engine.CaseSensitiveNames) == 0;
            }
            public int GetHashCode(TKey obj) => throw new NotImplementedException();
        }

        // ----------------------------------------------------

        /// <inheritdoc/>
        protected override bool SameItem(IItem source, IItem item)
        {
            return ReferenceEquals(source, item);
        }

        /// <inheritdoc/>
        protected override List<int> FindDuplicates(TKey key)
        {
            var nums = IndexesOf(x => x.Identifier.Match(key.Value));
            var temp = IndexesOf(x => key.Match(x.Identifier.Value));

            foreach (var num in temp) if (!nums.Contains(num)) nums.Add(num);
            return nums;
        }

        // ----------------------------------------------------

        /// <inheritdoc/>
        public virtual IHost CreateInstance() => new Schema(Engine, this);

        /// <inheritdoc/>
        public IEngine Engine { get; }

        // ----------------------------------------------------

        /// <inheritdoc/>
        public bool Contains(string identifier) => IndexOf(identifier) >= 0;

        /// <inheritdoc/>
        public int IndexOf(string identifier)
        {
            var key = Identifier.Create(Engine, identifier);
            return IndexOf(key);
        }

        /// <inheritdoc/>
        public int LastIndexOf(string identifier)
        {
            var key = Identifier.Create(Engine, identifier);
            return LastIndexOf(key);
        }

        /// <inheritdoc/>
        public List<int> IndexesOf(string identifier)
        {
            var key = Identifier.Create(Engine, identifier);
            return IndexesOf(key);
        }

        /// <inheritdoc/>
        public List<int> Match(string? specs) => Match(specs, out _);

        /// <inheritdoc/>
        public List<int> Match(string? specs, out IItem? unique)
        {
            List<int> list = [];

            for (int i = 0; i < Count; i++)
            {
                var item = this[i];
                var match = item.Identifier.Match(specs);
                if (match) list.Add(i);
            }

            unique = list.Count == 1 ? this[list[0]] : null;
            return list;
        }

        // ----------------------------------------------------

        /// <inheritdoc/>
        public virtual bool Remove(string identifier)
        {
            var index = IndexOf(identifier);
            if (index >= 0)
            {
                RemoveAt(index);
                return true;
            }
            return false;
        }
    }
}
