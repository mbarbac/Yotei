#pragma warning disable IDE1006

namespace Yotei.ORM.Internals;

/// <summary>
/// Represents the ability of parsing WHERE clauses.
/// <br/>- Standard syntax: 'x => Expression'.
/// <br/>- Alternate syntax: 'x => x.And(...)'.
/// <br/>- Alternate syntax: 'x => x.Or(...)'.
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

            // Setter translated to binary comparison, for convenience...
            if (body is DbTokenSetter setter)
            {
                Body = new DbTokenBinary(setter.Target, ExpressionType.Equal, setter.Value);
                body = Body;
            }

            // Literals...
            if (body is DbTokenLiteral literal)
            {
                var comparison = StringComparison.OrdinalIgnoreCase;
                var main = literal.Value;
                if (main.EndsWith(" OR", comparison) || main.EndsWith(" AND", comparison))
                    throw new ArgumentException("Literal body ends with a connector.").WithData(body);

                // Extracting heading connector, if any...
                main = main.NotNullNotEmpty(trim: true);
                string str;

                UseAND = null;
                str = "AND "; if (ExtractFromHead(ref main, ref str)) UseAND = true;
                str = "OR "; if (ExtractFromHead(ref main, ref str)) UseAND = false;

                // Transforming 'target==value' into proper 'target=value'...
                main = main.NotNullNotEmpty(trim: true);
                var tokenizer = new StrWrappedTokenizer(Engine.LeftTerminator, Engine.RightTerminator);
                var items = Engine.UseTerminators ? tokenizer.Tokenize(main) : new StrTokenText(main);

                var (left, right) = items.ExtractFirst("==", false, out var found);
                if (found)
                {
                    main = left.ToString()!.Trim();
                    if (main.StartsWith('(') && !main.EndsWith(')')) main = main[1..];
                    if (!main.StartsWith('(') && main.EndsWith(')')) main = main[..^1];
                    main = main.NotNullNotEmpty(trim: true, $"Target {Clause} part.");

                    str = right!.ToString()!.Trim();
                    if (str.StartsWith('(') && !str.EndsWith(')')) str = str[1..];
                    if (!str.StartsWith('(') && str.EndsWith(')')) str = str[..^1];
                    str = str.NotNullNotEmpty(trim: true, $"Value {Clause} part.");

                    Body = new DbTokenLiteral(main + str);
                    return;
                }
            }

            // Any other token instance is just accepted...
            return;
        }

        /// <summary>
        /// Copy constructor.
        /// </summary>
        /// <param name="source"></param>
        protected Entry(Entry source) : base(source) => UseAND = source.UseAND;

        /// <summary>
        /// If not <c>null</c>, whether this instance chains with any previous ones using an "AND"
        /// connector, or an "OR" one. If null, no connector is injected.
        /// </summary>
        public bool? UseAND
        {
            get => _UseAND;
            init => _UseAND = value;
        }
        internal protected bool? _UseAND
        {
            get => __UseAND;
            set => __UseAND = value;
        }
        bool? __UseAND;

        // ------------------------------------------------

        /// <inheritdoc/>
        public override string ToString()
        {
            if (UseAND is not null)
            {
                var str = UseAND.Value ? "AND" : "OR";
                return $"{str} {Body}";
            }
            return Body.ToString()!;
        }
            
            
            //=> UseAND is not null
            //? $"{(UseAND.Value ? "AND" : "OR")} {Body}"
            //: Body.ToString()!;

        /// <inheritdoc/>
        public override ICommandInfo.IBuilder Visit(DbTokenVisitor visitor)
        {
            var builder = visitor.Visit(Body);

            if (UseAND is not null)
            {
                var str = UseAND.Value ? "AND" : "OR";
                builder.ReplaceText($"{str} {Body}");
            }
            return builder;
        }
    }

    // ====================================================
    /// <summary>
    /// Represents the collection of fragments used to build WHERE-alike clauses.
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

        /// <inheritdoc/>
        public override Entry CreateEntry(IDbToken body)
        {
            // Intercepting first-level 'x.And()' and 'x.Or()' methods...
            bool? useAnd = null;
            var item = body.ExtractFirst(x =>
            {
                if (x is not DbTokenMethod method) return false;
                if (method.Host is not DbTokenArgument) return false;
                if (method.Name.ToUpper() is not "AND" and not "OR") return false;
                return true;
            }
            , out var removed); if (removed is not null)
            {
                var method = (DbTokenMethod)removed;
                var name = method.Name;
                var upper = name.ToUpper();

                if (method.TypeArguments.Length != 0) throw new ArgumentException(
                    $"No type arguments allowed for '{upper}()' virtual method.")
                    .WithData(body);

                // x => x.Name()...
                if (method.Arguments.Count == 0)
                {
                    useAnd = upper == "AND";
                    body = item;
                }

                // x => x.Name(...)
                else if (method.Arguments.Count == 1)
                {
                    if (item is not DbTokenArgument) throw new ArgumentException(
                        $"Remaining after '{upper}()' must be empty.")
                        .WithData(body);

                    useAnd = upper == "AND";
                    body = method.Arguments[0];
                }

                // Others...
                else throw new ArgumentException(
                    $"'{upper}(...) must have just one argument.")
                    .WithData(body);
            }

            // Finishing...
            var entry = new Entry(this, body);
            if (useAnd is not null) entry._UseAND = useAnd;
            return entry;
        }

        // ------------------------------------------------

        /// <inheritdoc/>
        protected override string? EntrySeparator(Fragment.Entry entry)
        {
            var valid = (Entry)entry;
            var index = IndexOf(entry);

            if (index > 0 && valid.UseAND is not null) return " ";
            return null;
        }
    }
}