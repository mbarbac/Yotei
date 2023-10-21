namespace Yotei.Tools;

// ========================================================
/// <summary>
/// <inheritdoc cref="ICoreList{T}"/>
/// </summary>
/// <typeparam name="T"></typeparam>
public class CoreList<T> : ICoreList<T>
{
    readonly List<T> Items = new(0);
    const bool DEFAULT_STRICT = false;

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
    /// Initializes a new instance with the elements from the given range.
    /// </summary>
    /// <param name="range"></param>
    public CoreList(IEnumerable<T> range) => AddRange(range);

    /// <summary>
    /// Copy constructor.
    /// <br/> By default this constructor copies the behavioral settings from the given source
    /// instance. If this is not appropriate, either overrides this constructor, or override the
    /// <see cref="CopyBehaviors(CoreList{T})"/> method.
    /// </summary>
    /// <param name="source"></param>
    protected CoreList(CoreList<T> source)
    {
        ArgumentNullException.ThrowIfNull(source);
        CopyBehaviors(source);
        AddRange(source);
    }

    /// <summary>
    /// Invoked to copy the behavioral settings from the given source.
    /// </summary>
    /// <param name="source"></param>
    protected virtual void CopyBehaviors(CoreList<T> source)
    {
        Validate = source.Validate;
        Compare = source.Compare;
        AcceptDuplicate = source.AcceptDuplicate;
        ExpandNested = source.ExpandNested;
    }

    /// <summary>
    /// <inheritdoc cref="ICoreList{T}.Clone"/>
    /// </summary>
    /// <returns></returns>
    public virtual CoreList<T> Clone() => new(this);
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
    public Func<T, bool, T> Validate
    {
        get => _Validate;
        set
        {
            if (ReferenceEquals(_Validate, value)) return;
            _Validate = value;

            if (Count > 0)
            {
                var range = ToArray();
                Clear();
                AddRange(range);
            }
        }
    }
    Func<T, bool, T> _Validate = (item, add) => item;

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public Func<T, T, bool> Compare
    {
        get => _Compare;
        set
        {
            if (ReferenceEquals(_Compare, value)) return;
            _Compare = value;

            if (Count > 0)
            {
                var range = ToArray();
                Clear();
                AddRange(range);
            }
        }
    }
    Func<T, T, bool> _Compare = EqualityComparer<T>.Default.Equals;

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public Func<T, bool> AcceptDuplicate
    {
        get => _AcceptDuplicate;
        set
        {
            if (ReferenceEquals(_AcceptDuplicate, value)) return;
            _AcceptDuplicate = value;

            if (Count > 0)
            {
                var range = ToArray();
                Clear();
                AddRange(range);
            }
        }
    }
    Func<T, bool> _AcceptDuplicate = (item) => true;

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public Func<T, bool> ExpandNested
    {
        get => _ExpandNested;
        set
        {
            if (ReferenceEquals(_ExpandNested, value)) return;
            _ExpandNested = value;

            if (Count > 0)
            {
                var range = ToArray();
                Clear();
                AddRange(range);
            }
        }
    }
    Func<T, bool> _ExpandNested = (item) => false;

