namespace Yotei.ORM.Internals;

/// <summary>
/// Represents the ability of parsing WHERE clauses.
/// <br/>- Standard syntax: 'x => Expression'.
/// <br/>- Alternate syntax: 'x => x.And(...)'.
/// <br/>- Alternate syntax: 'x => x.Or(...)'.
/// </summary>
/// <remarks>
/// A not-first entry without a proper AND or OR connector may be invalid SQL syntax.
/// </remarks>
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
            if (body is DbTokenLiteral literal)
            {
                // Invalid literals not intercepted afterwards...
                string main = literal.Value;
                if (main.EndsWith(" OR", StringComparison.OrdinalIgnoreCase) ||
                    main.EndsWith(" AND", StringComparison.OrdinalIgnoreCase))
                    throw new EmptyException("Source ends with a connector.").WithData(main);

                // Extracting connector from literal sources, if any...
                if (main.Length > 0 && main[0] == ' ') main = main[1..];

                string connector;
                UseOR = null;
                connector = "OR "; if (ExtractFirst(ref main, ref connector, false)) UseOR = true;
                connector = "AND "; if (ExtractFirst(ref main, ref connector, false)) UseOR = false;
                if (UseOR is not null) main = main.NotNullNotEmpty();

                // Transforming 'target==value' into proper 'target=value' SQL...
                var (target, value) = main.ExtractLeftRight("==", Engine, out var found);
                if (found)
                {
                    target = Adjust(target);
                    value = Adjust(value!);
                    main = $"({target} = {value})";

                    string Adjust(string source)
                    {
                        if (source is not null)
                        {
                            if (source.StartsWith('(') && !source.EndsWith(')')) source = source[1..];
                            if (!source.StartsWith('(') && source.EndsWith(')')) source = source[..^1];
                        }
                        return source.NotNullNotEmpty(trim: true);
                    }
                }

                // Finalizing...
                if (UseOR is not null || found) Body = new DbTokenLiteral(main);
            }

            // Using 'target=value' instead of the right 'target==value' one...
            else if (body is DbTokenSetter setter)
            {
                Body = new DbTokenBinary(setter.Target, ExpressionType.Equal, setter.Value);
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
        internal bool? _UseOR;

        // ------------------------------------------------

        /// <inheritdoc/>
        public override string ToString() => UseOR is not null
            ? $"{(UseOR.Value ? "OR" : "AND")} {Body}"
            : $"{Body}";

        /// <inheritdoc/>
        public override ICommandInfo.IBuilder Visit(DbTokenVisitor visitor)
        {
            var builder = visitor.Visit(Body);

            if (UseOR is not null)
            {
                var str = UseOR.Value ? "OR" : "AND";
                builder.ReplaceText($"{str} {builder.Text}");
            }
            return builder;
        }
    }

    // ====================================================
    /// <summary>
    /// Represents a collection of fragments used to build WHERE-alike clauses.
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
            // Intercepting first-level 'x => x.And(...)' and 'x => x.Or(...)' methods...
            bool? useOr = null;
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
                    $"No type arguments allowed for '{upper}()' virtual method.")
                    .WithData(body);

                if (method.Arguments.Count == 1)
                {
                    if (item is not DbTokenArgument) throw new ArgumentException(
                        $"Remaining after '{upper}()' must be empty.")
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
            if (useOr is not null) entry._UseOR = useOr;
            return entry;
        }

        // ------------------------------------------------

        /// <inheritdoc/>
        /// <remarks>If the entry has defined a connector, then injects a space to allow that
        /// connector to be represented in isolation. Otherwise returns null to just aggregate
        /// the entry's contents to any previous ones.</remarks>
        protected override string? EntrySeparator(Fragment.Entry entry)
        {
            var valid = (Entry)entry;
            var index = IndexOf(valid);

            if (index > 0 && valid.UseOR is not null) return " ";
            return null;
        }
    }
}