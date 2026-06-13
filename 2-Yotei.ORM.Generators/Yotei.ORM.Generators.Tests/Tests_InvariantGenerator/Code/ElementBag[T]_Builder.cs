using IItem = Yotei.ORM.InvariantGenerator.Tests.IElement;
using IHost = Yotei.ORM.InvariantGenerator.Tests.IElementBag_T;
using THost = Yotei.ORM.InvariantGenerator.Tests.ElementBag_T;

namespace Yotei.ORM.InvariantGenerator.Tests;

partial class ElementBag_T
{
    // ====================================================
    /// <summary>
    /// Represents a builder for <see cref="IHost"/> instances.
    /// </summary>
    [Cloneable]
    public partial class Builder : CoreBag<IItem>, IHost.IBuilder
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
        /// <inheritdoc cref="IHost.IBuilder.ToInstance"/>
        /// </summary>
        /// <returns></returns>
        public THost ToInstance()
        {
            var host = new THost(Engine) { AcceptDuplicates = AcceptDuplicates };
            if (Count > 0) host.AddRange(this);
            return host;
        }
        IHost IHost.IBuilder.ToInstance() => ToInstance();

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
        /// <inheritdoc/>
        /// </summary>
        /// <param name="value"></param>
        /// <param name="_"></param>
        /// <returns></returns>
        public override bool AllowDuplicate(IElement value)
        {
            if (AcceptDuplicates) return true;
            throw new DuplicateException("Duplicated value").WithData(value);
        }

        public bool AcceptDuplicates // For debug purposes...
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