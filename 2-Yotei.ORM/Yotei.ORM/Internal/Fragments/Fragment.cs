using System.Reflection.PortableExecutable;

namespace Yotei.ORM.Internal;

public partial class Fragment
{
    // ====================================================
    /// <summary>
    /// Represents an entry in a collection of fragments. Its <see cref="Body"/> part refers to
    /// its specific contents, whereas its <see cref="Head"/> and <see cref="Tail"/> ones refer
    /// to the optional heading or tailing contents before and after the main one.
    /// </summary>
    /// <remarks>
    /// You can, for instance, use the head and tail parts to inject contents using instances
    /// whose bodies are just <see cref="DbTokenArgument"/> ones.
    /// </remarks>
    [Cloneable]
    public abstract partial class Entry
    {
        /// <summary>
        /// Initializes a new instance.
        /// </summary>
        public Entry(DbToken body) => Body = body.ThrowWhenNull();

        /// <summary>
        /// Copy constructor.
        /// </summary>
        /// <param name="source"></param>
        public Entry(Entry source) : this(source.Body)
        {
            Head = source.Head;
            Tail = source.Tail;
        }

        /// <inheritdoc/>
        public override string ToString()
        {
            var sb = new StringBuilder();

            if (Head is not null)
            {
                sb.Append(Head.ToString());
                sb.Append(", ");
            }

            sb.Append(ToStringBody());

            if (Tail is not null)
            {
                sb.Append(", ");
                sb.Append(Tail.ToString());
            }

            return sb.ToString();
        }

        protected virtual string ToStringBody() => Body.ToString()!;

        /// <summary>
        /// The head part carried by this instance, if any.
        /// </summary>
        public DbTokenInvoke? Head { get; init; } = null;

        /// <summary>
        /// The body part carried by this instance.
        /// </summary>
        public virtual DbToken Body { get; }

        /// <summary>
        /// The tail part carried by this instance, if any.
        /// </summary>
        public DbTokenInvoke? Tail { get; init; } = null;

        /// <summary>
        /// Returns the command info object that represents the contents of this entry.
        /// </summary>
        /// <remarks>
        /// This method visits the head, body and tail parts, using a no-separator visitor for
        /// the head and tail ones, and then and just combine the results in that order, with
        /// no default separators. If any specific processing is required, then inheritors shall
        /// override this method.
        /// </remarks>
        /// <param name="visitor"></param>
        /// <returns></returns>
        public virtual ICommandInfo.IBuilder Visit(DbTokenVisitor visitor)
        {
            visitor.ThrowWhenNull();

            var engine = visitor.Connection.Engine;
            var builder = new CommandInfo.Builder(engine);
            var nsvisitor = visitor.ToNoSeparatorVisitor();
            ICommandInfo.IBuilder temp;

            if (Head is not null)
            {
                temp = nsvisitor.Visit(Head);
                builder.Add(temp);
            }
            if (Body is not DbTokenArgument)
            {
                temp = VisitBody(visitor);
                builder.Add(temp);
            }
            if (Tail is not null)
            {
                temp = nsvisitor.Visit(Tail);
                builder.Add(temp);
            }

            return builder;
        }

        /// <summary>
        /// Returns the command info object that represents the contents of the body of this
        /// instance. Note that, by default, if the body is a <see cref="DbTokenArgument"/>
        /// instance, then this method is not invoked.
        /// </summary>
        /// <param name="visitor"></param>
        /// <returns></returns>
        protected virtual ICommandInfo.IBuilder VisitBody(
            DbTokenVisitor visitor)
            => visitor.Visit(Body);
    }

    // ====================================================
    /// <summary>
    /// Represents a list-alike collection of fragments.
    /// </summary>
    [Cloneable]
    public abstract partial class Master : IEnumerable<Entry>
    {
        readonly List<Entry> Items = [];

        /// <summary>
        /// Initializes a new instance.
        /// </summary>
        /// <param name="command"></param>
        public Master(ICommand command) => Command = command.ThrowWhenNull();

        /// <summary>
        /// Copy constructor.
        /// </summary>
        /// <param name="source"></param>
        public Master(Master source) : this(source.Command) => Items.AddRange(source.Items);

        /// <inheritdoc/>
        public override string ToString() => Visit().ToString()!;

        /// <inheritdoc/>
        public IEnumerator<Entry> GetEnumerator() => Items.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        /// <summary>
        /// The command this instance is associated with.
        /// </summary>
        public ICommand Command { get; }

        /// <summary>
        /// Gets the number of elements in this collection.
        /// </summary>
        public int Count => Items.Count;

        /// <summary>
        /// Gets the entry stored at the given index.
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public Entry this[int index] => Items[index];

        /// <summary>
        /// Returns the index where the given entry is stored in this collection, or -1 if it is
        /// not found.
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public int IndexOf(Entry item) => Items.IndexOf(item);

        /// <summary>
        /// Invoked to validate that the given entry is a valid one for this collection. If not,
        /// then this method shall throw an appropriate exception.
        /// </summary>
        /// <param name="item"></param>
        protected virtual void Validate(Entry item) { }

        /// <summary>
        /// Adds the given entry to this collection.
        /// </summary>
        /// <param name="item"></param>
        public void Add(Entry item)
        {
            Validate(item);

            if (Items.Contains(item)) throw new DuplicateException(
                "Cannot add a duplicated fragment entry.")
                .WithData(item)
                .WithData(this);

            Items.Add(item);
        }

        /// <summary>
        /// Inserts the given entry into this collection at the given index.
        /// </summary>
        /// <param name="index"></param>
        /// <param name="item"></param>
        public void Insert(int index, Entry item)
        {
            Validate(item);

            if (Items.Contains(item)) throw new DuplicateException(
                "Cannot add a duplicated fragment entry.")
                .WithData(item)
                .WithData(this);

            Items.Insert(index, item);
        }

        /// <summary>
        /// Removes from this collection the given item.
        /// </summary>
        /// <param name="item"></param>
        public bool Remove(Entry item) => Items.Remove(item);

        /// <summary>
        /// Removes from this collection the entry at the given index.
        /// </summary>
        /// <param name="index"></param>
        public void RemoveAt(int index) => Items.RemoveAt(index);

        /// <summary>
        /// Clears all the contents captured by this instance.
        /// </summary>
        public void Clear() => Items.Clear();

        // ------------------------------------------------

        /// <summary>
        /// Creates a new entry using the contents obtained from parsing the given dynamic lambda
        /// expression, and adds that entry to this collection.
        /// </summary>
        /// <param name="spec"></param>
        public void Capture(Func<dynamic, object> spec)
        {
            spec.ThrowWhenNull();

            var engine = Command.Connection.Engine;
            var parser = new DbLambdaParser(engine);

            var token = parser.Parse(spec);
            var (head, body, tail) = token.ExtractParts();

            var entry = Create(head, body, tail);
            Add(entry);
        }

        /// <summary>
        /// Returns an appropriate entry based upon the given head, body and tail parts.
        /// </summary>
        /// <param name="head"></param>
        /// <param name="body"></param>
        /// <param name="tail"></param>
        /// <returns></returns>
        protected abstract Entry Create(DbTokenInvoke? head, DbToken body, DbTokenInvoke? tail);

        // ------------------------------------------------

        /// <summary>
        /// Returns the command info object that represents the contents of this collection of
        /// fragments.
        /// </summary>
        /// <returns></returns>
        public abstract ICommandInfo.IBuilder Visit();
    }
}