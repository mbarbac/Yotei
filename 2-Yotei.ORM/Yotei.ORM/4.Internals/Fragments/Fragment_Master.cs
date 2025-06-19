namespace Yotei.ORM.Internals;

public static partial class Fragment
{
    // ====================================================
    /// <summary>
    /// Represents the collection of fragments used to build a specific clause of the associated
    /// command.
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
        protected Master(Master source) : this(source.Command)
        {
            Items.AddRange(source.Select(x => x.Clone()));
            foreach (var item in this) item.Master = this;
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
        protected IEngine Engine => Command.Connection.Engine;

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
        /// Invoked to validate the given entry.
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
        /// Adds to this collection an entry created using the given dynamic lambda expression.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="spec"></param>
        /// <returns></returns>
        public virtual Entry Capture<T>(Func<dynamic, T> spec)
        {
            var entry = Create(spec);

            Add(entry);
            return entry;
        }

        /// <summary>
        /// Creates an appropriate entry for this collection using the given dynamic lambda
        /// expression.
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
        /// Creates an appropriate entry for this collection using the database token.
        /// </summary>
        /// <param name="body"></param>
        /// <returns></returns>
        public abstract Entry Create(IDbToken body);

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
            static ICommandInfo.IBuilder Itemize(
                Entry entry, DbTokenVisitor visitor) => entry.Visit(visitor);

            return Visit(Itemize);
        }

        /// <summary>
        /// Visits the entries in this instance using the given delegate and returns a command
        /// info object that can be used to build the related clause of the associated command.
        /// </summary>
        /// <param name="itemize"></param>
        /// <returns></returns>
        public ICommandInfo.IBuilder Visit(
            Func<Entry, DbTokenVisitor, ICommandInfo.IBuilder> itemize)
        {
            var connection = Command.Connection;
            var visitor = connection.Records.CreateDbTokenVisitor(Command.Locale);
            var engine = connection.Engine;
            var builder = new CommandInfo.Builder(engine);

            for (int i = 0; i < Items.Count; i++)
            {
                var item = Items[i];
                var separator = Separator(item);

                if (i != 0 && separator is not null) builder.Add(separator);
                var temp = itemize(item, visitor);
                builder.Add(temp);
            }

            return builder;
        }
    }
}