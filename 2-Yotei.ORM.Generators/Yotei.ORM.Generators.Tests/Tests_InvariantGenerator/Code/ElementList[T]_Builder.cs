using IItem = Yotei.ORM.InvariantGenerator.Tests.IElement;
using IHost = Yotei.ORM.InvariantGenerator.Tests.IElementList_T;
using THost = Yotei.ORM.InvariantGenerator.Tests.ElementList_T;

namespace Yotei.ORM.InvariantGenerator.Tests;

partial class ElementList_T
{
    // ====================================================
    /// <summary>
    /// <inheritdoc cref="IHost.IBuilder"/>
    /// </summary>
    [DebuggerDisplay("{ToDebugString(3)}")]
    [Cloneable]
    public partial class Builder : CoreList<IItem>, IHost.IBuilder
    {
        /// <summary>
        /// Initializes a new empty instance.
        /// </summary>
        /// <param name="engine"></param>
        public Builder(IEngine engine) => Engine = engine.ThrowWhenNull();

        /// <summary>
        /// Initializes a new instance with the elements of the given range.
        /// </summary>
        /// <param name="engine"></param>
        /// <param name="range"></param>
        public Builder(
            IEngine engine, IEnumerable<IItem> range) : this(engine) => AddRange(range);

        /// <summary>
        /// Copy constructor.
        /// </summary>
        /// <param name="other"></param>
        protected Builder(Builder other)
        {
            Engine = other.Engine;
            AddRange(other);
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <returns></returns>
        public IHost ToInstance() => Count == 0 ? new THost(Engine) : new THost(Engine, this);

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public IEngine Engine { get; }

        // ------------------------------------------------

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public override IItem ValidateElement(IItem value)
        {
            ArgumentNullException.ThrowIfNull(value);

            if (value is NamedElement named) named.Name.NotNullNotEmpty(trim: true);
            return value;
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="source"></param>
        /// <param name="target"></param>
        /// <returns></returns>
        public override bool CompareElements(IItem source, IItem target)
        {
            var comparer = new MyComparer(Engine.IgnoreCase);
            return comparer.Equals(source, target);
        }

        readonly struct MyComparer(bool IgnoreCase) : IEqualityComparer<IItem>
        {
            public bool Equals(IItem? x, IItem? y)
            {
                return x is NamedElement xname && y is NamedElement ynamed
                    ? string.Compare(xname.Name, ynamed.Name, IgnoreCase) == 0
                    : ReferenceEquals(x, y);
            }
            public int GetHashCode(IItem _) => throw new UnExpectedException();
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public override IEnumerable<IItem> FindDuplicates(IItem value) => base.FindDuplicates(value);

        /// <summary>
        /// Determines how duplicated elements are included in this collection:
        /// <br/>- <see langword="true"/>: the duplicated element is included in the collection.
        /// <br/>- <see langword="false"/>: a duplicated exception is thrown.
        /// <br/>- <see langword="null"/>: the duplicated element is ignored.
        /// </summary>
        public override bool? AllowDuplicates
        {
            get => base.AllowDuplicates;
            set => base.AllowDuplicates = value;
        }
        = false;
    }
}