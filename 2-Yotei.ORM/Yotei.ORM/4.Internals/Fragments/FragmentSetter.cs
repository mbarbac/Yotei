namespace Yotei.ORM.Internals;

/// <summary>
/// Represents the ability of parsing SETTER clauses.
/// <br/>- Standard syntax: 'x => Target = Value'.
/// <br/>- Alternate syntax: 'x => Target == Value'.
/// </summary>
/// <remarks>
/// SETTER elements are special because they only accept 'Source=Target' alike formats.
/// <br/> If '==' alike elements are used then, for convenience, they are translated into '=' ones.
/// </remarks>
public static partial class FragmentSetter
{
    // ====================================================
    /// <summary>
    /// Represents an entry in a collection of fragments used to build SETTER-alike clauses.
    /// </summary>
    [Cloneable]
    public partial class Entry : Fragment.Entry
    {
        /// <summary>
        /// Initializes a new instance.
        /// </summary>
        /// <param name="master"></param>
        /// <param name="body"></param>
        public Entry(Master master, IDbToken body) : base(master, body)
        {
            // Extracting target and value from literal sources...
            if (body is DbTokenLiteral literal)
            {
                var str = literal.Value.Trim();

                var (target, value) = str.ExtractLeftRight("==", Engine, out var found);
                if (!found) (target, value) = str.ExtractLeftRight("=", Engine, out found);
                if (!found) throw new ArgumentException(
                    "Literal has not a valid 'target=value' setter format.")
                    .WithData(body);

                LiteralTarget = Adjust(target);
                LiteralValue = Adjust(value!);

                string Adjust(string source)
                {
                    if (source is not null)
                    {
                        if (source.StartsWith('(') && !source.EndsWith(')')) source = source[1..];
                        if (!source.StartsWith('(') && source.EndsWith(')')) source = source[..^1];
                    }
                    return source.NotNullNotEmpty(trim: true);
                }
            }

            // Using 'target==value' instead of the right 'target=value' one...
            else if (body is DbTokenBinary binary && binary.Operation == ExpressionType.Equal)
            {
                Body = new DbTokenSetter(binary.Left, binary.Right);
            }

            // By default, only 'target=value' setters are allowed...
            else
            {
                if (body is not DbTokenSetter) throw new ArgumentException(
                    $"Invalid token type for a {Master.Clause} clause.")
                    .WithData(body);
            }
        }

        /// <summary>
        /// Copy constructor.
        /// </summary>
        /// <param name="source"></param>
        protected Entry(Entry source) : base(source)
        {
            LiteralTarget = source.LiteralTarget;
            LiteralValue = source.LiteralValue;
        }

        /// <summary>
        /// If not null, then the captured literal acting as this instance's target. This property
        /// is only set when <see cref="Body"/> is a literal one, otherwise it is <c>null</c>.
        /// </summary>
        public string? LiteralTarget { get; }

        /// <summary>
        /// If not null, then the captured literal acting as this instance's value. This property
        /// is only set when <see cref="Body"/> is a literal one, otherwise it is <c>null</c>.
        /// </summary>
        public string? LiteralValue { get; }

        // ------------------------------------------------

        /// <inheritdoc/>
        public override string ToString() => Body is DbTokenLiteral
            ? $"({LiteralTarget} = {LiteralValue})"
            : base.ToString();

        /// <inheritdoc/>
        public override ICommandInfo.IBuilder Visit(DbTokenVisitor visitor)
        {
            if (Body is DbTokenLiteral)
            {
                var builder = new CommandInfo.Builder(Engine, $"({LiteralTarget} = {LiteralValue})");
                return builder;
            }
            else
            {
                var token = (DbTokenSetter)Body;
                var builder = visitor.Visit(token);
                return builder;
            }
        }

        /// <summary>
        /// Visits the name of this instance and returns a command info object that represents
        /// that element in the related clause of the command.
        /// </summary>
        /// <param name="visitor"></param>
        /// <returns></returns>
        public virtual ICommandInfo.IBuilder VisitName(DbTokenVisitor visitor)
        {
            if (Body is DbTokenLiteral)
            {
                var builder = new CommandInfo.Builder(Engine, LiteralTarget);
                return builder;
            }
            else
            {
                var token = (DbTokenSetter)Body;
                var builder = visitor.Visit(token.Target);
                return builder;
            }
        }

        /// <summary>
        /// Visits the value of this instance and returns a command info object that represents
        /// that element in the related clause of the command.
        /// </summary>
        /// <param name="visitor"></param>
        /// <returns></returns>
        public virtual ICommandInfo.IBuilder VisitValue(DbTokenVisitor visitor)
        {
            if (Body is DbTokenLiteral)
            {
                var builder = new CommandInfo.Builder(Engine, LiteralValue);
                return builder;
            }
            else
            {
                var token = (DbTokenSetter)Body;
                var builder = visitor.Visit(token.Value);
                return builder;
            }
        }
    }

    // ====================================================
    /// <summary>
    /// Represents a collection of fragments used to build SETTER-alike clauses.
    /// </summary>
    [Cloneable]
    public partial class Master : Fragment.Master
    {
        /// <summary>
        /// Initializes a new instance.
        /// </summary>
        /// <param name="command"></param>
        /// <param name="clause"></param>
        public Master(ICommand command, string clause = "SETTER") : base(command, clause) { }

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
                $"Entry is not a valid {Clause} one.")
                .WithData(entry);

            return valid;
        }

        // ------------------------------------------------

        /// <inheritdoc/>
        public override Entry CreateEntry(IDbToken body) => new(this, body);

        // ------------------------------------------------

        /// <inheritdoc/>
        protected override string? EntrySeparator(Fragment.Entry entry) => ", ";

        /// <inheritdoc/>
        protected override ICommandInfo.IBuilder VisitEntries(DbTokenVisitor visitor)
        {
            var builder = base.VisitEntries(visitor);

            if (Count > 1 && builder.Text.Length > 0) builder.ReplaceText($"({builder.Text})");
            return builder;
        }

        /// <summary>
        /// Visits the names in this instance and returns a command info object that represents
        /// that elements in this clause of the related command.
        /// </summary>
        /// <returns></returns>
        public virtual ICommandInfo.IBuilder VisitNames()
        {
            static ICommandInfo.IBuilder VisitEntry(Fragment.Entry entry, DbTokenVisitor visitor)
            {
                var valid = (Entry)entry;
                var builder = valid.VisitName(visitor);
                return builder;
            }

            var visitor = Connection.Records.CreateDbTokenVisitor(Command.Locale);
            var builder = VisitEntries(visitor, VisitEntry);

            if (!builder.IsEmpty)
            {
                var str = builder.Text.UnWrap('(', ')').Wrap('(', ')');
                builder.ReplaceText(str);
            }
            return builder;
        }

        /// <summary>
        /// Visits the values in this instance and returns a command info object that represents
        /// that elements in this clause of the related command.
        /// </summary>
        /// <returns></returns>
        public virtual ICommandInfo.IBuilder VisitValues()
        {
            static ICommandInfo.IBuilder VisitEntry(Fragment.Entry entry, DbTokenVisitor visitor)
            {
                var valid = (Entry)entry;
                var builder = valid.VisitValue(visitor);
                return builder;
            }

            var visitor = Connection.Records.CreateDbTokenVisitor(Command.Locale);
            var builder = VisitEntries(visitor, VisitEntry);

            if (!builder.IsEmpty)
            {
                var str = builder.Text.UnWrap('(', ')').Wrap('(', ')');
                builder.ReplaceText(str);
            }
            return builder;
        }
    }
}