namespace Yotei.ORM.Internals;

/// <summary>
///  Represents the ability of parsing ORDER BY clauses.
/// <br/>- Standard syntax: 'x => Element'.
/// <br/>- Alternate syntax: 'x => Element.Ordering()', where 'Ordering' can be: ASCENDING,
/// ASC, DESCENDING, DESC.
/// </summary>
/// <remarks>
/// ORDER BY clauses accept complex specifications:
/// <br/>- Example: 'ORDER BY CASE WHEN class IN('A', 'B') THEN 1 ELSE 2 END, ...'
/// </remarks>
public static partial class FragmentOrderBy
{
    // ====================================================
    /// <summary>
    /// Represents an entry in a collection of fragments used to build a ORDER BY clause.
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

            string value;
            string main = literal.Value.Trim();

            Ordering = null;
            value = " ASC"; if (TryExtractLast(ref main, ref value, false)) Ordering = value;
            value = " ASCENDING"; if (TryExtractLast(ref main, ref value, false)) Ordering = value;
            value = " DESC"; if (TryExtractLast(ref main, ref value, false)) Ordering = value;
            value = " DESCENDING"; if (TryExtractLast(ref main, ref value, false)) Ordering = value;

            if (Ordering is not null) Body = new DbTokenLiteral(main);
        }

        /// <summary>
        /// Copy constructor.
        /// </summary>
        /// <param name="source"></param>
        protected Entry(Entry source) : base(source) => Ordering = source.Ordering;

        /// <summary>
        /// The order specification, or null if any.
        /// </summary>
        public string? Ordering
        {
            get => _Order;
            init => _Order = value?.NotNullNotEmpty();
        }
        internal string? _Order;

        // ------------------------------------------------

        /// <inheritdoc/>
        public override string ToString() => Ordering is null
            ? Body.ToString()!
            : $"{Body} {Ordering}";

        /// <inheritdoc/>
        protected override ICommandInfo.IBuilder VisitBody(DbTokenVisitor visitor)
        {
            var builder = visitor.Visit(Body);

            if (Ordering is not null) builder.ReplaceText($"{builder.Text} {Ordering}");
            return builder;
        }
    }

    // ====================================================
    /// <summary>
    /// Represents the collection of fragments used to build a ORDER BY clause.
    /// </summary>
    [Cloneable]
    public partial class Master : Fragment.Master
    {
        /// <inheritdoc/>
        public override string CLAUSE { get; set; } = "ORDER BY";

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
            // Intercepting 'Order(...)'...
            string? order = null;
            var item = body.RemoveFirst(x =>
            {
                if (x is not DbTokenMethod method) return false;
                if (method.Name.ToUpper()
                    is not "ASC" and not "ASCENDING"
                    and not "DESC" and not "DESCENDING") return false;

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
                    $"'{upper}()' virtual method must have no arguments.")
                    .WithData(body);

                if (item is DbTokenArgument) throw new ArgumentException(
                    $"Body after '{upper}()' cannot be empty.")
                    .WithData(body);

                order = upper;
                body = item;
            }

            // Finishing...
            var entry = new Entry(this, body);

            if (head is not null) entry._Head = head;
            if (tail is not null) entry._Tail = tail;
            if (order is not null) entry._Order = order;
            return entry;
        }

        // ------------------------------------------------

        /// <inheritdoc/>
        /// <remarks>We only need to provide a separator when the previous entry specifies a
        /// connector.</remarks>
        public override string? Separator(Fragment.Entry entry) => ", ";
    }
}
