using System.Runtime.InteropServices.Marshalling;
using IHost = Yotei.ORM.Tests.Generators.IElementList_KT;
using IItem = Yotei.ORM.Tests.Generators.IElement;
using TKey = string;

namespace Yotei.ORM.Tests.Generators;

partial class ElementList_KT
{
    // ====================================================
    /// <inheritdoc cref="IHost.IBuilder"/>
    [DebuggerDisplay("{ToDebugString(5)}")]
    [Cloneable]
    public partial class Builder : CoreList<TKey, IItem>, IHost.IBuilder
    {
        /// <summary>
        /// Initializes a new instance.
        /// </summary>
        /// <param name="sensitive"></param>
        public Builder(bool sensitive) => CaseSensitive = sensitive;

        /// <summary>
        /// Initializes a new empty instance with the given initial capacity.
        /// </summary>
        /// <param name="sensitive"></param>
        /// <param name="capacity"></param>
        public Builder(bool sensitive, int capacity) : this(sensitive) => Capacity = capacity;

        /// <summary>
        /// Initializes a new instance with the elements of the given range.
        /// </summary>
        /// <param name="sensitive"></param>
        /// <param name="range"></param>
        public Builder(bool sensitive, IEnumerable<IItem> range) : this(sensitive) => AddRange(range);

        /// <summary>
        /// Copy constructor.
        /// </summary>
        /// <param name="source"></param>
        protected Builder(Builder source) : this(source.CaseSensitive) => AddRange(source);

        /// <inheritdoc/>
        public override string ToString() => base.ToString();

        // ----------------------------------------------------

        /// <inheritdoc/>
        public override IItem ValidateItem(IItem item)
        {
            item.ThrowWhenNull();
            if (item is NamedElement named) named.Name.NotNullNotEmpty();

            return item;
        }

        /// <inheritdoc/>
        public override TKey GetKey(IItem item) => item.Name;

        /// <inheritdoc/>
        public override TKey ValidateKey(TKey key) => key.NotNullNotEmpty();

        /// <inheritdoc/>
        public override bool ExpandItems => true; // Flat collection allowed...

        /// <inheritdoc/>
        public override bool IsValidDuplicate(IItem source, IItem item)
        {
            // Only named elements are acceptable, but only if they are the same instance...
            if (source is NamedElement &&
                item is NamedElement &&
                ReferenceEquals(source, item)) return true;

            throw new DuplicateException("Duplicated element.").WithData(item).WithData(this);
        }

        /// <inheritdoc/>
        public override IEqualityComparer<TKey> Comparer => _Comparer ??= new ItemComparer(this);
        ItemComparer? _Comparer = null;

        readonly struct ItemComparer(Builder Master) : IEqualityComparer<TKey>
        {
            public bool Equals(TKey? x, TKey? y)
                => string.Compare(x, y, !Master.CaseSensitive) == 0;

            public int GetHashCode([DisallowNull] TKey obj) => throw new NotImplementedException();
        }

        // ------------------------------------------------

        /// <inheritdoc cref="IHost.IBuilder.CreateInstance"/>
        public virtual ElementList_KT CreateInstance() => new(CaseSensitive, this);
        IHost IHost.IBuilder.CreateInstance() => CreateInstance();

        /// <inheritdoc/>
        public bool CaseSensitive
        {
            get => _CaseSensitive;
            set
            {
                if (value != _CaseSensitive)
                {
                    var range = ToList();
                    
                    Clear(); _CaseSensitive = value;
                    AddRange(range);
                }
            }
        }
        bool _CaseSensitive;

        /// <inheritdoc/>
        public string Name => Count == 0 ? string.Empty : string.Concat(this.Select(x => x.Name));
    }
}