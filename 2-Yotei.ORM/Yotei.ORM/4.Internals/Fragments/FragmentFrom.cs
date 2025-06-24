namespace Yotei.ORM.Internals;

/// <summary>
///  Represents the ability of parsing FROM clauses.
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
        /// <summary>
        /// Initializes a new instance.
        /// </summary>
        /// <param name="master"></param>
        /// <param name="body"></param>
        public Entry(Master master, IDbToken body) : base(master, body)
        {
            if (body is not DbTokenLiteral literal) return;

            var (main, alias) = literal.Value.Trim().ExtractMainAlias(Engine, out var found);
            if (found)
            {
                Body = literal = new DbTokenLiteral(main.NotNullNotEmpty());
                Alias = alias.NotNullNotEmpty();
            }
        }

        /// <summary>
        /// Copy constructor.
        /// </summary>
        /// <param name="source"></param>
        protected Entry(Entry source) : base(source) => Alias = source.Alias;

        /// <summary>
        /// The alias that qualifies the source (Body), or null if any.
        /// </summary>
        public string? Alias
        {
            get => _Alias;
            init => _Alias = value?.NotNullNotEmpty();
        }
        internal string? _Alias;

        // ------------------------------------------------

        /// <inheritdoc/>
        public override string ToString() => Alias is null
            ? Body.ToString()!
            : $"{Body} AS {Alias}";

        /// <inheritdoc/>
        protected override ICommandInfo.IBuilder VisitBody(DbTokenVisitor visitor)
        {
            var builder = visitor.Visit(Body);

            if (Alias is not null) builder.ReplaceText($"{builder.Text} AS {Alias}");
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
        public override string CLAUSE { get; set; } = "FROM";

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
        protected override Entry OnCreate(DbTokenInvoke? head, IDbToken body, DbTokenInvoke? tail)
        {
            // Intercepting 'As(...)'...
            string? alias = null;
            var item = body.RemoveFirst(x =>
            {
                if (x is not DbTokenMethod method) return false;
                if (method.Name.ToUpper() is not "AS") return false;
                return true;
            }
            , out var removed);

            if (removed is not null) // Found...
            {
                var method = (DbTokenMethod)removed;
                var name = method.Name;
                var upper = name.ToUpper();

                if (method.TypeArguments.Length != 0) throw new ArgumentException(
                    $"No type arguments allowed for '{upper}()' virtual method.")
                    .WithData(body);

                if (item is DbTokenArgument) throw new ArgumentException(
                    $"Body after '{upper}()' cannot be empty.")
                    .WithData(body);

                var visitor = Connection.Records.CreateDbTokenVisitor(Command.Locale);
                alias = visitor.ChainToAlias(method.Arguments);
                body = item;
            }

            // Finishing...
            var entry = new Entry(this, body);

            if (head is not null) entry._Head = head;
            if (tail is not null) entry._Tail = tail;
            if (alias is not null) entry._Alias = alias;
            return entry;
        }

        // ------------------------------------------------

        /// <inheritdoc/>
        /// <remarks>We only need to provide a separator when the previous entry specifies a
        /// connector.</remarks>
        public override string? Separator(Fragment.Entry entry) => ", ";
    }
}
