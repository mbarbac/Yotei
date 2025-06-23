using System.Net.Http;

namespace Yotei.ORM.Internals;

public static partial class Fragment
{
    // ====================================================
    /// <summary>
    /// Represents a collection of fragments used to build a specific clause of the associated
    /// command.
    /// </summary>
    [Cloneable]
    public abstract partial class Master : IEnumerable<Entry>
    {
        readonly List<Entry> Items = [];

        /// <summary>
        /// Returns a descriptor of the clause this instance is built for.
        /// <br/> This property is mainly used for informational purposes.
        /// </summary>
        public abstract string CLAUSE { get; set; }

        /// <summary>
        /// Initializes a new instance.
        /// </summary>
        /// <param name="command"></param>
        public Master(ICommand command) => Command = command.ThrowWhenNull();

        /// <summary>
        /// Copy constructor.
        /// </summary>
        /// <param name="source"></param>
        protected Master(Master source)
        {
            source.ThrowWhenNull();

            Command = source.Command;

            // Preventing items to be associated with their former collection...
            var items = source.Select(x => x.Clone());
            Items.AddRange(items);
            foreach (var item in Items) item.Master = this;
        }

        /// <inheritdoc/>
        public virtual IEnumerator<Entry> GetEnumerator() => Items.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        /// <inheritdoc/>
        public override string ToString() => Visit().ToString()!;

        // ------------------------------------------------

        /// <summary>
        /// The command this instance is associated with.
        /// </summary>
        public ICommand Command { get; }
        protected IConnection Connection => Command.Connection;
        protected IEngine Engine => Connection.Engine;

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

        // ------------------------------------------------

        /// <summary>
        /// Invoked to validate that the given entry is suitable for this instance.
        /// </summary>
        protected virtual Entry Validate(Entry entry) => entry;

        /// <summary>
        /// Adds the given entry to this collection, provided it is a valid one for this instance
        /// and it is not a duplicated one.
        /// </summary>
        /// <param name="entry"></param>
        public void Add(Entry entry)
        {
            entry = Validate(entry);

            if (Items.Contains(entry)) throw new DuplicateException(
                "Cannot add a duplicated entry.")
                .WithData(entry)
                .WithData(this);

            Items.Add(entry);
        }

        /// <summary>
        /// Adds to this instance the entries from the given range, provided they are valid for
        /// this instance and not duplicated ones.
        /// </summary>
        /// <param name="range"></param>
        public void AddRange(IEnumerable<Entry> range)
        {
            range.ThrowWhenNull();
            foreach (var item in range) Add(item);
        }

        /// <summary>
        /// Inserts the given entry into this collection, at the given index, provided it is a
        /// valid one for this instance and it is not a duplicated one.
        /// </summary>
        /// <param name="index"></param>
        /// <param name="entry"></param>
        public void Insert(int index, Entry entry)
        {
            entry = Validate(entry);

            if (Items.Contains(entry)) throw new DuplicateException(
                "Cannot add a duplicated entry.")
                .WithData(entry)
                .WithData(this);

            Items.Insert(index, entry);
        }

        /// <summary>
        /// Inserts into this instance the entries from the given range, starting from the given
        /// index, provided they are valid for this instance and not duplicated ones.
        /// </summary>
        /// <param name="index"></param>
        /// <param name="range"></param>
        public void InsertRange(int index, IEnumerable<Entry> range)
        {
            range.ThrowWhenNull();
            foreach (var item in range) { Insert(index, item); index++; }
        }

        /// <summary>
        /// Removes from this collection the given entry, if possible.
        /// </summary>
        /// <param name="item"></param>
        public bool Remove(Entry entry) => Items.Remove(entry);

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
        /// Invoked to create and add to this instance a suitable entry based upon the given
        /// dynamic lambda expression.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="spec"></param>
        /// <returns></returns>
        public virtual Entry Capture<T>(Func<dynamic, T> spec)
        {
            spec.ThrowWhenNull();

            var entry = Create(spec);
            Add(entry);
            return entry;
        }

        /// <summary>
        /// Invoked to create a suitable entry for this instance based upon the given dynamic
        /// lambda expression.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="spec"></param>
        /// <returns></returns>
        public virtual Entry Create<T>(Func<dynamic, T> spec)
        {
            spec.ThrowWhenNull();

            var engine = Command.Connection.Engine;
            var token = DbLambdaParser.Parse(engine, spec);
            
            var entry = Create(token);
            return entry;
        }

        /// <summary>
        /// Invoked to create a suitable entry for this instance based upon the given token,
        /// where its unique (not recurrent) invoke head and tail elements are extracted and
        /// kept into the returned entry. In case of ambiguity, head invoke elements take
        /// precedence over tail ones.
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        public virtual Entry Create(IDbToken token)
        {
            token.ThrowWhenNull();

            token.ExtractHeadInvokes(out var body, out var head, recurrent: false);
            if (head is not null &&
                body is DbTokenInvoke bodyHead &&
                bodyHead.Host is DbTokenArgument &&
                bodyHead.Arguments.Count == 1)
                body = bodyHead.Arguments[0];

            body.ExtractTailInvokes(out body, out var tail, recurrent: false);
            if (tail is not null &&
                body is DbTokenInvoke bodyTail &&
                bodyTail.Host is DbTokenArgument &&
                bodyTail.Arguments.Count == 1)
                body = bodyTail.Arguments[0];

            return OnCreate(head, body, tail);
        }

        /// <summary>
        /// Invoked to create a suitable entry for this instance using the given body and the
        /// given head and tail parts, if any.
        /// </summary>
        protected abstract Entry OnCreate(DbTokenInvoke? head, IDbToken body, DbTokenInvoke? tail);

        // ------------------------------------------------

        /// <summary>
        /// Obtains the separator to use to separate the given entry from previous ones, if any.
        /// If the returned value is <c>null</c>, then is ignored.
        /// </summary>
        /// <param name="entry"></param>
        /// <returns></returns>
        public virtual string? Separator(Entry entry) => null;

        /// <summary>
        /// Visits the entries in this instance and returns a command info object that can be
        /// used to build the related clause of the associated command.
        /// </summary>
        /// <returns></returns>
        public virtual ICommandInfo.IBuilder Visit()
        {
            static ICommandInfo.IBuilder VisitItem(Entry entry, DbTokenVisitor visitor)
            {
                return entry.Visit(visitor);
            }
            return Visit(VisitItem);
        }

        /// <summary>
        /// Visits the entries in this instance using the given delegate and returns a command
        /// info object that can be used to build the related clause of the associated command.
        /// </summary>
        /// <param name="visitItem"></param>
        /// <returns></returns>
        protected ICommandInfo.IBuilder Visit(
            Func<Entry, DbTokenVisitor, ICommandInfo.IBuilder> visitItem)
        {
            visitItem.ThrowWhenNull();

            var visitor = Command.Connection.Records.CreateDbTokenVisitor(Command.Locale);
            var engine = Command.Connection.Engine;
            var builder = new CommandInfo.Builder(engine);

            for (int i = 0; i < Items.Count; i++)
            {
                var entry = Items[i];
                var separator = Separator(entry);

                if (i != 0 && separator is not null) builder.Add(separator);
                var temp = visitItem(entry, visitor);
                builder.Add(temp);
            }

            return builder;
        }
    }
}