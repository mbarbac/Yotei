namespace Yotei.ORM.Tools;

// ========================================================
/// <summary>
/// <inheritdoc cref="ICoreList{K, T}"/>
/// <br/> This type is the non-abstract version of the collection that uses a delegate provided
/// in the constructor to obtain the key associated with each element.
/// </summary>
/// <typeparam name="K"></typeparam>
/// <typeparam name="T"></typeparam>
[Cloneable(ReturnType = typeof(ICoreList<,>))]
[DebuggerDisplay("{ToDebugString(3)}")]
public partial class CoreListEx<K, T> : ICoreList<K, T>
{
    [Cloneable]
    partial class MyItems : CoreList<K, T>
    {
        readonly CoreListEx<K, T> Master;
        public MyItems(CoreListEx<K, T> master) => Master = master;
        protected MyItems(MyItems source) => throw new UnExpectedException();

        public override K GetKey(T item) => Master.GetKey(item);
        public override K ValidateKey(K key) => Master.ValidateKey(key);
        public override bool CompareKeys(K source, K target) => Master.CompareKeys(source, target);
        public override T ValidateElement(T item) => Master.ValidateElement(item);
        public override bool FlattenElements => Master.FlattenElements;
        public override IEnumerable<T> GetDuplicates(K key) => Master.GetDuplicates(key);
        public override bool IncludeDuplicate(T source, T target) => Master.IncludeDuplicate(source, target);
        protected override string ToDebugItem(T item) => Master.ToDebugItem(item);
    }

    // ----------------------------------------------------

    readonly MyItems Items;

    /// <summary>
    /// Initializes a new empty instance.
    /// </summary>
    /// <param name="getkey"></param>
    public CoreListEx(Func<T, K> getkey)
    {
        GetKey = getkey.ThrowWhenNull();
        Items = new(this);
    }

    /// <summary>
    /// Initializes a new instance with the elements of the given range.
    /// </summary>
    /// <param name="getkey"></param>
    /// <param name="range"></param>
    public CoreListEx(Func<T, K> getkey, IEnumerable<T> range) : this(getkey) => AddRange(range);

    /// <summary>
    /// Copy constructor.
    /// </summary>
    /// <param name="source"></param>
    protected CoreListEx(CoreListEx<K, T> source) : this(source.GetKey) => AddRange(source);

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    public IEnumerator<T> GetEnumerator() => Items.GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    /// <summary>
    /// Returns a string representation of this instance suitable for debug purposes with at most
    /// the requested number of elements.
    /// </summary>
    /// <param name="count"></param>
    /// <returns></returns>
    public virtual string ToDebugString(int count) => Items.ToDebugString(count);

    /// <summary>
    /// Invoked to obtain a string representation of the given element suitable for debug
    /// purposes.
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    protected virtual string ToDebugItem(T item) => item.Sketch();

    // ----------------------------------------------------

    /// <summary>
    /// The delegate to invoke to obtain the key associated with the given element.
    /// </summary>
    public Func<T, K> GetKey{ get; }

    /// <summary>
    /// Invoked to validate the given key before using it in this collection.
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    public virtual K ValidateKey(K key) => key;

    /// <summary>
    /// Invoked to determine, for the purposes of this collection, if the given source and target
    /// keys shall be considered equal or not.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="target"></param>
    /// <returns></returns>
    public virtual bool CompareKeys(
        K source, K target) => EqualityComparer<K>.Default.Equals(source, target);

    /// <summary>
    /// Invoked to validate the given element before using it in this collection.
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    public virtual T ValidateElement(T item) => item;

    /// <summary>
    /// Determines if the elements that are themselves collections of elements of the type of
    /// this collection ('<typeparamref name="T"/>') shall be flattened before using them, or
    /// not.
    /// </summary>
    public virtual bool FlattenElements => true;

    /// <summary>
    /// Invoked to find the duplicates of the given key.
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    public virtual IEnumerable<T> GetDuplicates(K key)
        => FindAll(x => CompareKeys(GetKey(x), key), out var found) ? found : [];

    /// <summary>
    /// Invoked to determine if the target element, which is considered a duplicate of the source
    /// one, can be included in this collection or not. Returns '<c>true</c>' to include it, or
    /// '<c>false</c>' to ignore the inclusion operation. In addition, an exception may be thrown
    /// if duplicates are not allowed.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="target"></param>
    /// <returns></returns>
    public virtual bool IncludeDuplicate(T source, T target) => true;

    // ----------------------------------------------------

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public int Count => Items.Count;

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="index"></param>
    /// <returns></returns>
    public T this[int index]
    {
        get => Items[index];
        set => Items[index] = value;
    }
    object? IList.this[int index]
    {
        get => this[index];
        set => this[index] = (T)value!;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    public bool Contains(K key) => Items.Contains(key);
    bool ICollection<T>.Contains(T item) => Contains(GetKey(item));
    bool IList.Contains(object? item) => Contains(GetKey((T)item!));

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    public int IndexOf(K key) => Items.IndexOf(key);
    int IList<T>.IndexOf(T item) => IndexOf(GetKey(item));
    int IList.IndexOf(object? item) => IndexOf(GetKey((T)item!));

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
    public T[] ToArray() => [.. Items];

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    public List<T> ToList() => [.. Items];

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="index"></param>
    /// <param name="count"></param>
    /// <returns></returns>
    public List<T> ToList(int index, int count) => Items.ToList(index, count);

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public void Trim() => Items.Trim();

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="array"></param>
    /// <param name="index"></param>
    public void CopyTo(T[] array, int index) => Items.CopyTo(array, index);

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
    public virtual int Add(T item) => Items.Add(item);
    void ICollection<T>.Add(T item) => Add(item);
    int IList.Add(object? value) => Add((T)value!) > 0 ? (Count - 1) : -1;

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="range"></param>
    /// <returns></returns>
    public virtual int AddRange(IEnumerable<T> range) => Items.AddRange(range);

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="index"></param>
    /// <param name="item"></param>
    /// <returns></returns>
    public virtual int Insert(int index, T item) => Items.Insert(index, item);
    void IList<T>.Insert(int index, T item) => Insert(index, item);
    void IList.Insert(int index, object? item) => Insert(index, (T)item!);

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="range"></param>
    /// <returns></returns>
    public virtual int InsertRange(
        int index, IEnumerable<T> range) => Items.InsertRange(index, range);

