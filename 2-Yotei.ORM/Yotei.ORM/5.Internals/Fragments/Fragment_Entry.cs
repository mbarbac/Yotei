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
                        arg is DbTokenValue temp && temp.Value is string tstr &&
                        LiteralBodyFromStringValue(tstr, true))
                        value = new DbTokenLiteral(tstr);
                }

                // First-level string values...
                if (value is DbTokenValue item && item.Value is string str &&
                    LiteralBodyFromStringValue(str, false))
                    value = new DbTokenLiteral(str);

                // Dynamic argument bodies are not acceptable...
                if (value is DbTokenArgument) throw new ArgumentException(
                    "Fragment bodies cannot just be a dynamic argument.").WithData(value);

                // Any other tokens, even empty literal ones, are acceptable in principle...
                __Body = value;
            }
        }
        IDbToken __Body = default!;

        /// <summary>
        /// Determines if the string value passed to this instance shall be translated into a
        /// literal token, or not. The <paramref name="invoke"/> argument is set to <c>true</c>
        /// when that string value was obtained from an invoke token, or to <c>false</c> when it
        /// was obtained from a straight value one.
        /// </summary>
        /// <param name="str"></param>
        /// <param name="invoke"></param>
        /// <returns></returns>
        protected virtual bool LiteralBodyFromStringValue(string str, bool invoke) => true;

        // ------------------------------------------------

        /// <inheritdoc/>
        public override string ToString() => Body.ToString() ?? string.Empty;

        /// <summary>
        /// Visits the contents of this instance and returns a command info object that can be
        /// used to build the related clause of the associated command. If an empty instance is
        /// returned, then it is ignored.
        /// <br/> The '<paramref name="separate"/>' argument indicates if this instance needs to
        /// include an appropriate separator between its contents and previous ones.
        /// </summary>
        /// <param name="visitor"></param>
        /// <param name="separate"></param>
        /// <returns></returns>
        public virtual ICommandInfo.IBuilder Visit(
            DbTokenVisitor visitor, bool separate) => visitor.Visit(Body);
    }
}