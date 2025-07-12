#pragma warning disable IDE1006

namespace Yotei.ORM.Internals;

public static partial class Fragment
{
    // ====================================================
    /// <summary>
    /// Represents an entry in a collection of fragments used to build a given clause of the
    /// associated command.
    /// </summary>
    [Cloneable]
    public abstract partial class Entry
    {
        /// <summary>
        /// Initializes a new instance.
        /// <br/> Bodies that are just a dynamic argument throw an exception.
        /// <br/> Bodies with a 'x => x(str)' format are translated into literal tokens.
        /// <br/> String-valued bodies as 'x => str' are translated into literal tokens.
        /// </summary>
        /// <param name="master"></param>
        /// <param name="body"></param>
        public Entry(Master master, IDbToken body)
        {
            Master = master.ThrowWhenNull();
            Body = body.ThrowWhenNull();
        }

        /// <summary>
        /// Copy constructor.
        /// </summary>
        /// <param name="source"></param>
        protected Entry(Entry source)
        {
            source.ThrowWhenNull();

            Master = source.Master;
            Body = source.Body.Clone();
        }

        // ------------------------------------------------

        /// <summary>
        /// The master collection this instance belongs to.
        /// </summary>
        public Master Master { get; internal set; }
        protected string Clause => Master.Clause;
        protected ICommand Command => Master.Command;
        protected IConnection Connection => Command.Connection;
        protected IEngine Engine => Connection.Engine;

        /// <summary>
        /// The actual body of contents of this instance.
        /// </summary>
        public IDbToken Body
        {
            get => _Body;
            init => _Body = value;
        }
        internal protected IDbToken _Body
        {
            get => __Body;
            set
            {
                value.ThrowWhenNull();

                // First-level single-parameter invokes...
                if (value is DbTokenInvoke invoke &&
                    invoke.Host is DbTokenArgument && invoke.Arguments.Count == 1)
                {
                    var arg = invoke.Arguments[0];

                    if (arg is DbTokenArgument argument) value = argument;

                    else if (arg is DbTokenLiteral literal) value = literal;

                    else if (
                        arg is DbTokenValue temp && temp.Value is string tstr)
                        value = new DbTokenLiteral(tstr);
                }

                // First-level values...
                if (value is DbTokenValue item)
                {
                    if (item.Value is string str) value = new DbTokenLiteral(str);
                    else throw new ArgumentException(
                        "Raw values are not valid fragment entries.")
                        .WithData(value);
                }

                // Dynamic argument bodies are not acceptable...
                if (value is DbTokenArgument) throw new ArgumentException(
                    "Fragment bodies cannot just be a dynamic argument.").WithData(value);

                // Any other tokens, even empty literal ones, are acceptable in principle...
                __Body = value;
            }
        }
        IDbToken __Body = default!;

        // ------------------------------------------------

        /// <inheritdoc/>
        public override string ToString() => Body.ToString() ?? string.Empty;

        /// <summary>
        /// Visits the contents of this instance and returns a command info builder object that
        /// can be used to build the related clause of the associated command.
        /// <br/> The '<paramref name="separate"/>' argument indicates if the contents of this
        /// instance may need a separation from any previous ones.
        /// </summary>
        /// <param name="visitor"></param>
        /// <param name="separate"></param>
        /// <returns></returns>
        public abstract ICommandInfo.IBuilder Visit(DbTokenVisitor visitor, bool separate);

        // ------------------------------------------------

        /// <summary>
        /// Extracts from the head of the given main string the first found spec. If so, both
        /// the main reference and the extracted one are updated.
        /// </summary>
        protected static bool ExtractHead(
            ref string main, ref string extracted, bool sensitive, params string[] specs)
        {
            var comparison = sensitive ? StringComparison.Ordinal : StringComparison.OrdinalIgnoreCase;
            var str = main.Trim();

            for (int i = 0; i < specs.Length; i++)
            {
                var spec = specs[i];
                var index = str.IndexOf(spec, comparison);
                if (index >= 0 && spec.Length == str.Length)
                {
                    extracted = str;
                    main = string.Empty;
                    return true;
                }

                spec += " ";
                index = str.IndexOf(spec, comparison);
                if (index == 0)
                {
                    extracted = str[..spec.Length].Trim();
                    main = str[spec.Length..].Trim();
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Extracts from the tail of the given main string the first found spec. If so, both
        /// the main reference and the extracted one are updated.
        /// </summary>
        protected static bool ExtractTail(
            ref string main, ref string extracted, bool sensitive, params string[] specs)
        {
            var comparison = sensitive ? StringComparison.Ordinal : StringComparison.OrdinalIgnoreCase;
            var str = main.Trim();

            for (int i = 0; i < specs.Length; i++)
            {
                var spec = specs[i];
                var index = str.LastIndexOf(spec, comparison);
                if (index >= 0 && spec.Length == str.Length)
                {
                    extracted = str;
                    main = string.Empty;
                    return true;
                }

                spec = " " + spec;
                index = str.LastIndexOf(spec, comparison);
                if (index >= 0 && (index + spec.Length) == str.Length)
                {
                    extracted = str[index..].Trim();
                    main = str[..index].Trim();
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Extracts from the given main string the left and right parts provided that they are
        /// separated by the given separator, and not protected by the engine terminators, if they
        /// are used.
        protected static bool ExtractSeparator(
            string main, string separator, IEngine engine, out string left, out string right)
        {
            var str = main.Trim();
            var tokenizer = new StrWrappedTokenizer(engine.LeftTerminator, engine.RightTerminator);
            var items = engine.UseTerminators ? tokenizer.Tokenize(str) : new StrTokenText(str);

            var (head, tail) = items.ExtractFirst(separator, engine.CaseSensitiveNames, out var found);
            if (found)
            {
                str = head.ToString()!.Trim();
                if (str.StartsWith('(') && !str.EndsWith(')')) str = str[1..];
                if (!str.StartsWith('(') && str.EndsWith(')')) str = str[..^1];
                str = str.NotNullNotEmpty(trim: true);
                left = str;

                str = tail!.ToString()!.Trim();
                if (str.StartsWith('(') && !str.EndsWith(')')) str = str[1..];
                if (!str.StartsWith('(') && str.EndsWith(')')) str = str[..^1];
                str = str.NotNullNotEmpty(trim: true);
                right = str;

                return true;
            }

            left = null!;
            right = null!;
            return false;
        }

        /// <summary>
        /// Determines if the given text contains encoded any name of the given parameters.
        /// </summary>
        protected static bool ContainsAnyParameter(string text, int ini, IParameterList pars)
        {
            var engine = pars.Engine;
            var comparison = engine.CaseSensitiveNames ? StringComparison.Ordinal : StringComparison.OrdinalIgnoreCase;

            for (int i = 0; i < pars.Count; i++)
            {
                var par = pars[i];
                var index = par.Name.FindIsolated(text, ini, comparison);
                if (index >= 0) return true;
            }

            return false;
        }
    }
}