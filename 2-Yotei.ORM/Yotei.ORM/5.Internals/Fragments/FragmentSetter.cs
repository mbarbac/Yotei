#pragma warning disable IDE1006

namespace Yotei.ORM.Internals;

/// <summary>
/// Represents the ability of parsing SETTER clauses.
/// <br/>- Standard syntax: 'x => Target = Value'.
/// <br/>- Literal syntax: 'x => "Target = Value"'.
/// </summary>
/// <remarks>
/// SETTER entries are special in the sense that they must always have a 'Target=Value' format,
/// which are kept in the corresponding properties. This is because we have the 'VisitName' and
/// the 'VisitValue' methods that require valid target and value parts.
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

            // Literal...
            if (Body is DbTokenLiteral literal)
            {
                var main = literal.Value;
                if (ExtractSeparator(main, "=", Engine, out var left, out var right, out _))
                {
                    Target = new DbTokenLiteral(left);
                    Value = new DbTokenLiteral(right);
                    return;
                }
            }

            // Command-info...
            if (Body is DbTokenCommandInfo command)
            {
                var main = command.CommandInfo.Text;
                if (ExtractSeparator(main, "=", Engine, out var left, out var right, out _))
                {
                    if (ContainsAnyParameter(left, 0, command.CommandInfo.Parameters))
                        throw new ArgumentException(
                            "Target part cannot contain any encoded parameter.")
                            .WithData(left)
                            .WithData(Body);

                    var info = new CommandInfo(Engine, right, command.CommandInfo.Parameters);

                    Target = new DbTokenLiteral(left);
                    Value = new DbTokenCommandInfo(info);
                    return;
                }
            }

            // Any other token is considered and invalid one...
            throw new ArgumentException(
                $"Entry has not an appropriate setter format.")
                .WithData(Body);
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
        public override ICommandInfo.IBuilder Visit(DbTokenVisitor visitor, bool separate)
        {
            var target = visitor.Visit(Target);
            var value = visitor.Visit(Value);
            
            target.Add(" = ");
            target.Add(value);
            target.ReplaceText($"({target.Text})");
            if (separate) target.ReplaceText($", {target.Text}");
            return target;
        }

        /// <summary>
        /// Visits the name of this instance and returns a command info builder object that can
        /// be used to build the related clause of the associated command.
        /// <br/> The '<paramref name="separate"/>' argument indicates if the contents of this
        /// instance may need a separation from any previous ones.
        /// </summary>
        /// <param name="visitor"></param>
        /// <param name="separate"></param>
        /// <returns></returns>
        public virtual ICommandInfo.IBuilder VisitName(DbTokenVisitor visitor, bool separate)
        {
            var builder = visitor.Visit(Target);

            if (separate) builder.ReplaceText($", {builder.Text}");
            return builder;
        }

        /// <summary>
        /// Visits the value of this instance and returns a command info builder object that can
        /// be used to build the related clause of the associated command.
        /// <br/> The '<paramref name="separate"/>' argument indicates if the contents of this
        /// instance may need a separation from any previous ones.
        /// </summary>
        /// <param name="visitor"></param>
        /// <param name="separate"></param>
        /// <returns></returns>
        public virtual ICommandInfo.IBuilder VisitValue(DbTokenVisitor visitor, bool separate)
        {
            var builder = visitor.Visit(Value);

            if (separate) builder.ReplaceText($", {builder.Text}");
            return builder;
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
        public override ICommandInfo.IBuilder Visit()
        {
            var builder = base.Visit();

            if (Count > 1 && builder.TextLen > 0) builder.ReplaceText($"({builder.Text})");
            return builder;
        }

        /// <summary>
        /// Visits the names in this instance and returns a command info builder object that
        /// can be used to build the related clause of the associated command.
        /// </summary>
        /// <returns></returns>
        public virtual ICommandInfo.IBuilder VisitNames()
        {
            var visitor = Connection.Records.CreateDbTokenVisitor(Command.Locale);
            static ICommandInfo.IBuilder EntryVisitor(
                Fragment.Entry entry, DbTokenVisitor visitor, bool separate)
                => ((Entry)entry).VisitName(visitor, separate);

            var builder = VisitEntries(visitor, EntryVisitor);
            if (!builder.IsEmpty) builder.ReplaceText($"({builder.Text})");
            return builder;
        }

        /// <summary>
        /// Visits the values in this instance and returns a command info builder object that
        /// can be used to build the related clause of the associated command.
        /// </summary>
        /// <returns></returns>
        public virtual ICommandInfo.IBuilder VisitValues()
        {
            var visitor = Connection.Records.CreateDbTokenVisitor(Command.Locale);
            static ICommandInfo.IBuilder EntryVisitor(
                Fragment.Entry entry, DbTokenVisitor visitor, bool separate)
                => ((Entry)entry).VisitValue(visitor, separate);

            var builder = VisitEntries(visitor, EntryVisitor);
            if (!builder.IsEmpty) builder.ReplaceText($"({builder.Text})");
            return builder;
        }
    }
}
