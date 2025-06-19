namespace Yotei.ORM.Internals;

/// <summary>
/// Represents the ability of parsing FROM clauses.
/// <br/>- Standard syntax: 'x => x.Source'.
/// <br/>- Alternate syntax: 'x => x.Source.As(...)'.
/// </summary>
public static partial class FragmentFrom
{
    // ====================================================
    /// <summary>
    /// Represents an entry in a collection of fragments used to build a FROM clause.
    /// </summary>
    [Cloneable]
    public partial class Entry : Fragment.Entry
    {
        /// <inheritdoc/>
        public override string CLAUSE => "FROM";

        /// <summary>
        /// Initializes a new instance.
        /// </summary>
        /// <param name="master"></param>
        /// <param name="body"></param>
        public Entry(Master master, IDbToken body) : base(master) => Body = body.ThrowWhenNull();

        /// <summary>
        /// Copy constructor.
        /// </summary>
        /// <param name="source"></param>
        protected Entry(Entry source) : base(source)
        {
            Body = source.Body;
            Alias = source.Alias?.Clone();
        }

        /// <inheritdoc/>
        public override string ToString() => Alias is null
            ? Body.ToString()!
            : $"{Body} AS {Alias}";

        /// <summary>
        /// The actual contents carried by this instance.
        /// </summary>
        public IDbToken Body { get; }

        /// <summary>
        /// The alias that qualifies the source (Body), or null if any.
        /// </summary>
        public DbTokenIdentifier? Alias { get; init; }

        /// <inheritdoc/>
        public override ICommandInfo.IBuilder Visit(DbTokenVisitor visitor)
        {
            var builder = visitor.Visit(Body);
            
            if (Alias is not null)
            {
                var str = $"{builder.Text} AS {Alias}";
                builder.ReplaceText(str);
            }
            return builder;
        }
    }

    // ====================================================
    /// <summary>
    /// Represents the collection of fragments used to build a FROM clause.
    /// </summary>
    [Cloneable]
    public partial class Master : Fragment.Master
    {
        /// <inheritdoc/>
        public override string CLAUSE => "FROM";

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
        public override Entry Create(IDbToken body) => throw null;

        /*
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
         */
    }
}
