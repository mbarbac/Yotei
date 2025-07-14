#pragma warning disable IDE0042
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
    /// Represents an entry in a collection of fragments used to build ORDERBY-alike clauses.
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
                var parts = Extractor.Tail(main, Engine, true, out var found, OrderSpecs);
                if (found)
                {
                    main = parts.Main.NotNullNotEmpty();

                    Body = new DbTokenLiteral(main);
                    Order = parts.Spec;
                    return;
                }
            }

            // Command-info...
            if (Body is DbTokenCommandInfo command)
            {
                var info = command.CommandInfo;
                var main = info.Text;
                var parts = Extractor.Tail(main, Engine, true, out var found, OrderSpecs);
                if (found)
                {
                    main = parts.Main.NotNullNotEmpty();
                    info = new CommandInfo(Engine, main, info.Parameters);

                    Body = new DbTokenCommandInfo(info);
                    Order = parts.Spec;
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

        readonly static string[] OrderSpecs = ["ASC", "ASCENDING", "DESC", "DESCENDING"];

        // ------------------------------------------------

        /// <inheritdoc/>
        public override string ToString() => Order is null ? Body.ToString()! : $"{Body} {Order}";

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
    /// Represents the collection of fragments used to build ORDERBY-alike clauses.
    /// </summary>
    [Cloneable]
    public partial class Master : Fragment.Master
    {
        /// <summary>
        /// Initializes a new instance.
        /// </summary>
        /// <param name="command"></param>
        /// <param name="clause"></param>
        public Master(ICommand command, string clause = "ORDERBY") : base(command, clause) { }

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
                    $"Body after '{method.Name}' removal cannot be empty.")
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