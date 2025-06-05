namespace Yotei.ORM.Internal;

public static partial class Fragment
{
    // ====================================================
    /// <summary>
    /// Represents an entry in a collection of fragments used to build a specific clause.
    /// </summary>
    [Cloneable]
    public partial class Entry
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
        /// Determines if the given separator is a valid one, and so this instance is not the
        /// first one being processed, or not. Valid separators are not null ones, even if empty.
        /// </summary>
        protected virtual bool IsValidSeparator(string? separator) => separator is not null;

        /// <summary>
        /// Visits the contents of this instance and returns a command-info object that can be
        /// used to build a related command.
        /// <br/> If the '<paramref name="separator"/>' parameter is a valid one, then this entry
        /// is not the first one being processed, and the separator's value is used before adding
        /// its contents. Otherwise, it is ignored.
        /// </summary>
        /// <param name="visitor"></param>
        /// <param name="separator"></param>
        /// <returns></returns>
        public virtual ICommandInfo.IBuilder Visit(DbTokenVisitor visitor, string? separator)
        {
            var builder = visitor.Visit(Body);

            if (IsValidSeparator(separator)) builder.ReplaceText($"{separator}{builder.Text}");
            return builder;
        }
    }
}