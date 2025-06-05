namespace Yotei.ORM.Internal;

/// <summary>
/// Represents the ability of parsing HEAD or TAIL clauses.
/// <br/>- Any arbitrary syntax is accepted.
/// </summary>
public static partial class FragmentTerminal
{
    // ====================================================
    /// <summary>
    /// Represents an entry in a collection of fragments used to build HEAD or TAIL clauses.
    /// </summary>
    [Cloneable]
    public partial class Entry : Fragment.Entry
    {
        /// <summary>
        /// Initializes a new instance.
        /// </summary>
        /// <param name="body"></param>
        public Entry(DbToken body) : base(body) { }

        /// <summary>
        /// Copy constructor.
        /// </summary>
        /// <param name="source"></param>
        public Entry(Entry source) : base(source) { }
    }

    // ====================================================
    /// <summary>
    /// Represents a collection of fragments used to build HEAD or TAIL clauses.
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
        protected override Entry Create(DbToken body) => new(body);

        // ------------------------------------------------

        /// <inheritdoc/>
        public override string? Separator => " ";
    }
}