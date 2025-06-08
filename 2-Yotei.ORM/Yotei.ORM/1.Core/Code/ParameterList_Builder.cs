namespace Yotei.ORM.Code;

partial class ParameterList
{
    // ====================================================
    /// <inheritdoc cref="IParameterList.IBuilder"/>
    [DebuggerDisplay("{Items.ToDebugString(5)}")]
    [Cloneable]
    public partial class Builder : CoreList<string, IParameter>, IParameterList.IBuilder
    {
        /// <summary>
        /// Initializes a new empty instance.
        /// </summary>
        /// <param name="engine"></param>
        public Builder(IEngine engine) => Engine = engine.ThrowWhenNull();

        /// <summary>
        /// Initializes a new instance with the elements from the given range.
        /// </summary>
        /// <param name="range"></param>
        public Builder(
            IEngine engine, IEnumerable<IParameter> range) : this(engine) => AddRange(range);

        /// <summary>
        /// Copy constructor.
        /// </summary>
        /// <param name="source"></param>
        protected Builder(Builder source) : this(source.Engine) => AddRange(source);

        // ------------------------------------------------

        public override IParameter ValidateItem(IParameter item) => item.ThrowWhenNull();
        public override string GetKey(IParameter item) => item.ThrowWhenNull().Name;
        public override string ValidateKey(string key) => key.NotNullNotEmpty();
        public override IEqualityComparer<string> Comparer
        {
            get => Engine.CaseSensitiveNames
                ? StringComparer.CurrentCulture
                : StringComparer.CurrentCultureIgnoreCase;
        }
        public override bool ExpandItems => true;
        public override bool IsValidDuplicate(IParameter source, IParameter item)
            => ReferenceEquals(source, item)
            ? true
            : throw new DuplicateException("Duplicated element.").WithData(item);

        // ------------------------------------------------

        /// <inheritdoc/>
        public IEngine Engine { get; }

        // ------------------------------------------------

        /// <inheritdoc/>
        public string NextName()
        {
            for (int i = Count; i < int.MaxValue; i++)
            {
                var name = $"{Engine.ParameterPrefix}{i}";
                var index = IndexOf(name);
                if (index < 0) return name;
            }

            throw new UnExpectedException("Range of integers exahusted.");
        }

        /// <inheritdoc/>
        public virtual int AddNew(object? value, out IParameter item)
        {
            item = new Parameter(NextName(), value);
            return Add(item);
        }

        /// <inheritdoc/>
        public virtual int InsertNew(int index, object? value, out IParameter item)
        {
            item = new Parameter(NextName(), value);
            return Insert(index, item);
        }
    }
}