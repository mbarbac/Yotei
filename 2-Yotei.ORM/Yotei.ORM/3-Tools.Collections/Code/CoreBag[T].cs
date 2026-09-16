#pragma warning disable IDE0305

namespace Yotei.ORM.Tools;

// ========================================================
/// <summary>
/// <inheritdoc cref="ICoreBag{T}"/>
/// </summary>
/// <typeparam name="T"></typeparam>
[Cloneable(ReturnType = typeof(ICoreBag<>))]
[DebuggerDisplay("{ToDebugString(3)}")]
public abstract partial class CoreBag<T> : ICoreBag<T>
{
    // Implementation detail...
    [Cloneable]
    partial class MyItems : CoreList<T>
    {
        public MyItems(CoreBag<T> master) => Master = master.ThrowWhenNull();
        protected MyItems(MyItems other) => throw new NotImplementedException();
        readonly CoreBag<T> Master;
        public override T ValidateElement(T value) => Master.ValidateElement(value);
        public override bool FlattenElements { get => Master.FlattenElements; set => Master.FlattenElements = value; }
        public override bool CompareElements(T source, T target) => Master.CompareElements(source, target);
        public override IEnumerable<T> FindDuplicates(T value) => Master.FindDuplicates(value);
        public override bool AllowDuplicate(T value, IEnumerable<T> range) => Master.AllowDuplicate(value, range);
        public override bool SameElement(T source, T target) => Master.SameElement(source, target);
    }
    readonly MyItems Items;

    // ----------------------------------------------------

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
    /// <param name="other"></param>
    protected CoreBag(CoreBag<T> other) : this()
    {
        ArgumentNullException.ThrowIfNull(other);

        Items = new(this);
        OnCreating(other);
        AddRange(other);
    }

    /// <summary>
    /// Provides a place to update settings BEFORE the copy constructor copies its contents.
    /// <br/> Inherit types shall invoke this method before updating its own settings.
    /// </summary>
    /// <param name="other"></param>
    protected virtual void OnCreating(CoreBag<T> other)
    {
        FlattenElements = other.FlattenElements;
    }

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
    /// Returns a string representation of this instance for debug purposes that at max includes
    /// the given number of elements.
    /// </summary>
    /// <param name="max"></param>
    /// <returns></returns>
    public virtual string ToDebugString(int max)
    {
        if (Count == 0) return "0:[]";
        if (max == 0) return $"{Count}:[...]";

        return Count <= max
            ? $"{Count}:[{string.Join(", ", this.Select(ToDebugItem))}]"
            : $"{Count}:[{string.Join(", ", this.Take(max).Select(ToDebugItem))}, ...]";
    }

    /// <summary>
    /// Invoked to return a string representation of the given element for debug purposes.
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    protected virtual string ToDebugItem(T value) => value.Sketch();

    // ----------------------------------------------------

    /// <summary>
    /// Invoked to validate the given element before using it in this collection.
    /// <br/> This method must take into account enumerable values, if allowed.
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public abstract T ValidateElement(T value);

    /// <summary>
    /// Determines if elements that are themselves collection shall be flattened, and their
    /// elements used instead of the original one, or not.
    /// <br/> The defaul value of this property is <see langword="true"/>.
    /// </summary>
    public virtual bool FlattenElements
    {
        get;
        set
        {
            if (field == value) return;
            if (Count == 0) { field = value; return; }

            var range = ToList(); Clear();
            field = value;
            AddRange(range);
        }
    }
    = true;

    /// <summary>
    /// Invoked to determine if the given elements shall be considered the same (for the sole
    /// purposes of this collection).
    /// </summary>
    /// <param name="source"></param>
    /// <param name="target"></param>
    /// <returns></returns>
    public virtual bool CompareElements(T source, T target)
        => EqualityComparer<T>.Default.Equals(source, target);

    /// <summary>
    /// Invoked to find the elements whose keys are considered the same as the given one.
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public virtual IEnumerable<T> FindDuplicates(T value)
        => TryFindAll(x => CompareElements(x, value), out var items) ? items : [];

    /// <summary>
    /// Determines if the given value, identified as a duplicate of existing ones, can be included
    /// in this collection of not. This method shall:
    /// <br/>- Return <see langword="true"/> to include that element (duplicates allowed).
    /// <br/>- Return <see langword="false"/> to ignore that element (duplicates ignored).
    /// <br/>- Throw an appropriate exception.
    /// </summary>
    /// <param name="value"></param>
    /// <param name="range"></param>
    /// <returns></returns>
    public virtual bool AllowDuplicate(T value, IEnumerable<T> range) => true;

    /// <summary>
    /// Determines, for the sole purposes of replacemente, if the two given elements shall be
    /// considered the same, or not.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="target"></param>
    /// <returns></returns>
    public virtual bool SameElement(T source, T target)
        => ReferenceEquals(source, target)
        || CompareElements(source, target);

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
    public bool TryFind(Predicate<T> predicate, out T value) => Items.TryFind(predicate, out value);

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="predicate"></param>
    /// <param name="range"></param>
    /// <returns></returns>
    public bool TryFindAll(
        Predicate<T> predicate, out List<T> range) => Items.TryFindAll(predicate, out range);

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
    /// <returns><inheritdoc/></returns>
    public virtual int Add(T value) => Items.Add(value);
    void ICollection<T>.Add(T value) => Add(value);

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="range"></param>
    /// <returns><inheritdoc/></returns>
    public virtual int AddRange(IEnumerable<T> range) => Items.AddRange(range);

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="value"></param>
    /// <returns><inheritdoc/></returns>
    public virtual int Remove(T value) => Items.Remove(value);
    bool ICollection<T>.Remove(T value) => Remove(value) > 0;

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="value"></param>
    /// <returns><inheritdoc/></returns>
    public virtual int RemoveAll(T value) => Items.RemoveAll(value);

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns><inheritdoc/></returns>
    public virtual int Remove(Predicate<T> predicate) => Items.Remove(predicate);

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns><inheritdoc/></returns>
    public virtual int RemoveAll(Predicate<T> predicate) => Items.RemoveAll(predicate);

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns><inheritdoc/></returns>
    public virtual int Clear() => Items.Clear();
    void ICollection<T>.Clear() => Clear();

    // ----------------------------------------------------

    bool ICollection<T>.IsReadOnly => false;
    object ICollection.SyncRoot => ((ICollection)Items).SyncRoot;
    bool ICollection.IsSynchronized => false;
}