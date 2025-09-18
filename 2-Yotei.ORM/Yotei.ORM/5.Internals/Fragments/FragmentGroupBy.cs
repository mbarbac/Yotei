namespace Yotei.ORM.Internals;

/// <summary>
/// Represents a GROUP BY fragment.
/// </summary>
public static class FragmentGroupBy
{
    // ====================================================
    /// <summary>
    /// Represents an entry in a collection of fragments used to build a GROUP BY-alike clause.
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
        public override ICommandInfo.IBuilder Visit(DbTokenVisitor visitor, bool separate)
        {
            var builder = visitor.Visit(Body);

            if (separate) builder.ReplaceText($", {builder.Text}");
            return builder;
        }
    }

    // ====================================================
    /// <summary>
    /// Represents a collection of fragments used to build a GROUP BY-alike clause.
    /// </summary>
    public class Master : Fragment.Master
    {
        /// <summary>
        /// Initializes a new instance.
        /// </summary>
        /// <param name="command"></param>
        public Master(ICommand command, string descriptor = "GROUP BY")
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
        public override Entry CreateEntry(IDbToken token) => new(this, token);
    }
}