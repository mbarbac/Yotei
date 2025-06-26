namespace Yotei.ORM.Internals;

/// <summary>
/// Represents the ability of parsing SETTER clauses.
/// <br/>- Standard syntax: 'x => Target = Value'.
/// <br/>- Alternate syntax: 'x => Target == Value'.
/// </summary>
/// <remarks>
/// SETTER elements are special in the sense that they only accept 'Source=Target' formats, either
/// literal or actual binary setter ones. If '==' alike elements are used then, for convenience,
/// they are translated into '=' ones.
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
        string? StrTarget;
        string? StrValue;

        /// <summary>
        /// Initializes a new instance.
        /// </summary>
        /// <param name="master"></param>
        /// <param name="body"></param>
        public Entry(Master master, IDbToken body) : base(master, body)
        {
            if (body is DbTokenLiteral literal)
            {
                var str = literal.Value.Trim().UnWrap('(', ')')!;

                var (target, value) = str.ExtractLeftRight("==", Engine, out var found);
                if (found)
                {
                    StrTarget = target.NotNullNotEmpty(trim: true);
                    StrValue = value.NotNullNotEmpty(trim: true);
                }
                else
                {
                    (target, value) = str.ExtractLeftRight("=", Engine, out found);
                    if (found)
                    {
                        StrTarget = target.NotNullNotEmpty(trim: true);
                        StrValue = value.NotNullNotEmpty(trim: true);
                    }
                    else
                    {
                        throw new ArgumentException(
                            "Literal has not a valid 'target = value' setter format.")
                            .WithData(body);
                    }
                }
            }
            else if (body is DbTokenBinary binary && binary.Operation == ExpressionType.Equal)
            {
                Body = new DbTokenSetter(binary.Left, binary.Right);
            }
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
            StrTarget = source.StrTarget;
            StrValue = source.StrValue;
        }

        // ------------------------------------------------

        /// <inheritdoc/>
        public override ICommandInfo.IBuilder Visit(DbTokenVisitor visitor)
        {
            if (Body is DbTokenLiteral)
            {
                var builder = new CommandInfo.Builder(Engine, $"({StrTarget} = {StrValue})");
                return builder;
            }
            else if (Body is DbTokenSetter setter)
            {
                var builder = visitor.Visit(setter);
                return builder;
            }
            else throw new UnExpectedException("Unexpected body type.").WithData(Body);
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
                var builder = new CommandInfo.Builder(Engine, StrTarget);
                return builder;
            }
            else if (Body is DbTokenSetter setter)
            {
                var builder = visitor.Visit(setter.Target);
                return builder;
            }
            else throw new UnExpectedException("Unexpected body type.").WithData(Body);
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
                var builder = new CommandInfo.Builder(Engine, StrValue);
                return builder;
            }
            else if (Body is DbTokenSetter setter)
            {
                var builder = visitor.Visit(setter.Value);
                return builder;
            }
            else throw new UnExpectedException("Unexpected body type.").WithData(Body);
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
        public Master(ICommand command, string clause) : base(command, clause) { }

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

        /// <inheritdoc/>
        public override Entry CreateEntry(IDbToken body) => new(this, body);

        // ------------------------------------------------

        /// <inheritdoc/>
        protected override string? EntrySeparator(Fragment.Entry entry) => ", ";

        /// <inheritdoc/>
        protected override ICommandInfo.IBuilder VisitEntries(DbTokenVisitor visitor)
        {
            var builder = base.VisitEntries(visitor);

            if (Count > 1 && !builder.IsEmpty)
            {
                var str = $"({builder.Text})";
                builder.ReplaceText(str);
            }
            return builder;
        }

        /// <summary>
        /// Visits the names of the entries in this instance and returns a command info object that
        /// represents that elements in the related clause of the command.
        /// <br/> This method DOES NOT use any head or tail in this instance.
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
        /// Visits the names of the entries in this instance and returns a command info object that
        /// represents that elements in the related clause of the command.
        /// <br/> This method DOES NOT use any head or tail in this instance.
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
