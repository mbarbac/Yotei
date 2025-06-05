namespace Yotei.ORM.Internal;

public static partial class Fragment
{
    // ====================================================
    /// <summary>
    /// Represents an entry in a collection of fragments used to build a specific clause.
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
        /// The body of contents carried by this instance. Inheritors may restrict the actual
        /// kind of tokens accepted by this instance.
        /// </summary>
        public DbToken Body { get; }

        // ------------------------------------------------

        /// <summary>
        /// Determines if the given separator is a valid one, and so this entry is not the
        /// first one being built.
        /// <br/> Valid separators are not null and not empty ones.
        /// </summary>
        /// <param name="separator"></param>
        /// <returns></returns>
        protected static bool ValidSeparator(
            string? separator) => separator is not null && separator.Length > 0;

        /// <summary>
        /// Visits the contents of this instance and returns a command-info object that can be
        /// used to build a related command.
        /// <para>If the '<paramref name="separator"/>' parameter is not null, then its value
        /// is added to the returned builder before the actual contents of this instance.
        /// </para>
        /// </summary>
        /// <param name="visitor"></param>
        /// <param name="separator"></param>
        /// <returns></returns>
        public virtual ICommandInfo.IBuilder Visit(DbTokenVisitor visitor, string? separator)
        {
            var builder = visitor.Visit(Body);

            if (ValidSeparator(separator)) builder.ReplaceText($"{separator}{builder.Text}");
            return builder;
        }
    }
}