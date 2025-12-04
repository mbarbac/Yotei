namespace Yotei.ORM.Tools;

// ========================================================
partial class InvariantList<K, T>
{
    /// <summary>
    /// <inheritdoc cref="IInvariantList{K, T}.ICoreBuilder"/>
    /// </summary>
    [Cloneable(ReturnType = typeof(IInvariantList<,>.ICoreBuilder))]
    [DebuggerDisplay("{ToDebugString(3)}")]
    public partial class CoreBuilder : CoreList<K, T>, IInvariantList<K, T>.ICoreBuilder
    {
        /// <summary>
        /// Initializes a new empty instance.
        /// </summary>
        /// <param name="getkey"></param>
        public CoreBuilder(Func<T, K> getkey) : base(getkey) { }

        /// <summary>
        /// Initializes a new instance with the elements of the given range.
        /// </summary>
        /// <param name="getkey"></param>
        /// <param name="range"></param>
        public CoreBuilder(
            Func<T, K> getkey, IEnumerable<T> range) : this(getkey) => AddRange(range);

        /// <summary>
        /// Copy constructor.
        /// </summary>
        /// <param name="source"></param>
        protected CoreBuilder(CoreBuilder source) : base(source) { }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <returns></returns>
        public virtual IInvariantList<K, T> ToInvariant() => Count == 0
            ? new InvariantList<K, T>(GetKey)
            : new InvariantList<K, T>(this);
    }
}

// ========================================================
/// <summary>
/// <inheritdoc  cref="IInvariantList{K, T}"/>
/// </summary>
/// <typeparam name="K"></typeparam>
/// <typeparam name="T"></typeparam>
[Cloneable(ReturnType = typeof(IInvariantList<,>))]
[DebuggerDisplay("{ToDebugString(3)}")]
public partial class InvariantList<K, T> : IInvariantList<K, T>
{
    /// <summary>
    /// The underlying collection used by this instance.
    /// </summary>
    protected virtual IInvariantList<K, T>.ICoreBuilder Items { get; }

    // ----------------------------------------------------

    /// <summary>
    /// Initializes a new empty instance.
    /// </summary>
    /// <param name="getkey"></param>
    public InvariantList(Func<T, K> getkey) => Items = new InvariantList<K, T>.CoreBuilder(getkey);

    /// <summary>
    /// Initializes a new instance using the given builder.
    /// </summary>
    /// <param name="builder"></param>
    public InvariantList(
        IInvariantList<K, T>.ICoreBuilder builder) => Items = builder.ThrowWhenNull().Clone();

    /// <summary>
    /// Initializes a new instance with the elements of the given range.
    /// </summary>
    /// <param name="getkey"></param>
    /// <param name="range"></param>
    public InvariantList(
        Func<T, K> getkey, IEnumerable<T> range) : this(getkey) => Items!.AddRange(range);

