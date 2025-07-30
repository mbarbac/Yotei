using THost = Yotei.ORM.Tests.Tools.Generators.Collections.ElementList_T;
using IHost = Yotei.ORM.Tests.Tools.Generators.Collections.IElementList_T;
using IItem = Yotei.ORM.Tests.Tools.Generators.Collections.IElement;

namespace Yotei.ORM.Tests.Tools.Generators.Collections;

partial class ElementList_T
{
    // ====================================================
    /// <inheritdoc cref="IHost.IBuilder"/>
    [Cloneable(ReturnInterface = true)]
    [DebuggerDisplay("{ToDebugString(5)}")]
    public partial class Builder : CoreList<IItem>, IHost.IBuilder
    {
        public Builder(bool sensitive) => CaseSensitive = sensitive;
        public Builder(bool sensitive, int capacity) : this(sensitive) => Capacity = capacity;
        public Builder(bool sensitive, IEnumerable<IElement> range) : this(sensitive) => AddRange(range);
        protected Builder(Builder source) : this(source.CaseSensitive) => AddRange(source);

        // ------------------------------------------------

        public override IItem ValidateItem(IItem item) => item.ThrowWhenNull();
        public override bool ExpandItems => true;
        public override bool IsValidDuplicate(IItem source, IItem item)
            => ReferenceEquals(source, item)
            ? true
            : throw new DuplicateException("Duplicated element.").WithData(item);
        public override IEqualityComparer<IItem> Comparer => _Comparer ??= new TComparer(this);
        TComparer? _Comparer = null;
        readonly struct TComparer(Builder Master) : IEqualityComparer<IItem>
        {
            public bool Equals(IItem? x, IItem? y)
            {
                return x is NamedElement xnamed && y is NamedElement ynamed
                    ? string.Compare(xnamed.Name, ynamed.Name, !Master.CaseSensitive) == 0
                    : x?.Equals(y) ?? false;
            }
            public int GetHashCode(IItem obj) => throw new NotImplementedException();
        }

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