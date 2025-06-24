namespace Yotei.ORM.Internals;

/// <summary>
/// Represents the ability of parsing JOIN clauses.
/// <br/>- Standard syntax: 'x => x.Source.As(...).On(...)'.
/// <br/>- Alternate syntax: 'x => x.JoinType(join-type).Source.As(...).On(...)'.
/// </summary>
public static partial class FragmentJoin
{
    // ====================================================
    /// <summary>
    /// Represents an entry in a collection of fragments used to build a JOIN clause.
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

            string? temp;
            string main = literal.Value.Trim();

            var parts = main.ExtractLeftRight(" ON ", sensitive: false, out var found);
            if (found)
            {
                main = parts.Left;
                temp = parts.Right?.NotNullNotEmpty();
                Condition = new DbTokenLiteral(temp!);
            }
            if (main.EndsWith(" ON", StringComparison.OrdinalIgnoreCase))
                throw new ArgumentException("Body cannot end with 'ON'").WithData(body);

            parts = main.ExtractLeftRight(" AS ", sensitive: false, out found);
            if (found)
            {
                main = parts.Left;
                temp = parts.Right?.NotNullNotEmpty();
                Alias = temp;
            }
            if (main.EndsWith(" AS", StringComparison.OrdinalIgnoreCase))
                throw new ArgumentException("Body cannot end with 'AS'").WithData(body);

            main = main.Trim();
            temp = "JOIN "; if (TryExtractFirst(ref main, ref temp, false)) JoinType = temp;
            temp = "INNER JOIN "; if (TryExtractFirst(ref main, ref temp, false)) JoinType = temp;
            temp = "LEFT JOIN "; if (TryExtractFirst(ref main, ref temp, false)) JoinType = temp;
            temp = "RIGHT JOIN "; if (TryExtractFirst(ref main, ref temp, false)) JoinType = temp;
            temp = "OUTER JOIN "; if (TryExtractFirst(ref main, ref temp, false)) JoinType = temp;
            temp = "SELF JOIN "; if (TryExtractFirst(ref main, ref temp, false)) JoinType = temp;
            temp = "CROSS JOIN "; if (TryExtractFirst(ref main, ref temp, false)) JoinType = temp;
            temp = "FULL JOIN "; if (TryExtractFirst(ref main, ref temp, false)) JoinType = temp;
            temp = "FULL OUTER JOIN "; if (TryExtractFirst(ref main, ref temp, false)) JoinType = temp;
            temp = "LEFT OUTER JOIN "; if (TryExtractFirst(ref main, ref temp, false)) JoinType = temp;
            temp = "RIGHT OUTER JOIN "; if (TryExtractFirst(ref main, ref temp, false)) JoinType = temp;

            if (main.Length == 0) throw new ArgumentException(
                "Join expression after join type specification cannot be empty.")
                .WithData(Body);

