
namespace Yotei.ORM.Internal;

public partial class FragmentWhere
{
    // ====================================================
    /// <summary>
    /// Represents an entry in a collection of fragments that is used to build a WHERE clause.
    /// </summary>
    [Cloneable]
    public partial class Entry : Fragment.Entry
    {
        /// <summary>
        /// Initializes a new instance.
        /// </summary>
        /// <param name="command"></param>
        public Entry(ICommand command) : base(command) { }
        
        /// <summary>
        /// Copy constructor.
        /// </summary>
        /// <param name="source"></param>
        public Entry(Entry source) : base(source) { }

        /// <inheritdoc/>
        public override ICommandInfo.IBuilder Visit() => throw null;
    }

    // ====================================================
    /// <summary>
    /// Represents a list-alike collection of fragments used to build a WHERE clause.
    /// </summary>
    [Cloneable]
    public partial class Master : Fragment.Master
    {
        /// <summary>
        /// Initializes a new instance.
        /// </summary>
        /// <param name="command"></param>
        public Master(ICommand command) : base(command) { }

        /// <summary>
        /// Copy constructor.
        /// </summary>
        /// <param name="source"></param>
        public Master(Master source) : base(source) { }

        /// <inheritdoc/>
        public override void Capture(Func<dynamic, object> expression) => throw null;

        /// <inheritdoc/>
        public override ICommandInfo.IBuilder Visit() => throw null;
    }
}