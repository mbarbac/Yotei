namespace Yotei.ORM.Internal;

/// <summary>
/// Represents the ability of parsing SETTER clauses.
/// <br/>- Standard syntax: 'x => Target = Value'.
/// </summary>
public static partial class FragmentSetter
{
    // ====================================================
    /// <summary>
    /// Represents an entry in a collection of fragments used to build SETTER clauses.
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
            if (body.Target is not DbTokenIdentifier)
                throw new ArgumentException(
                    $"Target of a SETTER clause must be a valid identifier.")
                    .WithData(body);
        }

        /// <summary>
        /// Copy constructor.
        /// </summary>
        /// <param name="source"></param>
        public Entry(Entry source) : base(source) { }

        // ------------------------------------------------

        /// <inheritdoc/>
        public override ICommandInfo.IBuilder Visit(DbTokenVisitor visitor, string? separator)
        {
            if (Body is DbTokenSetter setter)
            {
                var builder = visitor.Visit(setter);
                var str = builder.Text.UnWrap('(', ')');
                builder.ReplaceText(str);

                if (ValidSeparator(separator)) builder.ReplaceText($"{separator}{builder.Text}");
                return builder;
            }
            else
            {
                //var body = (DbTokenLiteral)Body;
                throw null;
            }
        }

        /// <summary>
        /// Visits the name part of this instance and returns a command-info object that can be
        /// used to build a related command.
        /// <para>If the '<paramref name="separator"/>' parameter is not null, then its value
        /// is added to the returned builder before the actual contents of this instance.
        /// </para>
        /// </summary>
        /// <param name="visitor"></param>
        /// <param name="separator"></param>
        /// <returns></returns>
        public ICommandInfo.IBuilder VisitName(DbTokenVisitor visitor, string? separator)
        {
            if (Body is DbTokenSetter setter)
            {
                var builder = visitor.Visit(setter.Target);
                
                if (ValidSeparator(separator)) builder.ReplaceText($"{separator}{builder.Text}");
                return builder;
            }
            else
            {
                //var body = (DbTokenLiteral)Body;
                throw null;
            }
        }

        /// <summary>
        /// Visits the value part of this instance and returns a command-info object that can be
        /// used to build a related command.
        /// <para>If the '<paramref name="separator"/>' parameter is not null, then its value
        /// is added to the returned builder before the actual contents of this instance.
        /// </para>
        /// </summary>
        /// <param name="visitor"></param>
        /// <param name="separator"></param>
        /// <returns></returns>
        public ICommandInfo.IBuilder VisitValue(DbTokenVisitor visitor, string? separator)
        {
            if (Body is DbTokenSetter setter)
            {
                var builder = visitor.Visit(setter.Value);

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
    /// Represents a collection of fragments used to build SETTER clauses.
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
                "Entry is not a valid SETTER one.")
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
                DbTokenSetter temp => new(temp),

                _ => throw new ArgumentException(
                    "Specification does not resolve into a valid SETTER clause.")
                    .WithData(body)
            };
        }

        // ------------------------------------------------

        /// <inheritdoc/>
        public override string? Separator => ", ";

        /// <inheritdoc/>
        public override ICommandInfo.IBuilder Visit()
        {
            var builder = base.Visit();
            var str = builder.Text.UnWrap('(', ')').Wrap('(', ')');
            
            builder.ReplaceText(str);
            return builder;
        }

        /// <summary>
        /// Visits the name parts of the entries in this instance, and returns a command-info
        /// object that can be used to build a related command. The returned text is wrapped
        /// between rounded brackets.
        /// </summary>
        /// <returns></returns>
        public virtual ICommandInfo.IBuilder VisitNames()
        {
            var connection = Command.Connection;
            var visitor = connection.Records.CreateDbTokenVisitor(Command.Locale);
            var engine = connection.Engine;
            var builder = new CommandInfo.Builder(engine);

            for (int i = 0; i < Count; i++)
            {
                var separator = i == 0 ? null : Separator;
                var item = (Entry)this[i];
                var temp = item.VisitName(visitor, separator);

                builder.Add(temp);
            }

            builder.ReplaceText($"({builder.Text})");
            return builder;
        }

        /// <summary>
        /// Visits the name parts of the entries in this instance, and returns a command-info
        /// object that can be used to build a related command. The returned text is wrapped
        /// between rounded brackets.
        /// </summary>
        /// <returns></returns>
        public virtual ICommandInfo.IBuilder VisitValues()
        {
            var connection = Command.Connection;
            var visitor = connection.Records.CreateDbTokenVisitor(Command.Locale);
            var engine = connection.Engine;
            var builder = new CommandInfo.Builder(engine);

            for (int i = 0; i < Count; i++)
            {
                var separator = i == 0 ? null : Separator;
                var item = (Entry)this[i];
                var temp = item.VisitValue(visitor, separator);

                builder.Add(temp);
            }

            if (Count == 1)
            {
                var str = builder.Text.UnWrap('(', ')').Wrap('(', ')');
                builder.ReplaceText(str);
            }
            if (Count > 1) builder.ReplaceText($"({builder.Text})");
            return builder;
        }
    }
}