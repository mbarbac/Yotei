namespace Yotei.ORM.Internals;

static partial class Fragment
{
    // ====================================================
    /// <summary>
    /// Represents the collecion of fragment entries used to build a clause of an associated
    /// command. They essentially wrap a list of token instance that can be later visited to
    /// produce the relevant command-info object.
    /// </summary>
    public abstract class Master : IEnumerable<Entry>
    {
        readonly List<Entry> Items = [];

        /// <summary>
        /// Initializes a new instance.
        /// </summary>
        /// <param name="command"></param>
        /// <param name="descriptor"></param>
        public Master(ICommand command, string descriptor)
        {
            Command = command.ThrowWhenNull();
            Descriptor = descriptor.NotNullNotEmpty();
        }

        /// <summary>
        /// Copy constructor.
        /// </summary>
        /// <param name="source"></param>
        protected Master(Master source)
        {
            source.ThrowWhenNull();

            Command = source.Command;
            Descriptor = source.Descriptor;

            Items = source.Items.Select(x => x.Clone()).ToList();
            foreach (var item in Items) item.Master = this;
        }

        /// <inheritdoc cref="ICloneable.Clone"/>
        public abstract Master Clone();

        /// <inheritdoc/>
        public virtual IEnumerator<Entry> GetEnumerator() => Items.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        /// <inheritdoc/>
        public override string ToString() => Visit().ToString()!;

        // ------------------------------------------------

        /// <summary>
        /// The descriptor of the clause this instance is built for.
        /// This property is mostly used for reporting purposes only.
        /// </summary>
        public string Descriptor { get; }

        /// <summary>
        /// The command this instance is associated with.
        /// </summary>
        public ICommand Command { get; }
        public IConnection Connection => Command.Connection;
        public IEngine Engine => Connection.Engine;

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
        /// Invoked to validate that the given entry is suitable for this instance. This method
        /// shall throw an exception if not.
        /// </summary>
        protected virtual Entry Validate(Entry entry) => entry.ThrowWhenNull();

        /// <summary>
        /// Adds the given entry to this collection, provided it is a valid one for this instance
        /// and it is not a duplicated one.
        /// </summary>
        /// <param name="entry"></param>
        public virtual void Add(Entry entry)
        {
            entry = Validate(entry);

            if (Items.Contains(entry)) throw new DuplicateException(
                "Cannot add a duplicated entry.")
                .WithData(entry)
                .WithData(this);

            Items.Add(entry);
        }

        /// <summary>
        /// Inserts the given entry into this collection at the given index, provided it is a
        /// valid one for this instance and it is not a duplicated one.
        /// </summary>
        /// <param name="index"></param>
        /// <param name="entry"></param>
        public virtual void Insert(int index, Entry entry)
        {
            entry = Validate(entry);

            if (Items.Contains(entry)) throw new DuplicateException(
                "Cannot add a duplicated entry.")
                .WithData(entry)
                .WithData(this);

            Items.Insert(index, entry);
        }

        /// <summary>
        /// Removes the entry stored at the given index.
        /// </summary>
        /// <param name="index"></param>
        public virtual void RemoveAt(int index) => Items.RemoveAt(index);

        /// <summary>
        /// Removes from this collection the given entry.
        /// </summary>
        /// <param name="entry"></param>
        /// <returns></returns>
        public virtual bool Remove(Entry entry) => Items.Remove(entry);

        /// <summary>
        /// Clears all contents captured by this instance.
        /// </summary>
        public virtual void Clear() => Items.Clear();

        // ------------------------------------------------

        /// <summary>
        /// Creates and adds to this collection a valid entry using the contents obtained from
        /// parsing the given dynamic lambda expression. If that expression resolved to a string
        /// and a collection of arguments is given, then those arguments are used to build a
        /// command-info token that will be used to create the entry instead.
        /// </summary>
        /// <param name="spec"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public Entry Capture(Func<dynamic, object> spec, params object?[]? args)
        {
            var token = CreateToken(Engine, spec, args);
            var entry = CreateEntry(token);

            Add(entry);
            return entry;
        }

        /// <summary>
        /// Invoked to create a valid entry for this instance based upon the given token.
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        protected abstract Entry CreateEntry(IDbToken token);

        // ------------------------------------------------

        /// <summary>
        /// Visits the entries in this instance returning a command info builder object that can
        /// be used to build the related clause in the associated command.
        /// </summary>
        /// <returns></returns>
        public virtual ICommandInfo.IBuilder Visit()
        {
            static ICommandInfo.IBuilder EntryVisitor(
                Entry entry, DbTokenVisitor visitor, bool separate)
                => entry.Visit(visitor, separate);

            var visitor = Connection.Records.CreateDbTokenVisitor(Command.Locale);
            return VisitEntries(visitor, EntryVisitor);
        }

        /// <summary>
        /// Invoked to visit the entries in this collection using the given delegate with each,
        /// and then concatenating their results into a single one. The delegate signature is:
        /// <br/>- ICommandInfo.IBuilder(Entry, DbTokenVisitor, bool NeedsSeparation)
        /// <br/>- Entry: the entry being visited.
        /// <br/>- Visitor: the visitor used to visit the entry.
        /// <br/>- Separate: whether the entry needs to be separated from any previous ones.
        /// </summary>
        /// <param name="visitor"></param>
        /// <param name="entryvisitor"></param>
        /// <returns></returns>
        protected ICommandInfo.IBuilder VisitEntries(
            DbTokenVisitor visitor,
            Func<Entry, DbTokenVisitor, bool, ICommandInfo.IBuilder> entryvisitor)
        {
            visitor.ThrowWhenNull();
            entryvisitor.ThrowWhenNull();

            var builder = new CommandInfo.Builder(Engine);

            for (int i = 0; i < Count; i++)
            {
                var entry = Items[i];
                var separate = i > 0 && builder.TextLen > 0;

                var temp = entryvisitor(entry, visitor, separate);
                if (!temp.IsEmpty) builder.Add(temp);
            }

            return builder;
        }
    }
}