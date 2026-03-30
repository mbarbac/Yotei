namespace Yotei.ORM.Tools.Code;

// ========================================================
/// <summary>
/// <inheritdoc/>
/// </summary>
/// <typeparam name="T"></typeparam>
[DebuggerDisplay("{ToDebugString(4)}")]
[Cloneable(ReturnType = typeof(ICoreBag<>))]
public partial class CoreBag<T> : ICoreBag<T>
{
    // Implementation detail: using an underlying list for simplicity.
    partial class MyItems(CoreBag<T> master) : CoreList<T>
    {
        readonly CoreBag<T> Master = master.ThrowWhenNull();

        public override T ValidateElement(T value) => Master.ValidateElement(value);
        public override bool CompareElements(T source, T target) => Master.CompareElements(source, target);
        public override bool FlattenInput(T value) => Master.FlattenInput(value);
        public override IEnumerable<T> FindDuplicates(T value) => Master.FindDuplicates(value);
        public override bool IncludeDuplicated(T source, T target) => Master.IncludeDuplicated(source, target);

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
    public CoreBag(IEnumerable<T> range) : this() => Items.AddRange(range);

    /// <summary>
    /// Copy constructor.
    /// </summary>
    /// <param name="source"></param>
    protected CoreBag(CoreBag<T> source) : this() => Items.AddRange(source);

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
    public override string ToString() => $"Count: {Count}";

    /// <summary>
    /// Returns a string representation of this instance for debug purposes.
    /// </summary>
    /// <param name="count"></param>
    /// <returns></returns>
    public virtual string ToDebugString(int count) => Items.ToDebugString(count);

    /// <summary>
    /// Returns a string representation of the given element for debug purposes.
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    protected virtual string ToDebugItem(T item) => item.Sketch();

    // ----------------------------------------------------

    /// <summary>
    /// Invoked to return a validated element before using it in this collection, or throws an
    /// exception if not.
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public virtual T ValidateElement(T value) => value;

    /// <summary>
    /// Invoked to determine if, for the purposes of this collection, the given source and target
    /// values shall be considered equal, or not.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="target"></param>
    /// <returns></returns>
    public virtual bool CompareElements(
        T source, T target)
        => EqualityComparer<T>.Default.Equals(source, target);

    /// <summary>
    /// When an input element is itself a collection of elements of the type of this one, determines
    /// if that element shall be used instead of the given one, or not. If so, when that element is
    /// an empty collection, then the scenario is equivalent to providing no elements at all.
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public virtual bool FlattenInput(T value) => false;

    /// <summary>
    /// Invoked to return, for the purposes of this collection, the existing elements that shall be
    /// considered duplicates of the given one, if any.
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public virtual IEnumerable<T> FindDuplicates(
        T value)
        => FindAll(x => CompareElements(x, value), out var items) ? items : [];

    /// <summary>
    /// Invoked to determine if the target element, which is considered to be a duplicate of the
    /// given source one, can be included in this collection or not. This method shall return
    /// <see langword="true"/> to include that duplicated element, <see langword="false"/> to
    /// discard it, or throw an appropriate exception if duplicates are not allowed.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="target"></param>
    /// <returns></returns>
    public virtual bool IncludeDuplicated(T source, T target) => true;

    // ----------------------------------------------------

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public int Count => Items.Count;

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public bool Contains(T value) => Items.Contains(value);

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public bool Contains(Predicate<T> predicate) => Items.Contains(predicate);

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="predicate"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    public bool Find(Predicate<T> predicate, out T value) => Items.Find(predicate, out value);

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="predicate"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    public bool FindAll(Predicate<T> predicate, out List<T> range) => Items.FindAll(predicate, out range);

    // ----------------------------------------------------

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
    public void CopyTo(Array array, int index) => ((ICollection)Items).CopyTo(array, index);

    // ----------------------------------------------------

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public virtual int Add(T value) => Items.Add(value);
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
    /// <param name="value"></param>
    /// <returns></returns>
    public virtual int Remove(T value) => Items.Remove(value);
    bool ICollection<T>.Remove(T item) => Remove(item) > 0;

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public virtual int RemoveAll(T value) => Items.RemoveAll(value);

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    public virtual int Remove(Predicate<T> predicate) => Items.Remove(predicate);

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    public virtual int RemoveAll(Predicate<T> predicate) => Items.RemoveAll(predicate);

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