//using THost = Yotei.ORM.Generators.Invariant.Tests.ElementList_KT;
using IHost = Yotei.ORM.Generators.Invariant.Tests.IElementList_KT;
using IItem = Yotei.ORM.Generators.Invariant.Tests.IElement;
using TKey = string;
/*
namespace Yotei.ORM.Generators.Invariant.Tests;

partial class ElementList_KT
{
    // ====================================================
    /// <summary>
    /// <inheritdoc cref="IHost.IBuilder"/>
    /// </summary>
    [Cloneable<IHost.IBuilder>]
    [DebuggerDisplay("{ToDebugString(5)}")]
    public partial class Builder : CoreList<TKey, IItem>, IHost.IBuilder
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

        // ------------------------------------------------

        /// <inheritdoc/>
        protected override IItem ValidateItem(IItem item) => item.ThrowWhenNull();

        /// <inheritdoc/>
        protected override TKey GetKey(IItem item) => item is NamedElement named
            ? named.Name
            : throw new ArgumentException("Element is not a named one.").WithData(item);

        /// <inheritdoc/>
        protected override TKey ValidateKey(TKey key) => key.NotNullNotEmpty(true);

        /// <inheritdoc/>
        protected override bool ExpandElements => true;

        /// <inheritdoc/>
        protected override bool IsValidDuplicate(IItem source, IItem item)
            => ReferenceEquals(source, item)
            ? true
            : throw new DuplicateException("Duplicated element.").WithData(item);

        /// <inheritdoc/>
        protected override IEqualityComparer<TKey> Comparer => _Comparer ??= new(Engine.CaseSensitive);
        MyComparer? _Comparer;

        readonly struct MyComparer(bool Sensitive) : IEqualityComparer<TKey>
        {
            public bool Equals(TKey? x, TKey? y)
                => string.Compare(x, y, !Sensitive) == 0;

            public int GetHashCode(TKey obj) => throw new NotImplementedException();
        }

        // ------------------------------------------------

        /// <inheritdoc/>
        protected override List<int> FindDuplicates(TKey key) => base.FindDuplicates(key);

        /// <inheritdoc/>
        protected override bool SameItem(IItem source, IItem target) => base.SameItem(source, target);

        // ------------------------------------------------

        // TODO: Enable CreateInstance method generation once supported.
        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <returns></returns>
        //public virtual IHost CreateInstance()
        //    => Count == 0 ? new THost(Engine) : new THost(Engine, this);

        /// <summary>
        /// The engine this instance is associated with.
        /// </summary>
        public IEngine Engine
        {
            get;
            set
            {
                if (ReferenceEquals(field, value.ThrowWhenNull())) return;
                var range = ToList();
                Clear();
                field = value; AddRange(range);
            }
        }
    }
}*/