using System.Security.Cryptography.X509Certificates;

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
        /// Initializes a new instance.
        /// </summary>
        /// <param name="clause"></param>
        /// <param name="command"></param>
        public Master(string clause, ICommand command)
        {
            Clause = clause.NotNullNotEmpty();
            Command = command.ThrowWhenNull();
        }

        /// <summary>
        /// Copy constructor.
        /// </summary>
        /// <param name="source"></param>
        protected Master(Master source)
        {
            source.ThrowWhenNull();

            Clause = source.Clause;
            Command = source.Command;
            Head = source.Head?.Clone();
            Tail = source.Tail?.Clone();

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
        /// Returns the descriptor of the clause this instance is built for.
        /// </summary>
        public string Clause { get; }

        /// <summary>
        /// The command this instance is associated with.
        /// </summary>
        public ICommand Command { get; }
        protected IConnection Connection => Command.Connection;
        protected IEngine Engine => Connection.Engine;

        /// <summary>
        /// The head element carried by this fragment master, or <c>null</c> if any.
        /// </summary>
        public IDbToken? Head { get; set; }

        /// <summary>
        /// The tail element carried by this fragment master, or <c>null</c> if any.
        /// </summary>
        public IDbToken? Tail { get; set; }

        /// <summary>
        /// Clears all the contents captured by this instance.
        /// </summary>
        public virtual void Clear()
        {
            Items.Clear();
            Head = null;
            Tail = null;
        }

        // ------------------------------------------------

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

            var entry = CreateEntry(spec);
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
        public virtual Entry CreateEntry<T>(Func<dynamic, T> spec)
        {
            var token = DbLambdaParser.Parse(Engine, spec);
            var entry = CreateEntry(token);
            return entry;
        }

        /// <summary>
        /// Invoked to create a suitable entry for this instance based upon the given token.
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        public abstract Entry CreateEntry(IDbToken token);

        // ------------------------------------------------

        /// <summary>
        /// Visits the entries in this instance and returns a command info object that can be
        /// used to build this clause of the related command.
        /// </summary>
        /// <returns></returns>
        public virtual ICommandInfo.IBuilder Visit()
        {
            ICommandInfo.IBuilder temp;
            var visitor = Connection.Records.CreateDbTokenVisitor(Command.Locale);
            var builder = new CommandInfo.Builder(Engine);

            temp = VisitHead(visitor);
            if (!temp.IsEmpty) builder.Add(temp);

            for (int i = 0; i < Items.Count; i++)
            {
                var entry = Items[i];
                var separator = EntrySeparator(entry);

                if (i > 0 && separator is not null) builder.Add(separator);
                temp = entry.Visit(visitor);
                if (!temp.IsEmpty) builder.Add(temp);
            }

            temp = VisitTail(visitor);
            if (!temp.IsEmpty) builder.Add(temp);

            return builder;
        }

        /// <summary>
        /// Invoked to obtain the separator to use between the contents of the given entry and
        /// the contents of the previous ones, if any. If <c>null</c>, then it is ignored.
        /// </summary>
        protected virtual string? EntrySeparator(Entry entry) => null;

        /// <summary>
        /// Invoked to visit the head of this instance.
        /// <br/> Returns an empty result if the head is null.
        /// </summary>
        protected virtual ICommandInfo.IBuilder VisitHead(DbTokenVisitor visitor)
            => Head is null
            ? new CommandInfo.Builder(Engine)
            : visitor.Visit(Head);

        /// <summary>
        /// Invoked to visit the tail of this instance.
        /// <br/> Returns an empty result if the tail is null.
        /// </summary>
        protected virtual ICommandInfo.IBuilder VisitTail(DbTokenVisitor visitor)
            => Tail is null
            ? new CommandInfo.Builder(Engine)
            : visitor.Visit(Tail);
    }
}