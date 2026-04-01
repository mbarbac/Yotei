using IItem = Yotei.ORM.Generators.InvariantGenerator.Tests.IElement;
using IHost = Yotei.ORM.Generators.InvariantGenerator.Tests.IElementList_T;
using THost = Yotei.ORM.Generators.InvariantGenerator.Tests.ElementList_T;

namespace Yotei.ORM.Generators.InvariantGenerator.Tests;

partial class ElementList_T
{
    // ====================================================
    /// <summary>
    /// <inheritdoc cref="IHost.IBuilder"/>
    /// </summary>
    [DebuggerDisplay("{ToDebugString(4)}")]
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

        // ------------------------------------------------

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public IEngine Engine { get; }

        // ------------------------------------------------

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public override IItem ValidateElement(IItem value)
        {
            ArgumentNullException.ThrowIfNull(value);

            if (value is NamedElement named) named.Name.NotNullNotEmpty(trim: true);
            return value;
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public override bool CompareElements(
            IItem source, IItem target) => ItemComparer.Equals(source, target);

        MyComparer ItemComparer => _ItemComparer ??= new(!Engine.CaseSensitiveNames);
        MyComparer? _ItemComparer;

        readonly struct MyComparer(bool IgnoreCase) : IEqualityComparer<IItem>
        {
            public bool Equals(IItem? x, IItem? y)
            {
                return x is NamedElement xnamed && y is NamedElement ynamed
                    ? string.Compare(xnamed.Name, ynamed.Name, IgnoreCase) == 0
                    : ReferenceEquals(x, y);
            }
            public int GetHashCode(IItem obj) => throw new NotImplementedException();
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public override bool FlattenInput(IItem _) => false;

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public override bool IncludeDuplicated(
            IItem source, IItem target)
            => throw new DuplicateException("Duplicates not allowed").WithData(source).WithData(target);

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public override IEnumerable<IItem> FindDuplicates(IItem value) => base.FindDuplicates(value);
    }
}