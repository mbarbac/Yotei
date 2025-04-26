using IHost = Yotei.ORM.IParameterList;
using THost = Yotei.ORM.Code.ParameterList;
using IItem = Yotei.ORM.IParameter;
using TKey = string;

namespace Yotei.ORM.Code;

public partial class ParameterList
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
        /// <param name="engine"></param>
        public Builder(IEngine engine) => Engine = engine.ThrowWhenNull();

        /// <summary>
        /// Initializes a new empty instance with the given initial capacity.
        /// </summary>
        /// <param name="engine"></param>
        /// <param name="capacity"></param>
        public Builder(IEngine engine, int capacity) : this(engine) => Capacity = capacity;

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
        /// <param name="source"></param>
        public Builder(Builder source) : this(source.Engine) => AddRange(source);

        // ----------------------------------------------------

        /// <inheritdoc/>
        public override IItem ValidateItem(IItem item) => item.ThrowWhenNull();

        /// <inheritdoc/>
        public override string GetKey(IItem item) => item.ThrowWhenNull().Name;

        /// <inheritdoc/>
        public override string ValidateKey(TKey key) => key.NotNullNotEmpty();

        /// <inheritdoc/>
        public override IEqualityComparer<TKey> Comparer => Engine.CaseSensitiveNames
            ? StringComparer.CurrentCulture
            : StringComparer.CurrentCultureIgnoreCase;

        /// <inheritdoc/>
        public override bool ExpandItems => false;

        /// <inheritdoc/>
        public override bool IncludeDuplicated(IItem item, IItem source)
            => ReferenceEquals(item, source)
            ? true
            : throw new DuplicateException("Duplicated element.").WithData(item);

        // ------------------------------------------------

        /// <inheritdoc/>
        public THost ToInstance() => new(Engine, this);
        IHost IHost.IBuilder.ToInstance() => ToInstance();

        /// <inheritdoc/>
        public IEngine Engine { get; }

        // ------------------------------------------------

        /// <inheritdoc/>
        protected override bool SameItem(
            IItem item, IItem source) => ReferenceEquals(item, source);

        // ------------------------------------------------

        /// <inheritdoc/>
        public string NextName()
        {
            for (int i = 0; i < int.MaxValue; i++)
            {
                var name = $"{Engine.ParameterPrefix}{i}";
                var index = IndexOf(name);
                if (index < 0) return name;
            }

            throw new UnExpectedException("Range of integers exahusted.");
        }

        /// <inheritdoc/>
        public int AddNew(object? value, out IItem item)
        {
            item = new Parameter(NextName(), value);
            return Add(item);
        }

        /// <inheritdoc/>
        public int InsertNew(int index, object? value, out IItem item)
        {
            item = new Parameter(NextName(), value);
            return Insert(index, item);
        }
    }
}