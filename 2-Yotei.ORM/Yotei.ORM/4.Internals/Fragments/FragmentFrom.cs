namespace Yotei.ORM.Internals;

/// <summary>
/// Represents the ability of parsing FROM clauses.
/// <br/>- Standard syntax: 'x => x.Source'.
/// <br/>- Alternate syntax: 'x => x.Source.As(...)'.
/// </summary>
/// <remarks>
/// FROM clauses accept complex specifications:
/// <br/>- Example: 'FROM (SELECT [Id], [Age] FROM Other) AS Another, ...'
/// </remarks>
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
        public Entry(Master master, IDbToken body) : base(master)
        {
            Body = body.ThrowWhenNull();

            if (Body is DbTokenLiteral literal)
            {
                var (main, alias) = literal.Value.ExtractMainAlias(Engine, out var found);
                if (found)
                {
                    Body = new DbTokenLiteral(main);
                    Alias = alias;
                }
            }
        }

        /// <summary>
        /// Copy constructor.
        /// </summary>
        /// <param name="source"></param>
        protected Entry(Entry source) : base(source)
        {
            Body = source.Body;
            Alias = source.Alias;
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
        public string? Alias { get; init; }

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
        public override Entry Create(IDbToken body)
        {
            // Intercepting alias...
            string? alias = null;
            var item = body.RemoveLast(x =>
            {
                if (x is not DbTokenMethod method) return false;
                if (method.Name.ToUpper() is not "AS") return false;
                return true;
            }
            , out var removed);

            if (removed is not null) // As(...) method found...
            {
                var method = (DbTokenMethod)removed;
                if (method.TypeArguments.Length != 0) throw new ArgumentException(
                    "No type arguments allowed for 'As(...)' virtual method.")
                    .WithData(body);

                var visitor = Connection.Records.CreateDbTokenVisitor(Command.Locale);
                alias = visitor.ParseAlias(method.Arguments);

                if (item is DbTokenArgument) throw new ArgumentException(
                    "Body cannot just carry an alias specification.")
                    .WithData(body);
                body = item;
            }

            // Finishing...
            return alias is null
                ? new(this, body)
                : new(this, body) { Alias = alias };
        }

        // ------------------------------------------------

        /// <inheritdoc/>
        public override string? Separator(Fragment.Entry _) => ", ";
    }
}
