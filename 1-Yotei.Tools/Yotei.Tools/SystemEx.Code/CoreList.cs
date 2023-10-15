namespace Yotei.Tools;

// ========================================================
/// <summary>
/// <inheritdoc cref="ICoreList{T}"/>
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
    /// <br/> By default this method copies both the delegates and the original contents from
    /// the source instance. If any delegate depends on the state of its concrete host instance,
    /// then override this method appropriately.
    /// </summary>
    /// <param name="source"></param>
    protected CoreList(CoreList<T> source)
    {
        ArgumentNullException.ThrowIfNull(source);

        Validate = source.Validate;
        Compare = source.Compare;
        AddDuplicate = source.AddDuplicate;
        ExpandNested = source.ExpandNested;

        AddRange(source);
    }

    /// <summary>
    /// <inheritdoc cref="ICloneable.Clone"/>
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
    /// The (item, add) delegate invoked to determine if the given element is a valid one for
    /// the scenario where it will be used, it being 'true' when the element is to be added or
    /// inserted into this collection, or 'false' otherwise. By default this delegate just
    /// returns the given element.
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
    /// The (inner, other) delegate invoked to determine if the existing element and the other
    /// one shall be considered equivalent or not. By default this delegate just invokes the
    /// default comparer for the elements in this collection.
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
    /// The (item) delegate invoked to determine the behavior of this collection when adding
    /// or inserting a duplicated element. By default this delegate returns 'true' to add the
    /// duplicated element. If it returns 'false', then the duplicated element will be ignored.
    /// This delegate can also throw a duplicate exception if it is needed.
    /// </summary>
    public Func<T, bool> AddDuplicate
    {
        get => _AddDuplicate;
        set
        {
            if (ReferenceEquals(_AddDuplicate, value)) return;
            _AddDuplicate = value;

            if (Count > 0)
            {
                var range = ToArray();
                Clear();
                AddRange(range);
            }
        }
    }
    Func<T, bool> _AddDuplicate = (item) => true;

    /// <summary>
    /// The (item) delegate invoked to determine if the given element, which is an enumeration
    /// of the type of the elements of this collection, shall be expanded and its own elements
    /// added or inserted instead of the original one, or not. By default this delegate returns
    /// 'false'.
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
        item = Validate(item, false);

        for (int i = 0; i < Items.Count; i++)
        {
            var same = Compare(Items[i], item);
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
            var same = Compare(Items[i], item);
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
            var same = Compare(Items[i], item);
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
    /// <returns></returns>
    public virtual int Replace(int index, T item)
    {
        item = Validate(item, true);

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
        if (item is IEnumerable<T> range && ExpandNested(item)) return AddRange(range);

        item = Validate(item, true);

        var temp = IndexOf(item);
        if (temp >= 0)
        {
            if (!AddDuplicate(item)) return 0;
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
        if (item is IEnumerable<T> range && ExpandNested(item)) return InsertRange(index, range);

        item = Validate(item, true);

        var temp = IndexOf(item);
        if (temp >= 0)
        {
            if (!AddDuplicate(item)) return 0;
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
    /// <param name="item"></param>
    /// <returns></returns>
    public virtual int Remove(T item)
    {
        if (item is IEnumerable<T> range && ExpandNested(item))
        {
            var count = 0; foreach (var temp in range) count += Remove(temp);
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
        if (item is IEnumerable<T> range && ExpandNested(item))
        {
            var count = 0; foreach (var temp in range) count += RemoveLast(temp);
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
        if (item is IEnumerable<T> range && ExpandNested(item))
        {
            var count = 0; foreach (var temp in range) count += RemoveAll(temp);
            return count;
        }
        else
        {
            item = Validate(item, false);

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
}