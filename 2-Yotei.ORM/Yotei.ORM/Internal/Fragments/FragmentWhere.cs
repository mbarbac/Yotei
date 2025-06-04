namespace Yotei.ORM.Internal;

/// <summary>
/// Represents the ability of parsing WHERE clauses.
/// <br/>- Standard syntax: 'x => Condition' (and AND connector is used by default).
/// <br/>- Alternate syntax: 'x => x.And(...)'.
/// <br/>- Alternate syntax: 'x => x.Or(...)'.
/// </summary>
public partial class FragmentWhere
{
    // ====================================================
    /// <summary>
    /// Represents an entry in a collection of fragments used to build WHERE clauses.
    /// </summary>
    [Cloneable]
    public partial class Entry : Fragment.Entry
    {
        protected virtual string FNAME => "WHERE";

        /// <summary>
        /// Initializes a new instance.
        /// </summary>
        /// <param name="body"></param>
        public Entry(DbTokenLiteral body) : base(body) { }

        /// <summary>
        /// Initializes a new instance.
        /// </summary>
        /// <param name="body"></param>
        public Entry(DbTokenBinary body) : base(body)
        {
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
                        $"Invalid binary {FNAME} entry.").WithData(body);
            }
        }

        /// <summary>
        /// Copy constructor.
        /// </summary>
        /// <param name="source"></param>
        public Entry(Entry source) : base(source) { }

        /// <summary>
        /// Determines if this entry chains with any previous ones using an 'OR' connector, or
        /// rather it shall use the default 'AND' one.
        /// </summary>
        public bool UseOR { get; init; }

        /// <inheritdoc/>
        public override ICommandInfo.IBuilder Visit(
            DbTokenVisitor visitor, Fragment.EntryPosition position)
        {
            var builder = base.Visit(visitor, position);

            if (position is not Fragment.EntryPosition.First)
            {
                // Master already adds a space when not the first entry...
                var str = UseOR ? "OR " : "AND ";
                builder.ReplaceText($"{str}{builder.Text}");
            }
            return builder;
        }
    }

    // ====================================================
    /// <summary>
    /// Represents a collection of fragments used to build WHERE clauses.
    /// </summary>
    [Cloneable]
    public partial class Master : Fragment.Master
    {
        protected virtual string FNAME => "WHERE";

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
                $"Entry is not a valid {FNAME} one.")
                .WithData(entry);

            return valid;
        }

        /// <inheritdoc/>
        protected override Entry Create(DbToken body)
        {
            var useOR = false;

            // Intercepting heading 'x => x.And(...)' and 'x => x.Or(...)' methods...
            var item = body.RemoveFirst(x =>
            {
                if (x is not DbTokenMethod method) return false;
                if (method.Host is not DbTokenArgument) return false;
                if (method.Name.ToUpper() is not "AND" and not "OR") return false;
                return true;
            }
            , out var removed);

            if (removed != null)
            {
                var method = (DbTokenMethod)removed;

                useOR = method.Name.Equals("OR", StringComparison.OrdinalIgnoreCase);

                if (method.Arguments.Count == 0)
                {
                    body = item;
                }
                if (method.Arguments.Count == 1)
                {
                    if (item is not DbTokenArgument) throw new ArgumentException(
                        $"Remaining body after '{method.Name}({method.Arguments[0]})' must be empty.")
                        .WithData(body);

                    item = method.Arguments[0];
                    body = item;
                }
                if (method.Arguments.Count > 1)
                {
                    throw new ArgumentException(
                        $"Too many arguments in '{method.Name}(...)' method.")
                        .WithData(body);
                }
            }

            // Finishing...
            if (body is DbTokenInvoke invoke &&
                invoke.Arguments.Count == 1 &&
                invoke.Arguments[0] is DbTokenLiteral literal)
            {
                return new Entry(literal) { UseOR = useOR };
            }
            if (body is DbTokenBinary binary)
            {
                return new Entry(binary) { UseOR = useOR };
            }
            throw new ArgumentException(
                $"Specification does not resolve into a valid {FNAME} entry.")
                .WithData(body);
        }

        // ------------------------------------------------

        /// <inheritdoc/>
        /// <remarks> The value of the '<paramref name="separator"/>' out argument is set to
        /// a space by default, it it is null after invoking the base method. </remarks>
        protected override ICommandInfo.IBuilder OnVisit(
            Fragment.Entry entry,
            DbTokenVisitor visitor, Fragment.EntryPosition position, out string? separator)
        {
            var builder = base.OnVisit(entry, visitor, position, out separator);
            separator ??= " ";
            return builder;
        }
    }
}