namespace Yotei.ORM.Internal;

/// <summary>
/// Represents the ability of parsing WHERE clauses.
/// <br/>- Standard syntax: 'x => Condition' (and AND connector is used by default).
/// <br/>- Alternate syntax: 'x => x.And(...)'.
/// <br/>- Alternate syntax: 'x => x.Or(...)'.
/// </summary>
public static partial class FragmentWhere
{
    // ====================================================
    /// <summary>
    /// Represents an entry in a collection of fragments used to build WHERE clauses.
    /// </summary>
    [Cloneable]
    public partial class Entry : Fragment.Entry
    {
        protected virtual string CLAUSE => "WHERE";

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
                        $"Invalid binary {CLAUSE} entry.").WithData(body);
            }
        }

        /// <summary>
        /// Copy constructor.
        /// </summary>
        /// <param name="source"></param>
        public Entry(Entry source) : base(source) => UseOR = source.UseOR;

        /// <inheritdoc/>
        public override string ToString() => $"{(UseOR ? "OR" : "AND")} {Body}";

        /// <summary>
        /// Determines if this entry chains with any previous ones using an 'OR' connector, or
        /// rather it shall use the default 'AND' one.
        /// </summary>
        public bool UseOR { get; init; }

        // ------------------------------------------------

        /// <inheritdoc/>
        public override ICommandInfo.IBuilder Visit(DbTokenVisitor visitor, string? separator)
        {
            var builder = visitor.Visit(Body);

            if (ValidSeparator(separator))
            {
                separator += UseOR ? "OR " : "AND ";
                builder.ReplaceText($"{separator}{builder.Text}");
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
        protected virtual string CLAUSE => "WHERE";

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

            if (removed != null) // Found...
            {
                var method = (DbTokenMethod)removed;

                useOR = method.Name.Equals("OR", StringComparison.OrdinalIgnoreCase);

                switch (method.Arguments.Count)
                {
                    case 0:
                        body = item;
                        break;

                    case 1:
                        if (item is not DbTokenArgument) throw new ArgumentException(
                            $"Body after '{method.Name}({method.Arguments[0]})' must be empty.")
                            .WithData(body);

                        item = method.Arguments[0];
                        body = item;
                        break;

                    default:
                        throw new ArgumentException(
                            $"Too many arguments in '{method.Name}(...)' method.")
                            .WithData(body);
                }
            }

            // Finishing...
            if (body is DbTokenInvoke invoke &&
                invoke.Arguments.Count == 1 &&
                invoke.Arguments[0] is DbTokenLiteral literal) body = literal;

            return body switch
            {
                DbTokenLiteral temp => new(temp) { UseOR = useOR },
                DbTokenBinary temp => new(temp) { UseOR = useOR },

                _ => throw new ArgumentException(
                    $"Specification does not resolve into a valid {CLAUSE} clause.")
                    .WithData(body)
            };
        }

        // ------------------------------------------------

        /// <inheritdoc/>
        public override string? Separator => " ";
    }
}