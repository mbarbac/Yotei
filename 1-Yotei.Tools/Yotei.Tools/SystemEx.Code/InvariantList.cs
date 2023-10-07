namespace Yotei.Tools;

// ========================================================
/// <summary>
/// <inheritdoc cref="IInvariantList{T}"/>
/// </summary>
/// <typeparam name="T"></typeparam>
public class InvariantList<T> : IInvariantList<T>
{
    /// <summary>
    /// Represents a core list that uses its master instance custom behaviors.
    /// </summary>
    /// <typeparam name="K"></typeparam>
    protected class Surrogate<K> : CoreList<K>
    {
        public Surrogate(InvariantList<K> master) => Master = master;
        readonly InvariantList<K> Master;
        protected override K Validate(K item, bool add) => Master.Validate(item, add);
        protected override bool Compare(K x, K y) => Master.Compare(x, y);
        protected override bool IgnoreDuplicate(K item) => Master.IgnoreDuplicate(item);
        protected override void ThrowWhenDuplicate(K item) => Master.ThrowWhenDuplicate(item);
        protected override bool ExpandNested => Master.ExpandNested;
    }

    /// <summary>
    /// Invoked to obtain a new surrogate core list.
    /// </summary>
    /// <returns></returns>
    protected virtual Surrogate<T> CreateItems() => new Surrogate<T>(this);

    /// <summary>
    /// The core list that uses its master instance custom behaviors.
    /// </summary>
    protected Surrogate<T> Items { get; }

    // ----------------------------------------------------

    /// <summary>
    /// Initializes a new empty instance.
    /// </summary>
    public InvariantList() => Items = new(this);

    /// <summary>
    /// Initializes a new instance that carries the given element.
    /// </summary>
    /// <param name="item"></param>
    public InvariantList(T item) : this() => Items.Add(item);

    /// <summary>
    /// Initializes a new instance that carries the elements from the given range.
    /// </summary>
    /// <param name="range"></param>
    public InvariantList(IEnumerable<T> range) : this() => Items.AddRange(range);

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
    /// <param name="predicate"></param>
    /// <returns></returns>
    public bool Contains(Predicate<T> predicate) => Items.Contains(predicate);

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
        if (count > 0)
        {
            var range = Items.GetRange(index, count);
            var temp = Clone();
            temp.Items.Clear();
            temp.Items.AddRange(range);
            return temp;
        }
        return this;
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
        var done = temp.Items.ReplaceItem(index, item);
        return done == 0 ? this : temp;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    public virtual IInvariantList<T> Add(T item)
    {
        var temp = Clone();
        var done = temp.Items.Add(item);
        return done == 0 ? this : temp;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="range"></param>
    /// <returns></returns>
    public virtual IInvariantList<T> AddRange(IEnumerable<T> range)
    {
        var temp = Clone();
        var done = temp.Items.AddRange(range);
        return done == 0 ? this : temp;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="index"></param>
    /// <param name="item"></param>
    /// <returns></returns>
    public virtual IInvariantList<T> Insert(int index, T item)
    {
        var temp = Clone();
        var done = temp.Items.Insert(index, item);
        return done == 0 ? this : temp;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="index"></param>
    /// <param name="range"></param>
    /// <returns></returns>
    public virtual IInvariantList<T> InsertRange(int index, IEnumerable<T> range)
    {
        var temp = Clone();
        var done = temp.Items.InsertRange(index, range);
        return done == 0 ? this : temp;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="index"></param>
    /// <returns></returns>
    public virtual IInvariantList<T> RemoveAt(int index)
    {
        var temp = Clone();
        var done = temp.Items.RemoveAt(index);
        return done == 0 ? this : temp;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    public virtual IInvariantList<T> Remove(T item)
    {
        var temp = Clone();
        var done = temp.Items.Remove(item);
        return done == 0 ? this : temp;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    public virtual IInvariantList<T> RemoveLast(T item)
    {
        var temp = Clone();
        var done = temp.Items.RemoveLast(item);
        return done == 0 ? this : temp;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    public virtual IInvariantList<T> RemoveAll(T item)
    {
        var temp = Clone();
        var done = temp.Items.RemoveAll(item);
        return done == 0 ? this : temp;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="index"></param>
    /// <param name="count"></param>
    /// <returns></returns>
    public virtual IInvariantList<T> RemoveRange(int index, int count)
    {
        var temp = Clone();
        var done = temp.Items.RemoveRange(index, count);
        return done == 0 ? this : temp;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    public virtual IInvariantList<T> Remove(Predicate<T> predicate)
    {
        var temp = Clone();
        var done = temp.Items.Remove(predicate);
        return done == 0 ? this : temp;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    public virtual IInvariantList<T> RemoveLast(Predicate<T> predicate)
    {
        var temp = Clone();
        var done = temp.Items.RemoveLast(predicate);
        return done == 0 ? this : temp;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    public virtual IInvariantList<T> RemoveAll(Predicate<T> predicate)
    {
        var temp = Clone();
        var done = temp.Items.RemoveAll(predicate);
        return done == 0 ? this : temp;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    public virtual IInvariantList<T> Clear()
    {
        var temp = Clone();
        var done = temp.Items.Clear();
        return done == 0 ? this : temp;
    }
}