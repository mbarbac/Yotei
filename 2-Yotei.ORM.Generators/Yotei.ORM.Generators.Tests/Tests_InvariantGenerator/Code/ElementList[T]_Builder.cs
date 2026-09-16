using IItem = Yotei.ORM.InvariantGenerator.Tests.IElement;
using IHost = Yotei.ORM.InvariantGenerator.Tests.IElementList_T;
using THost = Yotei.ORM.InvariantGenerator.Tests.ElementList_T;

namespace Yotei.ORM.InvariantGenerator.Tests;

partial class ElementList_T : IHost
{
    // ====================================================
    /// <summary>
    /// <inheritdoc cref="IHost.IBuilder"/>
    /// </summary>
    [Cloneable]
    public partial class Builder : CoreList<IItem>, IHost.IBuilder
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
        public Builder(bool ignoreCase, IEnumerable<IItem> range)
            : this(ignoreCase)
            => AddRange(range.ThrowWhenNull());

        /// <summary>
        /// Copy constructor.
        /// </summary>
        /// <param name="other"></param>
        protected Builder(Builder other) : base(other) { }

        protected override void OnCreating(CoreList<IItem> other)
        {
            base.OnCreating(other);
            IgnoreCase = ((Builder)other).IgnoreCase;
            AcceptDuplicates = ((Builder)other).AcceptDuplicates;
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
        public override bool CompareElements(IItem source, IItem target)
        {
            var comparer = new MyComparer(IgnoreCase);
            return comparer.Equals(source, target);
        }

        // This not needed for string comparisons...
        readonly struct MyComparer(bool IgnoreCase) : IEqualityComparer<IItem>
        {
            public bool Equals(IItem? x, IItem? y)
            {
                return x is NamedElement xnamed && y is NamedElement ynamed
                    ? string.Compare(xnamed.Name, ynamed.Name, IgnoreCase) == 0
                    : ReferenceEquals(x, y);
            }
            public int GetHashCode(IItem _) => throw new UnExpectedException();
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public override IEnumerable<IItem> FindDuplicates(IItem value) => base.FindDuplicates(value);

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="value"></param>
        /// <param name="range"></param>
        /// <returns></returns>
        public override bool AllowDuplicate(IItem value, IEnumerable<IItem> _)
        {
            if (AcceptDuplicates) return true;
            throw new DuplicateException("Duplicated value").WithData(value);
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="source"></param>
        /// <param name="target"></param>
        /// <returns></returns>
        public override bool SameElement(IItem source, IItem target) => base.SameElement(source, target);

        // ------------------------------------------------

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <returns><inheritdoc/></returns>
        public virtual IHost ToInstance()
        {
            var host = new THost(IgnoreCase) { AcceptDuplicates = AcceptDuplicates };
            host.Items.FlattenElements = FlattenElements;

            if (Count != 0) host.AddRange(this);
            return host;
        }

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
        /// <inh
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
}