namespace Yotei.Tools;

// ========================================================
/// <summary>
/// <inheritdoc cref="IInvariantList{T}"/>
/// </summary>
/// <typeparam name="T"></typeparam>
public class InvariantList<T> : IInvariantList<T>
{
    /// <summary>
    /// A helper surrogate class.
    /// </summary>
    /// <typeparam name="K"></typeparam>
    class Surrogate<K> : CoreList<K>
    {
        public Surrogate(InvariantList<K> master) => Master = master;
        readonly InvariantList<K> Master;
        protected override K Validate(K item, bool add) => Master.Validate(item, add);
        protected override bool Compare(K x, K y) => Master.Compare(x, y);
        protected override bool IgnoreDuplicate(K item) => Master.IgnoreDuplicate(item);
        protected override void ThrowWhenDuplicate(K item) => Master.ThrowWhenDuplicate(item);
        protected override bool ExpandNested => Master.ExpandNested;
    }
    readonly Surrogate<T> Items = default!;

    // ----------------------------------------------------

    /// <summary>
    /// Initializes a new empty instance.
    /// </summary>
    public InvariantList()
    {
        Items = new(this);
    }

    /// <summary>
    /// Initializes a new instance that carries the given element.
    /// </summary>
    /// <param name="item"></param>
    public InvariantList(T item)
    {
        Items = new(this);
        AddInternal(item);
    }

    /// <summary>
    /// Initializes a new instance that carries the elements from the given range.
    /// </summary>
    /// <param name="range"></param>
    public InvariantList(IEnumerable<T> range)
    {
        Items = new(this);
        AddRangeInternal(range);
    }

    /// <summary>
    /// <inheritdoc cref="IInvariantList{T}.Clone"/>
    /// </summary>
    /// <returns></returns>
    public virtual InvariantList<T> Clone() => new(Items);
    IInvariantList<T> IInvariantList<T>.Clone() => Clone();
    object ICloneable.Clone() => Clone();

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    public override string ToString() => $"Count: {Count}";

    // ----------------------------------------------------

    /// <summary>
    /// <inheritdoc cref="CoreList{T}.Validate(T, bool)"/>
    /// </summary>
    /// <param name="item"></param>
    /// <param name="add"></param>
    /// <returns></returns>
    protected virtual T Validate(T item, bool add) => item;

    /// <summary>
    /// <inheritdoc cref="CoreList{T}.Compare(T, T)"/>
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <returns></returns>
    protected virtual bool Compare(T x, T y) => EqualityComparer<T>.Default.Equals(x, y);

    /// <summary>
    /// <inheritdoc cref="CoreList{T}.IgnoreDuplicate(T)"/>
    /// </summary>
    protected virtual bool IgnoreDuplicate(T item) => false;

    /// <summary>
    /// <inheritdoc cref="CoreList{T}.ThrowWhenDuplicate(T)"/>
    /// </summary>
    protected virtual void ThrowWhenDuplicate(T item) { }

    /// <summary>
    /// <inheritdoc cref="CoreList{T}.ExpandNested"/>
    /// </summary>
    protected virtual bool ExpandNested => false;

    // ----------------------------------------------------

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns><inheritdoc/></returns>
    public IEnumerator<T> GetEnumerator() => Items.GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public int Count => Items.Count;

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public void Trim() => Items.Trim();

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="index"></param>
    /// <returns></returns>
    public T this[int index] => Items[index];

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    public bool Contains(T item) => Items.Contains(item);

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    public int IndexOf(T item) => Items.IndexOf(item);

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    public int LastIndexOf(T item) => Items.LastIndexOf(item);

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    public List<int> IndexesOf(T item) => Items.IndexesOf(item);

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    public bool Contains(Predicate<T> predicate) => Items.Contains(predicate);

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    public int IndexOf(Predicate<T> predicate) => Items.IndexOf(predicate);

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="item"></param>
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
    /// <returns></returns>
    public List<T> ToList() => Items.ToList();

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    public T[] ToArray() => Items.ToArray();

