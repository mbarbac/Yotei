namespace Yotei.ORM.Internals;

/// <summary>
/// Represents the ability of parsing TERMINAL clauses.
/// </summary>
public static partial class FragmentTerminal
{
    readonly static string CLAUSE = "TERMINAL";

    // ====================================================
    /// <summary>
    /// Represents an entry in a collection of fragments used to build a head or tail TERMINAL
    /// clause.
    /// </summary>
    [Cloneable]
    public partial class Entry : Fragment.Entry
    {
        /// <summary>
        /// Initializes a new instance.
        /// </summary>
        /// <param name="body"></param>
        public Entry(IDbToken body) => Body = body.ThrowWhenNull();

        /// <summary>
        /// Copy constructor.
        /// </summary>
        /// <param name="source"></param>
        protected Entry(Entry source) : this(source.Body) { }

        /// <inheritdoc/>
        public override string ToString() => Body.ToString()!;

        /// <summary>
        /// The actual contents carried by this instance.
        /// </summary>
        public IDbToken Body { get; }

        /// <inheritdoc/>
        public override ICommandInfo.IBuilder Visit(DbTokenVisitor visitor, bool first, bool last)
        {
            var builder = visitor.Visit(Body);
            return builder;
        }
    }

    // ====================================================
    /// <summary>
    /// Represents the collection of fragments used to build a head or tail TERMINAL clause.
    /// </summary>
    [Cloneable]
    public partial class Master : Fragment.Master
    {
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

        /// <inheritdoc/>
        public override Entry Create(IDbToken body) => new(body);

        // ------------------------------------------------

        /// <inheritdoc/>
        public override string? Separator => base.Separator;
    }
}