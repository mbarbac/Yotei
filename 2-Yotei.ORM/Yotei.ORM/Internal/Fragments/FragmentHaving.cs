namespace Yotei.ORM.Internal;

/// <summary>
/// Represents the ability of parsing HAVING clauses.
/// <br/>- Standard syntax: 'x => Condition' (and AND connector is used by default).
/// <br/>- Alternate syntax: 'x => x.And(...)'.
/// <br/>- Alternate syntax: 'x => x.Or(...)'.
/// </summary>
public static partial class FragmentHaving
{
    // ====================================================
    /// <summary>
    /// Represents an entry in a collection of fragments used to build HAVING clauses.
    /// </summary>
    [Cloneable]
    public partial class Entry : FragmentWhere.Entry
    {
        protected override string CLAUSE => "HAVING";

        /// <summary>
        /// Initializes a new instance.
        /// </summary>
        /// <param name="body"></param>
        public Entry(DbTokenLiteral body) : base(body) { }

        /// <summary>
        /// Initializes a new instance.
        /// </summary>
        /// <param name="body"></param>
        public Entry(DbTokenBinary body) : base(body) { }

        /// <summary>
        /// Copy constructor.
        /// </summary>
        /// <param name="source"></param>
        public Entry(Entry source) : base(source) { }
    }

    // ====================================================
    /// <summary>
    /// Represents a collection of fragments used to build HAVING clauses.
    /// </summary>
    [Cloneable]
    public partial class Master : FragmentWhere.Master
    {
        protected override string CLAUSE => "HAVING";

        /// <summary>
        /// Initializes a new instance.
        /// </summary>
        /// <param name="command"></param>
        public Master(ICommand command) : base(command) { }

        /// <summary>
        /// Copy constructor.
        /// </summary>
        /// <param name="source"></param>
        public Master(Master source) : base(source) { }

        // ------------------------------------------------

        /// <inheritdoc/>
        protected override Entry Validate(Fragment.Entry entry)
        {
            if (entry is not Entry valid) throw new ArgumentException(
                $"Entry is not a valid {CLAUSE} one.")
                .WithData(entry);

            return valid;
        }

        /// <inheritdoc/>
        protected override Entry Create(DbToken body)
        {
            var item = base.Create(body);

            // Finishing...
            return item.Body switch
            {
                DbTokenLiteral temp => new(temp) { UseOR = item.UseOR },
                DbTokenBinary temp => new(temp) { UseOR = item.UseOR },

                _ => throw new ArgumentException(
                    $"Specification does not resolve into a valid {CLAUSE} clause.")
                    .WithData(body)
            };
        }
    }
}