namespace Yotei.ORM.Internals;

/// <summary>
/// Represents the ability of parsing SELECT clauses.
/// <br/>- Standard syntax: 'x => x.Element'.
/// <br/>- Alternate syntax: 'x => x.Element.As(...)'.
/// <br/>- Alternate syntax: 'x => x.Source.All()'.
/// </summary>
/// <remarks>
/// SELECT clauses accept complex specifications:
/// <br/>- Example: 'SELECT SUM([Amount]) AS TotalAmount, ...'
/// </remarks>
public static partial class FragmentSelect
{
    // ====================================================
    /// <summary>
    /// Represents an entry in a collection of fragments used to build a SELECT clause.
    /// </summary>
    [Cloneable]
    public partial class Entry : Fragment.Entry
    {
        /// <inheritdoc/>
        public override string CLAUSE => "SELECT";

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
            AllColumns = source.AllColumns;
        }

        /// <inheritdoc/>
        public override string ToString()
        {
            var str = Body.ToString()!;
            if (AllColumns) str += ".*";
            if (Alias is not null) str += $" AS {Alias}";
            return str;
        }

        /// <summary>
        /// The actual contents carried by this instance.
        /// </summary>
        public IDbToken Body { get; }

        /// <summary>
        /// Whether the body is a source specification from which all columns are to be selected,
        /// as in 'Table.*'.
        /// </summary>
        public bool AllColumns { get; init; }

        /// <summary>
        /// The alias that qualifies the source (Body), or null if any.
        /// </summary>
        public string? Alias { get; init; }

        /// <inheritdoc/>
        public override ICommandInfo.IBuilder Visit(DbTokenVisitor visitor)
        {
            var builder = visitor.Visit(Body);

            if (AllColumns)
            {
                var str = $"{builder.Text}.*";
                builder.ReplaceText(str);
            }
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
    /// Represents the collection of fragments used to build a SELECT clause.
    /// </summary>
    [Cloneable]
    public partial class Master : Fragment.Master
    {
        /// <inheritdoc/>
        public override string CLAUSE => "SELECT";

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
            // Intercepting all columns...
            bool allcolumns = false;
            var item = body.RemoveLast(x =>
            {
                if (x is not DbTokenMethod method) return false;
                if (method.Name.ToUpper() is not "ALL") return false;
                return true;
            },
            out var removed);

            if (removed is not null) // All() method found...
            {
                var method = (DbTokenMethod)removed;
                if (method.Arguments.Count != 0) throw new ArgumentException(
                    "'All()' virtual method must have no arguments.")
                    .WithData(body);

                if (item is DbTokenArgument) throw new ArgumentException(
                    "Body cannot just carry an 'All()' specification.")
                    .WithData(body);

                allcolumns = true;
                body = item;
            }

            // Intercepting alias...
            string? alias = null;
            item = body.RemoveLast(x =>
            {
                if (x is not DbTokenMethod method) return false;
                if (method.Name.ToUpper() is not "AS") return false;
                return true;
            }
            , out removed);

            if (removed is not null) // As(...) method found...
            {
                var method = (DbTokenMethod)removed;
                if (method.TypeArguments.Length != 0) throw new ArgumentException(
                    "No type arguments allowed for 'As(...)' virtual method.")
                    .WithData(body);

                if (item is DbTokenArgument) throw new ArgumentException(
                    "Body cannot just carry an 'As()' specification.")
                    .WithData(body);

                var visitor = Connection.Records.CreateDbTokenVisitor(Command.Locale);
                alias = visitor.ParseAlias(method.Arguments);
                body = item;
            }

            // Finishing...
            if (body is DbTokenInvoke invoke &&
                invoke.Arguments.Count == 1 &&
                invoke.Arguments[0] is DbTokenLiteral literal) body = literal;

            return alias is null
                ? new(this, body) { AllColumns = allcolumns }
                : new(this, body) { AllColumns = allcolumns, Alias = alias };
        }

        // ------------------------------------------------

        /// <inheritdoc/>
        public override string? Separator(Fragment.Entry _) => ", ";
    }
}
