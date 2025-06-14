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
        public virtual IdentifierTags CreateInstance() => new(CaseSensitiveTags, this);
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

        /// <inheritdoc/>
        protected override bool SameItem(IItem source, IItem item) => source.Equals(item);

        /// <inheritdoc/>
        protected override List<int> FindDuplicates(IItem item)
        {
            var list = new List<int>();

            foreach (var name in item)
            {
                var index = IndexOf(name);
                if (index >= 0 && !list.Contains(index)) list.Add(index);
            }

            return list;
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
        public bool Contains(string name) => IndexOf(name) >= 0;

        /// <inheritdoc/>
        public bool Contains(IEnumerable<string> range) => IndexOf(range) >= 0;

        /// <inheritdoc/>
        public int IndexOf(string name)
        {
            name = name.NotNullNotEmpty();
            return IndexOf(x => x.Contains(name));
        }

        /// <inheritdoc/>
        public int IndexOf(IEnumerable<string> range)
        {
            range.ThrowWhenNull();

            return IndexOf(x =>
            {
                foreach (var name in range) if (x.Contains(name)) return true;
                return false;
            });
        }

        /// <inheritdoc/>
        public int LastIndexOf(IEnumerable<string> range)
        {
            range.ThrowWhenNull();

            return LastIndexOf(x =>
            {
                foreach (var name in range) if (x.Contains(name)) return true;
                return false;
            });
        }

        /// <inheritdoc/>
        public List<int> IndexesOf(IEnumerable<string> range)
        {
            range.ThrowWhenNull();

            return IndexesOf(x =>
            {
                foreach (var name in range) if (x.Contains(name)) return true;
                return false;
            });
        }

        // ------------------------------------------------

        /// <inheritdoc/>
        public virtual int Remove(string name)
        {
            var index = IndexOf(name);
            return index >= 0 ? RemoveAt(index) : 0;
        }

        /// <inheritdoc/>
        public virtual int Remove(IEnumerable<string> range)
        {
            range.ThrowWhenNull();

            var num = 0; while (true)
            {
                var index = IndexOf(range);

                if (index >= 0) { RemoveAt(index); num++; }
                else break;
            }
            return num;
        }
    }
}
