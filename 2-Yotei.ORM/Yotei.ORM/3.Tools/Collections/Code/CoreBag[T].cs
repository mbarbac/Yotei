namespace Yotei.ORM.Tools;

// ========================================================
/// <summary>
/// <inheritdoc cref="ICoreBag{T}"/>
/// </summary>
/// <typeparam name="T"></typeparam>
[Cloneable(ReturnType = typeof(ICoreBag<>))]
[DebuggerDisplay("{ToDebugString(3)}")]
public partial class CoreBag<T> : ICoreBag<T>
{
    [Cloneable]
    partial class MyItems : CoreList<T>
    {
        readonly CoreBag<T> Master;
        public MyItems(CoreBag<T> master) : base() => Master = master;
        protected MyItems(MyItems source) => throw new UnExpectedException();

        public override T ValidateElement(T item) => Master.ValidateElement(item);
        public override bool CompareElements(T source, T target) => Master.CompareElements(source, target);
        public override bool FlattenElements => Master.FlattenElements;
        public override IEnumerable<T> GetDuplicates(T item) => Master.GetDuplicates(item);
        public override bool IncludeDuplicate(T source, T target) => Master.IncludeDuplicate(source, target);
        protected override string ToDebugItem(T item) => Master.ToDebugItem(item);
    }

    // ----------------------------------------------------

    readonly MyItems Items;

    /// <summary>
    /// Initializes a new empty instance.
    /// </summary>
    public CoreBag() => Items = new(this);

    /// <summary>
    /// Initializes a new instance with the elements of the given range.
    /// </summary>
    /// <param name="range"></param>
    public CoreBag(IEnumerable<T> range) : this() => AddRange(range);

    /// <summary>
    /// Copy constructor.
    /// </summary>
    /// <param name="source"></param>
    protected CoreBag(CoreBag<T> source) : this() => AddRange(source);

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    public IEnumerator<T> GetEnumerator() => Items.GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    public override string ToString() => Items.ToString();

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
    /// Invoked to validate the given element before using it in this collection.
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    public virtual T ValidateElement(T item) => item;

    /// <summary>
    /// Invoked to determine, for the purposes of this collection, if the given source and target
    /// elements shall be considered equal or not.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="target"></param>
    /// <returns></returns>
    public virtual bool CompareElements(
        T source, T target) => EqualityComparer<T>.Default.Equals(source, target);

    /// <summary>
    /// Determines if the elements that are themselves collections of elements of the type of
    /// this collection ('<typeparamref name="T"/>') shall be flattened before using them, or
    /// not.
    /// </summary>
    public virtual bool FlattenElements => true;

    /// <summary>
    /// Invoked to find the duplicates of the given element.
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    public virtual IEnumerable<T> GetDuplicates(
        T item) => FindAll(x => CompareElements(x, item), out var found) ? found : [];

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

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="range"></param>
    /// <returns></returns>
    public virtual int AddRange(IEnumerable<T> range) => Items.AddRange(range);

    // ----------------------------------------------------

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="item"></param>
    /// <param name="removed"></param>
    /// <returns></returns>
    public virtual int Remove(T item, Action<T>? removed = null) => Items.Remove(item, removed);
    bool ICollection<T>.Remove(T item) => Remove(item) > 0;

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="item"></param>
    /// <param name="removed"></param>
    /// <returns></returns>
    public virtual int Remove(T item, out List<T> removed) => Items.Remove(item, out removed);

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="item"></param>
    /// <param name="removed"></param>
    /// <returns></returns>
    public virtual int RemoveAll(
        T item, Action<T>? removed = null) => Items.RemoveAll(item, removed);

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="item"></param>
    /// <param name="removed"></param>
    /// <returns></returns>
    public virtual int RemoveAll(T item, out List<T> removed) => Items.RemoveAll(item, out removed);

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

    // ----------------------------------------------------

    bool ICollection<T>.IsReadOnly => false;
    bool ICollection.IsSynchronized => false;
    object ICollection.SyncRoot => ((ICollection)Items).SyncRoot;
}