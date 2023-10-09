namespace Yotei.Tools;

// ========================================================
/// <summary>
/// Represents a list-alike collection of arbitrary elements whose behavior can be customized.
/// </summary>
/// <typeparam name="T"></typeparam>
public class CoreList<T> : ICoreList<T>
{
    readonly List<T> Items = new(0);

    /// <summary>
    /// Initializes a new empty instance.
    /// </summary>
    public CoreList() { }

    /// <summary>
    /// Initializes a new instance that carries the given element.
    /// </summary>
    /// <param name="item"></param>
    public CoreList(T item) => Add(item);

    /// <summary>
    /// Initializes a new instance that carries the elements from the given range.
    /// </summary>
    /// <param name="range"></param>
    public CoreList(IEnumerable<T> range) => AddRange(range);

    /// <summary>
    /// <inheritdoc cref="ICoreList{T}.Clone"/>
    /// </summary>
    /// <returns></returns>
    public virtual CoreList<T> Clone() => new(Items);
    ICoreList<T> ICoreList<T>.Clone() => Clone();
    object ICloneable.Clone() => Clone();

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    public override string ToString() => $"Count: {Count}";

    // ----------------------------------------------------

    /// <summary>
    /// Invoked to determine if the given element is valid for the given scenario, or not. If
    /// the '<paramref name="add"/>' is 'true', then the scenario is adding or inserting that
    /// element. Otherwise, it is just a generic validation.
    /// </summary>
    /// <param name="item"></param>
    /// <param name="add"></param>
    /// <returns></returns>
    protected virtual T Validate(T item, bool add) => item;

    /// <summary>
    /// Determines if the two given elements shall be considered equal or equivalent not. The
    /// default behavior of this method is to invoke the default comparer of the type of the
    /// elements in this instance.
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <returns></returns>
    protected virtual bool Compare(T x, T y) => EqualityComparer<T>.Default.Equals(x, y);

    /// <summary>
    /// If this method returns 'true', then this collection will ignore the request to add or
    /// insert the given duplicate element. Its default return value is 'false'. This method
    /// takes precedence over the <see cref="ThrowWhenDuplicate(T)"/> one.
    /// </summary>
    protected virtual bool IgnoreDuplicate(T item) => false;

    /// <summary>
    /// Invoked when there is an attempt to add or insert a duplicated elements. Its default
    /// behavior is just to do nothing. This method is only invoked after validating if the
    /// duplicated element shall not be ignored.
    /// </summary>
    protected virtual void ThrowWhenDuplicate(T item) { }

