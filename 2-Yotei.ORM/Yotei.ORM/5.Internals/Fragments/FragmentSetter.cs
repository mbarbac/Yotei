#pragma warning disable IDE1006

using System.Runtime.InteropServices.Marshalling;

namespace Yotei.ORM.Internals;

/// <summary>
/// Represents the ability of parsing SETTER clauses.
/// <br/>- Standard syntax: 'x => Target = Value'.
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

            // Literals...
            if (Body is DbTokenLiteral literal)
            {
                var str = literal.Value.Trim();
                var tokenizer = new StrWrappedTokenizer(Engine.LeftTerminator, Engine.RightTerminator);
                var items = Engine.UseTerminators ? tokenizer.Tokenize(str) : new StrTokenText(str);
                if (TryExtract(items, out var target, out var value))
                {
                    Target = new DbTokenLiteral(target);
                    Value = new DbTokenLiteral(value);
                    return;
                }
            }

            // CommandInfo...
            if (Body is DbTokenCommandInfo command)
            {
                var str = command.CommandInfo.Text.Trim();
                var tokenizer = new StrWrappedTokenizer(Engine.LeftTerminator, Engine.RightTerminator);
                var items = Engine.UseTerminators ? tokenizer.Tokenize(str) : new StrTokenText(str);
                if (TryExtract(items, out var target, out var value))
                {
                    if (ContainsParameters(target, command.CommandInfo.Parameters))
                        throw new ArgumentException(
                            $"Target {Clause} part cannot contain parameters.")
                            .WithData(Body);

                    Target = new DbTokenLiteral(target);

                    var info = new CommandInfo(Engine, value, command.CommandInfo.Parameters);
                    Value = new DbTokenCommandInfo(info);

                    return;
                }
            }

            bool ContainsParameters(string str, IParameterList pars)
            {
                for (int i = 0; i < pars.Count; i++)
                {
                    var par = pars[i];
                    var index = par.Name.FindIsolated(str, 0, StringComparison.OrdinalIgnoreCase);
                    if (index >= 0) return true;
                }
                return false;
            }


            /*
             // Invoked to determine if there are brackets in the given text...
            static bool RemainingBrackets(string str)
            {
                var ini = str.IndexOf('{'); if (ini < 0) return false;
                var end = str.IndexOf('}', ini); if (end < 0) return false;
                return true;
            }
             */

            // Any other token is considered and invalid one...
            throw new ArgumentException(
                $"{Clause} entry has not the appropriate 'Target=Value' format.")
                .WithData(Body);

            // Invoke to extract the target and value parts...
            bool TryExtract(IStrToken items, out string target, out string value)
            {
                var (left, right) = items.ExtractFirst("=", sensitive: false, out var found);
                if (found)
                {
                    var str = left.ToString()!.Trim();
                    if (str.StartsWith('(') && !str.EndsWith(')')) str = str[1..];
                    if (!str.StartsWith('(') && str.EndsWith(')')) str = str[..^1];
                    str = str.NotNullNotEmpty(trim: true, $"Target {Clause} part.");
                    target = str;

                    str = right!.ToString()!.Trim();
                    if (str.StartsWith('(') && !str.EndsWith(')')) str = str[1..];
                    if (!str.StartsWith('(') && str.EndsWith(')')) str = str[..^1];
                    str = str.NotNullNotEmpty(trim: true, $"Target {Clause} part.");
                    value = str;

                    return true;
                }

                target = null!;
                value = null!;
                return false;
            }
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

        // ------------------------------------------------

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
            var value = visitor.Visit(Value);
            var str = value.Text.UnWrap('(', ')');
            value.ReplaceText(str);

            var target = visitor.Visit(Target);
            target.Add(" = ");
            target.Add(value);
            target.ReplaceText($"({target.Text})");
            if (separate) target.ReplaceText($", {target.Text}");
            return target;
        }

        /// <summary>
        /// Visits the name carried by this instance and returns a command info object that can
        /// be used to build the related clause of the associated command. If an empty instance
        /// is returned, then it is ignored.
        /// <br/> The '<paramref name="separate"/>' argument indicates if this instance needs to
        /// include an appropriate separator between its contents and previous ones.
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
        /// Visits the value carried by this instance and returns a command info object that can
        /// be used to build the related clause of the associated command. If an empty instance
        /// is returned, then it is ignored.
        /// <br/> The '<paramref name="separate"/>' argument indicates if this instance needs to
        /// include an appropriate separator between its contents and previous ones.
        /// </summary>
        /// <param name="visitor"></param>
        /// <param name="separate"></param>
        /// <returns></returns>
        public virtual ICommandInfo.IBuilder VisitValue(DbTokenVisitor visitor, bool separate)
        {
            var builder = visitor.Visit(Value);
            var str = builder.Text.UnWrap('(', ')');
            builder.ReplaceText(str);

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
        /// Visits the names in this instance and returns a command info object that can be used
        /// to build the related clause of the associated command. If this instance contains no
        /// entries, then an empty result is returned.
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
        /// Visits the values in this instance and returns a command info object that can be used
        /// to build the related clause of the associated command. If this instance contains no
        /// entries, then an empty result is returned.
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
