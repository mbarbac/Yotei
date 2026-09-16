/*using TKey = string;
using IItem = Yotei.ORM.InvariantGenerator.Tests.IElement;
using IHost = Yotei.ORM.InvariantGenerator.Tests.IElementList_KT;
using THost = Yotei.ORM.InvariantGenerator.Tests.ElementList_KT;
using Xunit.v3;

namespace Yotei.ORM.InvariantGenerator.Tests;

partial class ElementList_KT : IHost
{
    // ====================================================
    /// <summary>
    /// <inheritdoc cref="IHost.IBuilder"/>
    /// </summary>
    [Cloneable]
    public partial class Builder : CoreList<TKey, IItem>, IHost.IBuilder
    {
        /// <summary>
        /// Initializes a new instance.
        /// </summary>
        /// <param name="ignoreCase"></param>
        public Builder(bool ignoreCase) => IgnoreCase = ignoreCase;

        /// <summary>
        /// Initializes a new instance with the elements of the given range.
        /// </summary>
        /// <param name="ignoreCase"></param>
        /// <param name="range"></param>
        public Builder(
            bool ignoreCase, IEnumerable<IItem> range) : this(ignoreCase) => AddRange(range);

        /// <summary>
        /// Copy constructor.
        /// </summary>
        /// <param name="other"></param>
        protected Builder(Builder other) : base(other) { }

        protected override void OnCreating(CoreList<string, IItem> other)
        {
            base.OnCreating(other);
            IgnoreCase = ((Builder)other).IgnoreCase;
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <returns></returns>
        public virtual IHost ToInstance()
        {
            var host = new THost(IgnoreCase);
            if (Count > 0) host.AddRange(this);
            return host;
        }

        // ------------------------------------------------

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public override IItem ValidateElement(IItem value) => value.ThrowWhenNull();

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public override TKey GetKey(IItem value) => value is NamedElement named
            ? named.Name
            : throw new ArgumentException("Element is not a named one.").WithData(value);

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public override TKey ValidateKey(TKey key) => key.NotNullNotEmpty(trim: true);

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public override bool FlattenElements
        {
            get => base.FlattenElements;
            set => base.FlattenElements = value;
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="source"></param>
        /// <param name="target"></param>
        /// <returns></returns>
        public override bool CompareKeys(TKey source, TKey target)
        {
            var comparer = new MyComparer(IgnoreCase);
            return comparer.Equals(source, target);
        }

        readonly struct MyComparer(bool IgnoreCase) : IEqualityComparer<TKey>
        {
            public bool Equals(TKey? x, TKey? y) => string.Compare(x, y, IgnoreCase) == 0;
            public int GetHashCode(TKey _) => throw new UnExpectedException();
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public override IEnumerable<IItem> FindDuplicates(TKey key) => base.FindDuplicates(key);

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="value"></param>
        /// <param name="range"></param>
        /// <returns></returns>
        public override bool AllowDuplicate(IItem value, IEnumerable<IItem> _)
        {
            if (AcceptDuplicates) return true;
            throw new DuplicateException("Duplicated value.").WithData(value);
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="source"></param>
        /// <param name="target"></param>
        /// <returns></returns>
        public override bool SameElement(            
            IItem source, IItem target) => base.SameElement(source, target);

        // ------------------------------------------------

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public bool IgnoreCase
        {
            get;
            set
            {
                if (field == value) return;
                if (Count == 0) { field = value; return; }

                var range = ToList(); Clear();
                field = value; AddRange(range);
            }
        }

        /// <summary>
        /// For DEBUG purposes only.
        /// </summary>
        public bool AcceptDuplicates
        {
            get;
            set
            {
                if (field == value) return;
                if (Count == 0) { field = value; return; }

                var range = ToList(); Clear();
                field = value; AddRange(range);
            }
        }
    }
}*/