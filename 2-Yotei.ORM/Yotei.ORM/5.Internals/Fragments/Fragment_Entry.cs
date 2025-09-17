namespace Yotei.ORM.Internals;

static partial class Fragment
{
    // ====================================================
    /// <summary>
    /// Represents an entry in a collection of fragments used to build a given clause of the
    /// associated command.
    /// </summary>
    public abstract class Entry
    {
        /// <summary>
        /// Initializes a new instance.
        /// </summary>
        /// <param name="master"></param>
        /// <param name="token"></param>
        public Entry(Master master, IDbToken token)
        {
            Master = master.ThrowWhenNull();
            Body = token.ThrowWhenNull();
        }

        /// <summary>
        /// Copy constructor.
        /// </summary>
        /// <param name="source"></param>
        protected Entry(Entry source)
        {
            source.ThrowWhenNull();

            Master = source.Master;
            Body = source.Body;
        }

        /// <inheritdoc/>
        public override string ToString() => Body.ToString() ?? string.Empty;

        /// <inheritdoc cref="ICloneable.Clone"/>
        public abstract Entry Clone();

        // ------------------------------------------------

        /// <summary>
        /// The master collection this instance belongs to.
        /// </summary>
        public Master Master { get; internal set; }
        public string Descriptor => Master.Descriptor;
        public ICommand Command => Master.Command;
        public IConnection Connection => Command.Connection;
        public IEngine Engine => Connection.Engine;

        /// <summary>
        /// The actual body carried by this instance.
        /// </summary>
        public IDbToken Body
        {
            get => _Body;
            init => _Body = value;
        }

        /// <summary>
        /// The actual repository of the body of contents of this instance.
        /// </summary>
#pragma warning disable IDE1006
        internal protected IDbToken _Body
        {
            get => __Body;
            set
            {
                value.ThrowWhenNull();

                // Any others, even empty ones...
                __Body = value;
            }
        }
        IDbToken __Body = default!;
#pragma warning restore IDE1006

        // ------------------------------------------------

        /// <summary>
        /// Visits the contents of this instance and returns a command info builder object that
        /// can be used to build the related clause in the associated command.
        /// <br/> The <paramref name="separate"/> argument indicates if this entry needs to be
        /// separated from any previous ones.
        /// </summary>
        /// <param name="visitor"></param>
        /// <param name="separate"></param>
        /// <returns></returns>
        public abstract ICommandInfo.IBuilder Visit(DbTokenVisitor visitor, bool separate);
    }
}