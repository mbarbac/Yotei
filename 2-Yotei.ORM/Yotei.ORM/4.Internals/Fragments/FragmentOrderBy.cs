namespace Yotei.ORM.Internals;

/// <summary>
/// Represents the ability of parsing ORDER BY clauses.
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
    /// Represents an entry in a collection of fragments used to build an ORDER BY clause.
    /// </summary>
    [Cloneable]
    public partial class Entry : Fragment.Entry
    {
        /// <inheritdoc/>
        public override string CLAUSE => "ORDER BY";

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

            string value;
            string main = text.Value.Trim();

            value = " ASC"; if (TryEnd(ref main, ref value)) Ordering = value;
            value = " ASCENDING"; if (TryEnd(ref main, ref value)) Ordering = value;            
            value = " DESC"; if (TryEnd(ref main, ref value)) Ordering = value;
            value = " DESCENDING"; if (TryEnd(ref main, ref value)) Ordering = value;

            Body = new DbTokenLiteral(main);
            return;

            static bool TryEnd(ref string source, ref string value)
            {
                var index = source.LastIndexOf(value, caseSensitive: false);
                if (index > 0 && (index + value.Length) == source.Length)
                {
                    source = source[..index];
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
            Body = source.Body.Clone();
            Ordering = source.Ordering;
        }

        /// <inheritdoc/>
        public override string ToString()
        {
            var str = Body.ToString()!;
            if (Ordering is not null) str += $" {Ordering}";
            return str;
        }

        /// <summary>
        /// The actual contents carried by this instance.
        /// </summary>
        public IDbToken Body { get; }

        /// <summary>
        /// The order specification, or null if any.
        /// </summary>
        public string? Ordering
        {
            get => _Order;
            init => _Order = value?.NotNullNotEmpty();
        }
        internal string? _Order;

        /// <inheritdoc/>
        public override ICommandInfo.IBuilder Visit(DbTokenVisitor visitor)
        {
            var builder = visitor.Visit(Body);

            if (Ordering is not null) builder.ReplaceText($"{builder.Text} {Ordering}");
            return builder;
        }
    }

    // ====================================================
    /// <summary>
    /// Represents the collection of fragments used to build an ORDER BY clause.
    /// </summary>
    [Cloneable]
    public partial class Master : Fragment.Master
    {
        /// <inheritdoc/>
        public override string CLAUSE => "ORDER BY";

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
            // Intercepting ordering specification...
            string? order = null;
            var item = body.RemoveLast(x =>
            {
                if (x is not DbTokenMethod method) return false;
                if (method.Name.ToUpper()
                    is not "ASC" and not "ASCENDING"
                    and not "DESC" and not "DESCENDING") return false;
                return true;
            },
            out var removed);

            if (removed is not null) // All() method found...
            {
                var method = (DbTokenMethod)removed;
                var upper = method.Name.ToUpper();

                if (method.TypeArguments.Length != 0) throw new ArgumentException(
                    $"No type arguments allowed for '{upper}()' virtual method.")
                    .WithData(body);

                if (method.Arguments.Count != 0) throw new ArgumentException(
                    $"'{upper}()' virtual method must have no arguments.")
                    .WithData(body);

                if (item is DbTokenArgument) throw new ArgumentException(
                    $"Body cannot just carry a '{upper}()' specification.")
                    .WithData(body);

                order = upper;
                body = item;
            }

            // Finishing...
            if (body is DbTokenInvoke invoke &&
                invoke.Host is DbTokenArgument &&
                invoke.Arguments.Count == 1 &&
                invoke.Arguments[0] is DbTokenLiteral literal) body = literal;

            var entry = new Entry(this, body);
            if (order is not null) entry._Order = order;
            return entry;
        }

        // ------------------------------------------------

        /// <inheritdoc/>
        public override string? Separator(Fragment.Entry _) => ", ";
    }
}
