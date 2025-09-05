using THost = Yotei.ORM.Records.Code.IdentifierTags;
using IHost = Yotei.ORM.Records.IIdentifierTags;
using IItem = Yotei.ORM.Records.IMetadataTag;

namespace Yotei.ORM.Records.Code;

partial class IdentifierTags
{
    // ====================================================
    /// <inheritdoc cref="IHost.IBuilder"/>
    [Cloneable<IHost.IBuilder>]
    [DebuggerDisplay("{ToDebugString(5)}")]
    public partial class Builder : CoreList<IItem>, IHost.IBuilder
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
        /// Initializes a new instance with the elements from the given range.
        /// </summary>
        /// <param name="sensitive"></param>
        /// <param name="range"></param>
        public Builder(
            bool sensitive, IEnumerable<IItem> range) : this(sensitive) => AddRange(range);

        /// <summary>
        /// Copy constructor.
        /// </summary>
        /// <param name="source"></param>
        protected Builder(Builder source) : this(source.CaseSensitiveTags) => AddRange(source);

        /// <inheritdoc/>
        public override string ToString() => Count == 0
            ? string.Empty
            : $"[{string.Join('.', this.Select(x => x.Default))}]";

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
        public override IEqualityComparer<IItem> Comparer => _Comparer ??= new();
        MyComparer? _Comparer;
        readonly struct MyComparer() : IEqualityComparer<IItem>
        {
            public bool Equals(IItem? x, IItem? y)
            {
                if (x is null && y is null) return true;
                if (x is null || y is null) return false;

                return x.Equals(y);
            }

            public int GetHashCode(IItem obj) => throw new NotImplementedException();
        }

        /// <inheritdoc/>
        protected override List<int> FindDuplicates(IItem item)
        {
            List<int> list = [];

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
        public IEnumerable<string> TagNames
        {
            get
            {
                foreach (var tag in this)
                    foreach (var name in tag) yield return name;
            }
        }

        /// <inheritdoc/>
        public virtual IHost CreateInstance()
            => Count == 0 ? new THost(CaseSensitiveTags) : new THost(CaseSensitiveTags, this);

        // ----------------------------------------------------

        /// <inheritdoc/>
        public bool Contains(string name) => throw null;

        /// <inheritdoc/>
        public bool Contains(IEnumerable<string> range) => throw null;

        /// <inheritdoc/>
        public int IndexOf(string name) => throw null;

        /// <inheritdoc/>
        public int IndexOf(IEnumerable<string> range) => throw null;

        /// <inheritdoc/>
        public List<int> IndexesOf(IEnumerable<string> range) => throw null;

        // ------------------------------------------------

        /// <inheritdoc/>
        public virtual int Remove(string name) => throw null;

        /// <inheritdoc/>
        public virtual int Remove(IEnumerable<string> range) => throw null;
    }
}