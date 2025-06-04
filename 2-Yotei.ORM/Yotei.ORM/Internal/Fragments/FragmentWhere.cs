namespace Yotei.ORM.Internal;

public partial class Fragment
{
    // ====================================================
    /// <summary>
    /// Determines the position of a fragment entry.
    /// </summary>
    public enum EntryPosition { First, Middle, Last }

    // ====================================================
    /// <summary>
    /// Represents an entry in a collection of fragments.
    /// </summary>
    [Cloneable]
    public abstract partial class Entry
    {
        /// <summary>
        /// Initializes a new instance.
        /// </summary>
        /// <param name="body"></param>
        public Entry(DbToken body) => Body = body.ThrowWhenNull();

        /// <summary>
        /// Copy constructor.
        /// </summary>
        /// <param name="source"></param>
        public Entry(Entry source) : this(source.Body) { }

        /// <inheritdoc/>
        public override string ToString() => Body.ToString()!;

        /// <summary>
        /// The body of contents carried by this instance.
        /// </summary>
        public DbToken Body { get; }
    }
}