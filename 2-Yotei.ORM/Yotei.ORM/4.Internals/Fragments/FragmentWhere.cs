namespace Yotei.ORM.Internals;

/// <summary>
/// Represents the ability of parsing WHERE clauses.
/// <br/>- Standard syntax: 'x => Condition' (and AND connector is used by default).
/// <br/>- Alternate syntax: 'x => x.And(...)'.
/// <br/>- Alternate syntax: 'x => x.Or(...)'.
/// </summary>
public static partial class FragmentWhere
{
    readonly static string CLAUSE = "WHERE";

    // ====================================================
    /// <summary>
    /// Represents an entry in a collection of fragments used to build a WHERE clause.
    /// </summary>
    [Cloneable]
    public partial class Entry : Fragment.Entry
    {
        /// <summary>
        /// Initializes a new instance.
        /// </summary>
        /// <param name="useOr"></param>
        /// <param name="body"></param>
        public Entry(bool useOr, DbTokenLiteral body) : base()
        {
            UseOR = useOr;
            Body = body.ThrowWhenNull();
        }

        /// <summary>
        /// Initializes a new instance.
        /// </summary>
        /// <param name="useOr"></param>
        /// <param name="body"></param>
        public Entry(bool useOr, DbTokenBinary body) : base()
        {
            UseOR = useOr;
            Body = body.ThrowWhenNull();

            switch (body.Operation)
            {
                case ExpressionType.Equal:
                case ExpressionType.NotEqual:
                case ExpressionType.And:
                case ExpressionType.Or:
                case ExpressionType.GreaterThan:
                case ExpressionType.GreaterThanOrEqual:
                case ExpressionType.LessThan:
                case ExpressionType.LessThanOrEqual:
                    break;

                default:
                    throw new ArgumentException(
                        $"Invalid binary {CLAUSE} entry.").WithData(body);
            }
        }

        /// <summary>
        /// Copy constructor.
        /// </summary>
        /// <param name="source"></param>
        protected Entry(Entry source) : base(source)
        {
            UseOR = source.UseOR;
            Body = source.Body;
        }

        /// <inheritdoc/>
        public override string ToString() => $"{(UseOR ? "OR" : "AND")} {Body}";

        /// <summary>
        /// Determines if this instance chains with any previous one using an "OR" connector, or
        /// rather it shall use the default "AND" one.
        /// </summary>
        public bool UseOR { get; }

        /// <summary>
        /// The actual contents carried by this instance.
        /// <br/> Only <see cref="DbTokenLiteral"/> and <see cref="DbTokenBinary"/> objects are
        /// allowed.
        /// </summary>
        public IDbToken Body { get; }

        /// <inheritdoc/>
        public override ICommandInfo.IBuilder Visit(DbTokenVisitor visitor, bool first, bool last)
        {
            var builder = visitor.Visit(Body);

            if (!first)
            {
                var separator = UseOR ? "OR" : "AND";
                builder.ReplaceText($"{separator} {builder.Text}");
            }
            return builder;
        }
    }

    // ====================================================
    /// <summary>
    /// Represents the collection of fragments used to build a head or tail WHERE clause.
    /// </summary>
    [Cloneable]
    public partial class Master : Fragment.Master
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
        protected Master(Master source) : base(source.Command) { }

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
        public override Entry Create(IDbToken body)
        {
            throw null;
        }

        // ------------------------------------------------

        /// <inheritdoc/>
        public override string? Separator => " ";
    }
}