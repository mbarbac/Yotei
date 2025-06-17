namespace Yotei.ORM.Internals;

public static partial class Fragment
{
    // ====================================================
    /// <summary>
    /// Represents an entry in a collection of fragments used to build a specific clause.
    /// </summary>
    [Cloneable]
    public abstract partial class Entry
    {
        /// <summary>
        /// Initializes a new instance.
        /// </summary>
        public Entry() { }

        /// <summary>
        /// Copy constructor.
        /// </summary>
        /// <param name="source"></param>
        [SuppressMessage("", "IDE0060")]
        protected Entry(Entry source) { }

        /// <summary>
        /// Visits the contents of this instance and returns the command info object that can be
        /// used to build the related clause of the command. The <paramref name="first"/> and
        /// <paramref name="last"/> arguments determine if this instance is the first or the last
        /// of the entries in the collection it belongs to.
        /// </summary>
        /// <param name="visitor"></param>
        /// <param name="first"></param>
        /// <param name="last"></param>
        /// <returns></returns>
        public abstract ICommandInfo.IBuilder Visit(DbTokenVisitor visitor, bool first, bool last);
    }
}