    /// <summary>
    /// Determines if this collection shall intercept attempts of adding or inserting elements
    /// that are themselves collections of items of the type of this instance. If so, then their
    /// elements are added or inserted instead. The default value of this setting is 'false'.
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
    public int Capacity
    {
        get => Items.Capacity;
        set => Items.Capacity = value;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public void Trim() => Items.TrimExcess();

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="index"></param>
    /// <returns></returns>
    public T this[int index]
    {
        get => Items[index];
        set => ReplaceItem(index, value);
    }
    object? IList.this[int index]
    {
        get => this[index];
        set => this[index] = (T)value!;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    public bool Contains(T item) => IndexOf(item) >= 0;
    bool IList.Contains(object? value) => Contains((T)value!);

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    public int IndexOf(T item)
    {
        item = Validate(item, false);

        for (int i = 0; i < Items.Count; i++)
        {
            var same = Compare(item, Items[i]);
            if (same) return i;
        }
        return -1;
    }
    int IList.IndexOf(object? value) => IndexOf((T)value!);

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    public int LastIndexOf(T item)
    {
        item = Validate(item, false);

        for (int i = Items.Count - 1; i >= 0; i--)
        {
            var same = Compare(item, Items[i]);
            if (same) return i;
        }
        return -1;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    public List<int> IndexesOf(T item)
    {
        item = Validate(item, false);

        var list = new List<int>();
        for (int i = 0; i < Items.Count; i++)
        {
            var same = Compare(item, Items[i]);
            if (same) list.Add(i);
        }
        return list;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    public bool Contains(Predicate<T> predicate) => IndexOf(predicate) >= 0;

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    public int IndexOf(Predicate<T> predicate)
    {
        predicate = predicate.ThrowWhenNull();

        for (int i = 0; i < Items.Count; i++)
        {
            var temp = predicate(Items[i]);
            if (temp) return i;
        }
        return -1;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    public int LastIndexOf(Predicate<T> predicate)
    {
        predicate = predicate.ThrowWhenNull();

        for (int i = Items.Count - 1; i >= 0; i--)
        {
            var temp = predicate(Items[i]);
            if (temp) return i;
        }
        return -1;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    public List<int> IndexesOf(Predicate<T> predicate)
    {
        predicate = predicate.ThrowWhenNull();

        var list = new List<int>(); for (int i = 0; i < Items.Count; i++)
        {
            var temp = predicate(Items[i]);
            if (temp) list.Add(i);
        }
        return list;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    public List<T> ToList() => new(Items);

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
    public List<T> GetRange(int index, int count) => Items.GetRange(index, count);

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="index"></param>
    /// <param name="item"></param>
    /// <returns></returns>
    public virtual int ReplaceItem(int index, T item)
    {
        item = Validate(item, false);

        var temp = IndexOf(item);
        if (temp == index) return 0;

        var range = ToArray();
        RemoveAt(index);
            
        var count = Insert(index, item);
        if (count == 0)
        {
            Clear();
            AddRange(range);
        }
        return count;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    public virtual int Add(T item)
    {
        if (item is IEnumerable<T> range && ExpandNested) return AddRange(range);

        item = Validate(item, true);

        var temp = IndexOf(item); if (temp >= 0)
        {
            if (IgnoreDuplicate(item)) return 0;
            ThrowWhenDuplicate(item);
        }

        Items.Add(item);
        return 1;
    }
    int IList.Add(object? value)
    {
        var index = Count; Add((T)value!);
        return index;
    }
    void ICollection<T>.Add(T item) => Add(item);

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="range"></param>
    /// <returns></returns>
    public virtual int AddRange(IEnumerable<T> range)
    {
        ArgumentNullException.ThrowIfNull(range);

        var count = 0; foreach (var item in range)
        {
            var temp = Add(item);
            count += temp;
        }
        return count;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="index"></param>
    /// <param name="item"></param>
    /// <returns></returns>
    public virtual int Insert(int index, T item)
    {
        if (item is IEnumerable<T> range && ExpandNested) return InsertRange(index, range);

        item = Validate(item, true);

        var temp = IndexOf(item); if (temp >= 0)
        {
            if (IgnoreDuplicate(item)) return 0;
            ThrowWhenDuplicate(item);
        }

        Items.Insert(index, item);
        return 1;
    }
    void IList<T>.Insert(int index, T item) => Insert(index, item);
    void IList.Insert(int index, object? value) => Insert(index, (T)value!);

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="index"></param>
    /// <param name="range"></param>
    /// <returns></returns>
    public virtual int InsertRange(int index, IEnumerable<T> range)
    {
        ArgumentNullException.ThrowIfNull(range);

        var count = 0; foreach (var item in range)
        {
            var temp = Insert(index, item);
            count += temp;
            index += temp;
        }
        return count;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="index"></param>
    /// <returns></returns>
    public virtual int RemoveAt(int index) { Items.RemoveAt(index); return 1; }
    void IList<T>.RemoveAt(int index) => RemoveAt(index);
    void IList.RemoveAt(int index) => RemoveAt(index);

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    public virtual int Remove(T item)
    {
        if (item is IEnumerable<T> range && ExpandNested)
        {
            var count = 0; foreach (var temp in range)
            {
                var num = Remove(temp);
                count += num;
            }
            return count;
        }
        else
        {
            item = Validate(item, false);

            var index = IndexOf(item);
            return index >= 0 ? RemoveAt(index) : 0;
        }
    }
    void IList.Remove(object? value) => Remove((T)value!);
    bool ICollection<T>.Remove(T item) => Remove(item) > 0;

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    public virtual int RemoveLast(T item)
    {
        if (item is IEnumerable<T> range && ExpandNested)
        {
            var count = 0; foreach (var temp in range)
            {
                var num = RemoveLast(temp);
                count += num;
            }
            return count;
        }
        else
        {
            item = Validate(item, false);

            var index = LastIndexOf(item);
            return index >= 0 ? RemoveAt(index) : 0;
        }
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    public virtual int RemoveAll(T item)
    {
        if (item is IEnumerable<T> range && ExpandNested)
        {
            var count = 0; foreach (var temp in range)
            {
                var num = RemoveAll(temp);
                count += num;
            }
            return count;
        }
        else
        {
            item = Validate(item, false);

            var count = 0; while (true)
            {
                var index = IndexOf(item);
                if (index >= 0)
                {
                    count += RemoveAt(index);
                }
                else break;
            }
            return count;
        }
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="index"></param>
    /// <param name="count"></param>
    /// <returns></returns>
    public virtual int RemoveRange(int index, int count)
    {
        if (count > 0) Items.RemoveRange(index, count);
        return count;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    public virtual int Remove(Predicate<T> predicate)
    {
        ArgumentNullException.ThrowIfNull(predicate);

        var index = IndexOf(predicate);
        return index >= 0 ? RemoveAt(index) : 0;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    public virtual int RemoveLast(Predicate<T> predicate)
    {
        ArgumentNullException.ThrowIfNull(predicate);

        var index = LastIndexOf(predicate);
        return index >= 0 ? RemoveAt(index) : 0;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    public virtual int RemoveAll(Predicate<T> predicate)
    {
        ArgumentNullException.ThrowIfNull(predicate);

        var count = 0; while (true)
        {
            var index = IndexOf(predicate);
            if (index >= 0)
            {
                count += RemoveAt(index);
            }
            else break;
        }
        return count;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    public virtual int Clear()
    {
        var count = Count; if (count > 0) Items.Clear();
        return count;
    }
    void ICollection<T>.Clear() => Clear();
    void IList.Clear() => Clear();

    // ----------------------------------------------------

    bool IList.IsFixedSize => false;
    bool IList.IsReadOnly => false;

    bool ICollection<T>.IsReadOnly => false;
    void ICollection<T>.CopyTo(T[] array, int arrayIndex) => ((ICollection<T>)Items).CopyTo(array, arrayIndex);

    bool ICollection.IsSynchronized => false;
    object ICollection.SyncRoot => ((ICollection)Items).SyncRoot;
    void ICollection.CopyTo(Array array, int index) => ((ICollection)Items).CopyTo(array, index);
}