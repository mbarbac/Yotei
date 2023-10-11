namespace Yotei.Tools;

// ========================================================
/// <summary>
/// <inheritdoc cref="ICoreList{T}"/>
/// </summary>
/// <typeparam name="T"></typeparam>
public class CoreList<T> : ICoreList<T>
{
    /// <summary>
    /// Invoked to validate the given element for the given scenario, which is 'true' if it is
    /// adding or inserting that element, or false otherwise. By default this method performs no
    /// validations.
    /// </summary>
    /// <param name="item"></param>
    /// <param name="adding"></param>
    /// <returns></returns>
    protected virtual T Validate(T item, bool adding = false) => item;

    /// <summary>
    /// Invoked to determine if the external other element is equivalent to an existing inner one
    /// or not. By default, this method uses the default comparer for the type of the elements in
    /// this collection.
    /// </summary>
    /// <param name="inner"></param>
    /// <param name="other"></param>
    /// <returns></returns>
    protected virtual bool Equivalent(T inner, T other) => EqualityComparer<T>.Default.Equals(inner, other);

    /// <summary>
    /// Invoked to determine the behavior of this instance when adding or inserting duplicate
    /// elements. By default they are just added into this collection.
    /// </summary>
    protected virtual CoreList.Behavior Behavior => CoreList.Behavior.Add;

    /// <summary>
    /// Invoked to determine if this collection shall intercept attemps of adding or inserting
    /// elements that are themselves enumerations of the type of the elements of this instance.
    /// If so, then their own elements are added or inserted instead of the original one.
    /// </summary>
    protected virtual bool ExpandNested => false;

    // ----------------------------------------------------

    readonly List<T> Items = new(0);

    /// <summary>
    /// Initializes a new empty instance.
    /// </summary>
    public CoreList() { }

    /// <summary>
    /// Initializes a new instance with the given element.
    /// </summary>
    /// <param name="item"></param>
    public CoreList(T item) => Add(item);

    /// <summary>
    /// Initializes a new instance with the given range of elements.
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
    public void Trim() => Items.TrimExcess();

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="index"></param>
    /// <returns></returns>
    public T this[int index]
    {
        get => Items[index];
        set => Replace(index, value);
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
        item = Validate(item);

        for (int i = 0; i < Items.Count; i++)
        {
            var same = Equivalent(Items[i], item);
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
        item = Validate(item);

        for (int i = Items.Count - 1; i >= 0; i--)
        {
            var same = Equivalent(Items[i], item);
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
        item = Validate(item);

        var list = new List<int>();
        for (int i = 0; i < Items.Count; i++)
        {
            var same = Equivalent(Items[i], item);
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

        var list = new List<int>();
        for (int i = 0; i < Items.Count; i++)
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
    public T[] ToArray() => Items.ToArray();

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    public List<T> ToList() => new(Items);
    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="index"></param>
    /// <param name="count"></param>
    /// <returns></returns>
    public List<T> GetRange(int index, int count) => Items.GetRange(index, count);

    // ----------------------------------------------------

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="index"></param>
    /// <param name="item"></param>
    /// <returns></returns>
    public int Replace(int index, T item)
    {
        item = Validate(item);

        var temp = IndexOf(item); if (temp == 0) return 0;
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
    public int Add(T item) => OnAdd(item);
    protected virtual int OnAdd(T item)
    {
        if (ExpandNested && item is IEnumerable<T> range) return AddRange(range);

        item = Validate(item, true);

        var temp = IndexOf(item); if (temp >= 0)
        {
            switch (Behavior)
            {
                case CoreList.Behavior.Ignore: return 0;
                case CoreList.Behavior.Throw:
                    throw new DuplicateException(
                        "Element is a duplicates one.")
                        .WithData(item)
                        .WithData(this);
            }
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
    public int AddRange(IEnumerable<T> range)
    {
        ArgumentNullException.ThrowIfNull(range);

        var count = 0; foreach (var item in range)
        {
            var num = Add(item);
            count += num;
        }
        return count;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="index"></param>
    /// <param name="item"></param>
    /// <returns></returns>
    public int Insert(int index, T item) => OnInsert(index, item);
    protected virtual int OnInsert(int index, T item)
    {
        if (ExpandNested && item is IEnumerable<T> range) return InsertRange(index, range);

        item = Validate(item, true);

        var temp = IndexOf(item); if (temp >= 0)
        {
            switch (Behavior)
            {
                case CoreList.Behavior.Ignore: return 0;
                case CoreList.Behavior.Throw:
                    throw new DuplicateException(
                        "Element is a duplicates one.")
                        .WithData(item)
                        .WithData(this);
            }
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
    public int InsertRange(int index, IEnumerable<T> range)
    {
        ArgumentNullException.ThrowIfNull(range);

        var count = 0; foreach (var item in range)
        {
            var num = Insert(index, item);
            count += num;
            index += num;
        }
        return count;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="index"></param>
    /// <returns></returns>
    public int RemoveAt(int index) => OnRemoveAt(index);
    protected virtual int OnRemoveAt(int index)
    {
        Items.RemoveAt(index);
        return 1;
    }
    void IList<T>.RemoveAt(int index) => RemoveAt(index);
    void IList.RemoveAt(int index) => RemoveAt(index);

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    public int Remove(T item)
    {
        if (ExpandNested && item is IEnumerable<T> range)
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
            item = Validate(item);

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
    public int RemoveLast(T item)
    {
        if (ExpandNested && item is IEnumerable<T> range)
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
            item = Validate(item);

            var index = LastIndexOf(item);
            return index >= 0 ? RemoveAt(index) : 0;
        }
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    public int RemoveAll(T item)
    {
        if (ExpandNested && item is IEnumerable<T> range)
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
            item = Validate(item);

            var count = 0; while (true)
            {
                var index = IndexOf(item);
                if (index >= 0) count += RemoveAt(index);
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
    public int RemoveRange(int index, int count) => OnRemoveRange(index, count);
    protected virtual int OnRemoveRange(int index, int count)
    {
        if (count > 0) Items.RemoveRange(index, count);
        return count;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    public int Remove(Predicate<T> predicate)
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
    public int RemoveLast(Predicate<T> predicate)
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
    public int RemoveAll(Predicate<T> predicate)
    {
        ArgumentNullException.ThrowIfNull(predicate);

        var count = 0; while (true)
        {
            var index = IndexOf(predicate);
            if (index >= 0) count += RemoveAt(index);
            else break;
        }
        return count;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    public int Clear() => OnClear();
    protected virtual int OnClear()
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