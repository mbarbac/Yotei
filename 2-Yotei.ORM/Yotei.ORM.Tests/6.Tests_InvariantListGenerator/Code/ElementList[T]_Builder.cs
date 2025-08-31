using THost = Yotei.ORM.Tests.InvariantListGenerator.ElementListT;
using IHost = Yotei.ORM.Tests.InvariantListGenerator.IElementListT;
using IItem = Yotei.ORM.Tests.InvariantListGenerator.IElement;

namespace Yotei.ORM.Tests.InvariantListGenerator;

partial class ElementListT
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
        public Builder(bool sensitive) : base() => CaseSensitive = sensitive;

        /// <summary>
        /// Initializes a new instance with the elements from the given range.
        /// </summary>
        /// <param name="range"></param>
        public Builder(
            bool sensitive, IEnumerable<IItem> range) : this(sensitive) => AddRange(range);

        /// <summary>
        /// Copy constructor.
        /// </summary>
        /// <param name="source"></param>
        protected Builder(Builder source) : this(source.CaseSensitive) => AddRange(source);

        // ------------------------------------------------

        /// <inheritdoc/>
        public override IItem ValidateItem(IItem item) => item.ThrowWhenNull();

        /// <inheritdoc/>
        public override bool ExpandItems => true;

        /// <inheritdoc/>
        public override bool IsValidDuplicate(IItem source, IItem item)
            => ReferenceEquals(source, item)
            ? true
            : throw new DuplicateException("Duplicated element.").WithData(item);

        /// <inheritdoc/>
        public override IEqualityComparer<IItem> Comparer => _Comparer ??= new(CaseSensitive);
        MyComparer? _Comparer;
        readonly struct MyComparer(bool Sensitive) : IEqualityComparer<IItem>
        {
            public bool Equals(IItem? x, IItem? y)
            {
                return x is NamedElement xnamed && y is NamedElement ynamed
                    ? string.Compare(xnamed.Name, ynamed.Name, !Sensitive) == 0
                    : x?.Equals(y) ?? false;
            }

            public int GetHashCode(IItem obj) => throw new NotImplementedException();
        }

        // ------------------------------------------------

        /// <inheritdoc/>
        protected override bool SameItem(IItem source, IItem item) => base.SameItem(source, item);

        /// <inheritdoc/>
        protected override List<int> FindDuplicates(IItem item) => base.FindDuplicates(item);

        // ------------------------------------------------

        /// <inheritdoc/>
        public virtual IHost CreateInstance() => new THost(CaseSensitive, this);

        /// <inheritdoc/>
        public bool CaseSensitive
        {
            get => _CaseSensitive;
            set
            {
                if (value == _CaseSensitive) return;
                _CaseSensitive = value;

                if (Count == 0) return;
                var range = ToList();
                Clear();
                AddRange(range);
            }
        }
        bool _CaseSensitive;
    }
}