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
        /// <inheritdoc/>
        public override string CLAUSE => "JOIN";

        /// <summary>
        /// Initializes a new instance.
        /// </summary>
        /// <param name="master"></param>
        /// <param name="body"></param>
        public Entry(Master master, IDbToken body) : base(master)
        {
            body.ThrowWhenNull();

            if (body is not DbTokenLiteral text)
            {
                Body = body.ThrowWhenNull();
                return;
            }

            string main = text.Value;
            string? temp;

            var parts = main.ExtractLeftRight(" ON ", sensitive: false, out var found);
            if (found)
            {
                main = parts.Left;
                temp = parts.Right?.NotNullNotEmpty();

                Condition = new DbTokenLiteral(temp!);
            }

            parts = main.ExtractLeftRight(" AS ", sensitive: found, out found);
            if (found)
            {
                main = parts.Left;
                temp = parts.Right?.NotNullNotEmpty();

                Alias = temp;
            }

            main = main.Trim();
            var value = "JOIN "; if (TryStart(ref main, ref value)) JoinType = value;
            value = "LEFT JOIN "; if (TryStart(ref main, ref value)) JoinType = value;
            value = "RIGHT JOIN "; if (TryStart(ref main, ref value)) JoinType = value;
            value = "INNER JOIN "; if (TryStart(ref main, ref value)) JoinType = value;
            value = "OUTER JOIN "; if (TryStart(ref main, ref value)) JoinType = value;
            value = "SELF JOIN "; if (TryStart(ref main, ref value)) JoinType = value;
            value = "CROSS JOIN "; if (TryStart(ref main, ref value)) JoinType = value;
            value = "FULL OUTER JOIN "; if (TryStart(ref main, ref value)) JoinType = value;
            value = "LEFT OUTER JOIN "; if (TryStart(ref main, ref value)) JoinType = value;
            value = "RIGHT OUTER JOIN "; if (TryStart(ref main, ref value)) JoinType = value;

            Body = new DbTokenLiteral(main);
            return;

            static bool TryStart(ref string source, ref string value)
            {
                var index = source.IndexOf(value, caseSensitive: false);
                if (index == 0)
                {
                    var len = value.Length;
                    source = source[len..];

                    value = value.Trim();
                    return true;
                }
                return false;
            }
        }

        /// <summary>
        /// Copy constructor.
        /// </summary>
        /// <param name="source"></param>
        protected Entry(Entry source) : base(source)
        {
            JoinType = source.JoinType;
            Body = source.Body.Clone();
            Alias = source.Alias;
            Condition = source.Condition?.Clone();
        }

        /// <inheritdoc/>
        public override string ToString()
        {
            var sb = new StringBuilder();

            sb.Append(JoinType is not null ? $"{JoinType} " : "JOIN ");
            sb.Append(Body.ToString());
            if (Alias is not null) sb.Append($" AS {Alias}");
            if (Condition is not null) sb.Append($" ON ({Condition})");

            return sb.ToString();
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
        /// The actual contents carried by this instance.
        /// </summary>
        public IDbToken Body { get; }

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

        /// <inheritdoc/>
        public override ICommandInfo.IBuilder Visit(DbTokenVisitor visitor)
        {
            var builder = visitor.Visit(Body);

            var str = JoinType is not null ? $"{JoinType} " : "JOIN ";
            builder.ReplaceText($"{str}{builder.Text}");

            if (Alias is not null) builder.ReplaceText($"{builder.Text} AS {Alias}");

            if (Condition is not null)
            {
                var temp = visitor.Visit(Condition);

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
        public override string CLAUSE => "JOIN";

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
            // Intercepting JoinType...
            string? joinType = null;
            var item = body.RemoveFirst(x =>
            {
                if (x is not DbTokenMethod method) return false;
                if (method.Name.ToUpper() is not "JOINTYPE") return false;
                return true;
            }
            , out var removed);

            if (removed is not null) // JoinType(...) found...
            {
                var method = (DbTokenMethod)removed;
                if (method.TypeArguments.Length != 0) throw new ArgumentException(
                    "No type arguments allowed for 'JOINTYPE(...)' virtual method.")
                    .WithData(body);

                if (method.Arguments.Count != 1) throw new ArgumentException(
                    "JOINTYPE(...) requires one and only one argument.")
                    .WithData(body);

                if (item is DbTokenArgument) throw new ArgumentException(
                    "Body cannot just carry a 'JOINTYPE(...)' specification.")
                    .WithData(body);

                var visitor = Connection.Records.CreateDbTokenVisitor(Command.Locale);
                var arg = method.Arguments[0];
                joinType = visitor.TokenToLiteral(arg);
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
                    "No type arguments allowed for 'AS(...)' virtual method.")
                    .WithData(body);

                if (item is DbTokenArgument) throw new ArgumentException(
                    "Body cannot just carry an 'AS()' specification.")
                    .WithData(body);

                var visitor = Connection.Records.CreateDbTokenVisitor(Command.Locale);
                alias = visitor.ChainToAlias(method.Arguments);
                body = item;
            }

            // Intercepting condition...
            IDbToken? condition = null;
            item = body.RemoveLast(x =>
            {
                if (x is not DbTokenMethod method) return false;
                if (method.Name.ToUpper() is not "ON") return false;
                return true;
            }
            , out removed);

            if (removed is not null) // On(...) method found...
            {
                var method = (DbTokenMethod)removed;
                if (method.TypeArguments.Length != 0) throw new ArgumentException(
                    "No type arguments allowed for 'ON(...)' virtual method.")
                    .WithData(body);

                if (method.Arguments.Count != 1) throw new ArgumentException(
                    "ON(...) requires one and only one argument.")
                    .WithData(body);

                if (item is DbTokenArgument) throw new ArgumentException(
                    "Body cannot just carry an 'ON()' specification.")
                    .WithData(body);

                condition = method.Arguments[0];
                body = item;
            }

            // Finishing...
            if (body is DbTokenInvoke invoke &&
                invoke.Host is DbTokenArgument &&
                invoke.Arguments.Count == 1 &&
                invoke.Arguments[0] is DbTokenLiteral literal) body = literal;

            var entry = new Entry(this, body);
            if (joinType is not null) entry._JoinType = joinType;
            if (alias is not null) entry._Alias = alias;
            if (condition is not null) entry._Condition = condition;
            return entry;
        }

        // ------------------------------------------------

        /// <inheritdoc/>
        public override string? Separator(Fragment.Entry _) => " ";
    }
}
