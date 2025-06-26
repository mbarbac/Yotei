using System.Security.Cryptography;

namespace Yotei.ORM.Internals;

/// <summary>
/// Represents the ability of parsing WHERE clauses.
/// <br/>- Standard syntax: 'x => Expression'.
/// <br/>- Alternate syntax: 'x => x.And(...)'.
/// <br/>- Alternate syntax: 'x => x.Or(...)'.
/// <br/> Note that using an entry without an appropriate AND or OR connector may be invalid SQL
/// syntax.
/// </summary>
public static partial class FragmentWhere
{
    // ====================================================
    /// <summary>
    /// Represents an entry in a collection of fragments used to build WHERE-alike clauses.
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
            if (body is DbTokenLiteral literal)
            {
                string value;
                string main = literal.Value; // DO NOT trim here!

                UseOR = null;
                value = "OR "; if (ExtractFirst(ref main, ref value, false)) UseOR = true;
                value = "AND "; if (ExtractFirst(ref main, ref value, false)) UseOR = false;

                if (UseOR is not null) Body = new DbTokenLiteral(main.NotNullNotEmpty());
            }
            else if (body is DbTokenSetter setter) // Used '=' instead of '=='...
            {
                Body = new DbTokenBinary(setter.Target, ExpressionType.Equal, setter.Value);
            }
        }

        /// <summary>
        /// Copy constructor.
        /// </summary>
        /// <param name="source"></param>
        protected Entry(Entry source) : base(source) => UseOR = source.UseOR;

        /// <summary>
        /// If not null, whether to chain this instance with a previous one using an "OR"
        /// connector, or a default "AND" one. If the value of this property is <c>null</c>,
        /// then it is ignored.
        /// </summary>
        public bool? UseOR
        {
            get => _UseOR;
            init => _UseOR = value;
        }
        internal bool? _UseOR;

        // ------------------------------------------------

        /// <inheritdoc/>
        public override string ToString() => UseOR is not null
            ? $"{(UseOR.Value ? "OR" : "AND")} {Body}"
            : $"{Body}";

        /// <inheritdoc/>
        public override ICommandInfo.IBuilder Visit(DbTokenVisitor visitor)
        {
            var builder = visitor.Visit(Body);

            if (UseOR is not null)
            {
                var str = UseOR.Value ? "OR" : "AND";
                builder.ReplaceText($"{str} {builder.Text}");
            }
            return builder;
        }
    }

    // ====================================================
    /// <summary>
    /// Represents a collection of fragments used to build WHERE-alike clauses.
    /// </summary>
    [Cloneable]
    public partial class Master : Fragment.Master
    {
        /// <summary>
        /// Initializes a new instance.
        /// </summary>
        /// <param name="command"></param>
        /// <param name="clause"></param>
        public Master(ICommand command, string clause = "WHERE") : base(command, clause) { }

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
        public override Entry CreateEntry(IDbToken body)
        {
            // Intercepting 'x => x.And(...)' and 'x => x.Or(...)' methods...
            bool? useOr = null;
            var item = body.RemoveFirst(x =>
            {
                if (x is not DbTokenMethod method) return false;
                if (method.Host is not DbTokenArgument) return false;
                if (method.Name.ToUpper() is not "AND" and not "OR") return false;
                return true;
            }
            , out var removed);

            if (removed is not null) // Found...
            {
                var method = (DbTokenMethod)removed;
                var name = method.Name;
                var upper = name.ToUpper();

                if (method.TypeArguments.Length != 0) throw new ArgumentException(
                    $"No type arguments allowed for '{upper}()' virtual method.")
                    .WithData(body);

                if (method.Arguments.Count == 1)
                {
                    if (item is not DbTokenArgument) throw new ArgumentException(
                        $"Remaining after '{upper}()' must be empty.")
                        .WithData(body);

                    if (upper == "OR") useOr = true;
                    else if (upper == "AND") useOr = false;
                    else useOr = null;

                    body = method.Arguments[0];
                }
                else
                {
                    throw new ArgumentException(
                        $"'{upper}(...) must have just one argument.")
                        .WithData(body);
                }
            }

            // Finishing...
            var entry = new Entry(this, body);

            if (useOr is not null) entry._UseOR = useOr;
            return entry;
        }

        // ------------------------------------------------

        /// <inheritdoc/>
        protected override string? EntrySeparator(Fragment.Entry entry)
        {
            var valid = (Entry)entry;
            var index = IndexOf(valid);

            if (index > 0 && valid.UseOR is not null) return " ";
            return null;
        }
    }
}
