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
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    public virtual CoreList<T> Clone()
    {
        var temp = new CoreList<T>();
        temp.CopySettings(this);
        temp.AddRange(Items);
        return temp;
    }
    ICoreList<T> ICoreList<T>.Clone() => Clone();
    object ICloneable.Clone() => Clone();

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="source"></param>
    public virtual void CopySettings(ICoreList<T> source)
    {
        ArgumentNullException.ThrowIfNull(source);

        Validator = source.Validator;
        Comparer = source.Comparer;
        Behavior = source.Behavior;
        Flatten = source.Flatten;
    }

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
        get => _Validator;
        set
        {
            ArgumentNullException.ThrowIfNull(value, nameof(Validator));
            if (ReferenceEquals(_Validator, value)) return;

            _Validator = value; if (Count > 0)
            {
                var range = ToArray();
                Clear();
                AddRange(range);
            }
        }
    }
    Func<T, bool, T> _Validator = (item, _) => item;

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public Func<T, T, bool> Comparer
    {
        get => _Comparer;
        set
        {
            ArgumentNullException.ThrowIfNull(value, nameof(Validator));
            if (ReferenceEquals(_Comparer, value)) return;

            _Comparer = value; if (Count > 0)
            {
                var range = ToArray();
                Clear();
                AddRange(range);
            }
        }
    }
    Func<T, T, bool> _Comparer = EqualityComparer<T>.Default.Equals;

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public CoreListBehavior Behavior
    {
        get => _Behavior;
        set
        {
            if (_Behavior == value) return;

            _Behavior = value; if (Count > 0)
            {
                var range = ToArray();
                Clear();
                AddRange(range);
            }
        }
    }
    CoreListBehavior _Behavior = CoreListBehavior.Add;

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public bool Flatten
    {
        get => _Flatten;
        set
        {
            if (_Flatten == value) return;

            _Flatten = value; if (Count > 0)
            {
                var range = ToArray();
                Clear();
                AddRange(range);
            }
        }
    }
    bool _Flatten = false;

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
        item = Validator(item, false);

        for (int i = 0; i < Items.Count; i++)
        {
            var temp = Comparer(item, Items[i]);
            if (temp) return i;
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
        item = Validator(item, false);

        for (int i = Items.Count - 1; i >= 0; i--)
        {
            var temp = Comparer(item, Items[i]);
            if (temp) return i;
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
        item = Validator(item, false);

        var list = new List<int>(); for (int i = 0; i < Items.Count; i++)
        {
            var temp = Comparer(item, Items[i]);
            if (temp) list.Add(i);
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
        ArgumentNullException.ThrowIfNull(predicate);

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
        ArgumentNullException.ThrowIfNull(predicate);

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
    public int ReplaceItem(int index, T item)
    {
        item = Validator(item, false);

        var temp = IndexOf(item);
        if (temp == index) return 0;

        var range = ToArray();
        RemoveAt(index);

        temp = Insert(index, item);
        if (temp == 0)
        {
            Clear();
            AddRange(range);
            return 0;
        }
        return temp;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    public int Add(T item)
    {
        if (Flatten && item is IEnumerable<T> range) return AddRange(range);

        item = Validator(item, true);

        if (Behavior is CoreListBehavior.Throw or CoreListBehavior.Ignore)
        {
            var temp = IndexOf(item);
            if (temp >= 0)
            {
                if (Behavior == CoreListBehavior.Ignore) return 0;

                throw new DuplicateException(
                    "The element to add is a duplicate one.")
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
    public int Insert(int index, T item)
    {
        if (Flatten && item is IEnumerable<T> range) return InsertRange(index, range);

        item = Validator(item, true);

        if (Behavior is CoreListBehavior.Throw or CoreListBehavior.Ignore)
        {
            var temp = IndexOf(item);
            if (temp >= 0)
            {
                if (Behavior == CoreListBehavior.Ignore) return 0;

                throw new DuplicateException(
                    "The element to insert is a duplicate one.")
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
    public int RemoveAt(int index)
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
        if (Flatten && item is IEnumerable<T> range)
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
        if (Flatten && item is IEnumerable<T> range)
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
        if (Flatten && item is IEnumerable<T> range)
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
            var count = 0; while (true)
            {
                var temp = IndexOf(item);

                if (temp >= 0) count += RemoveAt(temp);
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
    public int RemoveRange(int index, int count)
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
            if (index < 0) break;

            count += RemoveAt(index);
        }
        return count;
    }

    /// <summary>
    /// </summary>
    /// <returns></returns>
    public int Clear()
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