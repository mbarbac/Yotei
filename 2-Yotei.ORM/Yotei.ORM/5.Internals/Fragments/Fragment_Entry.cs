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

                if (value is DbTokenLiteral literal && literal.Value.Length == 0)
                    throw new ArgumentException(
                        "Fragment literal body cannot be empty.").WithData(value);

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

        // ------------------------------------------------

        /// <summary>
        /// Determines if the given source starts with the given value and, if so, extracts that
        /// value from the source, and then trims both strings which may become empty ones.
        /// <br/> If not, just returns false and does no trimming.
        /// <br/> By default, values are treated not case sensitive.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="value"></param>
        /// <param name="sensitive"></param>
        /// <returns></returns>
        protected static bool ExtractFromHead(
            ref string source, ref string value, bool sensitive = false)
        {
            var index = source.IndexOf(value, sensitive);
            if (index == 0)
            {
                var len = value.Length;
                source = source[len..];

                source = source.Trim();
                value = value.Trim();
                return true;
            }

            return false;
        }

        /// <summary>
        /// Determines if the given source ends with the given value and, if so, extracts that
        /// value from the source, and then trims both strings which may become empty ones.
        /// <br/> If not, just returns false and does no trimming.
        /// <br/> By default, values are treated not case sensitive.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="value"></param>
        /// <param name="sensitive"></param>
        /// <returns></returns>
        protected static bool ExtractFromTail(
            ref string source, ref string value, bool sensitive = false)
        {
            var index = source.LastIndexOf(value, sensitive);
            if (index > 0 && (index + value.Length) == source.Length)
            {
                source = source[..index];

                source = source.Trim();
                value = value.Trim();
                return true;
            }

            return false;
        }
    }
}