#pragma warning disable IDE1006

namespace Yotei.ORM.Internals;

/// <summary>
/// Represents the ability of parsing ORDER BY clauses.
/// <br/>- Standard syntax: 'x => Element'.
/// <br/>- Alternate syntax: 'x => Element.Order()': ASCENDING, ASC, DESCENDING, DESC.
/// <br/>- Literal syntax: 'x => "Element"' or 'x => "Element Order"'
/// </summary>
/// <remarks>
/// ORDER BY clauses accept complex specifications:
/// <br/>- Example: 'ORDER BY CASE WHEN class IN('A', 'B') THEN 1 ELSE 2 END, ...'
/// </remarks>
public static partial class FragmentOrderBy
{
    // ====================================================
    /// <summary>
    /// Represents an entry in a collection of fragments used to build ORDER BY-alike clauses.
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
            // Literal...
            if (Body is DbTokenLiteral literal)
            {
                string? order = null;
                var main = literal.Value;
                
                var done = ExtractTail(ref main, ref order, false, "ASCENDING", "ASC", "DESCENDING", "DESC");
                if (done)
                {
                    if (main.Length == 0) throw new ArgumentException(
                        "Body cannot just be an order specification.")
                        .WithData(Body);

                    Body = new DbTokenLiteral(main);
                    Order = order.NullWhenEmpty();
                    return;
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
                string? order = null;
                var main = command.CommandInfo.Text;

                var done = ExtractTail(ref main, ref order, false, "ASCENDING", "ASC", "DESCENDING", "DESC");
                if (done)
                {
                    if (main.Length == 0) throw new ArgumentException(
                        "Body cannot just be an order specification.")
                        .WithData(Body);

                    var info = new CommandInfo(Engine, main, command.CommandInfo.Parameters);
                    Body = new DbTokenCommandInfo(info);
                    Order = order.NullWhenEmpty();
                    return;
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
        protected Entry(Entry source) : base(source) => Order = source.Order;

        /// <summary>
        /// If not null, the order specification.
        /// </summary>
        public string? Order
        {
            get => _Order;
            init => _Order = value;
        }
        internal protected string? _Order
        {
            get => __Order;
            set => __Order = value?.NotNullNotEmpty(trim: true);
        }
        string? __Order;

        // ------------------------------------------------

        /// <inheritdoc/>
        public override string ToString()
            => Order is null ? Body.ToString()! : $"{Body} {Order}";

        /// <inheritdoc/>
        public override ICommandInfo.IBuilder Visit(DbTokenVisitor visitor, bool separate)
        {
            var builder = visitor.Visit(Body);

            if (Order is not null) builder.ReplaceText($"{builder.Text} {Order}");
            if (separate) builder.ReplaceText($", {builder.Text}");
            return builder;
        }
    }

    // ====================================================
    /// <summary>
    /// Represents the collection of fragments used to build ORDER BY-alike clauses.
    /// </summary>
    [Cloneable]
    public partial class Master : Fragment.Master
    {
        /// <summary>
        /// Initializes a new instance.
        /// </summary>
        /// <param name="command"></param>
        public Master(ICommand command) : base(command, "ORDER BY") { }

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
            // Intercepting '...Order()' methods...
            string? order = null;
            var item = body.ExtractLast(x =>
            {
                if (x is not DbTokenMethod method) return false;
                if (method.Name.ToUpper() is not "ASC" and not "ASCENDING" and not "DESC" and not "DESCENDING") return false;
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

                order = method.Name;
                body = item;
            }

            // Finishing...
            var entry = new Entry(this, body);
            if (order is not null) entry._Order = order;
            return entry;
        }
    }
}
