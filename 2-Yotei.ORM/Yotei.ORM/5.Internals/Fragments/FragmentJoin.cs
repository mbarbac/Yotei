#pragma warning disable IDE0079
#pragma warning disable IDE0042
#pragma warning disable IDE1006

namespace Yotei.ORM.Internals;

/// <summary>
/// Represents the ability of parsing JOIN clauses.
/// <br/>- Standard syntax: 'x => x.Source.As(...).On(...)'.
/// <br/>- Alternate syntax: 'x => x.Join(join-type).Source.As(...).On(...)'.
/// </summary>
public static partial class FragmentJoin
{
    // ====================================================
    /// <summary>
    /// Represents an entry in a collection of fragments used to build JOIN-alike clauses.
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
                //var main = literal.Value;
                //var parts = Extractor.LastSeparator(main, Engine, true, out var found, "AS");
                //if (found)
                //{
                //    main = parts.Left.NotNullNotEmpty();
                //    Body = new DbTokenLiteral(main);
                //    Alias = parts.Right.NotNullNotEmpty();
                //    AsSpec = parts.Spec;
                //}
                //parts = Extractor.FirstSeparator(main, Engine, false, out found, ".* ");
                //if (found)
                //{
                //    parts.Left = parts.Left.NotNullNotEmpty();
                //    parts.Right = parts.Right.Trim();

