namespace Yotei.ORM.Internals;

/// <summary>
/// Represents the ability of parsing WHERE clauses.
/// <br/>- Standard syntax: 'x => ArbitraryCondition'.
/// <br/>- Alternate syntax: 'x => x.And(...)'.
/// <br/>- Alternate syntax: 'x => x.Or(...)'.
/// </summary>
public static partial class FragmentWhere
{
    // ====================================================
    /// <summary>
    /// Represents an entry in a collection of fragments used to build a WHERE clause.
    /// </summary>
    [Cloneable]
    public partial class Entry : Fragment.Entry
    {
        /// <inheritdoc/>
        public override string CLAUSE => "WHERE";

        /// <summary>
        /// Initializes a new instance.
        /// </summary>
        /// <param name="master"></param>
        /// <param name="body"></param>
        public Entry(Master master, IDbToken body) : base(master, body)
        {
            if (body is DbTokenLiteral literal)
            {
                string value;
                string main = literal.Value.Trim();

                value = "OR "; if (TryExtractFirst(ref main, ref value, false)) UseOR = true;
                value = "AND "; if (TryExtractFirst(ref main, ref value, false)) UseOR = false;

                if (UseOR is not null) Body = new DbTokenLiteral(main);
            }
        }

        /// <summary>
        /// Copy constructor.
        /// </summary>
        /// <param name="source"></param>
        protected Entry(Entry source) : base(source) => UseOR = source.UseOR;

        /// <summary>
        /// If not null, whether to chain this instance with a previous one using an "OR"
        /// connector, or a default "AND" one. If the value of this property is <c>null</c>,
        /// then it is ignored.
        /// </summary>
        public bool? UseOR
        {
            get => _UseOR;
            init => _UseOR = value;
        }
        internal bool? _UseOR; // internal, because 'init' is a bit too restrictive

        // ------------------------------------------------

        /// <inheritdoc/>
        public override string ToString() => UseOR is not null
            ? $"{(UseOR.Value ? "OR" : "AND")} {Body}"
            : $"{Body}";

        /// <inheritdoc/>
        protected override ICommandInfo.IBuilder VisitHead(DbTokenVisitor visitor)
        {
            var index = Master.IndexOf(this); // Connectors are for not-first elements...
            if (index > 0)
            {
                var str = UseOR is null ? string.Empty : (UseOR.Value ? "OR " : "AND ");
                var builder = new CommandInfo.Builder(Engine, str);

                if (Head is not null)
                {
                    var temp = visitor.Visit(Head);
                    builder.Add(temp);
                }
                return builder;
            }
            else // First elements do not need any kind of connector...
            {
                return base.VisitHead(visitor);
            }
        }
    }

    // ====================================================
    /// <summary>
    /// Represents the collection of fragments used to build a WHERE clause.
    /// </summary>
    [Cloneable]
    public partial class Master : Fragment.Master
    {
        /// <inheritdoc/>
        public override string CLAUSE => "WHERE";

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
            bool? useOr = null;

            // Intercepting 'x => x.And(...)' and 'x => x.Or(...)' methods...
            var item = body.RemoveFirst(x =>
            {
                if (x is not DbTokenMethod method) return false;
                if (method.Host is not DbTokenArgument) return false;
                if (method.Name.ToUpper() is not "AND" and not "OR") return false;
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

                if (method.Arguments.Count == 1)
                {
                    if (item is not DbTokenArgument) throw new ArgumentException(
                        $"Remaining after '{upper}(arg)' must be empty.")
                        .WithData(body);

                    if (upper == "OR") useOr = true;
                    else if (upper == "AND") useOr = false;
                    else useOr = null;

                    body = method.Arguments[0];
                }
                else
                {
                    throw new ArgumentException(
                        $"'{upper}(...) must have just one argument.")
                        .WithData(body);
                }
            }

            // Finishing...
            var entry = new Entry(this, body);

            if (head is not null) entry._Head = head;
            if (tail is not null) entry._Tail = tail;
            if (useOr is not null) entry._UseOR = useOr;
            return entry;
        }

        // ------------------------------------------------

        /// <inheritdoc/>
        /// <remarks>We only need to provide a separator when the previous entry specifies a
        /// connector.</remarks>
        public override string? Separator(Fragment.Entry entry)
        {
            var valid = (Entry)entry;
            var index = IndexOf(entry);

            if (index > 0 && valid.UseOR is not null) return " ";
            return null;
        }
    }
}
