namespace Yotei.ORM.Internal;

/// <summary>
/// Represents the ability of parsing XXX clauses.
/// <br/>- 
/// </summary>
public static partial class FragmentXXX
{
    // ====================================================
    /// <summary>
    /// Represents an entry in a collection of fragments used to build XXX clauses.
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
        public Entry(DbTokenCoalesce body) : base(body) { }

        /// <summary>
        /// Copy constructor.
        /// </summary>
        /// <param name="source"></param>
        public Entry(Entry source) : base(source) { }

        // ------------------------------------------------

        /// <inheritdoc/>
        public override ICommandInfo.IBuilder Visit(DbTokenVisitor visitor, string? separator)
        {
            if (Body is DbTokenCoalesce item)
            {
                var builder = visitor.Visit(item);

                if (ValidSeparator(separator)) builder.ReplaceText($"{separator}{builder.Text}");
                return builder;
            }
            else
            {
                //var body = (DbTokenLiteral)Body;
                throw null;
            }
        }
    }

    // ====================================================
    /// <summary>
    /// Represents a collection of fragments used to build XXX clauses.
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
                "Entry is not a valid XXX one.")
                .WithData(entry);

            return valid;
        }

        /// <inheritdoc/>
        protected override Entry Create(DbToken body)
        {
            // Finishing...
            if (body is DbTokenInvoke invoke &&
                invoke.Arguments.Count == 1 &&
                invoke.Arguments[0] is DbTokenLiteral literal) body = literal;

            return body switch
            {
                DbTokenLiteral temp => new(temp),
                DbTokenCoalesce temp => new(temp),

                _ => throw new ArgumentException(
                    "Specification does not resolve into a valid XXX clause.")
                    .WithData(body)
            };
        }

        // ------------------------------------------------

        /// <inheritdoc/>
        public override string? Separator => " ";
    }
}
