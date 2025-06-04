namespace Yotei.ORM.Internal;

/// <summary>
/// Represents the ability of parsing COLUMN clauses.
/// <br/>- Standard syntax: 'x => Target = Value'.
/// </summary>
public partial class FragmentColumn
{
    // ====================================================
    /// <summary>
    /// Represents an entry in a collection of fragments used to build COLUMN clauses.
    /// </summary>
    [Cloneable]
    public partial class Entry : Fragment.Entry
    {
        /// <summary>
        /// Initializes a new instance.
        /// </summary>
        /// <param name="body"></param>
        public Entry(DbTokenLiteral body) : base(body) { }

        /// <summary>
        /// Initializes a new instance.
        /// </summary>
        /// <param name="body"></param>
        public Entry(DbTokenSetter body) : base(body)
        {
            if (body.Target is not DbTokenIdentifier) throw new ArgumentException(
                "Target of a COLUMN clause must be a valid identifier.")
                .WithData(body);
        }

        /// <summary>
        /// Copy constructor.
        /// </summary>
        /// <param name="source"></param>
        public Entry(Entry source) : base(source) { }

        /// <inheritdoc/>
        public override ICommandInfo.IBuilder Visit(
            DbTokenVisitor visitor, Fragment.EntryPosition position)
        {
            var builder = base.Visit(visitor, position);
            var str = builder.Text.UnWrap('(', ')');

            builder.ReplaceText(str);
            return builder;
        }
    }

    // ====================================================
    /// <summary>
    /// Represents a collection of fragments used to build COLUMN clauses.
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
        public Master(Master source) : base(source) { }

        // ------------------------------------------------

        /// <inheritdoc/>
        protected override Entry Validate(Fragment.Entry entry)
        {
            if (entry is not Entry valid) throw new ArgumentException(
                "Entry is not a valid HEAD or TAIL one.")
                .WithData(entry);

            return valid;
        }

        /// <inheritdoc/>
        protected override Entry Create(DbToken body)
        {
            return body switch
            {
                DbTokenLiteral item => new(item),
                DbTokenSetter item => new(item),

                _ => throw new ArgumentException(
                    "Specification does not resolve into a valid COLUMN clause.")
                    .WithData(body)
            };
        }

        // ------------------------------------------------

        /// <inheritdoc/>
        /// <remarks> The value of the '<paramref name="separator"/>' out argument is set to
        /// a comma by default, it it is null after invoking the base method. </remarks>
        protected override ICommandInfo.IBuilder OnVisit(
            Fragment.Entry entry,
            DbTokenVisitor visitor, Fragment.EntryPosition position, out string? separator)
        {
            var builder = base.OnVisit(entry, visitor, position, out separator);
            separator ??= ", ";
            return builder;
        }
    }
}