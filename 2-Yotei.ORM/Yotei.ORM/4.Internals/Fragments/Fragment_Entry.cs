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
        /// <param name="master"></param>
        public Entry(Master master) => Master = master.ThrowWhenNull();

        /// <summary>
        /// Copy constructor.
        /// </summary>
        /// <param name="source"></param>
        protected Entry(Entry source) : this(source.ThrowWhenNull().Master) { }

        /// <summary>
        /// The master collection this instance is associated with.
        /// </summary>
        public Master Master { get; internal set; }
        protected ICommand Command => Master.Command;
        protected IEngine Engine => Command.Connection.Engine;

        /// <summary>
        /// Visits the contents of this instance and returns the command info object that can be
        /// used to build the related clause of the command.
        /// </summary>
        /// <param name="visitor"></param>
        /// <returns></returns>
        public abstract ICommandInfo.IBuilder Visit(DbTokenVisitor visitor);
    }
}