#pragma warning disable IDE0079
#pragma warning disable IDE0042
#pragma warning disable IDE1006

namespace Yotei.ORM.Internals;

/// <summary>
/// Represents the ability of parsing SELECT clauses.
/// <br/>- Standard syntax: 'x => x.Element'.
/// <br/>- Alternate syntax: 'x => x.Element.As(...)'.
/// <br/>- Alternate syntax: 'x => x.Source.All()'.
/// </summary>
/// <remarks>
/// SELECT clauses accept complex specifications.
/// <br/>- Example: 'SELECT SUM([Amount]) AS TotalAmount, ...'
/// </remarks>
public static partial class FragmentSelect
{
    // ====================================================
    /// <summary>
    /// Represents an entry in a collection of fragments used to build SELECT-alike clauses.
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
            // Literals...
            if (Body is DbTokenLiteral literal)
            {
                var main = literal.Value;
                var parts = Extractor.LastSeparator(main, Engine, true, out var found, "AS");
                if (found)
                {
                    main = parts.Left.NotNullNotEmpty();
                    Body = new DbTokenLiteral(main);
                    Alias = parts.Right.NotNullNotEmpty();
                    AsSpec = parts.Spec;
                }
                parts = Extractor.FirstSeparator(main, Engine, false, out found, ".* ");
                if (found)
                {
                    parts.Left = parts.Left.NotNullNotEmpty();
                    parts.Right = parts.Right.Trim();

                    main = $"{parts.Left} {parts.Right}".NotNullNotEmpty();
                    Body = new DbTokenLiteral(main);
                    AllColumns = true;
                }
            }

            // Command-info...
            if (Body is DbTokenCommandInfo command)
            {
                var info = command.CommandInfo;
                var main = info.Text;

                var parts = Extractor.LastSeparator(main, Engine, true, out var found, "AS");
                if (found)
                {
                    main = parts.Left.NotNullNotEmpty();
                    info = new CommandInfo(Engine, main, info.Parameters);
                    Body = new DbTokenCommandInfo(info);
                    Alias = parts.Right.NotNullNotEmpty();
                    AsSpec = parts.Spec;
                }
                parts = Extractor.FirstSeparator(main, Engine, false, out found, ".* ");
                if (found)
                {
                    parts.Left = parts.Left.NotNullNotEmpty();
                    parts.Right = parts.Right.Trim();

                    main = $"{parts.Left} {parts.Right}".NotNullNotEmpty();
                    info = new CommandInfo(Engine, main, info.Parameters);
                    Body = new DbTokenCommandInfo(info);
                    AllColumns = true;
                }
            }

            // Any other token is just accepted, even empty ones...
            return;
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
        /// Determines if all columns from the given source are to be selected, as in 'Table.*'.
        /// </summary>
        public bool AllColumns
        {
            get => _AllColumns;
            init => _AllColumns = value;
        }
        internal protected bool _AllColumns { get; set; }

        /// <summary>
        /// The alias that qualifies the source, if any.
        /// </summary>
        public string? Alias
        {
            get => _Alias;
            init => _Alias = value;
        }
        internal protected string? _Alias
        {
            get => __Alias;
            set => __Alias = value?.NotNullNotEmpty(trim: true);
        }
        string? __Alias = default!;

        public string? AsSpec
        {
            get => _AsSpec ?? "AS";
            init => _AsSpec = value;
        }
        internal protected string? _AsSpec
        {
            get => __AsSpec;
            set => __AsSpec = value?.NotNullNotEmpty(trim: true);
        }
        string? __AsSpec = default!;

        // ------------------------------------------------

        /// <inheritdoc/>
        public override string ToString()
        {
            var str = Body.ToString()!;

            if (AllColumns) str += ".*";
            if (Alias is not null) str += $" {AsSpec} {Alias}";
            return str;
        }

        /// <inheritdoc/>
        public override ICommandInfo.IBuilder Visit(DbTokenVisitor visitor, bool separate)
        {
            var builder = visitor.Visit(Body);

            if (AllColumns) builder.ReplaceText($"{builder.Text}.*");
            if (Alias is not null) builder.ReplaceText($"{builder.Text} {AsSpec} {Alias}");

            if (separate) builder.ReplaceText($", {builder.Text}");
            return builder;
        }
    }

    // ====================================================
    /// <summary>
    /// Represents the collection of fragments used to build SELECT-alike clauses.
    /// </summary>
    [Cloneable]
    public partial class Master : Fragment.Master
    {
        /// <summary>
        /// Initializes a new instance.
        /// </summary>
        /// <param name="command"></param>
        /// <param name="clause"></param>
        public Master(ICommand command, string clause = "SELECT") : base(command, clause) { }

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
                $"Entry is not a valid {Clause} one.")
                .WithData(entry);

            return valid;
        }

        // ------------------------------------------------

        /// <inheritdoc/>
        public override Entry CreateEntry(IDbToken body)
        {
            // Intercepting '...All()' method...
            bool allcolumns = false;
            var item = body.ExtractLast(x =>
            {
                if (x is not DbTokenMethod method) return false;
                if (method.Name.ToUpper() is not "ALL") return false;
                return true;
            }
            , out var removed); if (removed is not null)
            {
                var method = (DbTokenMethod)removed;

                if (method.TypeArguments.Length != 0) throw new ArgumentException(
                    $"No type arguments allowed for '{method.Name}()' virtual method.")
                    .WithData(body);

                if (method.Arguments.Count != 0) throw new ArgumentException(
                    $"No arguments allowed for '{method.Name}()' virtual method.")
                    .WithData(body);

                if (item is DbTokenArgument) throw new ArgumentException(
                    "Body cannot just be an order specification.")
                    .WithData(body);

                allcolumns = true;
                body = item;
            }

            // Intercepting '...As()' method...
            string? alias = null;
            string? asspec = null;
            item = body.ExtractLast(x =>
            {
                if (x is not DbTokenMethod method) return false;
                if (method.Name.ToUpper() is not "AS") return false;
                return true;
            }
            , out removed); if (removed is not null)
            {
                var method = (DbTokenMethod)removed;

                if (method.TypeArguments.Length != 0) throw new ArgumentException(
                    $"No type arguments allowed for '{method.Name}()' virtual method.")
                    .WithData(body);

                if (method.Arguments.Count == 0) throw new ArgumentException(
                    $"'{method.Name}()' cannot be a parameterless method.")
                    .WithData(body);

                if (item is DbTokenArgument) throw new ArgumentException(
                    $"Body after '{method.Name}' removal cannot be empty.")
                    .WithData(body);

                var visitor = Connection.Records.CreateDbTokenVisitor(Command.Locale);
                alias = visitor.ChainToAlias(method.Arguments);
                asspec = method.Name;
                body = item;
            }

            // Finishing...
            var entry = new Entry(this, body);
            if (allcolumns) entry._AllColumns = true;
            if (alias is not null) entry._Alias = alias;
            if (asspec is not null) entry._AsSpec = asspec;
            return entry;
        }
    }
}