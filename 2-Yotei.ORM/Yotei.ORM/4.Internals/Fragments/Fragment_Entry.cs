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
            Head = source.Head?.Clone();
            Tail = source.Tail?.Clone();
        }

        /// <inheritdoc/>
        public override string ToString() => $"{Body}";

        // ------------------------------------------------

        /// <summary>
        /// Determines if the given source starts with the given value and, if so, extracts that
        /// value from the source and trims all results.
        /// </summary>
        protected static bool TryExtractFirst(ref string source, ref string value, bool caseSensitive)
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
        /// value from the source and trims all results.
        /// </summary>
        protected static bool TryExtractLast(ref string source, ref string value, bool caseSensitive)
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

        // ------------------------------------------------

        /// <summary>
        /// The master collection this instance is associated with.
        /// </summary>
        public Master Master { get; internal set; }
        protected ICommand Command => Master.Command;
        protected IConnection Connection => Command.Connection;
        protected IEngine Engine => Connection.Engine;

        /// <summary>
        /// The actual body of contents carried by this instance.
        /// </summary>
        public IDbToken Body
        {
            get => _Body;

            protected set
            {
                value.ThrowWhenNull();

                if (value is DbTokenArgument) throw new ArgumentException(
                    "Fragment body cannot be just a dynamic argument.")
                    .WithData(value);

                if (value is DbTokenLiteral literal && literal.Value.Length == 0)
                    throw new ArgumentException(
                        "Literal fragment body cannot be empty.")
                        .WithData(value);

                _Body = value;
            }
        }
        IDbToken _Body = default!;

        /// <summary>
        /// The head element carried by this instance, or <c>null</c> if any. If not null, then
        /// its arguments contain its actual contents.
        /// </summary>
        public DbTokenInvoke? Head
        {
            get => _Head;
            init => _Head = value;
        }
        internal DbTokenInvoke? _Head; // internal, because 'init' is a bit too restrictive

        /// <summary>
        /// The tail element carried by this instance, or <c>null</c> if any. If not null, then
        /// its arguments contain its actual contents.
        /// </summary>
        public DbTokenInvoke? Tail
        {
            get => _Tail;
            init => _Tail = value;
        }
        internal DbTokenInvoke? _Tail; // internal, because 'init' is a bit too restrictive

        // ------------------------------------------------

        /// <summary>
        /// Visits the contents of this instance and returns a command info object that represents
        /// the current sentence in the related clause of the command.
        /// </summary>
        /// <param name="visitor"></param>
        /// <returns></returns>
        public virtual ICommandInfo.IBuilder Visit(DbTokenVisitor visitor)
        {
            visitor.ThrowWhenNull();

            var builder = VisitHead(visitor);
            var temp = VisitBody(visitor); builder.Add(temp);
            temp = VisitTail(visitor); builder.Add(temp);

            return builder;
        }

        // ------------------------------------------------

        /// <summary>
        /// Invoked to visit the head of this instance. If <see cref="Head"/> is null, then
        /// this method shall return an empty instance.
        /// </summary>
        protected virtual ICommandInfo.IBuilder VisitHead(DbTokenVisitor visitor)
        {
            return Head is null ? new CommandInfo.Builder(Engine) : visitor.Visit(Head);
        }

        /// <summary>
        /// Invoked to visit the body of this instance.
        /// </summary>
        protected virtual ICommandInfo.IBuilder VisitBody(DbTokenVisitor visitor)
        {
            return visitor.Visit(Body);
        }

        /// <summary>
        /// Invoked to visit the tail of this instance. If <see cref="Tail"/> is null, then
        /// this method shall return an empty instance.
        /// </summary>
        protected virtual ICommandInfo.IBuilder VisitTail(DbTokenVisitor visitor)
        {
            return Tail is null ? new CommandInfo.Builder(Engine) : visitor.Visit(Tail);
        }
    }
}