    // ----------------------------------------------------

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="index"></param>
    /// <param name="item"></param>
    /// <param name="removed"></param>
    /// <returns></returns>
    public virtual int Replace(
        int index, T item, Action<T>? removed = null) => Items.Replace(index, item, removed);

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="index"></param>
    /// <param name="item"></param>
    /// <param name="removed"></param>
    /// <returns></returns>
    public virtual int Replace(
        int index, T item, out T removed) => Items.Replace(index, item, out removed);

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="index"></param>
    /// <param name="removed"></param>
    /// <returns></returns>
    public virtual bool RemoveAt(
        int index, Action<T>? removed = null) => Items.RemoveAt(index, removed);
    void IList<T>.RemoveAt(int index) => RemoveAt(index);
    void IList.RemoveAt(int index) => RemoveAt(index);

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="index"></param>
    /// <param name="removed"></param>
    /// <returns></returns>
    public virtual bool RemoveAt(int index, out T removed) => Items.RemoveAt(index, out removed);

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="index"></param>
    /// <param name="count"></param>
    /// <param name="removed"></param>
    /// <returns></returns>
    public virtual int RemoveRange(
        int index, int count, Action<T>? removed = null) => Items.RemoveRange(index, count, removed);

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="index"></param>
    /// <param name="count"></param>
    /// <param name="removed"></param>
    /// <returns></returns>
    public virtual int RemoveRange(
        int index, int count, out List<T> removed) => Items.RemoveRange(index, count, out removed);



    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="key"></param>
    /// <param name="removed"></param>
    /// <returns></returns>
    public virtual int Remove(
        K key, Action<T>? removed = null) => Items.Remove(key, removed);
    bool ICollection<T>.Remove(T item) => Remove(GetKey(item)) > 0;
    void IList.Remove(object? item) => Remove(GetKey((T)item!));

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="key"></param>
    /// <param name="removed"></param>
    /// <returns></returns>
    public virtual int Remove(K key, out T removed) => Items.Remove(key, out removed);

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="key"></param>
    /// <param name="removed"></param>
    /// <returns></returns>
    public virtual int RemoveLast(
        K key, Action<T>? removed = null) => Items.RemoveLast(key, removed);

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="key"></param>
    /// <param name="removed"></param>
    /// <returns></returns>
    public virtual int RemoveLast(K key, out T removed) => Items.RemoveLast(key, out removed);

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="key"></param>
    /// <param name="removed"></param>
    /// <returns></returns>
    public virtual int RemoveAll(
        K key, Action<T>? removed = null) => Items.RemoveAll(key, removed);

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="key"></param>
    /// <param name="removed"></param>
    /// <returns></returns>
    public virtual int RemoveAll(K key, out List<T> removed) => Items.RemoveAll(key, out removed);

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="predicate"></param>
    /// <param name="removed"></param>
    /// <returns></returns>
    public virtual int Remove(
        Predicate<T> predicate, Action<T>? removed = null) => Items.Remove(predicate, removed);

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="predicate"></param>
    /// <param name="removed"></param>
    /// <returns></returns>
    public virtual int Remove(
        Predicate<T> predicate, out T removed) => Items.Remove(predicate, out removed);

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="predicate"></param>
    /// <param name="removed"></param>
    /// <returns></returns>
    public virtual int RemoveLast(
        Predicate<T> predicate, Action<T>? removed = null) => Items.RemoveLast(predicate, removed);

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="predicate"></param>
    /// <param name="removed"></param>
    /// <returns></returns>
    public virtual int RemoveLast(
        Predicate<T> predicate, out T removed) => Items.RemoveLast(predicate, out removed);

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="predicate"></param>
    /// <param name="removed"></param>
    /// <returns></returns>
    public virtual int RemoveAll(
        Predicate<T> predicate, Action<T>? removed = null) => Items.RemoveAll(predicate, removed);

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="predicate"></param>
    /// <param name="removed"></param>
    /// <returns></returns>
    public virtual int RemoveAll(
        Predicate<T> predicate, out List<T> removed) => Items.RemoveAll(predicate, out removed);

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    public virtual int Clear() => Items.Clear();
    void ICollection<T>.Clear() => Clear();
    void IList.Clear() => Clear();

    // ----------------------------------------------------

    bool ICollection<T>.IsReadOnly => false;
    bool ICollection.IsSynchronized => false;
    object ICollection.SyncRoot => ((ICollection)Items).SyncRoot;

    bool IList.IsReadOnly => false;
    bool IList.IsFixedSize => false;
}