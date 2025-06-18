namespace Yotei.ORM.Internals;

public static partial class Fragment
{
    // ====================================================
    /// <summary>
    /// Represents an entry in a collection of fragments used to build a specific clause of the
    /// associated command.
    /// </summary>
    [Cloneable]
    public abstract partial class Entry
    {
        /// <summary>
        /// Initializes a new instance.
        /// </summary>
        /// <param name="command"></param>
        public Entry(ICommand command) => Command = command.ThrowWhenNull();

        /// <summary>
        /// Copy constructor.
        /// </summary>
        /// <param name="source"></param>
        protected Entry(Entry source) : this(source.ThrowWhenNull().Command) { }

        /// <summary>
        /// The command this instance is associated with.
        /// </summary>
        public ICommand Command { get; }
        protected IEngine Engine => Command.Connection.Engine;

        /// <summary>
        /// Visits the contents of this instance and returns the command info object that can be
        /// used to build the related clause of the command. The 'first' and 'last' parameters
        /// determine if this instance is the first or last one in the collection it belongs to.
        /// </summary>
        /// <param name="visitor"></param>
        /// <param name="first"></param>
        /// <param name="last"></param>
        /// <returns></returns>
        public abstract ICommandInfo.IBuilder Visit(DbTokenVisitor visitor, bool first, bool last);
    }
}