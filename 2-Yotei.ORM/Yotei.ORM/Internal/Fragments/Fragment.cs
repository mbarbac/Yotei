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
        /// <param name="info"></param>
        public Entry(ICommandInfo info) => CommandInfo = info.ThrowWhenNull();

        /// <summary>
        /// Copy constructor.
        /// </summary>
        /// <param name="source"></param>
        public Entry(Entry source) => CommandInfo = source.CommandInfo;

        /// <summary>
        /// The command info wrapped by this instance.
        /// </summary>
        public ICommandInfo CommandInfo { get; }

        /// <inheritdoc/>
        public override string ToString() => CommandInfo.ToString()!;

        /// <summary>
        /// Returns the database string appropriate for the contents of this element.
        /// </summary>
        /// <returns></returns>
        public abstract string Visit();
    }

    // ====================================================
    /// <summary>
    /// Represents a list-alike collection of fragments.
    /// </summary>
    [Cloneable]
    [DebuggerDisplay("{ToDebugString(5)}")]
    public abstract partial class Master : CoreList<Entry>
    {
        /// <summary>
        /// Initializes a new instance.
        /// </summary>
        /// <param name="command"></param>
        public Master(ICommand command) => Command = command.ThrowWhenNull();

        /// <summary>
        /// Copy constructor.
        /// </summary>
        /// <param name="source"></param>
        public Master(Master source) : this(source.Command) => AddRange(source);

        /// <inheritdoc/>
        public override string ToString() => $"Count: {Count}";

        /// <summary>
        /// The command this instance is associated with.
        /// </summary>
        public ICommand Command { get; }

        /// <summary>
        /// Returns the database string appropriate for the contents of this element.
        /// </summary>
        /// <returns></returns>
        public abstract string Visit();
    }
}