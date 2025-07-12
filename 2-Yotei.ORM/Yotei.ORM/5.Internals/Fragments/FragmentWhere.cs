#pragma warning disable IDE0018
#pragma warning disable IDE1006

namespace Yotei.ORM.Internals;

/// <summary>
/// Represents the ability of parsing WHERE clauses.
/// <br/>- Standard syntax: 'x => Expression'.
/// <br/>- Alternate syntax: 'x => x.And()...' or 'x => x.And(...)'. Also with 'Or'
/// <br/>- Literal syntax: 'x => "Expression"' or 'x => "AND Expression"'. Also with 'OR'.
/// </summary>
public static partial class FragmentWhere
{
    // ====================================================
    /// <summary>
    /// Represents an entry in a collection of fragments used to build WHERE-alike clauses.
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
            // Setter translated into comparison...
            if (Body is DbTokenSetter setter)
            {
                var op = ExpressionType.Equal;
                Body = new DbTokenBinary(setter.Target, op, setter.Value);
                return;
            }

            // Literal...
            if (Body is DbTokenLiteral literal)
            {
                string? left, right, separator;
                string? connector = null;
                var main = literal.Value;

                if (ExtractHead(ref main, ref connector, false, "AND", "OR"))
                {
                    Body = new DbTokenLiteral(main);
                    Connector = connector.NullWhenEmpty();
                }

                if (ExtractSeparator(main, "!=", Engine, out left, out right, out _) ||
                    ExtractSeparator(main, "<>", Engine, out left, out right, out _))
                {
                    main = $"({left} <> {right})";
                    Body = new DbTokenLiteral(main);
                }
                else if (ExtractSeparator(main, ">=", Engine, out left, out right, out separator) ||
                    ExtractSeparator(main, ">", Engine, out left, out right, out separator) ||
                    ExtractSeparator(main, "<=", Engine, out left, out right, out separator) ||
                    ExtractSeparator(main, "<", Engine, out left, out right, out separator))
                {
                    main = $"({left} {separator} {right})";
                    Body = new DbTokenLiteral(main);
                }
                else if (ExtractSeparator(main, "==", Engine, out left, out right, out _) ||
                    ExtractSeparator(main, "=", Engine, out left, out right, out _))
                {
                    main = $"({left} = {right})";
                    Body = new DbTokenLiteral(main);
                }

                if (main.StartsWith(' ') || main.EndsWith(' '))
                {
                    main = main.NotNullNotEmpty(trim: true);
                    Body = new DbTokenLiteral(main);
                    return;
                }
            }

            // Command-info...
            if (Body is DbTokenCommandInfo command)
            {
                string? left, right, separator;
                string? connector = null;
                var main = command.CommandInfo.Text;

                if (ExtractHead(ref main, ref connector, false, "AND", "OR"))
                {
                    var info = new CommandInfo(Engine, main, command.CommandInfo.Parameters);
                    Body = new DbTokenCommandInfo(info);
                    Connector = connector.NullWhenEmpty();
                }

                if (ExtractSeparator(main, "!=", Engine, out left, out right, out _) ||
                    ExtractSeparator(main, "<>", Engine, out left, out right, out _))
                {
                    main = $"({left} <> {right})";
                    var info = new CommandInfo(Engine, main, command.CommandInfo.Parameters);
                    Body = new DbTokenCommandInfo(info);
                }
                else if (ExtractSeparator(main, ">=", Engine, out left, out right, out separator) ||
                    ExtractSeparator(main, ">", Engine, out left, out right, out separator) ||
                    ExtractSeparator(main, "<=", Engine, out left, out right, out separator) ||
                    ExtractSeparator(main, "<", Engine, out left, out right, out separator))
                {
                    main = $"({left} {separator} {right})";
                    var info = new CommandInfo(Engine, main, command.CommandInfo.Parameters);
                    Body = new DbTokenCommandInfo(info);
                }
                else if (ExtractSeparator(main, "==", Engine, out left, out right, out _) ||
                    ExtractSeparator(main, "=", Engine, out left, out right, out _))
                {
                    main = $"({left} = {right})";
                    var info = new CommandInfo(Engine, main, command.CommandInfo.Parameters);
                    Body = new DbTokenCommandInfo(info);
                }

                if (main.StartsWith(' ') || main.EndsWith(' '))
                {
                    main = main.NotNullNotEmpty(trim: true);
                    var info = new CommandInfo(Engine, main, command.CommandInfo.Parameters);
                    Body = new DbTokenCommandInfo(info);
                    return;
                }
            }

            // Any other token is just accepted, even empty ones...
            return;
        }

        /// <summary>
        /// Copy constructor.
        /// </summary>
        /// <param name="source"></param>
        protected Entry(Entry source) : base(source) => Connector = source.Connector;

        /// <summary>
        /// If not null, the AND or OR connector to use to separate the contents of this instance
        /// from any previous ones. If null, contents are just concatenated.
        /// </summary>
        public string? Connector
        {
            get => _Connector;
            init => _Connector = value;
        }
        internal protected string? _Connector
        {
            get => __Connector;
            set => __Connector = value?.NotNullNotEmpty(trim: true);
        }
        string? __Connector;

        // ------------------------------------------------

        /// <inheritdoc/>
        public override string ToString()
            => Connector is null ? Body.ToString()! : $"[{Connector}] {Body}";

        /// <inheritdoc/>
        public override ICommandInfo.IBuilder Visit(DbTokenVisitor visitor, bool separate)
        {
            var builder = visitor.Visit(Body);

            if (separate && Connector is not null)
                builder.ReplaceText($" {Connector} {builder.Text}");

            return builder;
        }
    }

    // ====================================================
    /// <summary>
    /// Represents the collection of fragments used to build WHERE-alike clauses.
    /// </summary>
    [Cloneable]
    public partial class Master : Fragment.Master
    {
        /// <summary>
        /// Initializes a new instance.
        /// </summary>
        /// <param name="command"></param>
        public Master(ICommand command, string clause = "WHERE") : base(command, clause) { }

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
            // Intercepting first-level 'x.And()' and 'x.Or()' methods...
            string? connector = null;
            var item = body.ExtractLast(x =>
            {
                if (x is not DbTokenMethod method) return false;
                if (method.Host is not DbTokenArgument) return false;
                if (method.Name.ToUpper() is not "AND" and not "OR") return false;
                return true;
            }
            , out var removed); if (removed is not null)
            {
                var method = (DbTokenMethod)removed;

                if (method.TypeArguments.Length != 0) throw new ArgumentException(
                    $"No type arguments allowed for '{method.Name}()' virtual method.")
                    .WithData(body);

                // x => x.Name()...
                if (method.Arguments.Count == 0)
                {
                    connector = method.Name;
                    body = item;
                }

                // x => x.Name(...)
                else if (method.Arguments.Count == 1)
                {
                    if (item is not DbTokenArgument) throw new ArgumentException(
                        $"Remaining after '{method.Name}(...)' must be empty.")
                        .WithData(body);

                    connector = method.Name;
                    body = method.Arguments[0];
                }

                // Others...
                else throw new ArgumentException(
                    $"'{method.Name}(...) must have just none or just one argument.")
                    .WithData(body);
            }

            // Finishing...
            var entry = new Entry(this, body);
            if (connector is not null) entry._Connector = connector;
            return entry;
        }
    }
}
