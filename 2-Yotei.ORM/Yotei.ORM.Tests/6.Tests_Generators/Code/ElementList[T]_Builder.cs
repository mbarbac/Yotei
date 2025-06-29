using IHost = Yotei.ORM.Tests.Generators.IElementList_T;
using IItem = Yotei.ORM.Tests.Generators.IElement;

namespace Yotei.ORM.Tests.Generators;

partial class ElementList_T
{
    // ====================================================
    /// <inheritdoc cref="IHost.IBuilder"/>
    [DebuggerDisplay("{ToDebugString(5)}")]
    [Cloneable]
    public partial class Builder : CoreList<IItem>, IHost.IBuilder
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

        // ------------------------------------------------

        /// <inheritdoc/>
        public override IItem ValidateItem(IItem item)
        {
            item.ThrowWhenNull();

            if (item is NamedElement named) named.Name.NotNullNotEmpty();
            return item;
        }

        /// <inheritdoc/>
        public override bool ExpandItems => true;

        /// <inheritdoc/>
        public override bool IsValidDuplicate(IItem source, IItem item)
        {
            // Only named elements may qualify for acceptable duplication, but only if they
            // strictly are the same instance...
            if (source is NamedElement &&
                item is NamedElement &&
                ReferenceEquals(source, item)) return true;

            throw new DuplicateException("Duplicated element.").WithData(item).WithData(this);
        }

        /// <inheritdoc/>
        public override IEqualityComparer<IItem> Comparer => _Comparer ??= new ItemComparer(this);
        ItemComparer? _Comparer = null;

        readonly struct ItemComparer(Builder Master) : IEqualityComparer<IItem>
        {
            // Only named element may be considered equal...
            public bool Equals(IItem? x, IItem? y) => x is not null && x.Equals(y, Master.CaseSensitive);
            public int GetHashCode([DisallowNull] IItem obj) => throw new NotImplementedException();
        }

        // ------------------------------------------------

        /// <inheritdoc cref="IHost.IBuilder.CreateInstance"/>
        public virtual ElementList_T CreateInstance() => new(CaseSensitive, this);
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