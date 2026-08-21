namespace Yotei.ORM.Internals;

partial class DbTokenChain
{
    // ====================================================
    /// <summary>
    /// Represents a builder of <see cref="DbTokenChain"/> instances.
    /// </summary>
    [DebuggerDisplay("{ToDebugString(3)}")]
    [Cloneable]
    public partial class Builder : CoreList<IDbToken>
    {
        /// <summary>
        /// Initializes a new empty instance.
        /// </summary>
        public Builder() { }

        /// <summary>
        /// Initializes a new instance with the elements of the given range.
        /// </summary>
        /// <param name="range"></param>
        public Builder(IEnumerable<IDbToken> range) => AddRange(range);

        /// <summary>
        /// Copy constructor.
        /// </summary>
        /// <param name="other"></param>
        protected Builder(Builder other) => AddRange(other.ThrowWhenNull());

        /// <summary>
        /// Returns an alternate string representation of this instance.
        /// </summary>
        /// <param name="head"></param>
        /// <param name="tail"></param>
        /// <param name="separator"></param>
        /// <returns></returns>
        public string ToString(string head, string tail, string separator = ", ")
        {
            head = head.NotNullNotEmpty(trim: false);
            tail = tail.NotNullNotEmpty(trim: false);
            separator = separator.NotNullNotEmpty(trim: false);

            var sb = new StringBuilder();
            sb.Append(head);

            for (int i = 0; i < Count; i++)
            {
                var item = this[i];
                var str = item.ToString();

                if (i > 0) sb.Append(separator);
                sb.Append(str);
            }

            sb.Append(tail);
            return sb.ToString();
        }

        /// <summary>
        /// Returns a new instance based upon the current captured contents.
        /// </summary>
        /// <returns></returns>
        public virtual DbTokenChain ToInstance() => Count == 0 ? new() : new(this);

        // ------------------------------------------------

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public override IDbToken ValidateElement(IDbToken value) => value.ThrowWhenNull();

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public override bool CompareElements(IDbToken source, IDbToken target)
        {
            if (ReferenceEquals(source, target)) return true;
            if (source is null && target is null) return true;
            if (source is null || target is null) return false;

            return source.Equals(target);
        }

        /// <summary>
        /// <inheritdoc/>
        /// <br/> As duplicates are allowed, we need not to compute duplicates.
        /// </summary>
        [SuppressMessage("", "IDE0301")]
        public override IEnumerable<IDbToken> FindDuplicates(IDbToken _) => Array.Empty<IDbToken>();

        /// <summary>
        /// <inheritdoc/>
        /// <br/> Duplicates allowed.
        /// </summary>
        public override bool AllowDuplicate(IDbToken _, IEnumerable<IDbToken> _2) => true;

        /*
        // ------------------------------------------------

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <returns></returns>
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

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="value"></param>
        /// <param name="item"></param>
        /// <returns></returns>
        public virtual int AddNew(object? value, out IDbToken item)
        {
            item = new Parameter(NextName(), value);
            return Add(item);
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="index"></param>
        /// <param name="value"></param>
        /// <param name="item"></param>
        /// <returns></returns>
        public virtual int InsertNew(int index, object? value, out IDbToken item)
        {
            item = new Parameter(NextName(), value);
            return Insert(index, item);
        }
         */
    }
}