    // ----------------------------------------------------

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
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
        set => Replace(index, value, true);
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
    /// <param name="strict"></param>
    /// <returns></returns>
    public bool Contains(T item, bool strict = false) => IndexOf(item) >= 0;
    bool IList.Contains(object? value) => Contains((T)value!, DEFAULT_STRICT);
    bool ICollection<T>.Contains(T item) => Contains(item, DEFAULT_STRICT);

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="item"></param>
    /// <param name="strict"></param>
    /// <returns></returns>
    public int IndexOf(T item, bool strict = false)
    {
        item = Validate(item, false);

        for (int i = 0; i < Items.Count; i++)
        {
            var inner = Items[i];
            var same = strict
                ? ((inner is null && item is null) || (inner is not null && inner.Equals(item)))
                : Compare(inner, item);

            if (same) return i;
        }
        return -1;
    }
    int IList<T>.IndexOf(T item) => IndexOf(item, DEFAULT_STRICT);
    int IList.IndexOf(object? value) => IndexOf((T)value!, DEFAULT_STRICT);

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="item"></param>
    /// <param name="strict"></param>
    /// <returns></returns>
    public int LastIndexOf(T item, bool strict = false)
    {
        item = Validate(item, false);

        for (int i = Items.Count - 1; i >= 0; i--)
        {
            var inner = Items[i];
            var same = strict
                ? ((inner is null && item is null) || (inner is not null && inner.Equals(item)))
                : Compare(inner, item);

            if (same) return i;
        }
        return -1;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="item"></param>
    /// <param name="strict"></param>
    /// <returns></returns>
    public List<int> IndexesOf(T item, bool strict = false)
    {
        item = Validate(item, false);

        var list = new List<int>();
        for (int i = 0; i < Items.Count; i++)
        {
            var inner = Items[i];
            var same = strict
                ? ((inner is null && item is null) || (inner is not null && inner.Equals(item)))
                : Compare(inner, item);

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
        ArgumentNullException.ThrowIfNull(predicate);

        for (int i = 0; i < Items.Count; i++)
        {
            var same = predicate(Items[i]);
            if (same) return i;
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
        ArgumentNullException.ThrowIfNull(predicate);

        for (int i = Items.Count - 1; i >= 0; i--)
        {
            var same = predicate(Items[i]);
            if (same) return i;
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
        ArgumentNullException.ThrowIfNull(predicate);

        var list = new List<int>();
        for (int i = 0; i < Items.Count; i++)
        {
            var same = predicate(Items[i]);
            if (same) list.Add(i);
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
    /// <param name="strict"></param>
    /// <returns></returns>
    public virtual int Replace(int index, T item, bool strict = false)
    {
        item = Validate(item, true);

        var temp = IndexOf(item, strict);
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
        if (item is IEnumerable<T> range && ExpandNested(item))
        {
            return AddRange(range);
        }
        else
        {
            item = Validate(item, true);

            var temp = IndexOf(item);
            if (temp >= 0)
            {
                if (!AcceptDuplicate(item)) return 0;
            }

            Items.Add(item);
            return 1;
        }
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

        var count = 0; foreach (var item in range) count += Add(item);
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
        if (item is IEnumerable<T> range && ExpandNested(item))
        {
            return InsertRange(index, range);
        }
        else
        {
            item = Validate(item, true);

            var temp = IndexOf(item);
            if (temp >= 0)
            {
                if (!AcceptDuplicate(item)) return 0;
            }

            Items.Insert(index, item);
            return 1;
        }
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
    public virtual int RemoveAt(int index)
    {
        Items.RemoveAt(index);
        return 1;
    }
    void IList<T>.RemoveAt(int index) => RemoveAt(index);
    void IList.RemoveAt(int index) => RemoveAt(index);

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
    /// <param name="item"></param>
    /// <param name="strict"></param>
    /// <returns></returns>
    public virtual int Remove(T item, bool strict = false)
    {
        if (item is IEnumerable<T> range && ExpandNested(item))
        {
            var count = 0; foreach (var temp in range) count += Remove(temp, strict);
            return count;
        }
        else
        {
            item = Validate(item, false);

            var index = IndexOf(item, strict);
            return index >= 0 ? RemoveAt(index) : 0;
        }
    }
    void IList.Remove(object? value) => Remove((T)value!, DEFAULT_STRICT);
    bool ICollection<T>.Remove(T item) => Remove(item, DEFAULT_STRICT) > 0;

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="item"></param>
    /// <param name="strict"></param>
    /// <returns></returns>
    public virtual int RemoveLast(T item, bool strict = false)
    {
        if (item is IEnumerable<T> range && ExpandNested(item))
        {
            var count = 0; foreach (var temp in range) count += RemoveLast(temp, strict);
            return count;
        }
        else
        {
            item = Validate(item, false);

            var index = LastIndexOf(item, strict);
            return index >= 0 ? RemoveAt(index) : 0;
        }
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="item"></param>
    /// <param name="strict"></param>
    /// <returns></returns>
    public virtual int RemoveAll(T item, bool strict = false)
    {
        if (item is IEnumerable<T> range && ExpandNested(item))
        {
            var count = 0; foreach (var temp in range) count += Remove(temp, strict);
            return count;
        }
        else
        {
            item = Validate(item, false);

            var count = 0; while (true)
            {
                var index = IndexOf(item, strict);

                if (index >= 0) count += RemoveAt(index);
                else break;
            }
            return count;
        }
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

            if (index >= 0) count += RemoveAt(index);
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
        var count = Items.Count; if (count > 0) Items.Clear();
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

    public override bool Equals(object? obj) => ReferenceEquals(this, obj);
    public override int GetHashCode() => Items.GetHashCode();
}