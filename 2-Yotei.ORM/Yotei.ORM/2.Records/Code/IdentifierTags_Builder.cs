using THost = Yotei.ORM.Records.Code.IdentifierTags;
using IHost = Yotei.ORM.Records.IIdentifierTags;
using IItem = Yotei.ORM.Records.IMetadataTag;

namespace Yotei.ORM.Records.Code;

partial class IdentifierTags
{
    // ====================================================
    /// <summary>
    /// <inheritdoc cref="IHost.IBuilder"/>
    /// </summary>
    [Cloneable<IHost.IBuilder>]
    [DebuggerDisplay("{ToDebugString(5)}")]
    public partial class Builder : CoreList<IItem>, IHost.IBuilder
    {
        /// <summary>
        /// Initializes a new empty instance.
        /// </summary>
        /// <param name="sensitive"></param>
        public Builder(bool sensitive) : base() => CaseSensitiveTags = sensitive;

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

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <returns></returns>
        public override string ToString() => Count == 0
            ? string.Empty
            : $"[{string.Join('.', this.Select(x => x.Default))}]";

        // ----------------------------------------------------

        /// <inheritdoc/>
        protected override IItem ValidateItem(IItem item)
        {
            item.ThrowWhenNull();

            if (CaseSensitiveTags != item.CaseSensitiveTags) throw new ArgumentException(
                "Case Sensitive of the given element is not the same as this instance's one.")
                .WithData(item)
                .WithData(this);

            return item;
        }

        /// <inheritdoc/>
        protected override bool ExpandElements => false;

        /// <inheritdoc/>
        protected override bool IsValidDuplicate(IItem source, IItem item)
            => throw new DuplicateException("Duplicated element.").WithData(item);

        /// <inheritdoc/>
        protected override IEqualityComparer<IItem> Comparer => _Comparer ??= new();
        MyComparer? _Comparer;

        readonly struct MyComparer : IEqualityComparer<IItem>
        {
            public bool Equals(IItem? x, IItem? y) => x.EqualsEx(y);
            public int GetHashCode(IItem obj) => throw new NotImplementedException();
        }

        // ------------------------------------------------

        /// <inheritdoc/>
        protected override List<int> FindDuplicates(IItem item)
        {
            List<int> list = []; foreach (var name in item)
            {
                var index = IndexOf(name);
                if (index >= 0 && !list.Contains(index)) list.Add(index);
            }
            list.Sort();
            return list;
        }

        // ----------------------------------------------------

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <returns></returns>
        public virtual IHost CreateInstance() => Count == 0
            ? new THost(CaseSensitiveTags)
            : new THost(CaseSensitiveTags, this);

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public bool CaseSensitiveTags { get; }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public IEnumerable<string> TagNames
        {
            get
            {
                foreach (var tag in this)
                    foreach (var name in tag) yield return name;
            }
        }

        // ----------------------------------------------------

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="tagname"></param>
        /// <returns></returns>
        public bool Contains(string tagname) => IndexOf(tagname) >= 0;

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="range"></param>
        /// <returns></returns>
        public bool Contains(IEnumerable<string> range) => IndexOf(range) >= 0;

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="tagname"></param>
        /// <returns></returns>
        public int IndexOf(string tagname)
        {
            tagname = tagname.NotNullNotEmpty(true);
            return IndexOf(x => x.Contains(tagname));
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="range"></param>
        /// <returns></returns>
        public int IndexOf(IEnumerable<string> range)
        {
            range.ThrowWhenNull();

            return IndexOf(x =>
            {
                foreach (var name in range) if (x.Contains(name)) return true;
                return false;
            });
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="tagname"></param>
        /// <returns></returns>
        public IItem? Find(string tagname)
        {
            var index = IndexOf(tagname);
            return index >= 0 ? this[index] : null;
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="range"></param>
        /// <returns></returns>
        public IItem? Find(IEnumerable<string> range)
        {
            var index = IndexOf(range);
            return index >= 0 ? this[index] : null;
        }

        // ----------------------------------------------------

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="tagname"></param>
        /// <returns></returns>
        public virtual bool Remove(string tagname)
        {
            var index = IndexOf(tagname);
            return index >= 0 && RemoveAt(index) > 0;
        }
    }
}