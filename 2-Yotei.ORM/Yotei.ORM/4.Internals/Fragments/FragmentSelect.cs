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
        /// <summary>
        /// Initializes a new instance.
        /// </summary>
        /// <param name="master"></param>
        /// <param name="body"></param>
        public Entry(Master master, IDbToken body) : base(master, body)
        {
            if (body is not DbTokenLiteral literal) return;

            Alias = null;
            var (main, alias) = literal.Value.Trim().ExtractMainAlias(Engine, out var found);
            if (found)
            {
                Body = literal = new DbTokenLiteral(main.NotNullNotEmpty());
                Alias = alias.NotNullNotEmpty();
            }

            AllColumns = false;
            main = literal.Value.Trim();
            var value = ".*"; if (TryExtractLast(ref main, ref value, false))
            {
                Body = new DbTokenLiteral(main.NotNullNotEmpty());
                AllColumns = true;
            }
        }

        /// <summary>
        /// Copy constructor.
        /// </summary>
        /// <param name="source"></param>
        protected Entry(Entry source) : base(source)
        {
            AllColumns = source.AllColumns;
            Alias = source.Alias;
        }

        /// <summary>
        /// Determines if all columns from the given source shall be selected, as in 'Table.*'.
        /// </summary>
        public bool AllColumns
        {
            get => _AllColumns;
            init => _AllColumns = value;
        }
        internal bool _AllColumns;

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
        public override string ToString()
        {
            var str = Body.ToString()!;
            if (AllColumns) str += ".*";
            if (Alias is not null) str += $" AS {Alias}";
            return str;
        }

        /// <inheritdoc/>
        protected override ICommandInfo.IBuilder VisitBody(DbTokenVisitor visitor)
        {
            var builder = visitor.Visit(Body);

            if (AllColumns) builder.ReplaceText($"{builder.Text}.*");
            if (Alias is not null) builder.ReplaceText($"{builder.Text} AS {Alias}");

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
        public override string CLAUSE { get; set; } = "SELECT";

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
            // Intercepting 'All()'...
            bool allcolumns = false;
            var item = body.RemoveFirst(x =>
            {
                if (x is not DbTokenMethod method) return false;
                if (method.Name.ToUpper() is not "ALL") return false;
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

                if (method.Arguments.Count != 0) throw new ArgumentException(
                    $"'{upper}()' must be a parameterless method.")
                    .WithData(body);

                if (item is DbTokenArgument) throw new ArgumentException(
                    $"Body after '{upper}()' cannot be empty.")
                    .WithData(body);

                allcolumns = true;
                body = item;
            }

            // Intercepting 'As(...)'...
            string? alias = null;
            item = body.RemoveFirst(x =>
            {
                if (x is not DbTokenMethod method) return false;
                if (method.Name.ToUpper() is not "AS") return false;
                return true;
            }
            , out removed);

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
            if (allcolumns) entry._AllColumns = true;
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
