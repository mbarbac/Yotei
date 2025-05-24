using IHost = Yotei.ORM.IIdentifierTags;
using THost = Yotei.ORM.Code.IdentifierTags;
using IItem = Yotei.ORM.IMetadataTag;
using TItem = Yotei.ORM.Code.MetadataTag;

namespace Yotei.ORM.Code;

partial class IdentifierTags
{
    // ====================================================
    /// <summary>
    /// Represents a builder for <see cref="IHost"/> instances.
    /// </summary>
    [Cloneable]
    [DebuggerDisplay("{ToDebugString(5)}")]
    public sealed partial class Builder : CoreList<IItem>, IHost.IBuilder
    {
        /// <summary>
        /// Initializes a new empty instance.
        /// </summary>
        /// <param name="sensitive"></param>
        public Builder(bool sensitive) => CaseSensitiveTags = sensitive;

        /// <summary>
        /// Initializes a new empty instance with the given initial capacity.
        /// </summary>
        /// <param name="sensitive"></param>
        /// <param name="capacity"></param>
        public Builder(bool sensitive, int capacity) : this(sensitive) => Capacity = capacity;

        /// <summary>
        /// Initializes a new instance with the given element.
        /// </summary>
        /// <param name="engine"></param>
        /// <param name="item"></param>
        public Builder(bool sensitive, IItem item) : this(sensitive) => Add(item);

        /// <summary>
        /// Initializes a new instance with the tags in the given range.
        /// </summary>
        /// <param name="sensitive"></param>
        /// <param name="range"></param>
        public Builder(bool sensitive, IEnumerable<IItem> range) : this(sensitive) => AddRange(range);

        /// <summary>
        /// Copy constructor.
        /// </summary>
        /// <param name="source"></param>
        Builder(Builder source) : this(source.CaseSensitiveTags) => AddRange(source);

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
        public override IEqualityComparer<IItem> Comparer => _Comparer ??= new ItemComparer();
        IEqualityComparer<IItem>? _Comparer = null;

        readonly struct ItemComparer : IEqualityComparer<IItem>
        {
            public bool Equals(IItem? x, IItem? y) => x is not null && x.Equals(y);
            public int GetHashCode([DisallowNull] IItem obj) => throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public override bool ExpandItems => false;

        /// <inheritdoc/>
        public override bool IncludeDuplicated(
            IItem item, IItem source) => throw new DuplicateException(
                "Duplicated element.")
                .WithData(item);

        // ------------------------------------------------

        /// <inheritdoc/>
        public THost ToInstance() => new(CaseSensitiveTags, this);
        IHost IHost.IBuilder.ToInstance() => ToInstance();

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
        protected override bool SameItem(IItem item, IItem source) => item.Equals(source);

        /// <inheritdoc/>
        protected override List<int> FindDuplicates(IItem item)
        {
            var list = new List<int>();

            foreach (var name in item)
            {
                var index = IndexOf(name);
                if (index >= 0) list.Add(index);
            }

            return list;
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

        /// <inheritdoc/>
        public int Remove(string name)
        {
            var index = IndexOf(name);
            return index >= 0 ? RemoveAt(index) : 0;
        }

        /// <inheritdoc/>
        public int Remove(IEnumerable<string> range)
        {
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