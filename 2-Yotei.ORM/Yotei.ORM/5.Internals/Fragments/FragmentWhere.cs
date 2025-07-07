#pragma warning disable IDE1006

using System.Net.Sockets;

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
            // Setters translated to binary comparisons, for convenience...
            if (Body is DbTokenSetter setter)
            {
                Body = new DbTokenBinary(setter.Target, ExpressionType.Equal, setter.Value);
                return;
            }

            // Literals...
            if (Body is DbTokenLiteral literal)
            {
                var main = literal.Value;

                // Intercepting a heading connector...
                string conn;
                var str = main.Trim();
                var comparison = StringComparison.OrdinalIgnoreCase;

                conn = "AND ";
                if (str.StartsWith(conn, comparison))
                {
                    Connector = str[..conn.Length];
                    main = str[conn.Length..];
                    Body = new DbTokenLiteral(main);
                }
                conn = "AND";
                if (str.Equals(conn, comparison))
                {
                    Connector = str;
                    Body = DbTokenLiteral.Empty;
                    return;
                }
                conn = "OR ";
                if (str.StartsWith(conn, comparison))
                {
                    Connector = str[..conn.Length];
                    main = str[conn.Length..];
                    Body = new DbTokenLiteral(main);
                }
                conn = "OR";
                if (str.Equals(conn, comparison))
                {
                    Connector = str;
                    Body = DbTokenLiteral.Empty;
                    return;
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

            // Any other token is just accepted..
            return;
        }

        /// <summary>
        /// Copy constructor.
        /// </summary>
        /// <param name="source"></param>
        protected Entry(Entry source) : base(source) => Connector = source.Connector;

        /// <summary>
        /// If not null, the trimmed connector used to separate the contents of this entry from
        /// any previous one, if neeed.
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
        public override string ToString() => Connector is not null
            ? $"{Connector} {Body}"
            : Body.ToString()!;

        /// <inheritdoc/>
        public override ICommandInfo.IBuilder Visit(DbTokenVisitor visitor)
        {
            var builder = visitor.Visit(Body);

            if (Connector is not null) builder.ReplaceText(
                builder.TextLen > 0 ? $"{Connector} {builder.Text}" : Connector);

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

        /// <inheritdoc/>
        public override void Add(Fragment.Entry entry)
        {
            var valid = Validate(entry);
            if (valid.Connector is null && Count > 0)
            {
                var index = Count - 1;
                var temp = (Entry)this[index];
                if (temp.Connector is not null && temp.Body.ToString()!.Length == 0)
                {
                    valid._Connector = temp.Connector;
                    RemoveAt(index);
                }
            }

            base.Add(valid);
        }

        /// <inheritdoc/>
        public override void Insert(int index, Fragment.Entry entry)
        {
            var valid = Validate(entry);
            if (valid.Connector is null && Count > 0 && index > 0)
            {
                var temp = (Entry)this[index - 1];
                if (temp.Connector is not null && temp.Body.ToString()!.Length == 0)
                {
                    valid._Connector = temp.Connector;
                    RemoveAt(index);
                }
            }

            base.Insert(index, entry);
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
                var upper = method.Name.ToUpper();

                if (method.TypeArguments.Length != 0) throw new ArgumentException(
                    $"No type arguments allowed for '{upper}()' virtual method.")
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
                        $"Remaining after '{upper}()' must be empty.")
                        .WithData(body);

                    connector = method.Name;
                    body = method.Arguments[0];
                }

                // Others...
                else throw new ArgumentException(
                    $"'{upper}(...) must have just none or just one argument.")
                    .WithData(body);
            }

            // Finishing...
            var entry = new Entry(this, body);
            if (connector is not null) entry._Connector = connector;
            return entry;
        }

        // ------------------------------------------------

        /// <inheritdoc/>
        protected override string? EntrySeparator(Fragment.Entry entry)
        {
            var valid = (Entry)entry;
            var index = IndexOf(entry);

            if (index > 0 && valid.Connector is not null) return " ";
            return null;
        }
    }
}