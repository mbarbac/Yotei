namespace Yotei.ORM.Internals;

/// <summary>
/// Represents a FROM fragment.
/// <br/>- Standard syntax: 'x => x.Source'.
/// <br/>- Alternate syntax: 'x => x.Source.As(...)'.
/// <br/>- Complex: 'FROM (SELECT [Id], [Age] FROM Other) AS Another, ...'
/// </summary>
public static class FragmentFrom
{
    // ====================================================
    /// <summary>
    /// Represents an entry in a collection of fragments used to build a FROM-alike clause.
    /// </summary>
    public class Entry : Fragment.Entry
    {
        /// <summary>
        /// Initializes a new instance.
        /// </summary>
        /// <param name="master"></param>
        /// <param name="token"></param>
        public Entry(Master master, IDbToken token) : base(master, token) { }

        /// <summary>
        /// Copy constructor.
        /// </summary>
        /// <param name="source"></param>
        protected Entry(Entry source) : base(source)
        {
            Alias = source.Alias;
            AsLiteral = source.AsLiteral;
        }

        /// <inheritdoc/>
        public override Entry Clone() => new(this);

        // ------------------------------------------------

        /// <inheritdoc/>
        protected override IDbToken ParseBody(IDbToken token)
        {
            return base.ParseBody(token);
        }

        // ------------------------------------------------

        /// <summary>
        /// The alias used by this instance, if any.
        /// </summary>
        public string? Alias
        {
            get => _Alias;
            init => SetAlias(value);
        }
        string? _Alias = null;

        // Sets the value of the 'Alias' property.
        internal protected void SetAlias(
            string? value) => _Alias = value?.NotNullNotEmpty(trim: true);

        /// <summary>
        /// The actual 'AS' literal used by this instance, if any.
        /// </summary>
        public string? AsLiteral
        {
            get => _AsLiteral;
            init => SetAsLiteral(value);
        }
        string? _AsLiteral = null;

        // Sets the value of the 'As' property.
        internal protected void SetAsLiteral(
            string? value) => _AsLiteral = value?.NotNullNotEmpty(trim: true);

        // ------------------------------------------------

        /// <inheritdoc/>
        public override string ToString()
        {
            var str = Body.ToString()!;

            if (Alias is not null) str += $"{AsLiteral} {Alias}";
            return str;
        }

        /// <inheritdoc/>
        public override ICommandInfo.IBuilder Visit(DbTokenVisitor visitor, bool separate)
        {
            var builder = visitor.Visit(Body);

            if (Alias is not null) builder.ReplaceText($"{builder.Text} {AsLiteral} {Alias}");
            if (separate) builder.ReplaceText($", {builder.Text}");
            return builder;
        }
    }

    // ====================================================
    /// <summary>
    /// Represents a collection of fragments used to build a FROM-alike clause.
    /// </summary>
    public class Master : Fragment.Master
    {
        /// <summary>
        /// Initializes a new instance.
        /// </summary>
        /// <param name="command"></param>
        public Master(ICommand command, string descriptor = "FROM")
            : base(command, descriptor) { }

        /// <summary>
        /// Copy constructor.
        /// </summary>
        /// <param name="source"></param>
        protected Master(Master source) : base(source) { }

        /// <inheritdoc/>
        public override Master Clone() => new(this);

        // ------------------------------------------------

        protected override Entry Validate(Fragment.Entry entry)
        {
            if (entry is not Entry valid) throw new ArgumentException(
                $"Entry is not a valid '{Descriptor}' one.")
                .WithData(entry);

            return valid;
        }

        // ------------------------------------------------

        /// <inheritdoc/>
        public override Entry CreateEntry(IDbToken token)
        {
            throw null;
        }
    }
}