                //    main = $"{parts.Left} {parts.Right}".NotNullNotEmpty();
                //    Body = new DbTokenLiteral(main);
                //    AllColumns = true;
                //}
            }

            // Command-info...
            if (Body is DbTokenCommandInfo command)
            {
                //var info = command.CommandInfo;
                //var main = info.Text;

                //var parts = Extractor.LastSeparator(main, Engine, true, out var found, "AS");
                //if (found)
                //{
                //    main = parts.Left.NotNullNotEmpty();
                //    info = new CommandInfo(Engine, main, info.Parameters);
                //    Body = new DbTokenCommandInfo(info);
                //    Alias = parts.Right.NotNullNotEmpty();
                //    AsSpec = parts.Spec;
                //}
                //parts = Extractor.FirstSeparator(main, Engine, false, out found, ".* ");
                //if (found)
                //{
                //    parts.Left = parts.Left.NotNullNotEmpty();
                //    parts.Right = parts.Right.Trim();

                //    main = $"{parts.Left} {parts.Right}".NotNullNotEmpty();
                //    info = new CommandInfo(Engine, main, info.Parameters);
                //    Body = new DbTokenCommandInfo(info);
                //    AllColumns = true;
                //}
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
            JoinType = source.JoinType;
            Alias = source.Alias;
            AsSpec = source.AsSpec;
            Condition = source.Condition?.Clone();
        }

        /// <summary>
        /// If not null, then the actual join-alike operation specification. If <c>null</c>, then
        /// a default 'JOIN' will be used.
        /// </summary>
        public string? JoinType
        {
            get => _JoinType;
            init => _JoinType = value;
        }
        internal protected string? _JoinType
        {
            get => __JoinType;
            set => __JoinType = value?.NotNullNotEmpty(trim: true);
        }
        internal string? __JoinType;

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

        /// <summary>
        /// The actual 'AS'-alike specification to use.
        /// </summary>
        public string? AsSpec
        {
            get => _AsSpec ?? "ON";
            init => _AsSpec = value;
        }
        internal protected string? _AsSpec
        {
            get => __AsSpec;
            set => __AsSpec = value?.NotNullNotEmpty(trim: true);
        }
        string? __AsSpec = default!;

        /// <summary>
        /// If not null the condition of this join operation. If <c>null</c>, then it is ignored.
        /// </summary>
        public IDbToken? Condition
        {
            get => _Condition;
            init => _Condition = value;
        }
        internal protected IDbToken? _Condition { get; set; }

        /// <summary>
        /// The actual 'ON'-alike specification to use.
        /// </summary>
        public string? OnSpec
        {
            get => _OnSpec ?? "AS";
            init => _OnSpec = value;
        }
        internal protected string? _OnSpec
        {
            get => __OnSpec;
            set => __OnSpec = value?.NotNullNotEmpty(trim: true);
        }
        string? __OnSpec = default!;

        // ------------------------------------------------

        /// <inheritdoc/>
        public override string ToString()
        {
            var str = Body.ToString()!;

            str = $"{JoinType ?? "JOIN"} {str}";
            if (Alias is not null) str += $" {AsSpec} {Alias}";
            if (Condition is not null) str += $" {OnSpec} {Condition}";

            return str;
        }

        /// <inheritdoc/>
        public override ICommandInfo.IBuilder Visit(DbTokenVisitor visitor, bool separate)
        {
            var builder = visitor.Visit(Body);

            builder.ReplaceText($"{JoinType ?? "JOIN"} {builder.Text}");
            if (Alias is not null) builder.ReplaceText($"{builder.Text} {AsSpec} {Alias}");
            if (Condition is not null) builder.ReplaceText($"{builder.Text} {OnSpec} {Condition}");

            if (separate) builder.ReplaceText($" {builder.Text}");
            return builder;
        }
    }

    // ====================================================
    /// <summary>
    /// Represents the collection of fragments used to build JOIN-alike clauses.
    /// </summary>
    [Cloneable]
    public partial class Master : Fragment.Master
    {
        /// <summary>
        /// Initializes a new instance.
        /// </summary>
        /// <param name="command"></param>
        /// <param name="clause"></param>
        public Master(ICommand command, string clause = "JOIN") : base(command, clause) { }

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
            // Intercepting 1st-level 'x.Join(...)' method...
            string? jointype = null;
            var item = body.ExtractFirst(x =>
            {
                if (x is not DbTokenMethod method) return false;
                if (method.Host is not DbTokenArgument) return false;
                if (method.Name.ToUpper() is not "JOIN") return false;
                return true;
            }
            , out var removed); if (removed is not null)
            {
                var method = (DbTokenMethod)removed;

                if (method.TypeArguments.Length != 0) throw new ArgumentException(
                    $"No type arguments allowed for '{method.Name}()' virtual method.")
                    .WithData(body);

                if (method.Arguments.Count != 1) throw new ArgumentException(
                    $"'{method.Name}()' needs one and only one argument.")
                    .WithData(body);

                if (item is DbTokenArgument) throw new ArgumentException(
                    "Body cannot just be a join-type specification.")
                    .WithData(body);

                var visitor = Connection.Records.CreateDbTokenVisitor(Command.Locale);
                var arg = method.Arguments[0];
                jointype = visitor.TokenToLiteral(arg);
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

            // Intercepting '...On()' method...
            IDbToken? condition = null;
            string? onspec = null;
            item = body.ExtractLast(x =>
            {
                if (x is not DbTokenMethod method) return false;
                if (method.Name.ToUpper() is not "ON") return false;
                return true;
            }
            , out removed); if (removed is not null)
            {
                var method = (DbTokenMethod)removed;

                if (method.TypeArguments.Length != 0) throw new ArgumentException(
                    $"No type arguments allowed for '{method.Name}()' virtual method.")
                    .WithData(body);

                if (method.Arguments.Count != 1) throw new ArgumentException(
                    $"'{method.Name}()' needs one and only one argument.")
                    .WithData(body);

                if (item is DbTokenArgument) throw new ArgumentException(
                    $"Body after '{method.Name}' removal cannot be empty.")
                    .WithData(body);

                condition = method.Arguments[0];
                onspec = method.Name;
                body = item;
            }

            // Finishing...
            var entry = new Entry(this, body);
            if (jointype is not null) entry._JoinType = jointype;
            if (alias is not null) entry._Alias = alias;
            if (asspec is not null) entry._AsSpec = asspec;
            if (condition is not null) entry._Condition = condition;
            if (onspec is not null) entry._OnSpec = onspec;
            return entry;
        }
    }
}