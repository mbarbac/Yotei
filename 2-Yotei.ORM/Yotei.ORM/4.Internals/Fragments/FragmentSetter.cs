using System.Windows.Markup;

namespace Yotei.ORM.Internals;

/// <summary>
/// Represents the ability of parsing SETTER clauses.
/// <br/>- Standard syntax: 'x => Target = Value'.
/// </summary>
/// <remarks>
/// SETTER only accept 'Source=Target' alike statements, either literal or actual setter ones.
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
        public Entry(Master master, IDbToken body) : base(master, body) // Body captured by 'base'...
        {
            if (body is DbTokenLiteral text)
            {
                var (target, value) = text.Value.ExtractLeftRight("=", Engine, out var found);
                if (!found) throw new ArgumentException(
                    "Literal has not a valid 'target = value' setter format.")
                    .WithData(body);

                StrTarget = target.NotNullNotEmpty(trim: true);
                StrValue = value.NotNullNotEmpty(trim: true);
            }
            else if (body is DbTokenSetter)
            { }
            else throw new ArgumentException($"Invalid token type for a {CLAUSE} clause.").WithData(body);
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
        protected override ICommandInfo.IBuilder VisitBody(DbTokenVisitor visitor)
        {
            if (Body is DbTokenLiteral)
            {
                var str = $"{StrTarget} = {StrValue}";
                var builder = new CommandInfo.Builder(Engine, str);
                return builder;
            }
            else if (Body is DbTokenSetter)
            {
                var builder = visitor.Visit(Body);
                var str = builder.Text.UnWrap('(', ')').Wrap('(', ')');
                builder.ReplaceText(str);

                return builder;
            }
            else throw new UnExpectedException("Unexpected body type.").WithData(Body);
        }

        // ------------------------------------------------

        /// <summary>
        /// Visits the name of this instance.
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
        /// Visits the value of this instance.
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
    }
}
