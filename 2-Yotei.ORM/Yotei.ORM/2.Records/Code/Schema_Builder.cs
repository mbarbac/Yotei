﻿using IHost = Yotei.ORM.ISchema;
using IItem = Yotei.ORM.ISchemaEntry;
using TKey = Yotei.ORM.IIdentifier;

namespace Yotei.ORM.Code;

partial class Schema
{
    // ====================================================
    /// <inheritdoc cref="IHost.IBuilder"/>
    [DebuggerDisplay("{ToDebugString(5)}")]
    [Cloneable]
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
        public virtual Schema CreateInstance() => new(Engine, this);
        IHost IHost.IBuilder.CreateInstance() => CreateInstance();

        /// <inheritdoc/>
        public IEngine Engine { get; }

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
            => throw new DuplicateException("Duplicated element.").WithData(item);

        /// <inheritdoc/>
        public override IEqualityComparer<TKey> Comparer => _Comparer ??= new ItemComparer(Engine);
        IEqualityComparer<TKey>? _Comparer = null;

        readonly struct ItemComparer(IEngine Engine) : IEqualityComparer<TKey>
        {
            public bool Equals(TKey? x, TKey? y)
            {
                if (x is null && y is null) return true;
                if (x is null || y is null) return false;

                return string.Compare(x.Value, y.Value, !Engine.CaseSensitiveNames) == 0;
            }
            public int GetHashCode([DisallowNull] TKey obj) => throw new NotImplementedException();
        }

        /// <inheritdoc/>
        protected override bool SameItem(IItem item, IItem source) => ReferenceEquals(item, source);

        /// <inheritdoc/>
        protected override List<int> FindDuplicates(TKey key)
        {
            var nums = IndexesOf(x => x.Identifier.Match(key.Value));
            var temp = IndexesOf(x => key.Match(x.Identifier.Value));

            foreach (var num in temp) if (!nums.Contains(num)) nums.Add(num);
            return nums;
        }

        // ------------------------------------------------

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
            List<int> nums = [];

            for (int i = 0; i < Count; i++)
            {
                var item = this[i];
                if (item.Identifier.Match(specs)) nums.Add(i);
            }

            unique = nums.Count == 1 ? this[nums[0]] : null;
            return nums;
        }

        // ------------------------------------------------

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

        /// <inheritdoc/>
        public virtual bool RemoveLast(string identifier)
        {
            var index = LastIndexOf(identifier);
            if (index >= 0)
            {
                RemoveAt(index);
                return true;
            }
            return false;
        }

        /// <inheritdoc/>
        public virtual bool RemoveAll(string identifier)
        {
            var done = false;

            while (true)
            {
                var index = IndexOf(identifier);

                if (index >= 0)
                {
                    RemoveAt(index);
                    done = true;
                }
                else break;
            }
            return done;
        }
    }
}
