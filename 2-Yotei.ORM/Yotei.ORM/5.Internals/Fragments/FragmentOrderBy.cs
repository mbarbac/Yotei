#pragma warning disable IDE1006

namespace Yotei.ORM.Internals;

/// <summary>
/// Represents the ability of parsing ORDER BY clauses.
/// <br/>- Standard syntax: 'x => Element'.
/// <br/>- Alternate syntax: 'x => x.Element.Ordering()' (ASCENDING, ASC, DESCENDING, DESC).
/// <br/>- Literal syntax: 'x => x("Element")'.
/// <br/>- Literal syntax: 'x => x("Element Ordering")'.
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
            // Literals, intercepting ordering spec...
            if (Body is DbTokenLiteral literal)
            {
                var main = literal.Value;
                var str = main.Trim();
                var comparison = StringComparison.OrdinalIgnoreCase;

                bool Extract(string spec)
                {
                    var index = str.LastIndexOf(spec, comparison);
                    if (index >= 0 && (index + spec.Length) == str.Length)
                    {
                        _Ordering = str[index..].Trim();
                        main = str[..index].Trim();
                        return true;
                    }
                    spec = " " + spec;
                    index = str.LastIndexOf(spec, comparison);
                    if (index >= 0 && (index + spec.Length) == str.Length)
                    {
                        _Ordering = str[index..].Trim();
                        main = str[..index].Trim();
                        return true;
                    }
                    return false;
                }
                var done = Extract("ASCENDING") || Extract("ASC") || Extract("DESCENDING") || Extract("DESC");
                if (done) Body = main.Length == 0 ? DbTokenLiteral.Empty : new DbTokenLiteral(main);
            }

            // Any other token is just accepted, even empty ones...
            return;
        }

        /// <summary>
        /// Copy constructor.
        /// </summary>
        /// <param name="source"></param>
        protected Entry(Entry source) : base(source) => Ordering = source.Ordering;

        /// <summary>
        /// If not null, the AND or OR trimmed connector to use to separate the contents of this
        /// instance with previous ones, if any, provided this is not the first one in its master
        /// collection.
        /// </summary>
        public string? Ordering
        {
            get => _Ordering;
            init => _Ordering = value;
        }
        internal protected string? _Ordering
        {
            get => __Ordering;
            set => __Ordering = value?.NotNullNotEmpty(trim: true);
        }
        string? __Ordering;

        // ------------------------------------------------

        /// <inheritdoc/>
        public override string ToString()
        {
            return Ordering is null ? Body.ToString()! : $"{Body} {Ordering}";
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

            if (Ordering is not null)
                builder.ReplaceText(empty ? Ordering : $"{builder.Text} {Ordering}");

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
        /// <param name="clause"></param>
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

                order = method.Name;
                body = item is DbTokenArgument ? DbTokenLiteral.Empty : item;
            }

            // Finishing...
            var entry = new Entry(this, body);
            if (order is not null) entry._Ordering = order;
            return entry;
        }

        // ------------------------------------------------

        /// <inheritdoc/>
        protected override string? EntrySeparator(Fragment.Entry entry) => ", ";
    }
}