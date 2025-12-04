namespace Yotei.ORM.Tools;

// ========================================================
partial class InvariantBag<T>
{
    /// <summary>
    /// <inheritdoc cref="IInvariantBag{T}.ICoreBuilder"/>
    /// </summary>
    [Cloneable(ReturnType = typeof(IInvariantBag<>.ICoreBuilder))]
    [DebuggerDisplay("{ToDebugString(3)}")]
    public partial class CoreBuilder : CoreBag<T>, IInvariantBag<T>.ICoreBuilder
    {
        /// <summary>
        /// Initializes a new empty instance.
        /// </summary>
        public CoreBuilder() : base() { }

        /// <summary>
        /// Initializes a new instance with the elements of the given range.
        /// </summary>
        /// <param name="range"></param>
        public CoreBuilder(IEnumerable<T> range) : this() => AddRange(range);

        /// <summary>
        /// Copy constructor.
        /// </summary>
        /// <param name="source"></param>
        protected CoreBuilder(CoreBuilder source) : base(source) { }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <returns></returns>
        public virtual IInvariantBag<T> ToInvariant() => Count == 0
            ? []
            : new InvariantBag<T>(this);
    }
}

// ========================================================
/// <summary>
/// <inheritdoc cref="IInvariantBag{T}"/>
/// </summary>
/// <typeparam name="T"></typeparam>
[Cloneable(ReturnType = typeof(IInvariantBag<>))]
[DebuggerDisplay("{ToDebugString(3)}")]
public partial class InvariantBag<T> : IInvariantBag<T>
{
    /// <summary>
    /// The underlying collection used by this instance.
    /// </summary>
    protected virtual IInvariantBag<T>.ICoreBuilder Items { get; }

    // ----------------------------------------------------

    /// <summary>
    /// Initializes a new empty instance.
    /// </summary>
    public InvariantBag() => Items = new InvariantBag<T>.CoreBuilder();

    /// <summary>
    /// Initializes a new instance using the given builder.
    /// </summary>
    /// <param name="builder"></param>
    public InvariantBag(
        IInvariantBag<T>.ICoreBuilder builder) => Items = builder.ThrowWhenNull().Clone();

    /// <summary>
    /// Initializes a new instance with the elements of the given range.
    /// </summary>
    /// <param name="range"></param>
    public InvariantBag(IEnumerable<T> range) : this() => Items!.AddRange(range);

    /// <summary>
    /// Copy constructor.
    /// </summary>
    /// <param name="source"></param>
    protected InvariantBag(
        InvariantBag<T> source) => Items = source.ThrowWhenNull().Items.Clone();

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    public IEnumerator<T> GetEnumerator() => Items.GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    // ----------------------------------------------------

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    public virtual IInvariantBag<T>.ICoreBuilder ToBuilder() => Items.Clone();

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public int Count => Items.Count;

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    public bool Contains(T item) => Items.Contains(item);

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="predicate"></param>
    /// <param name="found"></param>
    /// <returns></returns>
    public bool Find(
        Predicate<T> predicate, Action<T>? found = null) => Items.Find(predicate, found);

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="predicate"></param>
    /// <param name="found"></param>
    /// <returns></returns>
    public bool Find(
        Predicate<T> predicate, out T found) => Items.Find(predicate, out found);

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="predicate"></param>
    /// <param name="found"></param>
    /// <returns></returns>
    public bool FindAll(
        Predicate<T> predicate, Action<T>? found = null) => Items.FindAll(predicate, found);

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="predicate"></param>
    /// <param name="found"></param>
    /// <returns></returns>
    public bool FindAll(
        Predicate<T> predicate, out List<T> found) => Items.FindAll(predicate, out found);

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    public T[] ToArray() => Items.ToArray();

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    public List<T> ToList() => Items.ToList();

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public void Trim() => Items.Trim();

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="array"></param>
    /// <param name="index"></param>
    public void CopyTo(Array array, int index) => Items.CopyTo(array, index);

    // ----------------------------------------------------

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    public virtual IInvariantBag<T> Add(T item)
    {
        var builder = Items.Clone();
        var num = builder.Add(item);
        return num > 0 ? builder.ToInvariant() : this;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="range"></param>
    /// <returns></returns>
    public virtual IInvariantBag<T> AddRange(IEnumerable<T> range)
    {
        var builder = Items.Clone();
        var num = builder.AddRange(range);
        return num > 0 ? builder.ToInvariant() : this;
    }

    // ----------------------------------------------------

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="item"></param>
    /// <param name="removed"></param>
    /// <returns></returns>
    public virtual IInvariantBag<T> Remove(T item, Action<T>? removed = null)
    {
        var builder = Items.Clone();
        var num = builder.Remove(item, removed);
        return num > 0 ? builder.ToInvariant() : this;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="item"></param>
    /// <param name="removed"></param>
    /// <returns></returns>
    public virtual IInvariantBag<T> Remove(T item, out List<T> removed)
    {
        var builder = Items.Clone();
        var num = builder.Remove(item, out removed);
        return num > 0 ? builder.ToInvariant() : this;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="item"></param>
    /// <param name="removed"></param>
    /// <returns></returns>
    public virtual IInvariantBag<T> RemoveAll(T item, Action<T>? removed = null)
    {
        var builder = Items.Clone();
        var num = builder.RemoveAll(item, removed);
        return num > 0 ? builder.ToInvariant() : this;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="item"></param>
    /// <param name="removed"></param>
    /// <returns></returns>
    public virtual IInvariantBag<T> RemoveAll(T item, out List<T> removed)
    {
        var builder = Items.Clone();
        var num = builder.RemoveAll(item, out removed);
        return num > 0 ? builder.ToInvariant() : this;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="predicate"></param>
    /// <param name="removed"></param>
    /// <returns></returns>
    public virtual IInvariantBag<T> Remove(Predicate<T> predicate, Action<T>? removed = null)
    {
        var builder = Items.Clone();
        var num = builder.Remove(predicate, removed);
        return num > 0 ? builder.ToInvariant() : this;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="predicate"></param>
    /// <param name="removed"></param>
    /// <returns></returns>
    public virtual IInvariantBag<T> Remove(Predicate<T> predicate, out T removed)
    {
        var builder = Items.Clone();
        var num = builder.Remove(predicate, out removed);
        return num > 0 ? builder.ToInvariant() : this;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="predicate"></param>
    /// <param name="removed"></param>
    /// <returns></returns>
    public virtual IInvariantBag<T> RemoveAll(Predicate<T> predicate, Action<T>? removed = null)
    {
        var builder = Items.Clone();
        var num = builder.RemoveAll(predicate, removed);
        return num > 0 ? builder.ToInvariant() : this;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="predicate"></param>
    /// <param name="removed"></param>
    /// <returns></returns>
    public virtual IInvariantBag<T> RemoveAll(Predicate<T> predicate, out List<T> removed)
    {
        var builder = Items.Clone();
        var num = builder.RemoveAll(predicate, out removed);
        return num > 0 ? builder.ToInvariant() : this;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    public virtual IInvariantBag<T> Clear()
    {
        var builder = Items.Clone();
        var num = builder.Clear();
        return num > 0 ? builder.ToInvariant() : this;
    }

    // ----------------------------------------------------

    bool ICollection.IsSynchronized => false;
    object ICollection.SyncRoot => Items.SyncRoot;
}