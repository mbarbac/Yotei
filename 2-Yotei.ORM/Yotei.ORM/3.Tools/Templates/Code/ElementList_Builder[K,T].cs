#pragma warning disable CS8597

using IHost = Yotei.ORM.Tools.Code.Templates.IElementList_KT;
using THost = Yotei.ORM.Tools.Code.Templates.ElementList_KT;
using IItem = Yotei.ORM.Tools.Code.Templates.IElement;
using TKey = string;

namespace Yotei.ORM.Tools.Code.Templates;

public partial class ElementList_KT
{
    // ====================================================
    /// <inheritdoc cref="IHost.IBuilder"/>
    [Cloneable]
    [DebuggerDisplay("{ToDebugString(5)}")]
    public partial class Builder : CoreList<TKey, IItem>, IHost.IBuilder
    {
        /// <summary>
        /// Initializes a new empty instance.
        /// </summary>
        public Builder() { }

        /// <summary>
        /// Initializes a new empty instance with the given initial capacity.
        /// </summary>
        /// <param name="capacity"></param>
        public Builder(int capacity) : this() => Capacity = capacity;

        /// <summary>
        /// Initializes a new instance with the elements of the given range.
        /// </summary>
        /// <param name="range"></param>
        public Builder(IEnumerable<IItem> range) : this() => AddRange(range);

        /// <summary>
        /// Copy constructor.
        /// </summary>
        /// <param name="source"></param>
        protected Builder(Builder source) : this() => AddRange(source);

        // ------------------------------------------------

        /// <inheritdoc/>
        public THost ToInstance() => new(this);
        IHost IHost.IBuilder.ToInstance() => ToInstance();

        // ----------------------------------------------------

        /// <inheritdoc/>
        public override IItem ValidateItem(IItem item) => throw null;

        /// <inheritdoc/>
        public override TKey GetKey(IItem item) => throw null;

        /// <inheritdoc/>
        public override TKey ValidateKey(TKey key) => throw null;

        /// <inheritdoc/>
        public override IEqualityComparer<TKey> Comparer => throw null;

        /// <inheritdoc/>
        public override bool ExpandItems => throw null;

        /// <inheritdoc/>
        public override bool IncludeDuplicated(IItem item, IItem source) => throw null;

        /// <inheritdoc/>
        protected override bool SameItem(IItem item, IItem source) => throw null;
    }
}