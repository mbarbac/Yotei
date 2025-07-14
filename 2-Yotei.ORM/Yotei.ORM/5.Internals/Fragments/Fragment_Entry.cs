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
        /// <br/> String-valued bodies as 'x => str' or 'x => x(str)' are translated to literal
        /// tokens.
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
                    switch (invoke.Arguments[0])
                    {
                        case DbTokenArgument arg: value = arg; break;
                        case DbTokenLiteral literal: value = literal; break;
                        case DbTokenValue temp:
                            if (temp.Value is string str) value = new DbTokenLiteral(str);
                            break;
                    }
                }

                // First-level values...
                if (value is DbTokenValue item)
                {
                    if (item.Value is string str) value = new DbTokenLiteral(str);
                    else throw new ArgumentException(
                        "Not string raw values are not valid fragment entries.")
                        .WithData(value);
                }

                // Empty literals are not acceptable...
                if (value is DbTokenLiteral text && text.Value.Length == 0)
                    throw new ArgumentException(
                        "Literal bodies cannot just be empty.").WithData(Body);

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
        /// Determines if the given text contains any encoded parameter name from the given ones.
        /// </summary>
        /// <param name="text"></param>
        /// <param name="ini"></param>
        /// <param name="pars"></param>
        /// <returns></returns>
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