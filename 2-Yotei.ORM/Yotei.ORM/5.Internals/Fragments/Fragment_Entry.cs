namespace Yotei.ORM.Internals;

static partial class Fragment
{
    // ====================================================
    /// <summary>
    /// Represents an entry in a collection of fragments used to build a clause of an associated
    /// command. They essentially wrap token instances that can be later visited to produce the
    /// relevant command-info object.
    /// </summary>
    public abstract class Entry
    {
        /// <summary>
        /// Initializes a new instance.
        /// </summary>
        /// <param name="master"></param>
        /// <param name="token"></param>
        public Entry(Master master, IDbToken token)
        {
            Master = master.ThrowWhenNull();
            Body = token.ThrowWhenNull();
        }

        /// <summary>
        /// Copy constructor.
        /// </summary>
        /// <param name="source"></param>
        protected Entry(Entry source)
        {
            source.ThrowWhenNull();

            Master = source.Master;
            _Body = source._Body;
        }

        /// <inheritdoc cref="ICloneable.Clone"/>
        public abstract Entry Clone();

        /// <inheritdoc/>
        public override string ToString() => Body.ToString() ?? string.Empty;

        // ------------------------------------------------

        /// <summary>
        /// The master collection this instance belongs to.
        /// </summary>
        public Master Master { get; internal set; }
        protected string Descriptor => Master.Descriptor;
        protected ICommand Command => Master.Command;
        protected IConnection Connection => Command.Connection;
        protected IEngine Engine => Connection.Engine;
        protected StringComparison Comparison => Engine.CaseSensitiveNames
            ? StringComparison.Ordinal
            : StringComparison.OrdinalIgnoreCase;

        /// <summary>
        /// The actual body carried by this instance.
        /// </summary>
        public IDbToken Body
        {
            get => _Body;
            init => _Body = ParseBody(value).ThrowWhenNull();
        }
        IDbToken _Body = default!;

        // ------------------------------------------------

        /// <summary>
        /// Invoked to parse and return what the body carried by this instance will be, maybe
        /// extracting and validating other contents before returning. Inheritors may want to
        /// call this base method that performs some cleaning and transformations.
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        protected virtual IDbToken ParseBody(IDbToken token)
        {
            token.ThrowWhenNull();

            // First-level single-parameter invokes
            if (token is DbTokenInvoke invoke &&
                invoke.Host is DbTokenArgument &&
                invoke.Arguments.Count == 1)
            {
                switch (invoke.Arguments[0])
                {
                    case DbTokenArgument arg: token = arg; break;
                    case DbTokenLiteral text: token = text; break;
                    case DbTokenValue temp:
                        if (temp.Value is string str) token = new DbTokenLiteral(str);
                        break;
                }
            }

            // First-level values...
            if (token is DbTokenValue value)
            {
                if (value.Value is string str) token = new DbTokenLiteral(str);
                else throw new ArgumentException(
                    "Raw values that are not string are not valid fragment entries.")
                    .WithData(value);
            }

            // Argument tokens...
            if (token is DbTokenArgument) throw new ArgumentException(
                "Argument tokens are not acceptable as fragment bodies.")
                .WithData(token);

            // Literal tokens...
            if (token is DbTokenLiteral literal)
            {
                var main = literal.Value;
                if (main.Length == 0) throw new ArgumentException(
                    "Empty literal bodies are not acceptable as fragment bodies.")
                    .WithData(token);

                if (main.Contains(Environment.NewLine))
                {
                    main = main.Replace(Environment.NewLine, " ");
                    token = new DbTokenLiteral(main);
                }
                else if (main.Contains('\n'))
                {
                    main = main.Replace('\n', ' ');
                    token = new DbTokenLiteral(main);
                }
            }

            // Command-info tokens...
            if (token is DbTokenCommandInfo command)
            {
                var info = command.CommandInfo;
                var main = info.Text;

                if (main.Contains(Environment.NewLine))
                {
                    main = main.Replace(Environment.NewLine, " ");
                    info = new CommandInfo(Engine, main, info.Parameters);
                    token = new DbTokenCommandInfo(info);
                }
                else if (main.Contains('\n'))
                {
                    main = main.Replace('\n', ' ');
                    info = new CommandInfo(Engine, main, info.Parameters);
                    token = new DbTokenCommandInfo(info);
                }
            }

            // Finishing...
            return token;
        }

        // ------------------------------------------------

        /// <summary>
        /// Visits the contents of this instance and returns a command info builder object that
        /// can be used to build the related clause in the associated command.
        /// <br/> The <paramref name="separate"/> argument indicates if this entry needs to be
        /// separated from any previous ones.
        /// </summary>
        /// <param name="visitor"></param>
        /// <param name="separate"></param>
        /// <returns></returns>
        public abstract ICommandInfo.IBuilder Visit(DbTokenVisitor visitor, bool separate);
    }
}