    /// <summary>
    /// Copy constructor.
    /// </summary>
    /// <param name="source"></param>
    protected InvariantList(
        InvariantList<K, T> source) => Items = source.ThrowWhenNull().Items.Clone();

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
    public virtual IInvariantList<K, T>.ICoreBuilder ToBuilder() => Items.Clone();

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public int Count => Items.Count;

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="index"></param>
    /// <returns></returns>
    public T this[int index] => Items[index];

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    public bool Contains(K key) => Items.Contains(key);

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    public int IndexOf(K key) => Items.IndexOf(key);

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    public int LastIndexOf(K key) => Items.LastIndexOf(key);

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    public List<int> IndexesOf(K key) => Items.IndexesOf(key);

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    public int IndexOf(Predicate<T> predicate) => Items.IndexOf(predicate);

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    public int LastIndexOf(Predicate<T> predicate) => Items.LastIndexOf(predicate);

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    public List<int> IndexesOf(Predicate<T> predicate) => Items.IndexesOf(predicate);

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
    public bool FindLast(
        Predicate<T> predicate, Action<T>? found = null) => Items.FindLast(predicate, found);

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="predicate"></param>
    /// <param name="found"></param>
    /// <returns></returns>
    public bool FindLast(
        Predicate<T> predicate, out T found) => Items.FindLast(predicate, out found);

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
    public virtual IInvariantList<K, T> Add(T item)
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
    public virtual IInvariantList<K, T> AddRange(IEnumerable<T> range)
    {
        var builder = Items.Clone();
        var num = builder.AddRange(range);
        return num > 0 ? builder.ToInvariant() : this;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="index"></param>
    /// <param name="item"></param>
    /// <returns></returns>
    public virtual IInvariantList<K, T> Insert(int index, T item)
    {
        var builder = Items.Clone();
        var num = builder.Insert(index, item);
        return num > 0 ? builder.ToInvariant() : this;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="index"></param>
    /// <param name="range"></param>
    /// <returns></returns>
    public virtual IInvariantList<K, T> InsertRange(int index, IEnumerable<T> range)
    {
        var builder = Items.Clone();
        var num = builder.InsertRange(index, range);
        return num > 0 ? builder.ToInvariant() : this;
    }

    // ----------------------------------------------------

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="index"></param>
    /// <param name="count"></param>
    /// <returns></returns>
    public virtual IInvariantList<K, T> GetRange(int index, int count)
    {
        ArgumentOutOfRangeException.ThrowIfLessThan(index, 0);
        ArgumentOutOfRangeException.ThrowIfLessThan(count, 0);
        ArgumentOutOfRangeException.ThrowIfGreaterThan(count, Items.Count - index);

        if (index == 0 && count == Count) return this;

        var builder = Items.Clone();
        builder.Clear();

        var range = Items.ToList().GetRange(index, count); // LOW: Optimize
        var num = builder.AddRange(range);
        return num > 0 ? builder.ToInvariant() : this;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="index"></param>
    /// <param name="item"></param>
    /// <param name="removed"></param>
    /// <returns></returns>
    public virtual IInvariantList<K, T> Replace(int index, T item, Action<T>? removed = null)
    {
        var builder = Items.Clone();
        var num = builder.Replace(index, item, removed);
        return num > 0 ? builder.ToInvariant() : this;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="index"></param>
    /// <param name="item"></param>
    /// <param name="removed"></param>
    /// <returns></returns>
    public virtual IInvariantList<K, T> Replace(int index, T item, out T removed)
    {
        var builder = Items.Clone();
        var num = builder.Replace(index, item, out removed);
        return num > 0 ? builder.ToInvariant() : this;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="index"></param>
    /// <param name="removed"></param>
    /// <returns></returns>
    public virtual IInvariantList<K, T> RemoveAt(int index, Action<T>? removed = null)
    {
        var builder = Items.Clone();
        var done = builder.RemoveAt(index, removed);
        return done ? builder.ToInvariant() : this;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="index"></param>
    /// <param name="removed"></param>
    /// <returns></returns>
    public virtual IInvariantList<K, T> RemoveAt(int index, out T removed)
    {
        var builder = Items.Clone();
        var done = builder.RemoveAt(index, out removed);
        return done ? builder.ToInvariant() : this;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="index"></param>
    /// <param name="count"></param>
    /// <param name="removed"></param>
    /// <returns></returns>
    public virtual IInvariantList<K, T> RemoveRange(int index, int count, Action<T>? removed = null)
    {
        var builder = Items.Clone();
        var num = builder.RemoveRange(index, count, removed);
        return num > 0 ? builder.ToInvariant() : this;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="index"></param>
    /// <param name="count"></param>
    /// <param name="removed"></param>
    /// <returns></returns>
    public virtual IInvariantList<K, T> RemoveRange(int index, int count, out List<T> removed)
    {
        var builder = Items.Clone();
        var num = builder.RemoveRange(index, count, out removed);
        return num > 0 ? builder.ToInvariant() : this;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="key"></param>
    /// <param name="removed"></param>
    /// <returns></returns>
    public virtual IInvariantList<K, T> Remove(K key, Action<T>? removed = null)
    {
        var builder = Items.Clone();
        var num = builder.Remove(key, removed);
        return num > 0 ? builder.ToInvariant() : this;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="key"></param>
    /// <param name="removed"></param>
    /// <returns></returns>
    public virtual IInvariantList<K, T> Remove(K key, out T removed)
    {
        var builder = Items.Clone();
        var num = builder.Remove(key, out removed);
        return num > 0 ? builder.ToInvariant() : this;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="key"></param>
    /// <param name="removed"></param>
    /// <returns></returns>
    public virtual IInvariantList<K, T> RemoveLast(K key, Action<T>? removed = null)
    {
        var builder = Items.Clone();
        var num = builder.RemoveLast(key, removed);
        return num > 0 ? builder.ToInvariant() : this;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="key"></param>
    /// <param name="removed"></param>
    /// <returns></returns>
    public virtual IInvariantList<K, T> RemoveLast(K key, out T removed)
    {
        var builder = Items.Clone();
        var num = builder.RemoveLast(key, out removed);
        return num > 0 ? builder.ToInvariant() : this;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="key"></param>
    /// <param name="removed"></param>
    /// <returns></returns>
    public virtual IInvariantList<K, T> RemoveAll(K key, Action<T>? removed = null)
    {
        var builder = Items.Clone();
        var num = builder.RemoveAll(key, removed);
        return num > 0 ? builder.ToInvariant() : this;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="key"></param>
    /// <param name="removed"></param>
    /// <returns></returns>
    public virtual IInvariantList<K, T> RemoveAll(K key, out List<T> removed)
    {
        var builder = Items.Clone();
        var num = builder.RemoveAll(key, out removed);
        return num > 0 ? builder.ToInvariant() : this;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="predicate"></param>
    /// <param name="removed"></param>
    /// <returns></returns>
    public virtual IInvariantList<K, T> Remove(Predicate<T> predicate, Action<T>? removed = null)
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
    public virtual IInvariantList<K, T> Remove(Predicate<T> predicate, out T removed)
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
    public virtual IInvariantList<K, T> RemoveLast(Predicate<T> predicate, Action<T>? removed = null)
    {
        var builder = Items.Clone();
        var num = builder.RemoveLast(predicate, removed);
        return num > 0 ? builder.ToInvariant() : this;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="predicate"></param>
    /// <param name="removed"></param>
    /// <returns></returns>
    public virtual IInvariantList<K, T> RemoveLast(Predicate<T> predicate, out T removed)
    {
        var builder = Items.Clone();
        var num = builder.RemoveLast(predicate, out removed);
        return num > 0 ? builder.ToInvariant() : this;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="predicate"></param>
    /// <param name="removed"></param>
    /// <returns></returns>
    public virtual IInvariantList<K, T> RemoveAll(Predicate<T> predicate, Action<T>? removed = null)
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
    public virtual IInvariantList<K, T> RemoveAll(Predicate<T> predicate, out List<T> removed)
    {
        var builder = Items.Clone();
        var num = builder.RemoveAll(predicate, out removed);
        return num > 0 ? builder.ToInvariant() : this;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    public virtual IInvariantList<K, T> Clear()
    {
        var builder = Items.Clone();
        var num = builder.Clear();
        return num > 0 ? builder.ToInvariant() : this;
    }

    // ----------------------------------------------------

    bool ICollection.IsSynchronized => false;
    object ICollection.SyncRoot => Items.SyncRoot;
}