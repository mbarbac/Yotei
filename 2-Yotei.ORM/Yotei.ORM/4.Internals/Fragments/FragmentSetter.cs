namespace Yotei.ORM.Internals;

/// <summary>
/// Represents the ability of parsing SETTER clauses.
/// <br/>- Standard syntax: 'x => Target = Value'.
/// </summary>
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
        [SuppressMessage("", "IDE0042")]
        public Entry(Master master, DbTokenLiteral body) : base(master)
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
        /// <param name="master"></param>
        /// <param name="body"></param>
        public Entry(Master master, DbTokenSetter body) : base(master)
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
        public override ICommandInfo.IBuilder Visit(DbTokenVisitor visitor)
        {
            var builder = visitor.Visit(Body);
            var str = builder.Text.UnWrap('(', ')').Wrap('(', ')');
            builder.ReplaceText(str);

            return builder;
        }

        /// <summary>
        /// Visits the name of this instance.
        /// </summary>
        /// <param name="visitor"></param>
        public virtual ICommandInfo.IBuilder VisitName(DbTokenVisitor visitor)
        {
            if (Body is DbTokenSetter item)
            {
                var builder = visitor.Visit(item.Target);
                return builder;
            }
            else if (Body is DbTokenLiteral)
            {
                var builder = new CommandInfo.Builder(Engine, StrTarget);
                return builder;
            }
            else throw new UnExpectedException("Unsupported token.").WithData(Body);
        }

        /// <summary>
        /// Visits the value of this instance.
        /// </summary>
        /// <param name="visitor"></param>
        public virtual ICommandInfo.IBuilder VisitValue(DbTokenVisitor visitor)
        {
            if (Body is DbTokenSetter item)
            {
                var builder = visitor.Visit(item.Value);
                return builder;
            }
            else if (Body is DbTokenLiteral)
            {
                var builder = new CommandInfo.Builder(Engine, StrValue);
                return builder;
            }
            else throw new UnExpectedException("Unsupported token.").WithData(Body);
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
        public override Entry Create(IDbToken body)
        {
            // Finishing...
            if (body is DbTokenInvoke invoke &&
                invoke.Arguments.Count == 1 &&
                invoke.Arguments[0] is DbTokenLiteral literal) body = literal;

            return body switch
            {
                DbTokenLiteral temp => new(this, temp),
                DbTokenSetter temp => new(this, temp),

                _ => throw new ArgumentException(
                    $"Specification does not resolve into a valid {CLAUSE} clause.")
                    .WithData(body)
            };
        }

        // ------------------------------------------------

        /// <inheritdoc/>
        public override string? Separator(Fragment.Entry entry) => ", ";

        ICommandInfo.IBuilder OnVisit(Func<Fragment.Entry, DbTokenVisitor, ICommandInfo.IBuilder> itemize)
        {
            var builder = Visit(itemize);
            var str = builder.Text;
            if (str.Length > 0)
            {
                str = Count == 1
                    ? builder.Text.UnWrap('(', ')').Wrap('(', ')')
                    : $"({builder.Text})";

                builder.ReplaceText(str);
            }
            return builder;
        }

        /// <inheritdoc/>
        public override ICommandInfo.IBuilder Visit()
        {
            static ICommandInfo.IBuilder Itemize(Fragment.Entry entry, DbTokenVisitor visitor)
            {
                var valid = (Entry)entry;
                var builder = valid.Visit(visitor);
                return builder;
            }
            return OnVisit(Itemize);
        }

        /// <summary>
        /// Visits the names of this instance.
        /// </summary>
        public virtual ICommandInfo.IBuilder VisitNames()
        {
            static ICommandInfo.IBuilder Itemize(Fragment.Entry entry, DbTokenVisitor visitor)
            {
                var valid = (Entry)entry;
                var builder = valid.VisitName(visitor);
                return builder;
            }
            return OnVisit(Itemize);
        }

        /// <summary>
        /// Visits the values of this instance.
        /// </summary>
        public virtual ICommandInfo.IBuilder VisitValues()
        {
            static ICommandInfo.IBuilder Itemize(Fragment.Entry entry, DbTokenVisitor visitor)
            {
                var valid = (Entry)entry;
                var builder = valid.VisitValue(visitor);
                return builder;
            }
            return OnVisit(Itemize);
        }
    }
}
