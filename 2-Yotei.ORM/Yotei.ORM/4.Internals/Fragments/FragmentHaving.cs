namespace Yotei.ORM.Internals;

/// <summary>
/// Represents the ability of parsing HAVING clauses.
/// <br/>- Standard syntax: 'x => Condition'.
/// <br/>- Alternate syntax: 'x => x.And(...)'.
/// <br/>- Alternate syntax: 'x => x.Or(...)'.
/// </summary>
public static partial class FragmentHaving
{
    // ====================================================
    /// <summary>
    /// Represents an entry in a collection of fragments used to build a HAVING clause.
    /// </summary>
    [Cloneable]
    public partial class Entry : FragmentWhere.Entry
    {
        /// <inheritdoc/>
        public override string CLAUSE => "HAVING";

        /// <summary>
        /// Initializes a new instance.
        /// </summary>
        /// <param name="master"></param>
        /// <param name="body"></param>
        public Entry(Master master, DbTokenLiteral body) : base(master, body) { }

        /// <summary>
        /// Initializes a new instance.
        /// </summary>
        /// <param name="master"></param>
        /// <param name="body"></param>
        public Entry(Master master, DbTokenBinary body) : base(master, body) { }

        /// <summary>
        /// Copy constructor.
        /// </summary>
        /// <param name="source"></param>
        protected Entry(Entry source) : base(source) { }
    }

    // ====================================================
    /// <summary>
    /// Represents the collection of fragments used to build a HAVING clause.
    /// </summary>
    [Cloneable]
    public partial class Master : FragmentWhere.Master
    {
        /// <inheritdoc/>
        public override string CLAUSE => "HAVING";

        /// <summary>
        /// Initializes a new instance.
        /// </summary>
        /// <param name="command"></param>
        public Master(ICommand command) : base(command) { }

        /// <summary>
        /// Copy constructor.
        /// </summary>
        /// <param name="source"></param>
        protected Master(Master source) : base(source) { }

        // ------------------------------------------------

        /// <inheritdoc/>
        protected override Entry Validate(Fragment.Entry entry)
        {
            if (entry is not Entry valid) throw new ArgumentException(
                $"Entry is not a valid {CLAUSE} one.")
                .WithData(entry);

            return valid;
        }

        // ------------------------------------------------

        /// <inheritdoc/>
        public override Entry Create(IDbToken body)
        {
            var item = base.Create(body);

            return item.Body switch
            {
                DbTokenLiteral temp => new(this, temp),
                DbTokenBinary temp => new(this, temp) { UseOR = item.UseOR },

                _ => throw new ArgumentException(
                    $"Specification does not resolve into a valid {CLAUSE} clause.")
                    .WithData(body)
            };
        }
    }
}
