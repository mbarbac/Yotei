using IHost = Yotei.ORM.IIdentifierTags;
using IItem = Yotei.ORM.IMetadataTag;

namespace Yotei.ORM.Code;

partial class IdentifierTags
{
    // ====================================================
    /// <inheritdoc cref="IHost.IBuilder"/>
    [Cloneable]
    public partial class Builder : CoreList<IItem>, IHost.IBuilder
    {
        /// <summary>
        /// Initializes a new empty instance.
        /// </summary>
        /// <param name="sensitiveTags"></param>
        public Builder(bool sensitiveTags) => CaseSensitiveTags = sensitiveTags;

        /// <summary>
        /// Initializes a new empty instance with the given initial capacity.
        /// </summary>
        /// <param name="sensitiveTags"></param>
        /// <param name="capacity"></param>
        public Builder(
            bool sensitiveTags, int capacity) : this(sensitiveTags) => Capacity = capacity;

        /// <summary>
        /// Initializes a new instance with the elements of the given range.
        /// </summary>
        /// <param name="sensitiveTags"></param>
        /// <param name="range"></param>
        public Builder(
            bool sensitiveTags, IEnumerable<IItem> range) : this(sensitiveTags) => AddRange(range);

        /// <summary>
        /// Copy constructor.
        /// </summary>
        /// <param name="source"></param>
        protected Builder(Builder source) : this(source.CaseSensitiveTags) => AddRange(source);

        /// <inheritdoc/>
        public virtual IdentifierTags CreateInstance() => throw null;
        IHost IHost.IBuilder.CreateInstance() => CreateInstance();

        /// <inheritdoc/>
        public override string ToString() => throw null;

        // ------------------------------------------------

        /// <inheritdoc/>
        public override IItem ValidateItem(IItem item)
        {
            item.ThrowWhenNull();

            if (CaseSensitiveTags != item.CaseSensitiveTags) throw new ArgumentException(
                "Case Sensitive of the given element is not the same as this instance's one.")
                .WithData(item)
                .WithData(this);

            return item;
        }

        /// <inheritdoc/>
        public override bool ExpandItems => true;

        /// <inheritdoc/>
        public override bool IsValidDuplicate(IItem source, IItem item)
            => throw new DuplicateException("Duplicated element.").WithData(item);

        /// <inheritdoc/>
        public override IEqualityComparer<IItem> Comparer => _Comparer ??= new ItemComparer();
        IEqualityComparer<IItem>? _Comparer = null;

        readonly struct ItemComparer() : IEqualityComparer<IItem>
        {
            public bool Equals(IItem? x, IItem? y)
            {
                if (x is null && y is null) return true;
                if (x is null || y is null) return false;

                return x.Equals(y);
            }
            public int GetHashCode([DisallowNull] IItem obj) => throw new NotImplementedException();
        }

        // ------------------------------------------------

        /// <inheritdoc/>
        public bool CaseSensitiveTags { get; }

        /// <inheritdoc/>
        public IEnumerable<string> Names
        {
            get
            {
                foreach (var tag in this)
                    foreach (var name in tag) yield return name;
            }
        }

        // ------------------------------------------------

        /// <inheritdoc/>
        public bool Contains(string name) => throw null;

        /// <inheritdoc/>
        public bool Contains(IEnumerable<string> range) => throw null;

        /// <inheritdoc/>
        public int IndexOf(string name) => throw null;

        /// <inheritdoc/>
        public int IndexOf(IEnumerable<string> range) => throw null;

        /// <inheritdoc/>
        public int LastIndexOf(IEnumerable<string> range) => throw null;

        /// <inheritdoc/>
        public List<int> IndexesOf(IEnumerable<string> range) => throw null;

        // ------------------------------------------------

        /// <inheritdoc/>
        public virtual int Remove(string name) => throw null;

        /// <inheritdoc/>
        public virtual int Remove(IEnumerable<string> range) => throw null;
    }
}
