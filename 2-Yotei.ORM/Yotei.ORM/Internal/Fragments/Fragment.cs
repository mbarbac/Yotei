namespace Yotei.ORM.Internal;

public partial class Fragment
{
    // ====================================================
    /// <summary>
    /// Determines the position of a fragment entry.
    /// </summary>
    public enum EntryPosition { First, Middle, Last }

    // ====================================================
    /// <summary>
    /// Represents an entry in a collection of fragments.
    /// </summary>
    [Cloneable]
    public partial class Entry
    {
        /// <summary>
        /// Initializes a new instance.
        /// </summary>
        /// <param name="body"></param>
        public Entry(DbToken body) => Body = body.ThrowWhenNull();

        /// <summary>
        /// Copy constructor.
        /// </summary>
        /// <param name="source"></param>
        public Entry(Entry source) : this(source.Body) { }

        /// <inheritdoc/>
        public override string ToString() => Body.ToString()!;

        /// <summary>
        /// The body of contents carried by this instance.
        /// </summary>
        public DbToken Body { get; }

        /// <summary>
        /// Visits the contents of this instance and returns a command-info object that can be
        /// used to build a related command.
        /// </summary>
        /// <param name="visitor"></param>
        /// <param name="position"></param>
        /// <returns></returns>
        public virtual ICommandInfo.IBuilder Visit(DbTokenVisitor visitor, EntryPosition position)
        {
            visitor.ThrowWhenNull();
            return visitor.Visit(Body);
        }
    }

    // ====================================================
    /// <summary>
    /// Represents an ordered list-alike collection of fragments.
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
        public Master(Master source) : this(source.Command) => Items.AddRange(source);

        /// <inheritdoc/>
        public IEnumerator<Entry> GetEnumerator() => Items.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        /// <inheritdoc/>
        public override string ToString() => Visit().ToString()!;

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

        // ------------------------------------------------

        /// <summary>
        /// Invoked to validate and return the given entry.
        /// </summary>
        /// <param name="entry"></param>
        /// <returns></returns>
        protected virtual Entry Validate(Entry entry) => entry;

        /// <summary>
        /// Adds the given entry to this collection, provided it is a valid one for this instance,
        /// and it is not a duplicated one.
        /// </summary>
        /// <param name="entry"></param>
        public void Add(Entry entry)
        {
            entry = Validate(entry);

            if (Items.Contains(entry)) throw new DuplicateException(
                "Cannot add a duplicated fragment entry.")
                .WithData(entry)
                .WithData(this);

            Items.Add(entry);
        }

        /// <summary>
        /// Inserts the given entry into this collection, at the given index, provided it a valid
        /// one for this instance, and it is not a duplicated one.
        /// </summary>
        /// <param name="index"></param>
        /// <param name="entry"></param>
        public void Insert(int index, Entry entry)
        {
            entry = Validate(entry);

            if (Items.Contains(entry)) throw new DuplicateException(
                "Cannot add a duplicated fragment entry.")
                .WithData(entry)
                .WithData(this);

            Items.Insert(index, entry);
        }

        /// <summary>
        /// Removes from this collection the given item, if possible.
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
        /// Creates and adds to this collection a new entry with the contents obtained from
        /// parsing the given dynamic lambda expression.
        /// </summary>
        /// <param name="spec"></param>
        public void Capture(Func<dynamic, object> spec)
        {
            spec.ThrowWhenNull();

            var engine = Command.Connection.Engine;
            var parser = new DbLambdaParser(engine);

            var token = parser.Parse(spec);
            var entry = Create(token);
            Add(entry);
        }

        /// <summary>
        /// Invoked to create and return an appropriate entry for this instance.
        /// </summary>
        /// <param name="body"></param>
        /// <returns></returns>
        protected abstract Entry Create(DbToken body);

        // ------------------------------------------------

        /// <summary>
        /// Visits the contents of this instance and returns a command-info object that can be
        /// used to build a related command.
        /// </summary>
        /// <returns></returns>
        public ICommandInfo.IBuilder Visit()
        {
            var connection = Command.Connection;
            var visitor = connection.Records.CreateDbTokenVisitor(Command.Locale);
            var engine = connection.Engine;
            var builder = new CommandInfo.Builder(engine);

            for (int i = 0; i < Items.Count; i++)
            {
                EntryPosition position;
                ICommandInfo.IBuilder temp;
                var entry = Items[i];

                if (i == 0) position = EntryPosition.First;
                else if (i == (Items.Count - 1)) position = EntryPosition.Last;
                else position = EntryPosition.Middle;

                temp = OnVisit(entry, visitor, position, out var separator);

                if (position is not EntryPosition.First &&
                    separator is not null &&
                    separator.Length > 0)
                    builder.Add(separator);

                builder.Add(temp);
            }

            return builder;
        }

        /// <summary>
        /// Invoked to visit the given entry.
        /// <br/> If the out '<paramref name="separator"/>' parameter is not null, then its value
        /// is injected into the final command-info object being built, before the representation
        /// of the given entry.
        /// </summary>
        /// <param name="entry"></param>
        /// <param name="visitor"></param>
        /// <param name="position"></param>
        /// <param name="separator"></param>
        /// <returns></returns>
        protected virtual ICommandInfo.IBuilder OnVisit(
            Entry entry,
            DbTokenVisitor visitor, EntryPosition position, out string? separator)
        {
            separator = null;
            return entry.Visit(visitor, position);
        }
    }
}