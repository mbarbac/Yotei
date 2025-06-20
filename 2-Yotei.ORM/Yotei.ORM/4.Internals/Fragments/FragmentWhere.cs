namespace Yotei.ORM.Internals;

/// <summary>
/// Represents the ability of parsing WHERE clauses.
/// <br/>- Standard syntax: 'x => Condition'.
/// <br/>- Alternate syntax: 'x => x.And(...)'.
/// <br/>- Alternate syntax: 'x => x.Or(...)'.
/// </summary>
public static partial class FragmentWhere
{
    // ====================================================
    /// <summary>
    /// Represents an entry in a collection of fragments used to build a WHERE clause.
    /// </summary>
    [Cloneable]
    public partial class Entry : Fragment.Entry
    {
        /// <inheritdoc/>
        public override string CLAUSE => "WHERE";

        /// <summary>
        /// Initializes a new instance.
        /// </summary>
        /// <param name="master"></param>
        /// <param name="body"></param>
        public Entry(
            Master master, DbTokenLiteral body) : base(master) => Body = body.ThrowWhenNull();

        /// <summary>
        /// Initializes a new instance.
        /// </summary>
        /// <param name="master"></param>
        /// <param name="body"></param>
        public Entry(Master master, DbTokenBinary body) : base(master)
        {
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
            Body = source.Body;
            UseOR = source.UseOR;
        }

        /// <inheritdoc/>
        public override string ToString() => Body is DbTokenBinary && UseOR is not null
            ? $"{(UseOR.Value ? "OR" : "AND")} {Body}"
            : Body.ToString()!;

        /// <summary>
        /// The actual contents carried by this instance.
        /// <br/> Only <see cref="DbTokenLiteral"/> and <see cref="DbTokenBinary"/> objects are
        /// allowed.
        /// </summary>
        public IDbToken Body { get; }

        /// <summary>
        /// Determines if this instance chains with any previous one using an "OR" connector, or
        /// rather is shall use the default "AND" one, or none of them.
        /// <br/> This property only affects instances whose body is a binary one.
        /// </summary>
        public bool? UseOR { get; init; }

        /// <inheritdoc/>
        public override ICommandInfo.IBuilder Visit(DbTokenVisitor visitor)
        {
            var builder = visitor.Visit(Body);

            var index = Master.IndexOf(this);
            if (index > 0 && Body is DbTokenBinary && UseOR is not null)
            {
                var valid = (Entry)Master[index - 1];
                if (valid.Body is DbTokenBinary)
                {
                    var separator = UseOR.Value ? "OR" : "AND";
                    builder.ReplaceText($"{separator} {builder.Text}");
                }
            }

            return builder;
        }
    }

    // ====================================================
    /// <summary>
    /// Represents the collection of fragments used to build a WHERE clause.
    /// </summary>
    [Cloneable]
    public partial class Master : Fragment.Master
    {
        /// <inheritdoc/>
        public override string CLAUSE => "WHERE";

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
            bool? useOR = null;

            // Intercepting 'x => x.And(...)' and 'x => x.Or(...)' methods...
            var item = body.RemoveFirst(x =>
            {
                if (x is not DbTokenMethod method) return false;
                if (method.Host is not DbTokenArgument) return false;
                if (method.Name.ToUpper() is not "AND" and not "OR") return false;
                return true;
            }
            , out var removed);

            if (removed is not null) // Virtual method found...
            {
                var method = (DbTokenMethod)removed;
                var name = method.Name;

                if (name.Equals("OR", StringComparison.OrdinalIgnoreCase)) useOR = true;
                else if (name.Equals("AND", StringComparison.OrdinalIgnoreCase)) useOR = false;
                else useOR = null;

                switch (method.Arguments.Count)
                {
                    // The remaining after the And() or Or() method...
                    case 0:
                        body = item;
                        break;

                    // The sole argument of the And(...) or Or(...) method...
                    case 1:
                        if (item is not DbTokenArgument) throw new ArgumentException(
                            $"Body after '{method.Name}(arg)' must be empty.")
                            .WithData(body);

                        body = method.Arguments[0];
                        break;

                    // Using many arguments, what does it mean?...
                    default:
                        throw new ArgumentException(
                            $"Too many arguments in '{method.Name}(...)' method.")
                            .WithData(body);
                }
            }

            // Finishing...
            if (body is DbTokenInvoke invoke &&
                invoke.Host is DbTokenArgument &&
                invoke.Arguments.Count == 1 &&
                invoke.Arguments[0] is DbTokenLiteral literal) body = literal;

            return body switch
            {
                DbTokenLiteral temp => new(this, temp),
                DbTokenBinary temp => new(this, temp) { UseOR = useOR },

                _ => throw new ArgumentException(
                    $"Specification does not resolve into a valid {CLAUSE} clause.")
                    .WithData(body)
            };
        }

        // ------------------------------------------------

        /// <inheritdoc/>
        public override string? Separator(Fragment.Entry entry)
        {
            var valid = (Entry)entry;
            var index = IndexOf(entry);

            if (index > 0 && valid.Body is DbTokenBinary)
            {
                var item = this[index - 1];
                var temp = (Entry)item;

                if (temp.Body is DbTokenBinary) return " ";
            }

            return null;
        }
    }
}
