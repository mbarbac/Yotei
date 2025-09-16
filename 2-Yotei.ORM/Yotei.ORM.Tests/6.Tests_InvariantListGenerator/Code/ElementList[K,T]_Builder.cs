using THost = Yotei.ORM.Tests.InvariantListGenerator.ElementListKT;
using IHost = Yotei.ORM.Tests.InvariantListGenerator.IElementListKT;
using IItem = Yotei.ORM.Tests.InvariantListGenerator.IElement;
using TKey = string;

namespace Yotei.ORM.Tests.InvariantListGenerator;

partial class ElementListKT
{
    // ====================================================
    /// <inheritdoc cref="IHost.IBuilder"/>
    [DebuggerDisplay("{ToDebugString(5)}")]
    public class Builder : CoreList<TKey, IItem>, IHost.IBuilder
    {
        /// <summary>
        /// Initializes a new empty instance.
        /// </summary>
        /// <param name="engine"></param>
        public Builder(IEngine engine) : base() => Engine = engine.ThrowWhenNull();

        /// <summary>
        /// Initializes a new empty instance with the given initial capacity.
        /// </summary>
        /// <param name="engine"></param>
        /// <param name="capacity"></param>
        public Builder(IEngine engine, int capacity) : this(engine) => Capacity = capacity;

        /// <summary>
        /// Initializes a new instance with the elements from the given range.
        /// </summary>
        /// <param name="engine"></param>
        /// <param name="range"></param>
        public Builder(
            IEngine engine, IEnumerable<IItem> range) : this(engine) => AddRange(range);

        /// <summary>
        /// Copy constructor.
        /// </summary>
        /// <param name="source"></param>
        protected Builder(Builder source) : this(source.Engine) => AddRange(source);

        /// <inheritdoc/>
        public override IHost.IBuilder Clone() => new Builder(this);

        // ------------------------------------------------

        /// <inheritdoc/>
        public override IItem ValidateItem(IItem item) => item.ThrowWhenNull();

        /// <inheritdoc/>
        public override TKey GetKey(IItem item) => item is NamedElement named
            ? named.Name
            : throw new ArgumentException("Element is not a named one.").WithData(item);

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
        public override IEqualityComparer<TKey> Comparer => _Comparer ??= new(Engine.CaseSensitiveNames);
        MyComparer? _Comparer;
        readonly struct MyComparer(bool Sensitive) : IEqualityComparer<TKey>
        {
            public bool Equals(TKey? x, TKey? y)
                => string.Compare(x, y, !Sensitive) == 0;

            public int GetHashCode(TKey obj) => throw new NotImplementedException();
        }

        // ------------------------------------------------

        /// <inheritdoc/>
        protected override bool SameItem(IItem source, IItem item) => base.SameItem(source, item);

        /// <inheritdoc/>
        protected override List<int> FindDuplicates(TKey key) => base.FindDuplicates(key);

        // ------------------------------------------------

        /// <inheritdoc/>
        public virtual IHost CreateInstance()
            => Count == 0 ? new THost(Engine) : new THost(Engine, this);

        /// <inheritdoc/>
        public IEngine Engine
        {
            get => _Engine;
            set
            {
                value.ThrowWhenNull();

                if (_Engine?.Equals(value) ?? false) return;
                _Engine = value;

                if (Count == 0) return;
                var range = ToList();
                Clear();
                AddRange(range);
            }
        }
        IEngine _Engine = default!;
    }
}