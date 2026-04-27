#pragma warning disable CS0436

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
    [DebuggerDisplay("{ToDebugString(3)}")]
    [Cloneable]
    public partial class Builder : CoreList<IItem>, IHost.IBuilder
    {
        /// <summary>
        /// Initializes a new instance.
        /// </summary>
        /// <param name="engine"></param>
        public Builder(IEngine engine) => Engine = engine.ThrowWhenNull();

        /// <summary>
        /// Initializes a new instance with the elements of the given range.
        /// </summary>
        /// <param name="engine"></param>
        /// <param name="range"></param>
        public Builder(IEngine engine, IEnumerable<IItem> range) : this(engine) => AddRange(range);

        /// <summary>
        /// Copy constructor.
        /// </summary>
        /// <param name="other"></param>
        protected Builder(Builder other)
        {
            ArgumentNullException.ThrowIfNull(other);

            Engine = other.Engine;
            AllowDuplicates = other.AllowDuplicates;
            AddRange(other);
        }

        // ------------------------------------------------

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
        public override IItem ValidateElement(IItem value)
        {
            ArgumentNullException.ThrowIfNull(value);

            if (value is NamedElement named) named.Name.NotNullNotEmpty(trim: true);
            return value;
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public override bool CompareElements(IItem source, IItem target)
        {
            return source is NamedElement xnamed && target is NamedElement ynamed
                ? string.Compare(xnamed.Name, ynamed.Name, Engine.IgnoreCase) == 0
                : source.EqualsEx(target);
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public override IEnumerable<IItem> FindDuplicates(
            IItem value)
            => base.FindDuplicates(value);

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public override bool AcceptDuplicated(IItem source, IItem duplicate)
            => AllowDuplicates
            ? true
            : throw new DuplicateException("Duplicates not allowed.")
            .WithData(source)
            .WithData(duplicate);

        // Used by the 'AcceptDuplicated' method, for debug purposes.
        public bool AllowDuplicates
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