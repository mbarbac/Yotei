namespace Yotei.ORM.Internals;

/// <summary>
/// Represents the ability of parsing SETTER clauses.
/// <br/>- Standard syntax: 'x => Target = Value'.
/// </summary>
/// <remarks>
/// SETTER elements are special as that they only accept 'Source=Target' formats, either literal
/// or actual setter ones. It is ok because they are intended to act as column specifications.
/// </remarks>
public static partial class FragmentSetter
{
    // ====================================================
    /// <summary>
    /// Represents an entry in a collection of fragments used to build a SETTER clause.
    /// </summary>
    [Cloneable]
    public partial class Entry : Fragment.Entry
    {
        /// <inheritdoc/>
        public override string CLAUSE => "SETTER";

        /// <summary>
        /// Initializes a new instance.
        /// </summary>
        /// <param name="master"></param>
        /// <param name="body"></param>
        public Entry(Master master, IDbToken body) : base(master, body)
        {
            switch (body)
            {
                case DbTokenLiteral literal:
                    var str = literal.Value.Trim().UnWrap('(', ')')!;
                    var (target, value) = str.ExtractLeftRight("=", Engine, out var found);
                    if (!found) throw new ArgumentException(
                        "Literal has not a valid 'target = value' setter format.")
                        .WithData(body);

                    StrTarget = target.NotNullNotEmpty(trim: true);
                    StrValue = value.NotNullNotEmpty(trim: true);
                    break;

                case DbTokenSetter: break; // Body already captured by 'base'...

                default:
                    throw new ArgumentException(
                        $"Invalid token type for a {CLAUSE} clause.").WithData(body);
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

        /// <summary>
        /// <inheritdoc cref="Fragment.Entry.Body"/>
        /// <br/> Only <see cref="DbTokenLiteral"/> and <see cref="DbTokenSetter"/> instances
        /// are allowed.
        /// </summary>
        public new IDbToken Body => base.Body;
        string? StrTarget { get; }
        string? StrValue { get; }

        // ------------------------------------------------

        /// <inheritdoc/>
        /// <remarks>Elements are wrapped: 'Target=Value'.</remarks>
        protected override ICommandInfo.IBuilder VisitBody(DbTokenVisitor visitor)
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
        /// Visits the name of this instance.
        /// </summary>
        /// <param name="visitor"></param>
        /// <returns></returns>
        /// <remarks>Elements are NOT wrapped: 'Target'.</remarks>
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
        /// Visits the value of this instance.
        /// </summary>
        /// <param name="visitor"></param>
        /// <returns></returns>
        /// <remarks>Elements are NOT wrapped: 'Value'.</remarks>
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
    /// Represents the collection of fragments used to build a SETTER clause.
    /// </summary>
    [Cloneable]
    public partial class Master : Fragment.Master
    {
        /// <inheritdoc/>
        public override string CLAUSE => "SETTER";

        /// <summary>
        /// Initializes a new instance.
        /// </summary>
        /// <param name="command"></param>
        public Master(ICommand command) : base(command) { }

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
                $"Entry is not a valid {CLAUSE} one.")
                .WithData(entry);

            return valid;
        }

        // ------------------------------------------------

        /// <inheritdoc/>
        protected override Entry OnCreate(DbTokenInvoke? head, IDbToken body, DbTokenInvoke? tail)
        {
            if (body is DbTokenInvoke invoke &&
                invoke.Host is DbTokenArgument &&
                invoke.Arguments.Count == 1 &&
                invoke.Arguments[0] is DbTokenLiteral literal) body = literal;

            var entry = new Entry(this, body);
            if (head is not null) entry._Head = head;
            if (tail is not null) entry._Tail = tail;

            return entry;
        }

        // ------------------------------------------------

        /// <inheritdoc/>
        public override string? Separator(Fragment.Entry entry) => ", ";

        /// <inheritdoc/>
        public override ICommandInfo.IBuilder Visit()
        {
            static ICommandInfo.IBuilder VisitItem(Fragment.Entry entry, DbTokenVisitor visitor)
            {
                var valid = (Entry)entry;
                var builder = valid.Visit(visitor);
                return builder;
            }

            // Elements are wrapped, but we need an additional one if many are involved...
            var builder = Visit(VisitItem);
            if (Count > 1) builder.ReplaceText($"({builder.Text})");
            return builder;
        }

        /// <summary>
        /// Visits the names on this instaance and returns a command info object that can be
        /// used to build the related clause of the associated command.
        /// </summary>
        /// <returns></returns>
        public ICommandInfo.IBuilder VisitNames()
        {
            static ICommandInfo.IBuilder VisitItem(Fragment.Entry entry, DbTokenVisitor visitor)
            {
                var valid = (Entry)entry;
                var builder = valid.VisitName(visitor);
                return builder;
            }

            // Elements are not wrapped...
            var builder = Visit(VisitItem);
            builder.ReplaceText($"({builder.Text})");
            return builder;
        }

        /// <summary>
        /// Visits the values on this instaance and returns a command info object that can be
        /// used to build the related clause of the associated command.
        /// </summary>
        /// <returns></returns>
        public ICommandInfo.IBuilder VisitValues()
        {
            static ICommandInfo.IBuilder VisitItem(Fragment.Entry entry, DbTokenVisitor visitor)
            {
                var valid = (Entry)entry;
                var builder = valid.VisitValue(visitor);
                return builder;
            }

            // Elements are not wrapped...
            var builder = Visit(VisitItem);
            builder.ReplaceText($"({builder.Text})");
            return builder;
        }
    }
}
