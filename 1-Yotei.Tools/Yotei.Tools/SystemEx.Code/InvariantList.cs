namespace Yotei.Tools;

// ========================================================
/// <summary>
/// <inheritdoc cref="IInvariantList{T}"/>
/// </summary>
/// <typeparam name="T"></typeparam>
public class InvariantList<T> : IInvariantList<T>
{
    readonly CoreList<T> Items;

    /// <summary>
    /// Initializes a new empty instance.
    /// </summary>
    public InvariantList() => Items = new();

    /// <summary>
    /// Initializes a new instance that carries the given element.
    /// </summary>
    /// <param name="item"></param>
    public InvariantList(T item) : this() => AddInternal(item);

    /// <summary>
    /// Initializes a new instance that carries the elements from the given range.
    /// </summary>
    /// <param name="range"></param>
    public InvariantList(IEnumerable<T> range) : this() => AddRangeInternal(range);

    /// <summary>
    /// <inheritdoc cref="IInvariantList{T}.Clone"/>
    /// </summary>
    /// <returns></returns>
    public virtual InvariantList<T> Clone()
    {
        var temp = OnClone();
        temp.Items.AddRange(this);
        return temp;
    }
    IInvariantList<T> IInvariantList<T>.Clone() => Clone();
    object ICloneable.Clone() => Clone();

    /// <summary>
    /// Invoked while cloning to obtain a new empty instance but with the appropriate settings.
    /// </summary>
    /// <returns></returns>
    protected virtual InvariantList<T> OnClone() => new()
    {
        Validator = Validator,
        Comparer = Comparer,
        Behavior = Behavior,
        Flatten = Flatten,
    };

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    public override string ToString() => $"Count: {Count}";

    // ----------------------------------------------------

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public Func<T, bool, T> Validator
    {
        get => Items.Validator;
        init => Items.Validator = value;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public IInvariantList<T> WithValidator(Func<T, bool, T> value)
    {
        var temp = Clone();
        var done = temp.WithValidatorInternal(value);
        return done ? temp : this;
    }
    protected bool WithValidatorInternal(Func<T, bool, T> value)
    {
        value = value.ThrowWhenNull();

        if (ReferenceEquals(Validator, value)) return false;

        Items.Validator = value;
        return true;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public Func<T, T, bool> Comparer
    {
        get => Items.Comparer;
        init => Items.Comparer = value;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public IInvariantList<T> WithComparer(Func<T, T, bool> value)
    {
        var temp = Clone();
        var done = temp.WithComparerInternal(value);
        return done ? temp : this;
    }
    protected bool WithComparerInternal(Func<T, T, bool> value)
    {
        value = value.ThrowWhenNull();

        if (ReferenceEquals(Comparer, value)) return false;

        Items.Comparer = value;
        return true;
    }

    /// <summary>
    /// <inheritdoc cref="ICoreList{T}.Behavior"/>
    /// </summary>
    public CoreListBehavior Behavior
    {
        get => Items.Behavior;
        init => Items.Behavior = value;
    }

    /// <summary>
    /// Returns a copy of this instance where the value of the <see cref="Behavior"/> property
    /// has been replaced by the new given one.
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public IInvariantList<T> WithBehavior(CoreListBehavior value)
    {
        var temp = Clone();
        var done = temp.WithBehaviorInternal(value);
        return done ? temp : this;
    }
    protected bool WithBehaviorInternal(CoreListBehavior value)
    {
        if (Behavior == value) return false;

        Items.Behavior = value;
        return true;
    }

    /// <summary>
    /// <inheritdoc cref="ICoreList{T}.Flatten"/>
    /// </summary>
    public bool Flatten
    {
        get => Items.Flatten;
        init => Items.Flatten = value;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public IInvariantList<T> WithFlatten(bool value)
    {
        var temp = Clone();
        var done = temp.WithFlattenInternal(value);
        return done ? temp : this;
    }
    protected bool WithFlattenInternal(bool value)
    {
        if (Flatten == value) return false;

        Items.Flatten = value;
        return true;
    }

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
    public IInvariantList<T> GetRange(int index, int count)
    {
        if (count == 0) return this;

        var range = Items.GetRange(index, count);
        var temp = Clone();
        temp.Items.Clear();
        temp.Items.AddRange(range);
        return temp;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="index"></param>
    /// <param name="item"></param>
    /// <returns></returns>
    public IInvariantList<T> ReplaceItem(int index, T item)
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
    public IInvariantList<T> Add(T item)
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
    public IInvariantList<T> AddRange(IEnumerable<T> range)
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
    public IInvariantList<T> Insert(int index, T item)
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
    public IInvariantList<T> InsertRange(int index, IEnumerable<T> range)
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
    public IInvariantList<T> RemoveAt(int index)
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
    public IInvariantList<T> Remove(T item)
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
    public IInvariantList<T> RemoveLast(T item)
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
    public IInvariantList<T> RemoveAll(T item)
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
    public IInvariantList<T> RemoveRange(int index, int count)
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
    public IInvariantList<T> Remove(Predicate<T> predicate)
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
    public IInvariantList<T> RemoveLast(Predicate<T> predicate)
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
    public IInvariantList<T> RemoveAll(Predicate<T> predicate)
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
    public IInvariantList<T> Clear()
    {
        var temp = Clone();
        var done = temp.ClearInternal();
        return done == 0 ? this : temp;
    }
    protected int ClearInternal() => Items.Clear();
}