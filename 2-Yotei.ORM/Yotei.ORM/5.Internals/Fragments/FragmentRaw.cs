namespace Yotei.ORM.Internals;

/// <summary>
/// Represents a RAW fragment.
/// </summary>
public static class FragmentRaw
{
    // ====================================================
    /// <summary>
    /// Represents an entry in a collection of fragments used to build a RAW clause.
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
        protected Entry(Entry source) : base(source) { }

        /// <inheritdoc/>
        public override Entry Clone() => new(this);

        // ------------------------------------------------

        /// <inheritdoc/>
        protected override IDbToken ParseBody(IDbToken token) => base.ParseBody(token);

        // ------------------------------------------------

        /// <inheritdoc/>
        public override string ToString() => base.ToString();

        /// <inheritdoc/>
        public override ICommandInfo.IBuilder Visit(DbTokenVisitor visitor, bool _)
        {
            var builder = visitor.Visit(Body);
            return builder;
        }
    }

    // ====================================================
    /// <summary>
    /// Represents a collection of fragments used to build a RAW clause.
    /// </summary>
    public class Master : Fragment.Master
    {
        /// <summary>
        /// Initializes a new instance.
        /// </summary>
        /// <param name="command"></param>
        public Master(ICommand command) : base(command, "RAW") { }

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
        protected override Entry CreateEntry(IDbToken token) => new(this, token);
    }
}