            Body = new DbTokenLiteral(main);
        }

        /// <summary>
        /// Copy constructor.
        /// </summary>
        /// <param name="source"></param>
        protected Entry(Entry source) : base(source)
        {
            JoinType = source.JoinType;
            Alias = source.Alias;
            Condition = source.Condition?.Clone();
        }

        /// <summary>
        /// The join-alike operation used by this instance, or <c>null</c> to use the default
        /// 'JOIN' one.
        /// </summary>
        public string? JoinType
        {
            get => _JoinType;
            init => _JoinType = value?.NotNullNotEmpty();
        }
        internal string? _JoinType;

        /// <summary>
        /// The alias that qualifies the source (Body), or null if any.
        /// </summary>
        public string? Alias
        {
            get => _Alias;
            init => _Alias = value?.NotNullNotEmpty();
        }
        internal string? _Alias;

        /// <summary>
        /// The condition of the join-alike operation, or <c>null</c> if any.
        /// </summary>
        public IDbToken? Condition
        {
            get => _Condition;
            init => _Condition = value.ThrowWhenNull();
        }
        internal IDbToken? _Condition;

        // ------------------------------------------------

        /// <inheritdoc/>
        public override string ToString()
        {
            var sb = new StringBuilder();

            sb.Append(JoinType is null ? "JOIN " : $"{JoinType} ");
            sb.Append(Body.ToString());
            if (Alias is not null) sb.Append($" AS {Alias}");
            if (Condition is not null) sb.Append($" ON {Condition}");

            return sb.ToString();
        }

        /// <inheritdoc/>
        protected override ICommandInfo.IBuilder VisitBody(DbTokenVisitor visitor)
        {
            var builder = visitor.Visit(Body);

            var str = JoinType is null ? "JOIN" : JoinType;
            builder.ReplaceText($"{str} {builder.Text}");

            if (Alias is not null) builder.ReplaceText($"{builder.Text} AS {Alias}");

            if (Condition is not null)
            {
                var temp = visitor.Visit(Condition);
                var xstr = temp.Text.UnWrap('(', ')');
                temp.ReplaceText(xstr);

                builder.ReplaceText($"{builder.Text} ON (");
                builder.Add(temp);
                builder.Add(")");
            }

            return builder;
        }
    }

    // ====================================================
    /// <summary>
    /// Represents the collection of fragments used to build a JOIN clause.
    /// </summary>
    [Cloneable]
    public partial class Master : Fragment.Master
    {
        /// <inheritdoc/>
        public override string CLAUSE { get; set; } = "JOIN";

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
            // Intercepting 'JoinType(type)'...
            string? joinType = null;
            var item = body.RemoveFirst(x =>
            {
                if (x is not DbTokenMethod method) return false;
                if (method.Host is not DbTokenArgument) return false;
                if (method.Name.ToUpper() is not "JOINTYPE") return false;
                return true;
            }
            , out var removed);

            if (removed is not null) // Found...
            {
                var method = (DbTokenMethod)removed;
                var name = method.Name;
                var upper = name.ToUpper();

                if (method.TypeArguments.Length != 0) throw new ArgumentException(
                    $"No type arguments allowed for '{upper}(...)' virtual method.")
                    .WithData(body);

                if (method.Arguments.Count != 1) throw new ArgumentException(
                    $"'{upper}(type)' requieres one and only one argument..")
                    .WithData(body);

                if (item is DbTokenArgument) throw new ArgumentException(
                    $"Body after '{upper}(type)' cannot be empty.")
                    .WithData(body);

                var visitor = Connection.Records.CreateDbTokenVisitor(Command.Locale);
                var arg = method.Arguments[0];
                joinType = visitor.TokenToLiteral(arg);
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
                    $"No type arguments allowed for '{upper}(...)' virtual method.")
                    .WithData(body);

                if (item is DbTokenArgument) throw new ArgumentException(
                    $"Body after '{upper}(type)' cannot be empty.")
                    .WithData(body);

                var visitor = Connection.Records.CreateDbTokenVisitor(Command.Locale);
                alias = visitor.ChainToAlias(method.Arguments);
                body = item;
            }

            // Intercepting 'On(...)'...
            IDbToken? condition = null;
            item = body.RemoveFirst(x =>
            {
                if (x is not DbTokenMethod method) return false;
                if (method.Name.ToUpper() is not "ON") return false;
                return true;
            }
            , out removed);

            if (removed is not null) // Found...
            {
                var method = (DbTokenMethod)removed;
                var name = method.Name;
                var upper = name.ToUpper();

                if (method.TypeArguments.Length != 0) throw new ArgumentException(
                    $"No type arguments allowed for '{upper}(...)' virtual method.")
                    .WithData(body);

                if (method.Arguments.Count != 1) throw new ArgumentException(
                    $"'{upper}(type)' requieres one and only one argument..")
                    .WithData(body);

                if (item is DbTokenArgument) throw new ArgumentException(
                    $"Body after '{upper}(type)' cannot be empty.")
                    .WithData(body);

                condition = method.Arguments[0];
                body = item;
            }

            // Finishing...
            var entry = new Entry(this, body);

            if (head is not null) entry._Head = head;
            if (tail is not null) entry._Tail = tail;
            if (joinType is not null) entry._JoinType = joinType;
            if (alias is not null) entry._Alias = alias;
            if (condition is not null) entry._Condition = condition;
            return entry;
        }

        // ------------------------------------------------

        /// <inheritdoc/>
        /// <remarks>We only need to provide a separator when the previous entry specifies a
        /// connector.</remarks>
        public override string? Separator(Fragment.Entry entry) => " ";
    }
}
