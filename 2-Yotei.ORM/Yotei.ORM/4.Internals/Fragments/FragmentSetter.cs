namespace Yotei.ORM.Internals;

/// <summary>
/// Represents the ability of parsing SETTER clauses.
/// <br/>- Standard syntax: 'x => Target = Value'.
/// </summary>
public static partial class FragmentSetter
{
    readonly static string CLAUSE = "SETTER";

    // ====================================================
    /// <summary>
    /// Represents an entry in a collection of fragments used to build a SETTER clause.
    /// </summary>
    [Cloneable]
    public partial class Entry : Fragment.Entry
    {
        /// <summary>
        /// Initializes a new instance.
        /// </summary>
        /// <param name="command"></param>
        /// <param name="body"></param>
        [SuppressMessage("", "IDE0042")]
        public Entry(ICommand command, DbTokenLiteral body) : base(command)
        {
            Body = body.ThrowWhenNull();

            var parts = body.Value.ExtractLeftRight("=", Engine, out var found);
            if (!found) throw new ArgumentException(
                "Literal has not a valid 'target = value' setter format.")
                .WithData(body.Value);

            StrTarget = parts.Left.NotNullNotEmpty(trim: true);
            StrValue = parts.Right?.Trim() ?? string.Empty;
        }

        /// <summary>
        /// Initializes a new instance.
        /// </summary>
        /// <param name="command"></param>
        /// <param name="body"></param>
        public Entry(ICommand command, DbTokenSetter body) : base(command)
        {
            Body = body.ThrowWhenNull();
        }

        /// <summary>
        /// Copy constructor.
        /// </summary>
        /// <param name="source"></param>
        protected Entry(Entry source) : base(source)
        {
            Body = source.Body;
            StrTarget = source.StrTarget;
            StrValue = source.StrValue;
        }

        /// <inheritdoc/>
        public override string ToString() => Body.ToString()!;

        /// <summary>
        /// The actual contents carried by this instance.
        /// <br/> Only <see cref="DbTokenLiteral"/> and <see cref="DbTokenSetter"/> objects are
        /// allowed.
        /// </summary>
        public IDbToken Body { get; }
        string? StrTarget { get; }
        string? StrValue { get; }

        /// <inheritdoc/>
        public override ICommandInfo.IBuilder Visit(DbTokenVisitor visitor, bool first, bool last)
        {
            var builder = visitor.Visit(Body);
            var str = builder.Text.UnWrap('(', ')');
            builder.ReplaceText(str);

            return builder;
        }

        /// <summary>
        /// Visits the name of this instance.
        /// </summary>
        public virtual ICommandInfo.IBuilder VisitName(DbTokenVisitor visitor, bool first, bool last)
        {
            if (Body is DbTokenSetter item)
            {
                var builder = visitor.Visit(item.Target);
                return builder;
            }
            else
            {
                var builder = new CommandInfo.Builder(Engine, StrTarget);
                return builder;
            }
        }

        /// <summary>
        /// Visits the value of this instance.
        /// </summary>
        public virtual ICommandInfo.IBuilder VisitValue(DbTokenVisitor visitor, bool first, bool last)
        {
            if (Body is DbTokenSetter item)
            {
                var builder = visitor.Visit(item.Value);
                return builder;
            }
            else
            {
                var builder = new CommandInfo.Builder(Engine, StrValue);
                return builder;
            }
        }
    }

    // ====================================================
    /// <summary>
    /// Represents the collection of fragments used to build a SETTER clause.
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
        public override Entry Create(IDbToken body)
        {
            // Finishing...
            if (body is DbTokenInvoke invoke &&
                invoke.Arguments.Count == 1 &&
                invoke.Arguments[0] is DbTokenLiteral literal) body = literal;

            return body switch
            {
                DbTokenLiteral temp => new(Command, temp),
                DbTokenSetter temp => new(Command, temp),

                _ => throw new ArgumentException(
                    $"Specification does not resolve into a valid {CLAUSE} clause.")
                    .WithData(body)
            };
        }

        // ------------------------------------------------

        /// <inheritdoc/>
        public override string? Separator => ", ";

        /// <inheritdoc/>
        public override ICommandInfo.IBuilder Visit()
        {
            static ICommandInfo.IBuilder Itemize(
                Fragment.Entry entry, DbTokenVisitor visitor, bool first, bool last)
            {
                var valid = (Entry)entry;
                var builder = valid.Visit(visitor, first, last);
                return builder;
            }

            return Visit(Itemize);
        }

        /// <summary>
        /// Visits the names of this instance.
        /// </summary>
        public virtual ICommandInfo.IBuilder VisitNames()
        {
            static ICommandInfo.IBuilder Itemize(
                Fragment.Entry entry, DbTokenVisitor visitor, bool fisrt, bool last)
            {
                var valid = (Entry)entry;
                var builder = valid.VisitName(visitor, fisrt, last);
                return builder;
            }

            return Visit(Itemize);
        }

        /// <summary>
        /// Visits the values of this instance.
        /// </summary>
        public virtual ICommandInfo.IBuilder VisitValues()
        {
            static ICommandInfo.IBuilder Itemize(
                Fragment.Entry entry, DbTokenVisitor visitor, bool fisrt, bool last)
            {
                var valid = (Entry)entry;
                var builder = valid.VisitValue(visitor, fisrt, last);
                return builder;
            }

            return Visit(Itemize);
        }
    }
}
