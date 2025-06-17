namespace Yotei.ORM.Internals;

/// <summary>
/// Represents the ability of parsing HAVING clauses.
/// <br/>- Standard syntax: 'x => Condition' (and AND connector is used by default).
/// <br/>- Alternate syntax: 'x => x.And(...)'.
/// <br/>- Alternate syntax: 'x => x.Or(...)'.
/// </summary>
public static partial class FragmentHaving
{
    readonly static string CLAUSE = "HAVING";

    // ====================================================
    /// <summary>
    /// Represents an entry in a collection of fragments used to build a HAVING clause.
    /// </summary>
    [Cloneable]
    public partial class Entry : FragmentWhere.Entry
    {
        /// <summary>
        /// Initializes a new instance.
        /// </summary>
        /// <param name="useOr"></param>
        /// <param name="body"></param>
        public Entry(bool useOr, DbTokenLiteral body) : base(useOr, body) { }

        /// <summary>
        /// Initializes a new instance.
        /// </summary>
        /// <param name="useOr"></param>
        /// <param name="body"></param>
        public Entry(bool useOr, DbTokenBinary body) : base(useOr, body) { }

        /// <summary>
        /// Copy constructor.
        /// </summary>
        /// <param name="source"></param>
        protected Entry(Entry source) : base(source) { }
    }

    // ====================================================
    /// <summary>
    /// Represents the collection of fragments used to build a head or tail HAVING clause.
    /// </summary>
    [Cloneable]
    public partial class Master : FragmentWhere.Master
    {
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

            // Finishing...
            return item.Body switch
            {
                DbTokenLiteral temp => new(item.UseOR, temp),
                DbTokenBinary temp => new(item.UseOR, temp),

                _ => throw new ArgumentException(
                    $"Specification does not resolve into a valid {CLAUSE} clause.")
                    .WithData(body)
            };
        }
    }
}