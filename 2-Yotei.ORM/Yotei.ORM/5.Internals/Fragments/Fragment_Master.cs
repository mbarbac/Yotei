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

        /// <inheritdoc/>
        public virtual IEnumerator<Entry> GetEnumerator() => Items.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

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

    }
}