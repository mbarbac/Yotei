#pragma warning disable IDE1006

namespace Yotei.ORM.Internals;

/// <summary>
/// Represents the ability of parsing WHERE clauses.
/// <br/>- Standard syntax: 'x => Expression'.
/// <br/>- Alternate syntax: 'x => x.And(...)'.
/// <br/>- Alternate syntax: 'x => x.Or(...)'.
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
            // Setters translates into binary comparisons for convenience...
            if (Body is DbTokenSetter setter)
            {
                Body = new DbTokenBinary(setter.Target, ExpressionType.Equal, setter.Value);
                return;
            }

            // Literals...
            if (Body is DbTokenLiteral literal)
            {
                // Intercepting heading connector...
                var main = literal.Value;
                var str = main.Trim();
                var comparison = StringComparison.OrdinalIgnoreCase;

                if (!Extract("AND")) Extract("OR");
                bool Extract(string conn)
                {
                    if (str.StartsWith(conn, comparison))
                    {
                        _Connector = str[..conn.Length].Trim();
                        Body = new DbTokenLiteral(main = str[conn.Length..].Trim());
                        return true;
                    }
                    return false;
                }

                // Transforming 'Target==Value' into SQL's 'left=right'...
                var tokenizer = new StrWrappedTokenizer(Engine.LeftTerminator, Engine.RightTerminator);
                var items = Engine.UseTerminators ? tokenizer.Tokenize(main) : new StrTokenText(main);
                var (left, right) = items.ExtractFirst("==", false, out var found);
                if (found)
                {
                    main = left.ToString()!.Trim();
                    if (main.StartsWith('(') && !main.EndsWith(')')) main = main[1..];
                    if (!main.StartsWith('(') && main.EndsWith(')')) main = main[..^1];
                    main = main.NotNullNotEmpty(trim: true, $"Left {Clause} part.");

                    str = right!.ToString()!.Trim();
                    if (str.StartsWith('(') && !str.EndsWith(')')) str = str[1..];
                    if (!str.StartsWith('(') && str.EndsWith(')')) str = str[..^1];
                    str = str.NotNullNotEmpty(trim: true, $"Right {Clause} part.");

                    Body = new DbTokenLiteral($"({main} = {str})");
                    return;
                }

                // Intercepting 'left=right'...
                tokenizer = new StrWrappedTokenizer(Engine.LeftTerminator, Engine.RightTerminator);
                items = Engine.UseTerminators ? tokenizer.Tokenize(main) : new StrTokenText(main);
                (left, right) = items.ExtractFirst("=", false, out found);
                if (found)
                {
                    main = left.ToString()!.Trim();
                    if (main.StartsWith('(') && !main.EndsWith(')')) main = main[1..];
                    if (!main.StartsWith('(') && main.EndsWith(')')) main = main[..^1];
                    main = main.NotNullNotEmpty(trim: true, $"Left {Clause} part.");

                    str = right!.ToString()!.Trim();
                    if (str.StartsWith('(') && !str.EndsWith(')')) str = str[1..];
                    if (!str.StartsWith('(') && str.EndsWith(')')) str = str[..^1];
                    str = str.NotNullNotEmpty(trim: true, $"Right {Clause} part.");

                    Body = new DbTokenLiteral($"({main} = {str})");
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
        /// If not null, the AND or OR trimmed connector to use to separate the contents of this
        /// instance with previous ones, if any, provided this is not the first one in its master
        /// collection.
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
        {
            return Connector is not null ? $"[{Connector}] {Body}" : Body.ToString()!;
        }

        /// <inheritdoc/>
        /// <remarks>
        /// The <see cref="Connector"/> property goberns the separation of this instance from
        /// any previous ones, so we deal with this complexity here, and the master's separator
        /// producer will always return null.
        /// </remarks>
        public override ICommandInfo.IBuilder Visit(DbTokenVisitor visitor)
        {
            var builder = visitor.Visit(Body);
            var empty = builder.TextLen == 0;

            if (Connector is not null)
            {
                var index = Master.IndexOf(this);

                if (empty) // Stand-alone connector...
                {
                    builder.ReplaceText(index == 0
                        ? $"{Connector} "
                        : $" {Connector} ");
                }
                else // Connector with contents
                {
                    if (index > 0) builder.ReplaceText($" {Connector} {builder.Text}");
                }
            }

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
        /// <param name="clause"></param>
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
            var item = body.ExtractFirst(x =>
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
                    body = item is DbTokenArgument ? DbTokenLiteral.Empty : item;
                }

                // x => x.Name(...)
                else if (method.Arguments.Count == 1)
                {
                    if (item is not DbTokenArgument) throw new ArgumentException(
                        $"Remaining after '{method.Name}()' must be empty.")
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