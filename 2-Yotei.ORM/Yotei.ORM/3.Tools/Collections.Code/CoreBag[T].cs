namespace Yotei.ORM.Tools;

// ========================================================
/// <summary>
/// <inheritdoc cref="ICoreBag{T}"/>
/// </summary>
/// <typeparam name="T"></typeparam>
[DebuggerDisplay("{ToDebugString(3)}")]
[Cloneable(ReturnType = typeof(ICoreBag<>))]
public partial class CoreBag<T> : ICoreBag<T>
{
    // Implementation detail...
    partial class MyItems(CoreBag<T> master) : CoreList<T>
    {
        readonly CoreBag<T> Master = master.ThrowWhenNull();

        public override T ValidateElement(T value) => Master.ValidateElement(value);
        public override bool CompareElements(T source, T target) => Master.CompareElements(source, target);
        public override IEnumerable<T> FindDuplicates(T value) => Master.FindDuplicates(value);
        public override bool AcceptDuplicated(T source, T duplicate) => Master.AcceptDuplicated(source, duplicate);
    }

    // ----------------------------------------------------

    readonly MyItems Items;

    /// <summary>
    /// Initializes a new empty instance.
    /// </summary>
    public CoreBag() => Items = new(this);

    /// <summary>
    /// Initializes a new instance with the elements from the given range.
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

    // ----------------------------------------------------

    /// <summary>
    /// Invoked to return a validated element, or to throw an appropriate exception otherwise.
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public virtual T ValidateElement(T value) => value;

    /// <summary>
    /// Invoked to determine if the given keys, for the sole purposes of this collection, shall be
    /// considered the same or not.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="target"></param>
    /// <returns></returns>
    public virtual bool CompareElements(T source, T target)
        => EqualityComparer<T>.Default.Equals(source, target);

    /// <summary>
    /// Invoked to obtain the elements in this collection whose keys are the same as the given one.
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    public virtual IEnumerable<T> FindDuplicates(T value)
        => FindAll(x => CompareElements(x, value), out var items) ? items : [];

    /// <summary>
    /// Invoked to determine if the given element, which is considered to be a duplicate of the
    /// given source one, can be included in this collection, or not. This method shall return
    /// <see langword="true"/> to include the duplicated element, <see langword="false"/> to
    /// ignore it, or throw an appropriate exception if duplicates are not allowed.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="duplicate"></param>
    /// <returns></returns>
    public virtual bool AcceptDuplicated(T source, T duplicate) => true;

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
    /// <param name="predicate"></param>
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
    /// <param name="range"></param>
    /// <returns></returns>
    public bool FindAll(
        Predicate<T> predicate, out List<T> range) => Items.FindAll(predicate, out range);

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
    public void CopyTo(Array array, int index) => Items.CopyTo(array, index);

    // ----------------------------------------------------

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public virtual int Add(T value) => Items.Add(value);
    void ICollection<T>.Add(T value) => Add(value);

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="range"></param>
    /// <returns></returns>
    public virtual int AddRange(IEnumerable<T> range) => Items.AddRange(range);

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public virtual int Remove(T value) => Items.Remove(value);
    bool ICollection<T>.Remove(T value) => Remove(value) > 0;

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
    object ICollection.SyncRoot => ((ICollection)Items).SyncRoot;
    bool ICollection.IsSynchronized => false;
}