    // ----------------------------------------------------

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="index"></param>
    /// <param name="count"></param>
    /// <returns></returns>
    public virtual IInvariantList<T> GetRange(int index, int count)
    {
        var temp = Clone();
        var done = temp.GetRangeInternal(index, count);
        return done == 0 ? this : temp;
    }
    protected int GetRangeInternal(int index, int count)
    {
        if (count > 0)
        {
            var range = Items.GetRange(index, count);
            Items.Clear();
            Items.AddRange(range);
        }
        return count;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="index"></param>
    /// <param name="item"></param>
    /// <returns></returns>
    public virtual IInvariantList<T> ReplaceItem(int index, T item)
    {
        var temp = Clone();
        var done = temp.ReplaceItemInternal(index, item);
        return done == 0 ? this : temp;
    }
    protected int ReplaceItemInternal(int index, T item) => Items.ReplaceItem(index, item);

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    public virtual IInvariantList<T> Add(T item)
    {
        var temp = Clone();
        var done = temp.AddInternal(item);
        return done == 0 ? this : temp;
    }
    protected int AddInternal(T item) => Items.Add(item);

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="range"></param>
    /// <returns></returns>
    public virtual IInvariantList<T> AddRange(IEnumerable<T> range)
    {
        var temp = Clone();
        var done = temp.AddRangeInternal(range);
        return done == 0 ? this : temp;
    }
    protected int AddRangeInternal(IEnumerable<T> range) => Items.AddRange(range);

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="index"></param>
    /// <param name="item"></param>
    /// <returns></returns>
    public virtual IInvariantList<T> Insert(int index, T item)
    {
        var temp = Clone();
        var done = temp.InsertInternal(index, item);
        return done == 0 ? this : temp;
    }
    protected int InsertInternal(int index, T item) => Items.Insert(index, item);

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="index"></param>
    /// <param name="range"></param>
    /// <returns></returns>
    public virtual IInvariantList<T> InsertRange(int index, IEnumerable<T> range)
    {
        var temp = Clone();
        var done = temp.InsertRangeInternal(index, range);
        return done == 0 ? this : temp;
    }
    protected int InsertRangeInternal(int index, IEnumerable<T> range) => Items.InsertRange(index, range);

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="index"></param>
    /// <returns></returns>
    public virtual IInvariantList<T> RemoveAt(int index)
    {
        var temp = Clone();
        var done = temp.RemoveAtInternal(index);
        return done == 0 ? this : temp;
    }
    protected int RemoveAtInternal(int index) => Items.RemoveAt(index);

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    public virtual IInvariantList<T> Remove(T item)
    {
        var temp = Clone();
        var done = temp.RemoveInternal(item);
        return done == 0 ? this : temp;
    }
    protected int RemoveInternal(T item) => Items.Remove(item);

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    public virtual IInvariantList<T> RemoveLast(T item)
    {
        var temp = Clone();
        var done = temp.RemoveLastInternal(item);
        return done == 0 ? this : temp;
    }
    protected int RemoveLastInternal(T item) => Items.RemoveLast(item);

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    public virtual IInvariantList<T> RemoveAll(T item)
    {
        var temp = Clone();
        var done = temp.RemoveAllInternal(item);
        return done == 0 ? this : temp;
    }
    protected int RemoveAllInternal(T item) => Items.RemoveAll(item);

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="index"></param>
    /// <param name="count"></param>
    /// <returns></returns>
    public virtual IInvariantList<T> RemoveRange(int index, int count)
    {
        var temp = Clone();
        var done = temp.RemoveRangeInternal(index, count);
        return done == 0 ? this : temp;
    }
    protected int RemoveRangeInternal(int index, int count) => Items.RemoveRange(index, count);

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    public virtual IInvariantList<T> Remove(Predicate<T> predicate)
    {
        var temp = Clone();
        var done = temp.RemoveInternal(predicate);
        return done == 0 ? this : temp;
    }
    protected int RemoveInternal(Predicate<T> predicate) => Items.Remove(predicate);

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    public virtual IInvariantList<T> RemoveLast(Predicate<T> predicate)
    {
        var temp = Clone();
        var done = temp.RemoveLastInternal(predicate);
        return done == 0 ? this : temp;
    }
    protected int RemoveLastInternal(Predicate<T> predicate) => Items.RemoveLast(predicate);

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    public virtual IInvariantList<T> RemoveAll(Predicate<T> predicate)
    {
        var temp = Clone();
        var done = temp.RemoveAllInternal(predicate);
        return done == 0 ? this : temp;
    }
    protected int RemoveAllInternal(Predicate<T> predicate) => Items.RemoveAll(predicate);

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    public virtual IInvariantList<T> Clear()
    {
        var temp = Clone();
        var done = temp.ClearInternal();
        return done == 0 ? this : temp;
    }
    protected int ClearInternal() => Items.Clear();
}