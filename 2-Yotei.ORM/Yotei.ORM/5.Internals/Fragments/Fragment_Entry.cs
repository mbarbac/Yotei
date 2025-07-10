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

            // First-level single-parameter special invokes...
            if (Body is DbTokenInvoke invoke &&
                invoke.Host is DbTokenArgument &&
                invoke.Arguments.Count == 1)
            {
                if (invoke.Arguments[0] is DbTokenLiteral literal)
                    Body = literal;
                else if (
                    invoke.Arguments[0] is DbTokenValue value &&
                    value.Value is string str)
                    Body = new DbTokenLiteral(str);
                else if (
                    invoke.Arguments[0] is DbTokenArgument argument)
                    Body = argument;
            }

            // Argument tokens are not acceptable...
            if (Body is DbTokenArgument) throw new ArgumentException(
                $"Body of {Clause} clause cannot just be a dynamic argument.").WithData(body);

            // Any other ones, even empty ones, are in principle acceptable...
            return;
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

        /// <inheritdoc/>
        public override string ToString() => Body.ToString() ?? string.Empty;

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

            protected set
            {
                value.ThrowWhenNull();

                if (value is DbTokenArgument)
                    throw new ArgumentException(
                        "Fragment body cannot just be a dynamic argument.").WithData(value);

                _Body = value;
            }
        }
        IDbToken _Body = default!;

        // ------------------------------------------------

        /// <summary>
        /// Visits the contents of this instance and returns a command info object that represents
        /// the current entry in the related clause of the command.
        /// </summary>
        /// <param name="visitor"></param>
        /// <returns></returns>
        public virtual ICommandInfo.IBuilder Visit(DbTokenVisitor visitor) => visitor.Visit(Body);
    }
}