#pragma warning disable IDE1006

namespace Yotei.ORM.Internals;

/// <summary>
/// Represents the ability of parsing SETTER clauses.
/// <br/>- Standard syntax: 'x => Target = Value'.
/// <br/>- Alternate syntax: 'x => Target == Value'.
/// </summary>
/// <remarks>
/// SETTER elements are special because their Body must be a 'Target=Value' -alike element,
/// either a <see cref="DbTokenSetter"/>, or a <see cref="DbTokenLiteral"/> whose string
/// contents have the right "=" format.
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
            // First-level invokes with just one argument translated to that argument...
            while (body is DbTokenInvoke invoke &&
                invoke.Host is DbTokenArgument &&
                invoke.Arguments.Count == 1)
            {
                Body = invoke.Arguments[0];
                body = Body;
            }
            
            // Values carrying a string translated to literals...
            if (body is DbTokenValue value && value.Value is string vstr)
            {
                Body = new DbTokenLiteral(vstr);
                body = Body;
            }

            // Literals...
            if (body is DbTokenLiteral literal)
            {
                var tokenizer = new StrWrappedTokenizer(Engine.LeftTerminator, Engine.RightTerminator);
                var str = literal.Value.Trim();
                var items = Engine.UseTerminators ? tokenizer.Tokenize(str) : new StrTokenText(str);

                var (left, right) = items.ExtractFirst("=", false, out var found);
                if (found)
                {
                    str = left.ToString()!.Trim();
                    if (str.StartsWith('(') && !str.EndsWith(')')) str = str[1..];
                    if (!str.StartsWith('(') && str.EndsWith(')')) str = str[..^1];
                    str = str.NotNullNotEmpty(trim: true, $"Target {Clause} entry part.");
                    Target = new DbTokenLiteral(str);

                    str = right!.ToString()!.Trim();
                    if (str.StartsWith('(') && !str.EndsWith(')')) str = str[1..];
                    if (!str.StartsWith('(') && str.EndsWith(')')) str = str[..^1];
                    str = str.NotNullNotEmpty(trim: true, $"Value {Clause} entry part.");
                    Value = new DbTokenLiteral(str);

                    return;
                }
            }

            // Standard setters...
            if (body is DbTokenSetter setter)
            {
                Target = setter.Target;
                Value = setter.Value;
                return;
            }

            // Invalid ones...
            throw new ArgumentException(
                $"{Clause} entry body has not the appropriate 'Target='Value' format.")
                .WithData(body);
        }

        /// <summary>
        /// Copy constructor.
        /// </summary>
        /// <param name="source"></param>
        protected Entry(Entry source) : base(source)
        {
            Target = source.Target.Clone();
            Value = source.Value.Clone();
        }

        /// <summary>
        /// The target of this setter operation.
        /// </summary>
        public IDbToken Target
        {
            get => _Target;
            init => _Target = value;
        }
        internal protected IDbToken _Target
        {
            get => __Target;
            set => __Target = value.ThrowWhenNull();
        }
        IDbToken __Target = default!;

        /// <summary>
        /// The value of this setter operation.
        /// </summary>
        public IDbToken Value
        {
            get => _Value;
            init => _Value = value.ThrowWhenNull();
        }
        internal protected IDbToken _Value
        {
            get => __Value;
            set => __Value = value.ThrowWhenNull();
        }
        IDbToken __Value = default!;

        // ------------------------------------------------

        /// <inheritdoc/>
        public override string ToString() => $"({Target} = {Value})";

        /// <inheritdoc/>
        public override ICommandInfo.IBuilder Visit(DbTokenVisitor visitor)
        {
            var target = visitor.Visit(Target);
            var value = visitor.Visit(Value);

            target.ReplaceText($"({target} = ");
            target.Add(value);
            target.Add(")");
            return target;
        }

        /// <summary>
        /// Visits the name carried by this instance and returns a command info object that
        /// represents the current entry in the related clause of the command.
        /// </summary>
        /// <param name="visitor"></param>
        /// <returns></returns>
        public virtual ICommandInfo.IBuilder VisitName(DbTokenVisitor visitor)
        {
            var item = visitor.Visit(Target);
            return item;
        }

        /// <summary>
        /// Visits the name carried by this instance and returns a command info object that
        /// represents the current entry in the related clause of the command.
        /// </summary>
        /// <param name="visitor"></param>
        /// <returns></returns>
        public virtual ICommandInfo.IBuilder VisitValue(DbTokenVisitor visitor)
        {
            var item = visitor.Visit(Value);
            return item;
        }
    }

    // ====================================================
    /// <summary>
    /// Represents the collection of fragments used to build SETTER-alike clauses.
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

        /// <inheritdoc/>
        public override Entry CreateEntry(IDbToken body) => new(this, body);

        // ------------------------------------------------

        /// <inheritdoc/>
        public override ICommandInfo.IBuilder Visit()
        {
            var builder = base.Visit();

            if (Count > 1 && builder.TextLen > 0) builder.ReplaceText($"({builder.Text})");
            return builder;
        }

        /// <inheritdoc/>
        protected override string? EntrySeparator(Fragment.Entry entry) => ", ";

        /// <summary>
        /// Visits the names carried by the entries in this instance and returns a command info
        /// object that can be used to build the related clause of the associated command, or an
        /// empty result if no entries are captured.
        /// </summary>
        /// <returns></returns>
        public virtual ICommandInfo.IBuilder VisitNames()
        {
            static ICommandInfo.IBuilder EntryVisitor(
                Fragment.Entry entry, DbTokenVisitor visitor) => ((Entry)entry).VisitName(visitor);

            var visitor = Connection.Records.CreateDbTokenVisitor(Command.Locale);
            var builder = Visit(visitor, EntryVisitor);

            if (!builder.IsEmpty) builder.ReplaceText($"({builder.Text})");
            return builder;
        }

        /// <summary>
        /// Visits the values carried by the entries in this instance and returns a command info
        /// object that can be used to build the related clause of the associated command, or an
        /// empty result if no entries are captured.
        /// </summary>
        /// <returns></returns>
        public virtual ICommandInfo.IBuilder VisitValues()
        {
            static ICommandInfo.IBuilder EntryVisitor(
                Fragment.Entry entry, DbTokenVisitor visitor) => ((Entry)entry).VisitValue(visitor);

            var visitor = Connection.Records.CreateDbTokenVisitor(Command.Locale);
            var builder = Visit(visitor, EntryVisitor);

            if (!builder.IsEmpty) builder.ReplaceText($"({builder.Text})");
            return builder;
        }
    }
}