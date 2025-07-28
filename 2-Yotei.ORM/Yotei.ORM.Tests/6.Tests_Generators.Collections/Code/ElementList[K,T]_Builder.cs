using THost = Yotei.ORM.Tests.Tools.Generators.Collections.ElementList_KT;
using IHost = Yotei.ORM.Tests.Tools.Generators.Collections.IElementList_KT;
using IItem = Yotei.ORM.Tests.Tools.Generators.Collections.IElement;
using TKey = string;

namespace Yotei.ORM.Tests.Tools.Generators.Collections;

partial class ElementList_KT
{
    // ====================================================
    /// <inheritdoc cref="IHost.IBuilder"/>
    [Cloneable(ReturnInterface = true)]
    [DebuggerDisplay("{ToDebugString(5)}")]
    public partial class Builder : CoreList<TKey, IItem>, IHost.IBuilder
    {
        public Builder(bool sensitive) => CaseSensitive = sensitive;
        public Builder(bool sensitive, int capacity) : this(sensitive) => Capacity = capacity;
        public Builder(bool sensitive, IEnumerable<IElement> range) : this(sensitive) => AddRange(range);
        protected Builder(Builder source) : this(source.CaseSensitive) => AddRange(source);

        // ------------------------------------------------

        public override IItem ValidateItem(IItem item) => item.ThrowWhenNull();
        public override string GetKey(IItem item) => item is NamedElement named
            ? named.Name
            : throw new ArgumentException("Element is not a named one.").WithData(item);
        public override TKey ValidateKey(TKey key) => key.NotNullNotEmpty();
        public override bool ExpandItems => true;
        public override bool IsValidDuplicate(IItem source, IItem item)
            => ReferenceEquals(source, item)
            ? true
            : throw new DuplicateException("Duplicated element.").WithData(item);
        public override IEqualityComparer<TKey> Comparer => _Comparer ??= new ComparerType(this);
        ComparerType? _Comparer = null;
        readonly struct ComparerType(Builder Master) : IEqualityComparer<TKey>
        {
            public bool Equals(TKey? x, TKey? y)
                => string.Compare(x, y, !Master.CaseSensitive) == 0;
            public int GetHashCode(TKey obj) => throw new NotImplementedException();
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