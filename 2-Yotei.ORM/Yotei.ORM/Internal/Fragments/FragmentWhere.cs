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
        public override string ToString() => throw null;
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
        public override void Add(Fragment.Entry item) => base.Add(item);

        /// <inheritdoc/>
        public override Entry Create(Func<dynamic, object> spec) => throw null;
    }
}