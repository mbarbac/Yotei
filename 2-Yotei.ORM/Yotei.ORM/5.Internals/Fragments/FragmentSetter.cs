#pragma warning disable IDE1006

namespace Yotei.ORM.Internals;

/// <summary>
/// Represents the ability of parsing SETTER clauses.
/// <br/>- Standard syntax: 'x => Target = Value'.
/// <br/>- Literal syntax: 'x => x("target = value")'.
/// </summary>
/// <remarks>
/// SETTER entries are special in the sense that they must always have a 'Target=Value' format.
/// Reason is that we have the 'VisitName' and 'VisitValue' methods that require a valid target
/// and value part.
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
            // Standard setter...
            if (Body is DbTokenSetter setter)
            {
                Target = setter.Target;
                Value = setter.Value;
                return;
            }

            // Literals...
            if (Body is DbTokenLiteral literal)
            {
                var str = literal.Value.Trim();

                var tokenizer = new StrWrappedTokenizer(Engine.LeftTerminator, Engine.RightTerminator);
                var items = Engine.UseTerminators ? tokenizer.Tokenize(str) : new StrTokenText(str);
                var (left, right) = items.ExtractFirst("=", false, out var found);
                if (found)
                {
                    str = left.ToString()!.Trim();
                    if (str.StartsWith('(') && !str.EndsWith(')')) str = str[1..];
                    if (!str.StartsWith('(') && str.EndsWith(')')) str = str[..^1];
                    str = str.NotNullNotEmpty(trim: true, $"Target {Clause} part.");
                    Target = new DbTokenLiteral(str);

                    str = right!.ToString()!.Trim();
                    if (str.StartsWith('(') && !str.EndsWith(')')) str = str[1..];
                    if (!str.StartsWith('(') && str.EndsWith(')')) str = str[..^1];
                    str = str.NotNullNotEmpty(trim: true, $"Target {Clause} part.");
                    Value = new DbTokenLiteral(str);

                    return;
                }
            }

            // Any other token is considered an invalid one...
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
            return visitor.Visit(Target);
        }

        /// <summary>
        /// Visits the value carried by this instance and returns a command info object that
        /// represents the current entry in the related clause of the command.
        /// </summary>
        /// <param name="visitor"></param>
        /// <returns></returns>
        public virtual ICommandInfo.IBuilder VisitValue(DbTokenVisitor visitor)
        {
            return visitor.Visit(Value);
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

        // ------------------------------------------------

        /// <inheritdoc/>
        public override Entry CreateEntry(IDbToken body) => new(this, body);

        // ------------------------------------------------

        /// <inheritdoc/>
        protected override string? EntrySeparator(Fragment.Entry entry) => ", ";

        /// <inheritdoc/>
        public override ICommandInfo.IBuilder Visit()
        {
            var builder = base.Visit();

            if (Count > 1 && builder.TextLen > 0) builder.ReplaceText($"({builder.Text})");
            return builder;
        }

        /// <summary>
        /// Visits the names carried by the entries in this instance and returns a command info
        /// object that can be used to build the related clause of the associated command, or an
        /// empty result if no entries are captured.
        /// </summary>
        /// <returns></returns>
        public virtual ICommandInfo.IBuilder VisitNames()
        {
            var visitor = Connection.Records.CreateDbTokenVisitor(Command.Locale);
            static ICommandInfo.IBuilder EntryVisitor(
                Fragment.Entry entry, DbTokenVisitor visitor) => ((Entry)entry).VisitName(visitor);

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
            var visitor = Connection.Records.CreateDbTokenVisitor(Command.Locale);
            static ICommandInfo.IBuilder EntryVisitor(
                Fragment.Entry entry, DbTokenVisitor visitor) => ((Entry)entry).VisitValue(visitor);

            var builder = Visit(visitor, EntryVisitor);
            if (!builder.IsEmpty) builder.ReplaceText($"({builder.Text})");
            return builder;
        }
    }
}