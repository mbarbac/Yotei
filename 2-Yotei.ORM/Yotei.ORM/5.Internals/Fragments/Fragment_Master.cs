using System.Reflection.PortableExecutable;
using System.Security.Cryptography;

namespace Yotei.ORM.Internals;

public static partial class Fragment
{
    // ====================================================
    /// <summary>
    /// Represents the collection of fragments used to build a given clause of the associated
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
        /// <param name="clause"></param>
        public Master(ICommand command, string clause)
        {
            Command = command.ThrowWhenNull();
            Clause = clause.NotNullNotEmpty();
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
        /// The descriptor of the clause this instance is built for.
        /// </summary>
        public string Clause { get; }

        /// <summary>
        /// The command this instance is associated with.
        /// </summary>
        public ICommand Command { get; }
        protected IConnection Connection => Command.Connection;
        protected IEngine Engine => Connection.Engine;

        // ------------------------------------------------

        /// <summary>
        /// Gets the number of elements in this collection, without taking into consideration
        /// not the <see cref="Head"/> neither the <see cref="Tail"/> elements.
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
        public virtual bool Remove(Entry entry) => Items.Remove(entry);

        /// <summary>
        /// Removes from this collection the entry at the given index.
        /// </summary>
        /// <param name="index"></param>
        public virtual void RemoveAt(int index) => Items.RemoveAt(index);

        /// <summary>
        /// Clears all the contents captured by this instance.
        /// </summary>
        /// <param name="head"></param>
        /// <param name="tail"></param>
        public virtual void Clear() => Items.Clear();

        // ------------------------------------------------

        /// <summary>
        /// Invoked to create and add to this instance a suitable entry based upon the given
        /// dynamic lambda expression.
        /// <para>
        /// This method accepts an optional collection of arguments that will only be used when
        /// the given expression resolves into an entry text, and those arguments are codified
        /// in that text using either a positional '{n}' or a named '{name}' format. Otherwise,
        /// an exception is thrown.
        /// </para>
        /// </summary>
        /// <param name="spec"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public Entry Capture(Func<dynamic, object> spec, params object?[]? args)
        {
            var entry = CreateEntry(spec, args);
            Add(entry);
            return entry;
        }

        /// <summary>
        /// Invoked to create a suitable entry for this instance based upon the given dynamic
        /// lambda expression.
        /// <para>
        /// This method accepts an optional collection of arguments that will only be used when
        /// the given expression resolves into an entry text, and those arguments are codified
        /// in that text using either a positional '{n}' or a named '{name}' format. Otherwise,
        /// an exception is thrown.
        /// </para>
        /// </summary>
        /// <param name="spec"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public Entry CreateEntry(Func<dynamic, object> spec, params object?[]? args)
        {
            var token = DbLambdaParser.Parse(Engine, spec);

            if (args is null || args.Length > 0)
            {
                if (token is DbTokenValue value && value.Value is string str)
                {
                    args ??= [null];
                    var info = new CommandInfo(Engine, str, args);
                    token = new DbTokenCommandInfo(info);
                }
                else
                {
                    throw new ArgumentException(
                        "Using a collection of arguments with no corresponding entry text.")
                        .WithData(token)
                        .WithData(args);
                }
            }

            var entry = CreateEntry(token);
            return entry;
        }

        /// <summary>
        /// Invoked to create a suitable entry for this instance based upon the given token.
        /// </summary>
        /// <param name="body"></param>
        /// <returns></returns>
        public abstract Entry CreateEntry(IDbToken body);

        // ------------------------------------------------

        /// <summary>
        /// Visits the entries in this instance and returns a command info object that can be
        /// used to build the related clause of the associated command. If this instance contains
        /// no entries, then an empty result is returned.
        /// </summary>
        /// <returns></returns>
        public virtual ICommandInfo.IBuilder Visit()
        {
            var visitor = Connection.Records.CreateDbTokenVisitor(Command.Locale);
            static ICommandInfo.IBuilder EntryVisitor(
                Entry entry, DbTokenVisitor visitor, bool separate)
                => entry.Visit(visitor, separate);

            return VisitEntries(visitor, EntryVisitor);
        }

        /// <summary>
        /// Invoked to visit the entries in this instance using the given entry visitor delegate,
        /// and to return a command info object that can be used to build the related clause of
        /// the associated command, or an empty instance otherwise.
        /// <br/> Delegate's format:
        /// ICommandInfo.IBuilder (Entry, DbTokenVisitor, bool need separation).
        /// </summary>
        /// <param name="visitor"></param>
        /// <param name="entryVisitor"></param>
        /// <returns></returns>
        protected ICommandInfo.IBuilder VisitEntries(
            DbTokenVisitor visitor,
            Func<Entry, DbTokenVisitor, bool, ICommandInfo.IBuilder> entryVisitor)
        {
            visitor.ThrowWhenNull();
            entryVisitor.ThrowWhenNull();

            var builder = new CommandInfo.Builder(Engine);
            for (int i = 0; i < Count; i++)
            {
                var entry = Items[i];
                var separate = i > 0 && builder.TextLen > 0;
                var temp = entryVisitor(entry, visitor, separate);

                if (!temp.IsEmpty) builder.Add(temp);
            }
            return builder;
        }
    }
}