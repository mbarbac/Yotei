namespace Yotei.ORM.Internals;

public static partial class Fragment
{
    // ====================================================
    /// <summary>
    /// Represents an entry in a collection of fragments used to build a specific clause of the
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
        /// The master collection this instance is associated with.
        /// </summary>
        public Master Master { get; internal set; }
        protected ICommand Command => Master.Command;
        protected IConnection Connection => Command.Connection;
        protected IEngine Engine => Connection.Engine;

        /// <summary>
        /// The actual contents carried by this instance.
        /// </summary>
        public IDbToken Body
        {
            get => _Body;
            protected set
            {
                _Body = value.ThrowWhenNull();

                if (value is DbTokenArgument) throw new ArgumentException(
                    "Body of a fragment cannot be just a dynamic argument.").WithData(value);

                if (value is DbTokenLiteral literal && literal.Value.Length == 0)
                    throw new EmptyException("Literal body cannot be empty.").WithData(value);
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
        /// value from the source and then trims both the source and the value. If not found,
        /// then just returns false without any trimming.
        /// </summary>
        protected static bool ExtractFirst(ref string source, ref string value, bool caseSensitive)
        {
            var index = source.IndexOf(value, caseSensitive);
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
        /// value from the source and then trims both the source and the value. If not found,
        /// then just returns false without any trimming.
        /// </summary>
        protected static bool ExtractLast(ref string source, ref string value, bool caseSensitive)
        {
            var index = source.IndexOf(value, caseSensitive);
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

