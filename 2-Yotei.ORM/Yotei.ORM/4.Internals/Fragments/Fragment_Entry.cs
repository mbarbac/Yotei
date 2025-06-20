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
        /// Returns a descriptor of the clause this instance is built for.
        /// <br/> This property is mainly used for informational purposes.
        /// </summary>
        public abstract string CLAUSE { get; }

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
        public override string ToString()
        {
            var visitor = Connection.Records.CreateDbTokenVisitor(Command.Locale);
            var builder = Visit(visitor);
            return builder.ToString()!;
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
        public IDbToken Body { get; }

        /// <summary>
        /// The head element carried by this instance, or <c>null</c> if any. If not null, then
        /// its arguments contain its actual contents.
        /// </summary>
        public DbTokenInvoke? Head
        {
            get => _Head;
            init => _Head = value;
        }
        internal DbTokenInvoke? _Head;

        /// <summary>
        /// The tail element carried by this instance, or <c>null</c> if any. If not null, then
        /// its arguments contain its actual contents.
        /// </summary>
        public DbTokenInvoke? Tail
        {
            get => _Tail;
            init => _Tail = value;
        }
        internal DbTokenInvoke? _Tail;

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