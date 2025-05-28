namespace Yotei.ORM.Internal;

public partial class Fragment
{
    // ====================================================
    /// <summary>
    /// Represents an entry in a collection of fragments.
    /// </summary>
    [Cloneable]
    public abstract partial class Entry
    {
        /// <summary>
        /// Initializes a new instance.
        /// </summary>
        public Entry(ICommand command) => Command = command.ThrowWhenNull();

        /// <summary>
        /// Copy constructor.
        /// </summary>
        /// <param name="source"></param>
        public Entry(Entry source) : this(source.Command) { }

        /// <inheritdoc/>
        public override string ToString() => throw null;

        /// <summary>
        /// The command this instance is associated with.
        /// </summary>
        public ICommand Command { get; }

        /*
        /// <summary>
        /// Returns the command info object that represents the contents of this entry.
        /// </summary>
        /// <returns></returns>
        public abstract ICommandInfo.IBuilder Visit();
        */
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
        public override string ToString() => throw null;

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
        /// Adds the given entry to this collection.
        /// <br/> Inheritors can override this method to, for instance, intercept the type of
        /// entries to add.
        /// </summary>
        /// <param name="item"></param>
        public virtual void Add(Entry item) => Items.Add(item);

        /// <summary>
        /// Removes from this collection the given item.
        /// </summary>
        /// <param name="item"></param>
        public bool Remove(Entry item) => Items.Remove(item);

        /// <summary>
        /// Clears all the contents captured by this instance.
        /// </summary>
        public void Clear() => Items.Clear();

        /// <summary>
        /// Creates a new entry using the contents obtained from parsing the given dynamic lambda
        /// expression, and adds that entry to this collection.
        /// </summary>
        /// <param name="spec"></param>
        public void Capture(Func<dynamic, object> spec)
        {
            var item = Create(spec);
            Add(item);
        }

        /// <summary>
        /// Returns a new entry based upon the contents obtained from parsing the given dynamic
        /// lambda expression.
        /// </summary>
        /// <param name="spec"></param>
        /// <returns></returns>
        public abstract Entry Create(Func<dynamic, object> spec);

        /*
        /// <summary>
        /// Returns the command info object that represents the contents of this collection of
        /// fragments.
        /// </summary>
        /// <returns></returns>
        public abstract ICommandInfo.IBuilder Visit();
        */
    }
}